<%@ Page Language="C#" AutoEventWireup="true" Inherits="eRxWeb.SSO" Codebehind="SSO.aspx.cs" Async="true" %>
<% 
Response.ExpiresAbsolute = DateTime.Now.AddDays(-1d) ;
Response.AddHeader("cache-control", "no-store, must-revalidate, private");
Response.AddHeader("Pragma", "no-cache");
%>
