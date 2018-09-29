﻿using System.Collections.Generic;
using System.Linq;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.DataCache.Stl;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.DataCache
{
    public static class ContentManager
    {
        private static class ListCache
        {
            private static readonly string CachePrefix = DataCacheManager.GetCacheKey(nameof(ContentManager)) + "." + nameof(ListCache);

            private static string GetCacheKey(int channelId)
            {
                return $"{CachePrefix}.{channelId}";
            }

            public static void Remove(int channelId)
            {
                var cacheKey = GetCacheKey(channelId);
                DataCacheManager.Remove(cacheKey);
            }

            public static List<int> GetContentIdList(int channelId)
            {
                var cacheKey = GetCacheKey(channelId);
                var list = DataCacheManager.Get<List<int>>(cacheKey);
                if (list != null) return list;

                list = new List<int>();
                DataCacheManager.Insert(cacheKey, list);
                return list;
            }
        }

        private static class ContentCache
        {
            private static readonly string CachePrefix = DataCacheManager.GetCacheKey(nameof(ContentManager)) + "." + nameof(ContentCache);

            private static string GetCacheKey(int channelId)
            {
                return $"{CachePrefix}.{channelId}";
            }

            public static void Remove(int channelId)
            {
                var cacheKey = GetCacheKey(channelId);
                DataCacheManager.Remove(cacheKey);
            }

            public static Dictionary<int, ContentInfo> GetContentDict(int channelId)
            {
                var cacheKey = GetCacheKey(channelId);
                var dict = DataCacheManager.Get<Dictionary<int, ContentInfo>>(cacheKey);
                if (dict == null)
                {
                    dict = new Dictionary<int, ContentInfo>();
                    DataCacheManager.InsertHours(cacheKey, dict, 12);
                }

                return dict;
            }
        }

        private static class CountCache
        {
            private static readonly object LockObject = new object();

            private static readonly string CacheKey =
                DataCacheManager.GetCacheKey(nameof(ContentManager)) + "." + nameof(CountCache);

            public static void Clear(string tableName)
            {
                var dict = GetAllContentCounts();
                dict.Remove(tableName);
            }

            public static Dictionary<string, List<ContentCountInfo>> GetAllContentCounts()
            {
                var retval = DataCacheManager.Get<Dictionary<string, List<ContentCountInfo>>>(CacheKey);
                if (retval != null) return retval;

                lock (LockObject)
                {
                    retval = DataCacheManager.Get<Dictionary<string, List<ContentCountInfo>>>(CacheKey);
                    if (retval == null)
                    {
                        retval = new Dictionary<string, List<ContentCountInfo>>();
                        DataCacheManager.Insert(CacheKey, retval);
                    }
                }

                return retval;
            }
        }

        public static void RemoveCache(string tableName, int channelId)
        {
            ListCache.Remove(channelId);
            ContentCache.Remove(channelId);
            CountCache.Clear(tableName);
            StlContentCache.ClearCache();
        }

        public static void RemoveCountCache(string tableName)
        {
            CountCache.Clear(tableName);
            StlContentCache.ClearCache();
        }

        public static void InsertCache(SiteInfo siteInfo, ChannelInfo channelInfo, IContentInfo contentInfo)
        {
            if (contentInfo.SourceId == SourceManager.Preview) return;

            var contentIdList = ListCache.GetContentIdList(channelInfo.Id);

            if (ETaxisTypeUtils.Equals(ETaxisType.OrderByTaxisDesc, channelInfo.Additional.DefaultTaxisType))
            {
                contentIdList.Insert(0, contentInfo.Id);
            }
            else
            {
                ListCache.Remove(channelInfo.Id);
            }

            var dict = ContentCache.GetContentDict(channelInfo.Id);
            dict[contentInfo.Id] = (ContentInfo)contentInfo;

            var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
            var countInfoList = GetContentCountInfoList(tableName);
            var countInfo = countInfoList.FirstOrDefault(x =>
                x.SiteId == siteInfo.Id && x.ChannelId == channelInfo.Id &&
                x.IsChecked == contentInfo.IsChecked.ToString() && x.CheckedLevel == contentInfo.CheckedLevel);
            if (countInfo != null) countInfo.Count++;

            StlContentCache.ClearCache();
            CountCache.Clear(tableName);
        }

        public static void UpdateCache(SiteInfo siteInfo, ChannelInfo channelInfo, IContentInfo contentInfoToUpdate)
        {
            var dict = ContentCache.GetContentDict(channelInfo.Id);

            var contentInfo = GetContentInfo(siteInfo, channelInfo, contentInfoToUpdate.Id);
            if (contentInfo != null)
            {
                var isClearCache = contentInfo.IsTop != contentInfoToUpdate.IsTop;

                if (!isClearCache)
                {
                    var orderAttributeName =
                        ETaxisTypeUtils.GetContentOrderAttributeName(
                            ETaxisTypeUtils.GetEnumType(channelInfo.Additional.DefaultTaxisType));
                    if (contentInfo.Get(orderAttributeName) != contentInfoToUpdate.Get(orderAttributeName))
                    {
                        isClearCache = true;
                    }
                }

                if (isClearCache)
                {
                    ListCache.Remove(channelInfo.Id);
                }
            }

            
            dict[contentInfoToUpdate.Id] = (ContentInfo)contentInfoToUpdate;

            StlContentCache.ClearCache();
        }

        public static List<int> GetContentIdList(SiteInfo siteInfo, ChannelInfo channelInfo, int offset, int limit)
        {
            var list = ListCache.GetContentIdList(channelInfo.Id);
            if (list.Count >= offset + limit)
            {
                return list.Skip(offset).Take(limit).ToList();
            }

            if (list.Count == offset)
            {
                var dict = ContentCache.GetContentDict(channelInfo.Id);
                var pageContentInfoList = DataProvider.ContentDao.GetCacheContentInfoList(siteInfo, channelInfo, offset, limit);
                foreach (var contentInfo in pageContentInfoList)
                {
                    dict[contentInfo.Id] = contentInfo;
                }

                var pageContentIdList = pageContentInfoList.Select(x => x.Id).ToList();
                list.AddRange(pageContentIdList);
                return pageContentIdList;
            }

            return DataProvider.ContentDao.GetCacheContentIdList(siteInfo, channelInfo, offset, limit);
        }

        public static ContentInfo GetContentInfo(SiteInfo siteInfo, int channelId, int contentId)
        {
            var dict = ContentCache.GetContentDict(channelId);
            dict.TryGetValue(contentId, out var contentInfo);
            if (contentInfo != null) return contentInfo;

            contentInfo = DataProvider.ContentDao.GetCacheContentInfo(ChannelManager.GetTableName(siteInfo, channelId), contentId);
            dict[contentId] = contentInfo;

            return contentInfo;
        }

        public static ContentInfo GetContentInfo(SiteInfo siteInfo, ChannelInfo channelInfo, int contentId)
        {

            var dict = ContentCache.GetContentDict(channelInfo.Id);
            dict.TryGetValue(contentId, out var contentInfo);
            if (contentInfo != null) return contentInfo;

            contentInfo = DataProvider.ContentDao.GetCacheContentInfo(ChannelManager.GetTableName(siteInfo, channelInfo), contentId);
            dict[contentId] = contentInfo;

            return contentInfo;
        }

        public static int GetCount(SiteInfo siteInfo, bool isChecked)
        {
            var tableNames = SiteManager.GetTableNameList(siteInfo);
            var count = 0;
            foreach (var tableName in tableNames)
            {
                var list = GetContentCountInfoList(tableName);
                count += list.Where(x => x.SiteId == siteInfo.Id && x.IsChecked == isChecked.ToString()).Sum(x => x.Count);
            }

            return count;
        }

        public static int GetCount(SiteInfo siteInfo)
        {
            var tableNames = SiteManager.GetTableNameList(siteInfo);
            var count = 0;
            foreach (var tableName in tableNames)
            {
                var list = GetContentCountInfoList(tableName);
                count += list.Where(x => x.SiteId == siteInfo.Id).Sum(x => x.Count);
            }

            return count;
        }

        public static int GetCount(SiteInfo siteInfo, ChannelInfo channelInfo)
        {
            var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
            var list = GetContentCountInfoList(tableName);
            return list.Where(x => x.SiteId == siteInfo.Id && x.ChannelId == channelInfo.Id).Sum(x => x.Count);
        }

        public static int GetCount(SiteInfo siteInfo, ChannelInfo channelInfo, bool isChecked)
        {
            var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
            var list = GetContentCountInfoList(tableName);
            return list.Where(x => x.SiteId == siteInfo.Id && x.ChannelId == channelInfo.Id && x.IsChecked == isChecked.ToString()).Sum(x => x.Count);
        }

        private static List<ContentCountInfo> GetContentCountInfoList(string tableName)
        {
            var dict = CountCache.GetAllContentCounts();
            dict.TryGetValue(tableName, out var countList);
            if (countList != null) return countList;

            countList = DataProvider.ContentDao.GetTableContentCounts(tableName);
            dict[tableName] = countList;

            return countList;
        }
    }
}