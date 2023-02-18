﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class LoginController
    {
        [HttpGet, Route(Route)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var redirectUrl = await AdminRedirectCheckAsync();
            if (!string.IsNullOrEmpty(redirectUrl))
            {
                return new GetResult
                {
                    Success = false,
                    RedirectUrl = redirectUrl
                };
            }

            var config = await _configRepository.GetAsync();
            var smsSettings = await _smsManager.GetSmsSettingsAsync();
            var isSmsAdmin = smsSettings.IsSms && smsSettings.IsSmsAdmin;
            var isSmsAdminAndDisableAccount = isSmsAdmin && smsSettings.IsSmsAdminAndDisableAccount;

            return new GetResult
            {
                Success = true,
                Version = _settingsManager.Version,
                AdminFaviconUrl = config.AdminFaviconUrl,
                AdminTitle = config.AdminTitle,
                IsSmsAdmin = isSmsAdmin,
                IsSmsAdminAndDisableAccount = isSmsAdminAndDisableAccount
            };
        }
    }
}
