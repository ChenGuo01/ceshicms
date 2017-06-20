﻿using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BaiRong.Core;
using SiteServer.CMS.Controllers;
using SiteServer.Plugin;

namespace SiteServer.CMS.Core.Plugin
{
    /// <summary>
    /// The entry for managing SiteServer plugins
    /// </summary>
    public static class PluginManager
    {
        /// <summary>
        /// Directories that will hold SiteServer plugin directory
        /// </summary>

        public static List<PluginPair> AllPlugins { get; private set; }

        private static void ValidateDirectory()
        {
            var pluginsPath = PathUtils.GetPluginsPath();
            if (!Directory.Exists(pluginsPath))
            {
                Directory.CreateDirectory(pluginsPath);
            }
        }

        static PluginManager()
        {
            ValidateDirectory();
        }

        public static void LoadPlugins()
        {
            var metadatas = PluginConfig.Parse();
            AllPlugins = PluginsLoader.Plugins(metadatas);

            Parallel.ForEach(AllPlugins, pair =>
            {
                var s = Stopwatch.StartNew();
                pair.Plugin.Init(new PluginInitContext(pair.Metadata, new PublicApiInstance(pair.Metadata)));
                s.Stop();

                var milliseconds = s.ElapsedMilliseconds;
                pair.Metadata.InitTime += milliseconds;
            });
        }

        public static void InstallPlugin(string path)
        {
            PluginInstaller.Install(path);
        }

        /// <summary>
        /// get specified plugin, return null if not found
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static PluginPair GetPluginForId(string id)
        {
            return AllPlugins.FirstOrDefault(o => o.Metadata.Id == id);
        }

        public static IEnumerable<PluginPair> GetPluginsForInterface<T>() where T : IFeatures
        {
            return AllPlugins.Where(pluginPair => pluginPair.Plugin is T).ToList();
        }

        private static PluginMenu GetMenu(string pluginId, PluginMenu metadataMenu, string apiUrl, int siteId, int i)
        {
            var menu = new PluginMenu
            {
                Id = metadataMenu.Id,
                TopId = metadataMenu.TopId,
                ParentId = metadataMenu.ParentId,
                Text = metadataMenu.Text,
                Href = metadataMenu.Href,
                Selected = metadataMenu.Selected,
                Target = metadataMenu.Target,
                IconUrl = metadataMenu.IconUrl
            };

            if (string.IsNullOrEmpty(menu.Id))
            {
                menu.Id = pluginId + i;
            }
            if (!string.IsNullOrEmpty(menu.Href))
            {
                if (!PageUtils.IsProtocolUrl(menu.Href) && !StringUtils.StartsWith(menu.Href, "/"))
                {
                    menu.Href = PageUtils.GetPluginUrl(pluginId, menu.Href);
                }
                menu.Href = PageUtils.AddQueryString(menu.Href, new NameValueCollection
                {
                    {"apiUrl", Plugins.GetUrl(apiUrl, pluginId, siteId)},
                    {"siteId", siteId.ToString()}
                });
            }
            if (!string.IsNullOrEmpty(menu.IconUrl))
            {
                menu.IconUrl = PageUtils.GetPluginUrl(pluginId, menu.IconUrl);
            }
            if (string.IsNullOrEmpty(menu.Target))
            {
                menu.Target = "right";
            }
            if (metadataMenu.Permissions != null && metadataMenu.Permissions.Count > 0)
            {
                menu.Permissions = new List<string>();
                foreach (var metadataMenuPermission in metadataMenu.Permissions)
                {
                    menu.Permissions.Add(pluginId + "_" + metadataMenuPermission);
                }
            }

            if (metadataMenu.Menus != null && metadataMenu.Menus.Count > 0)
            {
                var chlildren = new List<PluginMenu>();
                var x = 1;
                foreach (var childMetadataMenu in metadataMenu.Menus)
                {
                    var child = GetMenu(pluginId, childMetadataMenu, apiUrl, siteId, x++);

                    chlildren.Add(child);
                }
                menu.Menus = chlildren;
            }

            return menu;
        }

        public static List<PluginMenu> GetAllMenus(string topId, int siteId)
        {
            var plugins = AllPlugins.Where(o => !o.Metadata.Disabled && o.Metadata.Menus != null);

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(siteId);
            var apiUrl = PageUtility.GetApiUrl(publishmentSystemInfo);

            var menus = new List<PluginMenu>();
            foreach (var pluginPair in plugins)
            {
                var i = 1;
                foreach (var metadataMenu in pluginPair.Metadata.Menus)
                {
                    if (!StringUtils.EqualsIgnoreCase(metadataMenu.TopId, topId)) continue;

                    var menu = GetMenu(pluginPair.Metadata.Id, metadataMenu, apiUrl, siteId, i++);

                    menus.Add(menu);
                }
            }

            return menus;
        }
    }
}
