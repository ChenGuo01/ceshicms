using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using SiteServer.Plugin.Models;

namespace BaiRong.Core.Provider
{
    public class RecordDao : DataProviderBase
    {
        public override string TableName => "bairong_Record";

        public override List<TableColumnInfo> TableColumns => new List<TableColumnInfo>
        {
            new TableColumnInfo
            {
                ColumnName = "Id",
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumnInfo
            {
                ColumnName = "Text",
                DataType = DataType.VarChar,
                Length = 2000
            },
            new TableColumnInfo
            {
                ColumnName = "Summary",
                DataType = DataType.VarChar,
                Length = 2000
            },
            new TableColumnInfo
            {
                ColumnName = "Source",
                DataType = DataType.VarChar,
                Length = 200
            },
            new TableColumnInfo
            {
                ColumnName = "AddDate",
                DataType = DataType.DateTime
            }
        };

        private const string ParmText = "@Text";
        private const string ParmSummary = "@Summary";
        private const string ParmSource = "@Source";
        private const string ParmAddDate = "@AddDate";

        private void Insert(string text, string summary, string source)
        {
            var sqlString = $"INSERT INTO {TableName} (Text, Summary, Source, AddDate) VALUES (@Text, @Summary, @Source, @AddDate)";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmText, DataType.VarChar, 2000, text),
                GetParameter(ParmSummary, DataType.VarChar, 2000, summary),
                GetParameter(ParmSource, DataType.VarChar, 200, source),
                GetParameter(ParmAddDate, DataType.DateTime, DateTime.Now)
            };

            ExecuteNonQuery(sqlString, parms);
        }

        public void Delete(List<int> idList)
        {
            if (idList != null && idList.Count > 0)
            {
                var sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

                ExecuteNonQuery(sqlString);
            }
        }

        public void DeleteAll()
        {
            var sqlString = $"DELETE FROM {TableName}";

            ExecuteNonQuery(sqlString);
        }

        public string GetSelectCommend()
        {
            return $"SELECT Id, Text, Summary, Source, AddDate FROM {TableName}";
        }

        public string GetSelectCommend(string keyword, string dateFrom, string dateTo)
        {
            if (string.IsNullOrEmpty(keyword) && string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo))
            {
                return GetSelectCommend();
            }

            var whereString = new StringBuilder("WHERE ");

            var isWhere = false;

            if (!string.IsNullOrEmpty(keyword))
            {
                isWhere = true;
                var filterKeyword = PageUtils.FilterSql(keyword);
                whereString.Append(
                    $"(Text LIKE '%{filterKeyword}%' OR Summary LIKE '%{filterKeyword}%' OR Source LIKE '%{filterKeyword}%')");
            }

            if (!string.IsNullOrEmpty(dateFrom))
            {
                if (isWhere)
                {
                    whereString.Append(" AND ");
                }
                isWhere = true;
                whereString.Append($"(AddDate >= '{dateFrom}')");
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                if (isWhere)
                {
                    whereString.Append(" AND ");
                }
                whereString.Append($"(AddDate <= '{dateTo}')");
            }

            return $"SELECT Id, Text, Summary, Source, AddDate FROM {TableName} {whereString}";
        }

        private static readonly object LockObject = new object();

        public bool IsRecord()
        {
#if (DEBUG)
            const string cacheKey = "BaiRong.Core.Provider.RecordDao.IsRecord";
            var retval = CacheUtils.GetInt(cacheKey, -1);
            if (retval != -1) return retval == 1;

            lock (LockObject)
            {
                retval = CacheUtils.GetInt(cacheKey, -1);
                if (retval == -1)
                {
                    retval = FileUtils.IsFileExists(PathUtils.GetTemporaryFilesPath("record.txt")) ? 1 : 0;
                    CacheUtils.Insert(cacheKey, retval);
                }
            }

            return retval == 1;
#else
            return false;
#endif
        }

        public void RecordCommandExecute(IDbCommand command, string source)
        {
            if (!IsRecord()) return;
            if (command.CommandText.Contains(TableName)) return;

            var builder = new StringBuilder();
            foreach (var parameter in command.Parameters)
            {
                var commandParameter = parameter as IDataParameter;
                if (commandParameter != null)
                {
                    builder.Append(commandParameter.ParameterName + "=" + commandParameter.Value + "<br />").AppendLine();
                }
            }

            Insert(command.CommandText, builder.ToString(), source);
        }
    }
}
