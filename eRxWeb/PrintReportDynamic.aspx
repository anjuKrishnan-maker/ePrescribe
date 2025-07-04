<%@ Page Language="C#" AutoEventWireup="true" Inherits="eRxWeb.PrintReportDynamic" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" Codebehind="PrintReportDynamic.aspx.cs" %>
<%@ Register TagPrefix="rsweb" Namespace="Microsoft.Reporting.WebForms" Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Print Reports</title>
    <link href="Style/Style.css" rel="stylesheet" type="text/css" />
</head>
<script language="javascript" type="text/javascript">
function fnCheck()
{
    
    var b=document.getElementById("<%=btnPrint.ClientID%>");
    b.click();
   
}

</script>
<body>
    <form id="form1" runat="server">
        <div class="reportViewerPanel"> <br />
            <div class="divReportWrapper">
                <rsweb:ReportViewer ID="rptViewer" runat="server" Height="400px" Width="800px" Font-Names="Verdana" Font-Size="8pt" ShowZoomControl="false" ShowPrintButton="false" meta:resourcekey="rptViewerResource1"></rsweb:ReportViewer>
            </div>
            <asp:objectdatasource id="ObjDSDefaultReport" runat="server" OldValuesParameterFormatString="original_{0}" ConvertNullToDBNull="False" TypeName="Allscripts.Impact.Reporting" SelectMethod="GetDefaultReportData">
                <SelectParameters>
                    <asp:Parameter Name="LicenseID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false"/>
                    <asp:Parameter Name="SiteID" Type="int32" DefaultValue="0" />
                    <asp:Parameter Name="InventoryID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false"/>
                    <asp:Parameter Name="UserID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false"/>
                    <asp:Parameter Name="ProviderID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false"/>
                    <asp:Parameter Name="POBID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false"/>
                    <asp:Parameter Name="StartDate" Type="string" DefaultValue="" ConvertEmptyStringToNull="false"/>
                    <asp:Parameter Name="EndDate" Type="string" DefaultValue="" ConvertEmptyStringToNull="false"/>
                    <asp:Parameter Name="PatientID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false"/>
                    <asp:Parameter Name="PlanID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false"/>
                    <asp:Parameter Name="PayorID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false"/>
                    <asp:Parameter Name="DDI" Type="string" DefaultValue="" ConvertEmptyStringToNull="false"/>
                    <asp:Parameter Name="Schedules" Type="string" DefaultValue="" ConvertEmptyStringToNull="false"/>
                    <asp:Parameter Name="ReportID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false"/>
                    <asp:Parameter Name="SortCd" Type="string" DefaultValue="" ConvertEmptyStringToNull="false"/>
                    <asp:Parameter Name="UseReplica" Type="string" DefaultValue="N" ConvertEmptyStringToNull="false"/>
                    <asp:Parameter Name="SessionUserID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false"/>
                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                </SelectParameters>
            </asp:objectdatasource>
            <asp:objectdatasource id="ObjDSPatientPersonalInfo" runat="server" SelectMethod="GetPatientData" ConvertNullToDBNull=False
            TypeName="Allscripts.Impact.Patient" OldValuesParameterFormatString="original_{0}">
                <SelectParameters>                        
                    <asp:Parameter Name="PatientID" DefaultValue="" Type="String" />
                    <asp:Parameter Name="LicenseID" DefaultValue="" Type="String" />
                    <asp:Parameter Name="UserID" DefaultValue="" Type="String" />
                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                </SelectParameters>
            </asp:objectdatasource>
            <asp:objectdatasource id="ObjDSPatientAllergyInfo" runat="server" OldValuesParameterFormatString="original_{0}" ConvertNullToDBNull="false" TypeName="Allscripts.Impact.Patient" SelectMethod="GetPatientAllergy">
                <SelectParameters>
                    <asp:Parameter Name="PatientID" DefaultValue="" Type="String" />
                    <asp:Parameter Name="LicenseID" DefaultValue="" Type="String" />
                    <asp:Parameter Name="UserID" DefaultValue="" Type="String" />
                    <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                    <asp:Parameter Name="nodeID" ConvertEmptyStringToNull="true" Type="object" />
                </SelectParameters>
            </asp:objectdatasource>            
        </div>
       <asp:button id="btnPrint" width="1px" Height="1px" cssclass="btnstyle" runat="server" text="Print Report" OnClick="btnPrint_Click" meta:resourcekey="btnPrintResource1"/>
    </form>
</body>
</html>
