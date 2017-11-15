﻿using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageConfigurationCreate : BasePageCms
    {
        public DropDownList DdlIsCreateContentIfContentChanged;
        public DropDownList DdlIsCreateChannelIfChannelChanged;
        public DropDownList DdlIsCreateShowPageInfo;
        public DropDownList DdlIsCreateIe8Compatible;
        public DropDownList DdlIsCreateBrowserNoCache;
        public DropDownList DdlIsCreateJsIgnoreError;
        public DropDownList DdlIsCreateSearchDuplicate;
        public DropDownList DdlIsCreateWithJQuery;
        public DropDownList DdlIsCreateDoubleClick;
        public TextBox TbCreateStaticMaxPage;
        public DropDownList DdlIsCreateStaticContentByAddDate;
        public PlaceHolder PhIsCreateStaticContentByAddDate;
        public DateTimeTextBox TbCreateStaticContentAddDate;

        public DropDownList IsCreateMultiThread; // 是否启用多线程生成页面

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

			if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdCreate, "页面生成设置", AppManager.Permissions.WebSite.Create);

                EBooleanUtils.AddListItems(DdlIsCreateContentIfContentChanged, "生成", "不生成");
                ControlUtils.SelectListItemsIgnoreCase(DdlIsCreateContentIfContentChanged, PublishmentSystemInfo.Additional.IsCreateContentIfContentChanged.ToString());

                EBooleanUtils.AddListItems(DdlIsCreateChannelIfChannelChanged, "生成", "不生成");
                ControlUtils.SelectListItemsIgnoreCase(DdlIsCreateChannelIfChannelChanged, PublishmentSystemInfo.Additional.IsCreateChannelIfChannelChanged.ToString());

                EBooleanUtils.AddListItems(DdlIsCreateShowPageInfo, "显示", "不显示");
                ControlUtils.SelectListItemsIgnoreCase(DdlIsCreateShowPageInfo, PublishmentSystemInfo.Additional.IsCreateShowPageInfo.ToString());

                EBooleanUtils.AddListItems(DdlIsCreateIe8Compatible, "强制兼容", "不设置");
                ControlUtils.SelectListItemsIgnoreCase(DdlIsCreateIe8Compatible, PublishmentSystemInfo.Additional.IsCreateIe8Compatible.ToString());

                EBooleanUtils.AddListItems(DdlIsCreateBrowserNoCache, "强制清除缓存", "不设置");
                ControlUtils.SelectListItemsIgnoreCase(DdlIsCreateBrowserNoCache, PublishmentSystemInfo.Additional.IsCreateBrowserNoCache.ToString());

                EBooleanUtils.AddListItems(DdlIsCreateJsIgnoreError, "包含JS容错代码", "不设置");
                ControlUtils.SelectListItemsIgnoreCase(DdlIsCreateJsIgnoreError, PublishmentSystemInfo.Additional.IsCreateJsIgnoreError.ToString());

                EBooleanUtils.AddListItems(DdlIsCreateSearchDuplicate, "包含", "不包含");
                ControlUtils.SelectListItemsIgnoreCase(DdlIsCreateSearchDuplicate, PublishmentSystemInfo.Additional.IsCreateSearchDuplicate.ToString());

                EBooleanUtils.AddListItems(DdlIsCreateWithJQuery, "是", "否");
                ControlUtils.SelectListItemsIgnoreCase(DdlIsCreateWithJQuery, PublishmentSystemInfo.Additional.IsCreateWithJQuery.ToString());

                EBooleanUtils.AddListItems(DdlIsCreateDoubleClick, "启用双击生成", "不启用");
                ControlUtils.SelectListItemsIgnoreCase(DdlIsCreateDoubleClick, PublishmentSystemInfo.Additional.IsCreateDoubleClick.ToString());

                TbCreateStaticMaxPage.Text = PublishmentSystemInfo.Additional.CreateStaticMaxPage.ToString();

                EBooleanUtils.AddListItems(DdlIsCreateStaticContentByAddDate, "启用", "不启用");
                ControlUtils.SelectListItemsIgnoreCase(DdlIsCreateStaticContentByAddDate, PublishmentSystemInfo.Additional.IsCreateStaticContentByAddDate.ToString());
                PhIsCreateStaticContentByAddDate.Visible = PublishmentSystemInfo.Additional.IsCreateStaticContentByAddDate;
                if (PublishmentSystemInfo.Additional.CreateStaticContentAddDate != DateTime.MinValue)
                {
                    TbCreateStaticContentAddDate.DateTime = PublishmentSystemInfo.Additional.CreateStaticContentAddDate;
                }

                EBooleanUtils.AddListItems(IsCreateMultiThread, "启用", "不启用");
                ControlUtils.SelectListItemsIgnoreCase(IsCreateMultiThread, PublishmentSystemInfo.Additional.IsCreateMultiThread.ToString());

            }
        }

        public void DdlIsCreateStaticContentByAddDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhIsCreateStaticContentByAddDate.Visible = TranslateUtils.ToBool(DdlIsCreateStaticContentByAddDate.SelectedValue);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                PublishmentSystemInfo.Additional.IsCreateContentIfContentChanged = TranslateUtils.ToBool(DdlIsCreateContentIfContentChanged.SelectedValue);
                PublishmentSystemInfo.Additional.IsCreateChannelIfChannelChanged = TranslateUtils.ToBool(DdlIsCreateChannelIfChannelChanged.SelectedValue);

                PublishmentSystemInfo.Additional.IsCreateShowPageInfo = TranslateUtils.ToBool(DdlIsCreateShowPageInfo.SelectedValue);
                PublishmentSystemInfo.Additional.IsCreateIe8Compatible = TranslateUtils.ToBool(DdlIsCreateIe8Compatible.SelectedValue);
                PublishmentSystemInfo.Additional.IsCreateBrowserNoCache = TranslateUtils.ToBool(DdlIsCreateBrowserNoCache.SelectedValue);
                PublishmentSystemInfo.Additional.IsCreateJsIgnoreError = TranslateUtils.ToBool(DdlIsCreateJsIgnoreError.SelectedValue);
                PublishmentSystemInfo.Additional.IsCreateSearchDuplicate = TranslateUtils.ToBool(DdlIsCreateSearchDuplicate.SelectedValue);
                PublishmentSystemInfo.Additional.IsCreateWithJQuery = TranslateUtils.ToBool(DdlIsCreateWithJQuery.SelectedValue);

                PublishmentSystemInfo.Additional.IsCreateDoubleClick = TranslateUtils.ToBool(DdlIsCreateDoubleClick.SelectedValue);
                PublishmentSystemInfo.Additional.CreateStaticMaxPage = TranslateUtils.ToInt(TbCreateStaticMaxPage.Text);

                PublishmentSystemInfo.Additional.IsCreateStaticContentByAddDate = TranslateUtils.ToBool(DdlIsCreateStaticContentByAddDate.SelectedValue);
                if (PublishmentSystemInfo.Additional.IsCreateStaticContentByAddDate)
                {
                    PublishmentSystemInfo.Additional.CreateStaticContentAddDate = TbCreateStaticContentAddDate.DateTime;
                } 

                PublishmentSystemInfo.Additional.IsCreateMultiThread = TranslateUtils.ToBool(IsCreateMultiThread.SelectedValue);

                try
				{
                    DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);

                    Body.AddSiteLog(PublishmentSystemId, "修改页面生成设置");

                    SuccessMessage("页面生成设置修改成功！");
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "页面生成设置修改失败！");
				}
			}
		}
	}
}
