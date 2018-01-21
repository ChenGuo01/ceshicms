﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Table;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin.Features;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageContent : BasePageCms
    {
        public Repeater RptContents;
        public SqlPager SpContents;
        public Literal LtlColumnsHead;
        public Literal LtlCommandsHead;
        public Literal LtlButtons;
        public Literal LtlMoreButtons;
        public DateTimeTextBox TbDateFrom;
        public DropDownList DdlSearchType;
        public TextBox TbKeyword;

        private NodeInfo _nodeInfo;
        private string _tableName;
        private List<int> _relatedIdentities;
        private List<TableStyleInfo> _styleInfoList;
        private StringCollection _attributesOfDisplay;
        private List<TableStyleInfo> _attributesOfDisplayStyleInfoList;
        private Dictionary<string, IContentRelated> _pluginChannels;
        private bool _isEdit;
        private readonly Dictionary<string, string> _nameValueCacheDict = new Dictionary<string, string>();

        public static string GetRedirectUrl(int publishmentSystemId, int nodeId)
        {
            return PageUtils.GetCmsUrl(nameof(PageContent), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"nodeId", nodeId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var permissions = PermissionsManager.GetPermissions(Body.AdminName);

            PageUtils.CheckRequestParameter("publishmentSystemId", "nodeId");
            var nodeId = Body.GetQueryInt("nodeId");
            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, nodeId);
            _nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);
            _tableName = NodeManager.GetTableName(PublishmentSystemInfo, _nodeInfo);
            _styleInfoList = TableStyleManager.GetTableStyleInfoList(_tableName, _relatedIdentities);
            _attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(NodeManager.GetContentAttributesOfDisplay(PublishmentSystemId, nodeId));
            _attributesOfDisplayStyleInfoList = ContentUtility.GetColumnTableStyleInfoList(PublishmentSystemInfo, _styleInfoList);
            _pluginChannels = PluginManager.GetContentRelatedFeatures(_nodeInfo);
            _isEdit = TextUtility.IsEdit(PublishmentSystemInfo, nodeId, Body.AdminName);

            if (_nodeInfo.Additional.IsPreviewContents)
            {
                new Action(() =>
                {
                    DataProvider.ContentDao.DeletePreviewContents(PublishmentSystemId, _tableName, _nodeInfo);
                }).BeginInvoke(null, null);
            }

            if (!HasChannelPermissions(nodeId, AppManager.Permissions.Channel.ContentView, AppManager.Permissions.Channel.ContentAdd, AppManager.Permissions.Channel.ContentEdit, AppManager.Permissions.Channel.ContentDelete, AppManager.Permissions.Channel.ContentTranslate))
            {
                if (!Body.IsAdminLoggin)
                {
                    PageUtils.RedirectToLoginPage();
                    return;
                }
                PageUtils.RedirectToErrorPage("您无此栏目的操作权限！");
                return;
            }

            SpContents.ControlToPaginate = RptContents;
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            SpContents.ItemsPerPage = PublishmentSystemInfo.Additional.PageSize;

            var administratorName = AdminUtility.IsViewContentOnlySelf(Body.AdminName, PublishmentSystemId, nodeId)
                    ? Body.AdminName
                    : string.Empty;

            if (Body.IsQueryExists("searchType"))
            {
                var owningNodeIdList = new List<int>
                {
                    nodeId
                };
                SpContents.SelectCommand = DataProvider.ContentDao.GetSelectCommend(_tableName, PublishmentSystemId, nodeId, permissions.IsSystemAdministrator, owningNodeIdList, Body.GetQueryString("searchType"), Body.GetQueryString("keyword"), Body.GetQueryString("dateFrom"), string.Empty, false, ETriState.All, false, false, false, administratorName);
            }
            else
            {
                SpContents.SelectCommand = DataProvider.ContentDao.GetSelectCommend(_tableName, nodeId, ETriState.All, administratorName);
            }

            //spContents.SortField = BaiRongDataProvider.ContentDao.GetSortFieldName();
            //spContents.SortMode = SortMode.DESC;
            //spContents.OrderByString = ETaxisTypeUtils.GetOrderByString(tableStyle, ETaxisType.OrderByTaxisDesc);
            SpContents.OrderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisTypeUtils.GetEnumType(_nodeInfo.Additional.DefaultTaxisType));
            SpContents.TotalCount = _nodeInfo.ContentNum;

            if (IsPostBack) return;

            LtlButtons.Text = WebUtils.GetContentCommands(Body.AdminName, PublishmentSystemInfo, _nodeInfo, PageUrl);
            LtlMoreButtons.Text = WebUtils.GetContentMoreCommands(Body.AdminName, PublishmentSystemInfo, _nodeInfo, PageUrl);

            SpContents.DataBind();

            if (_styleInfoList != null)
            {
                foreach (var styleInfo in _styleInfoList)
                {
                    var listitem = new ListItem(styleInfo.DisplayName, styleInfo.AttributeName);
                    DdlSearchType.Items.Add(listitem);
                }
            }

            //添加隐藏属性
            DdlSearchType.Items.Add(new ListItem("内容ID", ContentAttribute.Id));
            DdlSearchType.Items.Add(new ListItem("添加者", ContentAttribute.AddUserName));
            DdlSearchType.Items.Add(new ListItem("最后修改者", ContentAttribute.LastEditUserName));
            DdlSearchType.Items.Add(new ListItem("内容组", ContentAttribute.ContentGroupNameCollection));

            if (Body.IsQueryExists("searchType"))
            {
                TbDateFrom.Text = Body.GetQueryString("dateFrom");
                ControlUtils.SelectSingleItem(DdlSearchType, Body.GetQueryString("searchType"));
                TbKeyword.Text = Body.GetQueryString("keyword");
                if (!string.IsNullOrEmpty(Body.GetQueryString("searchType")) || !string.IsNullOrEmpty(TbDateFrom.Text) ||
                    !string.IsNullOrEmpty(TbKeyword.Text))
                {
                    LtlButtons.Text += @"
<script>
$(document).ready(function() {
	$('#contentSearch').show();
});
</script>
";
                }
                
            }

            LtlColumnsHead.Text = TextUtility.GetColumnsHeadHtml(_styleInfoList, _attributesOfDisplay, PublishmentSystemInfo);
            LtlCommandsHead.Text = TextUtility.GetCommandsHeadHtml(PublishmentSystemInfo, _pluginChannels, _isEdit);
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var contentInfo = new ContentInfo(e.Item.DataItem);

            var ltlTitle = (Literal)e.Item.FindControl("ltlTitle");
            var ltlColumns = (Literal)e.Item.FindControl("ltlColumns");
            var ltlStatus = (Literal)e.Item.FindControl("ltlStatus");
            var ltlCommands = (Literal)e.Item.FindControl("ltlCommands");
            var ltlSelect = (Literal)e.Item.FindControl("ltlSelect");

            ltlTitle.Text = WebUtils.GetContentTitle(PublishmentSystemInfo, contentInfo, PageUrl);

            ltlColumns.Text = TextUtility.GetColumnsHtml(_nameValueCacheDict, PublishmentSystemInfo, contentInfo, _attributesOfDisplay, _attributesOfDisplayStyleInfoList);

            ltlStatus.Text =
                $@"<a href=""javascript:;"" title=""设置内容状态"" onclick=""{ModalCheckState.GetOpenWindowString(PublishmentSystemId, contentInfo, PageUrl)}"">{CheckManager.GetCheckState(PublishmentSystemInfo, contentInfo.IsChecked, contentInfo.CheckedLevel)}</a>";

            ltlCommands.Text = TextUtility.GetCommandsHtml(PublishmentSystemInfo, _pluginChannels, contentInfo, PageUrl, Body.AdminName, _isEdit);

            ltlSelect.Text = $@"<input type=""checkbox"" name=""ContentIDCollection"" value=""{contentInfo.Id}"" />";
        }

        public void Search_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageUrl);
        }

        private string _pageUrl;
        private string PageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_pageUrl))
                {
                    _pageUrl = PageUtils.GetCmsUrl(nameof(PageContent), new NameValueCollection
                    {
                        {"publishmentSystemId", PublishmentSystemId.ToString()},
                        {"nodeId", _nodeInfo.NodeId.ToString()},
                        {"dateFrom", TbDateFrom.Text},
                        {"searchType", DdlSearchType.SelectedValue},
                        {"keyword", TbKeyword.Text},
                        {"page", Body.GetQueryInt("page", 1).ToString()}
                    });
                }
                return _pageUrl;
            }
        }
    }
}
