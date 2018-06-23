﻿using System;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Api;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages
{
    public class PageLogin : BasePage
	{
        public Literal LtlMessage;
        public TextBox TbAccount;
        public TextBox TbPassword;
        public TextBox TbValidateCode;
        public Literal LtlValidateCodeImage;
	    public PlaceHolder PhFindPassword;

        private VcManager _vcManager; // 验证码类

        protected override bool IsAccessable => true; // 设置本页面是否能直接访问 如果为false，则必须管理员登录后才能访问

        public void Page_Load(object sender, EventArgs e)
		{
            if (IsForbidden) return; // 如果无权访问页面，则返回空白页

            try
            {
                _vcManager = VcManager.GetInstance(); // 构建验证码实例
                if (Page.IsPostBack) return;

                PhFindPassword.Visible = ConfigManager.SystemConfigInfo.IsAdminFindPassword;

                if (AuthRequest.IsQueryExists("error")) // 如果url参数error不为空，则把错误信息显示到页面上
                {
                    LtlMessage.Text = GetMessageHtml(AuthRequest.GetQueryString("error"));
                }

                LtlValidateCodeImage.Text =
                        $@"<a href=""javascript:;"" onclick=""$('#imgVerify').attr('src', $('#imgVerify').attr('src') + '&' + new Date().getTime())""><img id=""imgVerify"" name=""imgVerify"" src=""{PageValidateCode.GetRedirectUrl(_vcManager.GetCookieName())}"" align=""absmiddle"" /></a>";
            }
            catch
            {
                // 再次探测是否需要安装或升级
                if (SystemManager.IsNeedInstall())
                {
                    PageUtils.Redirect(PageInstaller.GetRedirectUrl());
                }
                else if (SystemManager.IsNeedUpdate())
                {
                    PageUtils.Redirect(PageSyncDatabase.GetRedirectUrl());
                }
                else
                {
                    throw;
                }
            }
		}
		
		public override void Submit_OnClick(object sender, EventArgs e)
		{
            var account = TbAccount.Text;
            var password = TbPassword.Text;

            if (!_vcManager.IsCodeValid(TbValidateCode.Text)) // 检测验证码是否正确
            {
                LtlMessage.Text = GetMessageHtml("验证码不正确，请重新输入！");
                return;
            }

            string userName;
            string errorMessage;
            if (!DataProvider.AdministratorDao.Validate(account, password, false, out userName, out errorMessage)) // 检测密码是否正确
            {
                LogUtils.AddAdminLog(userName, "后台管理员登录失败");
                DataProvider.AdministratorDao.UpdateLastActivityDateAndCountOfFailedLogin(userName); // 记录最后登录时间、失败次数+1
                LtlMessage.Text = GetMessageHtml(errorMessage); // 把错误信息显示在页面上
                return;
            }

            DataProvider.AdministratorDao.UpdateLastActivityDateAndCountOfLogin(userName); // 记录最后登录时间、失败次数清零
            AuthRequest.AdminLogin(userName); // 写Cookie并记录管理员操作日志
		    PageUtils.Redirect(PageInitialization.GetRedirectUrl()); // 跳转到系统初始化页面
        }

        private string GetMessageHtml(string message) => $@"<div class=""alert alert-danger"">{message}</div>";
	}
}
