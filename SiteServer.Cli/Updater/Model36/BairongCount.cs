﻿using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model36
{
    public partial class BairongCount
    {
        [JsonProperty("countID")]
        public long CountId { get; set; }

        [JsonProperty("applicationName")]
        public string ApplicationName { get; set; }

        [JsonProperty("relatedTableName")]
        public string RelatedTableName { get; set; }

        [JsonProperty("relatedIdentity")]
        public string RelatedIdentity { get; set; }

        [JsonProperty("countType")]
        public string CountType { get; set; }

        [JsonProperty("countNum")]
        public long CountNum { get; set; }
    }

    public partial class BairongCount
    {
        public static readonly string NewTableName = DataProvider.CountDao.TableName;

        public static readonly List<TableColumnInfo> NewColumns = DataProvider.CountDao.TableColumns;

        public static readonly Dictionary<string, string> ConvertDict =
            new Dictionary<string, string>
            {
                {nameof(CountInfo.Id), nameof(CountId)}
            };
    }
}
