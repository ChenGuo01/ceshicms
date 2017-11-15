﻿<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalContentArchive" Trace="false"%>
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

					<div class="alert alert-info">
						<h5>此操作将归档所选内容，确认吗？</h5>
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