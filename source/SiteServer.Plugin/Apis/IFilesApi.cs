﻿using System.Collections.Generic;

namespace SiteServer.Plugin.Apis
{
    public interface IFilesApi
    {
        void MoveFiles(int sourcePublishmentSystemId, int targetPublishmentSystemId, List<string> relatedUrls);

        void AddWaterMark(int publishmentSystemId, string filePath);

        string GetUploadFilePath(int publishmentSystemId, string relatedPath);

        string GetTemporaryFilesPath(string relatedPath);

        string GetPluginPath(string relatedPath);

        string GetPluginUrl(string relatedUrl = "");

        string GetApiJsonUrl(string action = "", string id = "");

        string GetApiHttpUrl(string action = "", string id = "");

        string GetPublishmentSystemUrl(int publishmentSystemId);

        string GetPublishmentSystemUrlByFilePath(string filePath);

        string GetChannelUrl(int publishmentSystemId, int channelId);

        string GetContentUrl(int publishmentSystemId, int channelId, int contentId);

        string GetRootUrl(string relatedUrl);
    }
}
