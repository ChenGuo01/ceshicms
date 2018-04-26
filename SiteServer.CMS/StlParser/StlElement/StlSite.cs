﻿using System.Text;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Cache;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlClass(Usage = "获取站点值", Description = "通过 stl:site 标签在模板中显示站点值")]
    public class StlSite
	{
        private StlSite() { }
		public const string ElementName = "stl:site";

        private static readonly Attr SiteName = new Attr("siteName", "站点名称");
        private static readonly Attr SiteDir = new Attr("siteDir", "站点文件夹");

        //循环解析型标签
        internal static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
		{
			var parsedContent = string.Empty;

            if (!string.IsNullOrEmpty(contextInfo.InnerXml))
            {
                var siteName = string.Empty;
                var siteDir = string.Empty;

                foreach (var name in contextInfo.Attributes.Keys)
                {
                    var value = contextInfo.Attributes[name];

                    if (StringUtils.EqualsIgnoreCase(name, SiteName.Name))
                    {
                        siteName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                    }
                    else if (StringUtils.EqualsIgnoreCase(name, SiteDir.Name))
                    {
                        siteDir = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                    }
                }

                parsedContent = ParseImpl(pageInfo, contextInfo, siteName, siteDir);
            }

            return parsedContent;
		}

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string siteName, string siteDir)
        {
            SiteInfo siteInfo = null;

            if (!string.IsNullOrEmpty(siteName))
            {
                siteInfo = SiteManager.GetSiteInfoBySiteName(siteName);
            }
            else if (!string.IsNullOrEmpty(siteDir))
            {
                siteInfo = SiteManager.GetSiteInfoByDirectory(siteDir);
            }
            else
            {
                //var siteId = DataProvider.SiteDao.GetSiteIdByIsRoot();
                var siteId = Site.GetSiteIdByIsRoot();
                if (siteId > 0)
                {
                    siteInfo = SiteManager.GetSiteInfo(siteId);
                }
            }

            if (siteInfo == null) return string.Empty;

            var preSiteInfo = pageInfo.SiteInfo;
            var prePageChannelId = pageInfo.PageChannelId;
            var prePageContentId = pageInfo.PageContentId;

            pageInfo.ChangeSite(siteInfo, siteInfo.Id, 0, contextInfo);

            var innerBuilder = new StringBuilder(contextInfo.InnerXml);
            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
            var parsedContent = innerBuilder.ToString();

            pageInfo.ChangeSite(preSiteInfo, prePageChannelId, prePageContentId, contextInfo);

            return parsedContent;
        }
	}
}
