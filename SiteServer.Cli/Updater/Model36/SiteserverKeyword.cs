﻿using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model36
{
    public partial class SiteserverKeyword
    {
        [JsonProperty("keywordID")]
        public long KeywordId { get; set; }

        [JsonProperty("keyword")]
        public string Keyword { get; set; }

        [JsonProperty("alternative")]
        public string Alternative { get; set; }

        [JsonProperty("grade")]
        public string Grade { get; set; }
    }

    public partial class SiteserverKeyword
    {
        public static readonly string NewTableName = DataProvider.KeywordDao.TableName;

        public static readonly List<TableColumnInfo> NewColumns = DataProvider.KeywordDao.TableColumns;

        public static readonly Dictionary<string, string> ConvertDict =
            new Dictionary<string, string>
            {
                {nameof(KeywordInfo.Id), nameof(KeywordId)}
            };
    }
}
