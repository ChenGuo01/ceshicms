﻿<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageConfigurationUploadVideo" %>
  <%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <!--#include file="../inc/openWindow.html"-->

      <form class="container" runat="server">
        <bairong:alerts runat="server" />

        <div class="raw">
          <div class="card-box">
            <h4 class="m-t-0 header-title">
              <b>视频上传设置</b>
            </h4>
            <p class="text-muted font-13 m-b-25">
              在此设置站点的视频上传功能选项
            </p>

            <ul class="nav nav-pills m-b-30">
              <li class="">
                <a href="pageConfigurationUploadImage.aspx?publishmentSystemId=<%=PublishmentSystemId%>">图片上传设置</a>
              </li>
              <li class="active">
                <a href="javascript:;">视频上传设置</a>
              </li>
              <li class="">
                <a href="pageConfigurationUploadFile.aspx?publishmentSystemId=<%=PublishmentSystemId%>">附件上传设置</a>
              </li>
              <li class="">
                <a href="pageConfigurationWatermark.aspx?publishmentSystemId=<%=PublishmentSystemId%>">图片水印设置</a>
              </li>
            </ul>

            <div class="form-horizontal">

              <div class="form-group">
                <label class="col-sm-3 control-label">视频上传文件夹</label>
                <div class="col-sm-3">
                  <asp:TextBox class="form-control" MaxLength="50" id="TbVideoUploadDirectoryName" runat="server" />
                </div>
                <div class="col-sm-6">
                  <asp:RequiredFieldValidator ControlToValidate="TbVideoUploadDirectoryName" errorMessage=" *" foreColor="red" display="Dynamic"
                    runat="server" />
                  <asp:RegularExpressionValidator runat="server" ControlToValidate="TbVideoUploadDirectoryName" ValidationExpression="[^']+"
                    errorMessage=" *" foreColor="red" display="Dynamic" />
                </div>
              </div>
              <div class="form-group">
                <label class="col-sm-3 control-label">视频上传保存方式</label>
                <div class="col-sm-3">
                  <asp:DropDownList ID="DdlVideoUploadDateFormatString" class="form-control" runat="server"></asp:DropDownList>
                </div>
                <div class="col-sm-6">
                  <span class="help-block">
                    本设置只影响新上传的视频, 设置更改之前的视频仍存放在原来位置
                  </span>
                </div>
              </div>
              <div class="form-group">
                <label class="col-sm-3 control-label">是否按时间重命名上传的视频</label>
                <div class="col-sm-3">
                  <asp:DropDownList ID="DdlIsVideoUploadChangeFileName" class="form-control" runat="server"></asp:DropDownList>
                </div>
                <div class="col-sm-6">
                  <span class="help-block">
                    本设置只影响新上传的视频, 设置更改之前的视频名仍保持不变
                  </span>
                </div>
              </div>
              <div class="form-group">
                <label class="col-sm-3 control-label">上传视频类型</label>
                <div class="col-sm-3">
                  <asp:TextBox TextMode="MultiLine" class="form-control" Height="100" id="TbVideoUploadTypeCollection" runat="server" />
                </div>
                <div class="col-sm-6">
                  <asp:RegularExpressionValidator runat="server" ControlToValidate="TbVideoUploadTypeCollection" ValidationExpression="[^']+"
                    errorMessage=" *" foreColor="red" display="Dynamic" />
                  <span class="help-block">类型之间用“,”分割</span>
                </div>
              </div>
              <div class="form-group">
                <label class="col-sm-3 control-label">上传视频最大大小</label>
                <div class="col-sm-2">
                  <asp:TextBox class="form-control" Columns="10" MaxLength="50" id="TbVideoUploadTypeMaxSize" runat="server" />
                </div>
                <div class="col-sm-1">
                  <asp:DropDownList id="DdlVideoUploadTypeUnit" class="form-control" runat="server">
                    <asp:ListItem Value="KB" Text="KB" Selected="true"></asp:ListItem>
                    <asp:ListItem Value="MB" Text="MB"></asp:ListItem>
                  </asp:DropDownList>
                </div>
                <div class="col-sm-6">
                  <asp:RequiredFieldValidator ControlToValidate="TbVideoUploadTypeMaxSize" errorMessage=" *" foreColor="red" display="Dynamic"
                    runat="server" />
                  <asp:RegularExpressionValidator ControlToValidate="TbVideoUploadTypeMaxSize" ValidationExpression="\d+" Display="Dynamic"
                    ErrorMessage="上传视频最大大小必须为整数" foreColor="red" runat="server" />
                </div>
              </div>

              <hr />

              <div class="form-group m-b-0">
                <div class="col-sm-offset-3 col-sm-9">
                  <asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server" />
                </div>
              </div>

            </div>
          </div>

        </div>

        </div>
        </div>

      </form>
    </body>

    </html>