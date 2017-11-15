﻿using System.Collections.Generic;
using System.Data;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using SiteServer.Plugin.Models;

namespace BaiRong.Core.Provider
{
    public class AdministratorsInRolesDao : DataProviderBase
    {
        public override string TableName => "bairong_AdministratorsInRoles";

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
                ColumnName = "RoleName",
                DataType = DataType.VarChar,
                Length = 255
            },
            new TableColumnInfo
            {
                ColumnName = "UserName",
                DataType = DataType.VarChar,
                Length = 255
            }
        };

        public string[] GetRolesForUser(string userName)
        {
            var tmpRoleNames = string.Empty;
            var sqlString = "SELECT RoleName FROM bairong_AdministratorsInRoles WHERE UserName = @UserName ORDER BY RoleName";
            var parms = new IDataParameter[]
            {
                GetParameter("@UserName", DataType.VarChar, 255, userName)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                while (rdr.Read())
                {
                    tmpRoleNames += GetString(rdr, 0) + ",";
                }
                rdr.Close();
            }

            if (tmpRoleNames.Length > 0)
            {
                tmpRoleNames = tmpRoleNames.Substring(0, tmpRoleNames.Length - 1);
                return tmpRoleNames.Split(',');
            }

            return new string[0];
        }

        public string[] GetUsersInRole(string roleName)
        {
            var tmpUserNames = string.Empty;
            var sqlString = "SELECT UserName FROM bairong_AdministratorsInRoles WHERE RoleName = @RoleName ORDER BY userName";
            var parms = new IDataParameter[]
            {
                GetParameter("@RoleName", DataType.VarChar, 255, roleName)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                while (rdr.Read())
                {
                    tmpUserNames += GetString(rdr, 0) + ",";
                }
                rdr.Close();
            }

            if (tmpUserNames.Length > 0)
            {
                tmpUserNames = tmpUserNames.Substring(0, tmpUserNames.Length - 1);
                return tmpUserNames.Split(',');
            }

            return new string[0];
        }

        public void RemoveUserFromRoles(string userName, string[] roleNames)
        {
            var sqlString = "DELETE FROM bairong_AdministratorsInRoles WHERE UserName = @UserName AND RoleName = @RoleName";
            foreach (var roleName in roleNames)
            {
                var parms = new IDataParameter[]
                {
                    GetParameter("@UserName", DataType.VarChar, 255, userName),
                    GetParameter("@RoleName", DataType.VarChar, 255, roleName)
                };
                ExecuteNonQuery(sqlString, parms);
            }
        }

        public void RemoveUserFromRole(string userName, string roleName)
        {
            var sqlString = "DELETE FROM bairong_AdministratorsInRoles WHERE UserName = @UserName AND RoleName = @RoleName";
            var parms = new IDataParameter[]
            {
                GetParameter("@UserName", DataType.VarChar, 255, userName),
                GetParameter("@RoleName", DataType.VarChar, 255, roleName)
            };

            ExecuteNonQuery(sqlString, parms);
        }

        public string[] FindUsersInRole(string roleName, string userNameToMatch)
        {
            var tmpUserNames = string.Empty;
            string sqlString =
                $"SELECT UserName FROM bairong_AdministratorsInRoles WHERE RoleName = @RoleName AND UserName LIKE '%{PageUtils.FilterSql(userNameToMatch)}%'";

            var parms = new IDataParameter[]
            {
                GetParameter("@RoleName", DataType.VarChar, 255, roleName)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                while (rdr.Read())
                {
                    tmpUserNames += GetString(rdr, 0) + ",";
                }
                rdr.Close();
            }

            if (tmpUserNames.Length > 0)
            {
                tmpUserNames = tmpUserNames.Substring(0, tmpUserNames.Length - 1);
                return tmpUserNames.Split(',');
            }

            return new string[0];
        }

        public bool IsUserInRole(string userName, string roleName)
        {
            var isUserInRole = false;
            const string sqlString = "SELECT * FROM bairong_AdministratorsInRoles WHERE UserName = @UserName AND RoleName = @RoleName";
            var parms = new IDataParameter[]
            {
                GetParameter("@UserName", DataType.VarChar, 255, userName),
                GetParameter("@RoleName", DataType.VarChar, 255, roleName)
            };
            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    if (!rdr.IsDBNull(0))
                    {
                        isUserInRole = true;
                    }
                }
                rdr.Close();
            }
            return isUserInRole;
        }

        public void AddUserToRoles(string userName, string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                AddUserToRole(userName, roleName);
            }
        }

        public void AddUserToRole(string userName, string roleName)
        {
            if (!BaiRongDataProvider.RoleDao.IsRoleExists(roleName)) return;
            if (!BaiRongDataProvider.AdministratorDao.IsUserNameExists(userName)) return;
            if (!IsUserInRole(userName, roleName))
            {
                var sqlString = "INSERT INTO bairong_AdministratorsInRoles (UserName, RoleName) VALUES (@UserName, @RoleName)";

                var parms = new IDataParameter[]
                {
                    GetParameter("@UserName", DataType.VarChar, 255, userName),
                    GetParameter("@RoleName", DataType.VarChar, 255, roleName)
                };

                ExecuteNonQuery(sqlString, parms);
            }
        }
    }
}
