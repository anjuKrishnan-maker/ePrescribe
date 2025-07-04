<%@ Page Language="C#" MasterPageFile="~/Help/HelpMasterPageNew.master" AutoEventWireup="true" Inherits="eRxWeb.Help_Default" Title="Veradigm ePrescribe Help" Codebehind="Default.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="col-sm-6 col-md-4 col-lg-3">
        <a id="lnkIlearn" runat="server" href="#" target="_blank" class="thumbnail dashboard-tile">
            <div class="ilearn home"></div>
            <div class="caption">
                i-LEARN
            </div>
            <div class="white-body">
                <p style="margin: 0px;">Looking for Guides, eLearning Tutorials, Tool Tips, FAQs? Click the i-Learn icon in ePrescribe or click here</p>
            </div>
        </a>
    </div>

    <div class="col-sm-6 col-md-4 col-lg-3">
        <a id="lnkWhatNew" runat="server" href="#" target="_blank" class="thumbnail dashboard-tile">
            <div class="ilearn new"></div>
            <div class="caption">
                WHAT'S NEW
            </div>
            <div class="white-body">
                <p style="margin: 0px;">Latest features and enhancements in Veradigm ePrescribe</p>
            </div>
        </a>
    </div>
    <div class="col-sm-6 col-md-4 col-lg-3">
        <a href="import.aspx" class="thumbnail dashboard-tile">
            <div class="ilearn impPtn"></div>
            <div class="caption">
                INTERFACE WITH YOUR PRACTICE MANAGEMENT SYSTEM
            </div>
            <div class="white-body">
                <p style="margin: 0px;">Whether you are an ePrescribe Deluxe or Platinum user, we have a solution that meets your needs.</p>
            </div>
        </a>
    </div>
    <div class="col-sm-6 col-md-4 col-lg-3">
        <a  href="Add-on.aspx" class="thumbnail dashboard-tile">
            <div class="ilearn addOn"></div>
            <div class="caption">
                ADD ON FEATURES
            </div>
            <div class="white-body">
                <p style="margin: 0px;">Veradigm ePrescribe partners offer add-on solutions</p>
            </div>
        </a>
    </div>
</asp:Content>

