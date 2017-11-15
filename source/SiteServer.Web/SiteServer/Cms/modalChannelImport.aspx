﻿<%@ Page Language="C#" Trace="false" Inherits="SiteServer.BackgroundPages.Cms.ModalChannelImport" %>
  <%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <!--#include file="../inc/openWindow.html"-->

      <form runat="server">
        <bairong:alerts runat="server" />

        <div class="form-horizontal">

          <div class="form-group">
            <label class="col-xs-4 control-label text-right">栏目文件</label>
            <div class="col-xs-7">
              <input type="file" class="form-control" id="HifFile" size="35" runat="server" />
              <asp:RequiredFieldValidator ControlToValidate="HifFile" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
              />
            </div>
            <div class="col-xs-1">

            </div>
          </div>
          <div class="form-group">
            <label class="col-xs-4 control-label text-right">父栏目</label>
            <div class="col-xs-7">
              <asp:DropDownList class="form-control" ID="DdlParentNodeId" runat="server"></asp:DropDownList>
            </div>
            <div class="col-xs-1">

            </div>
          </div>
          <div class="form-group">
            <label class="col-xs-4 control-label text-right">是否覆盖同名栏目</label>
            <div class="col-xs-7">
              <asp:DropDownList ID="DdlIsOverride" runat="server" class="form-control">
                <asp:ListItem Text="覆盖" Value="True" Selected="true"></asp:ListItem>
                <asp:ListItem Text="不覆盖" Value="False"></asp:ListItem>
              </asp:DropDownList>
            </div>
            <div class="col-xs-1">

            </div>
          </div>

          <hr />

          <div class="form-group m-b-0">
            <div class="col-xs-11 text-right">
              <asp:Button class="btn btn-primary m-l-10" text="确 定" runat="server" onClick="Submit_OnClick" />
              <button type="button" class="btn btn-default m-l-10" onclick="window.parent.layer.closeAll()">取 消</button>
            </div>
            <div class="col-xs-1"></div>
          </div>
        </div>

      </form>
    </body>

    </html>