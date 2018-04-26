﻿using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model36
{
    public partial class BairongRoles
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("roleName")]
        public string RoleName { get; set; }

        [JsonProperty("modules")]
        public string Modules { get; set; }

        [JsonProperty("creatorUserName")]
        public string CreatorUserName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public partial class BairongRoles
    {
        public static readonly string NewTableName = DataProvider.RoleDao.TableName;

        public static readonly List<TableColumnInfo> NewColumns = DataProvider.RoleDao.TableColumns;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
