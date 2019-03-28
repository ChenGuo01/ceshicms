using System.Data;
using SiteServer.CMS.Apis;
using SiteServer.CMS.Caches.Core;

namespace SiteServer.CMS.Caches.Stl
{
    public static class StlDatabaseCache
    {
        private static readonly object LockObject = new object();

        public static int GetPageTotalCount(string sqlString)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlDatabaseCache), nameof(GetPageTotalCount),
                    sqlString);
            var retVal = StlCacheManager.GetInt(cacheKey);
            if (retVal != -1) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.GetInt(cacheKey);
                if (retVal == -1)
                {
                    retVal = DatabaseApi.Instance.GetPageTotalCount(sqlString);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }

        public static string GetStlPageSqlString(string sqlString, string orderByString, int totalNum, int pageNum, int currentPageIndex)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlDatabaseCache), nameof(GetStlPageSqlString),
                       sqlString, orderByString, totalNum.ToString(), pageNum.ToString(), currentPageIndex.ToString());
            var retVal = StlCacheManager.Get<string>(cacheKey);
            if (retVal != null) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.Get<string>(cacheKey);
                if (retVal == null)
                {
                    retVal = DatabaseApi.Instance.GetStlPageSqlString(sqlString, orderByString, totalNum, pageNum,
                    currentPageIndex);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }

        public static string GetString(string connectionString, string queryString)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlDatabaseCache), nameof(GetString),
                       connectionString, queryString);
            var retVal = StlCacheManager.Get<string>(cacheKey);
            if (retVal != null) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.Get<string>(cacheKey);
                if (retVal == null)
                {
                    retVal = DatabaseApi.Instance.GetString(connectionString, queryString);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }

        public static DataSet GetDataSet(string connectionString, string queryString)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlDatabaseCache), nameof(GetDataSet),
                connectionString, queryString);
            var retVal = StlCacheManager.Get<DataSet>(cacheKey);
            if (retVal != null) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.Get<DataSet>(cacheKey);
                if (retVal == null)
                {
                    retVal = DatabaseApi.Instance.GetDataSet(connectionString, queryString);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }

        public static DataTable GetDataTable(string connectionString, string queryString)
        {
            var cacheKey = StlCacheManager.GetCacheKey(nameof(StlDatabaseCache), nameof(GetDataTable),
                connectionString, queryString);
            var retVal = StlCacheManager.Get<DataTable>(cacheKey);
            if (retVal != null) return retVal;

            lock (LockObject)
            {
                retVal = StlCacheManager.Get<DataTable>(cacheKey);
                if (retVal == null)
                {
                    retVal = DatabaseApi.Instance.GetDataTable(connectionString, queryString);
                    StlCacheManager.Set(cacheKey, retVal);
                }
            }

            return retVal;
        }
    }
}
