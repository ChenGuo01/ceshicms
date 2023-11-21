﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerVideoUploadController : ControllerBase
    {
        private const string Route = "cms/material/layerVideoUpload";
        private const string RouteUpload = "cms/material/layerVideoUpload/actions/upload";

        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IMaterialVideoRepository _materialVideoRepository;

        public LayerVideoUploadController(IPathManager pathManager, ISiteRepository siteRepository, IMaterialVideoRepository materialVideoRepository)
        {
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _materialVideoRepository = materialVideoRepository;
        }

        public class Options
        {
            public bool IsEditor { get; set; }
            public bool IsLibrary { get; set; }
            public bool IsThumb { get; set; }
            public int ThumbWidth { get; set; }
            public int ThumbHeight { get; set; }
            public bool IsLinkToOriginal { get; set; }
        }

        public class SubmitRequest : Options
        {
            public int SiteId { get; set; }
            public List<string> FilePaths { get; set; }
        }

        public class UploadResult
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }

        public class SubmitResult
        {
            public string ImageUrl { get; set; }
            public string PreviewUrl { get; set; }
        }
    }
}
