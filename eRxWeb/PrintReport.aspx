<%@ Page Language="C#" AutoEventWireup="True" Inherits="eRxWeb.PrintReport"
    Culture="auto" UICulture="auto" Codebehind="PrintReport.aspx.cs" %>
<%@ Register TagPrefix="rsweb" Namespace="Microsoft.Reporting.WebForms" Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=10"/>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" >
    <title>Print Reports</title>
    <link href="Style/Style.css" rel="stylesheet" type="text/css" />
</head>
<script language="javascript" type="text/javascript">
    function printPdf(Url) {
        location.href = Url;//"printPdf.aspx";
    }
    function ignoreIeEvents() {
        var isIE11 = navigator.userAgent.match(/Trident/);
        if (isIE11 != undefined && (document.documentMode && document.documentMode == 11))
            try {
                if (Sys.Browser)
                    Sys.Browser.agent = null
                if ($telerik) {
                    $telerik.isIE = false;
                }

            } catch (ex) {
                //console.log(ex);
            }
    }
    ignoreIeEvents();
    function exportToPdfAndAudit()
    {
        $find('rptViewer').exportReport('PDF');
    }

    function exportToWordAndAudit() {
        $find('rptViewer').exportReport('WORDOPENXML');
    }


    function fnCheck() {
        var b = document.getElementById("<%=btnPrint.ClientID%>");
        b.click();
    }

    function detectBrowser() {
        var N = navigator.appName;
        var UA = navigator.userAgent;
        var temp;
        var browserVersion = UA.match(/(opera|chrome|safari|firefox|msie)\/?\s*(\.?\d+(\.\d+)*)/i);
        if (browserVersion && (temp = UA.match(/version\/([\.\d]+)/i)) != null)
            browserVersion[2] = temp[1];
        browserVersion = browserVersion ? [browserVersion[1], browserVersion[2]] : [N, navigator.appVersion, '-?'];
        return browserVersion;
    };

</script>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="masterScriptManager" runat="server" EnablePartialRendering="true" ScriptMode="Debug" EnablePageMethods="true" EnableScriptGlobalization="true">
		</asp:ScriptManager>	

    <table width="100%">
        <tr>
                <td class="h2title" colspan="4"></td>
        </tr>
        <tr>
                <td colspan="4" class="h4title"></td>
        </tr>
        <tr>
            <td colspan="4">
                <div style="position: absolute; width: 100%;">                  
                    <div class="divReportWrapper">
                        <rsweb:ReportViewer ID="rptViewer" runat="server" Height="100%" Width="100%" AsyncRendering="true" Font-Names="Verdana" ShowZoomControl="false"
                                Font-Size="8pt" ShowPrintButton="false" ShowExportControls="False" PageCountMode="Actual" SizeToReportContent="false" ZoomMode="Percent">
                        </rsweb:ReportViewer>
                    </div>
                    <!-- Added by JJ Bug No:96 Provier Drug Report Format is incomplete-->
                    <!-- Modified by HA/Sandeep to add LicenseID to the report-->
                    <asp:ObjectDataSource ID="ObjDSProviderDrug" runat="server" SelectMethod="GetProviderMedList"
                        TypeName="Allscripts.Impact.Provider" OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                            <asp:QueryStringParameter Name="providerID" QueryStringField="providerID" Type="String" />
                                <asp:QueryStringParameter Type="String" Name="dtStart" QueryStringField="StartDate"></asp:QueryStringParameter>
                                <asp:QueryStringParameter Type="String" Name="dtEnd" QueryStringField="EndDate"></asp:QueryStringParameter>
                            <asp:SessionParameter Name="LicenseID" SessionField="LICENSEID" Type="String" />
                            <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                    <asp:ObjectDataSource ID="ObjDSPrescriptionDetail" runat="server" SelectMethod="GetDefaultReportData"
                        ConvertNullToDBNull="False" TypeName="Allscripts.Impact.Reporting" OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                            <asp:SessionParameter Name="LicenseID" SessionField="LICENSEID" Type="String" />
			                <asp:SessionParameter Name="SiteID" SessionField="SITEID" Type="int32" />
			                <asp:Parameter Name="InventoryID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
			                <asp:SessionParameter Name="UserID" SessionField="USERID" Type="String" />
			                <asp:QueryStringParameter Name="ProviderID" QueryStringField="ProviderID" />
			                <asp:Parameter Name="POBID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
			                <asp:QueryStringParameter Type="String" Name="StartDate" QueryStringField="StartDate"></asp:QueryStringParameter>
			                <asp:QueryStringParameter Type="String" Name="EndDate" QueryStringField="EndDate"></asp:QueryStringParameter>
			                <asp:Parameter Name="PatientID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
			                <asp:Parameter Name="DDI" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
			                <asp:Parameter Name="Schedules" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
			                <asp:QueryStringParameter Name="ReportID" QueryStringField="ReportID" />
			                <asp:Parameter Name="UseReplica" Type="string" DefaultValue="N" ConvertEmptyStringToNull="false" />
                            <asp:SessionParameter Name="SessionUserID" SessionField="USERID" Type="String" />
                            <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                    <asp:ObjectDataSource ID="ObjPatientAdd" runat="server" TypeName="Allscripts.Impact.Reporting"
                        ConvertNullToDBNull="False" SelectMethod="GetDefaultReportData" OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                            <asp:SessionParameter Name="LicenseID" SessionField="LICENSEID" Type="String" />
                            <asp:SessionParameter Name="SiteID" SessionField="SITEID" Type="int32" />
                            <asp:Parameter Name="InventoryID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:SessionParameter Name="UserID" SessionField="USERID" Type="String" />
                            <asp:Parameter Name="ProviderID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="POBID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="StartDate" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="EndDate" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="PatientID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="DDI" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="Schedules" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:QueryStringParameter Name="ReportID" QueryStringField="ReportID" />
                            <asp:Parameter Name="UseReplica" Type="string" DefaultValue="N" ConvertEmptyStringToNull="false" />
                            <asp:SessionParameter Name="SessionUserID" SessionField="USERID" Type="String" />
                            <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                    <!-- Added by JJ Nov 24 Patient Medication Information for patient med history report -->                    
                    <asp:ObjectDataSource ID="ObjDSPatientMedInfo" runat="server" SelectMethod="GetDefaultReportData"
                        ConvertNullToDBNull="False" TypeName="Allscripts.Impact.Reporting" OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                        <asp:SessionParameter Name="LicenseID" SessionField="LICENSEID" Type="String" />
                            <asp:SessionParameter Name="SiteID" SessionField="SITEID" Type="int32" />
                            <asp:Parameter Name="InventoryID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:SessionParameter Name="UserID" SessionField="USERID" Type="String" />
                            <asp:Parameter Name="ProviderID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="POBID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:QueryStringParameter Type="String" Name="StartDate" QueryStringField="StartDate"></asp:QueryStringParameter>
                            <asp:QueryStringParameter Type="String" Name="EndDate" QueryStringField="EndDate"></asp:QueryStringParameter>
                                <asp:QueryStringParameter Type="String" Name="PatientID" QueryStringField="patientID"></asp:QueryStringParameter>
                            <asp:Parameter Name="DDI" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="Schedules" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:QueryStringParameter Name="ReportID" QueryStringField="ReportID" />
                            <asp:Parameter Name="UseReplica" Type="string" DefaultValue="N" ConvertEmptyStringToNull="false" />
                            <asp:SessionParameter Name="SessionUserID" SessionField="USERID" Type="String" />
                            <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                    <asp:ObjectDataSource ID="ObjDSPatientPersonalInfo_PatMedReport" runat="server" SelectMethod="GetPatientData"
                        ConvertNullToDBNull="False" TypeName="Allscripts.Impact.Patient" OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                            <asp:Parameter Name="PatientID" DefaultValue="" Type="String" />
                            <asp:Parameter Name="LicenseID" DefaultValue="" Type="String" />
                            <asp:Parameter Name="UserID" DefaultValue="" Type="String" />
                                <asp:Parameter Name="dbID" DefaultValue="REPLICA_DB" Type="object" />
                                <asp:SessionParameter Name="NodeID" SessionField="DBID" Type="Int32" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                     <asp:ObjectDataSource ID="ObjDSPatientAllergyInfo_PatMedReport" runat="server" OldValuesParameterFormatString="original_{0}"
                        ConvertNullToDBNull="False" TypeName="Allscripts.Impact.Patient" SelectMethod="GetPatientAllergy">
                        <SelectParameters>
                            <asp:Parameter Name="PatientID" DefaultValue="" Type="String" />
                            <asp:Parameter Name="LicenseID" DefaultValue="" Type="String" />
                            <asp:Parameter Name="UserID" DefaultValue="" Type="String" />
                            <asp:Parameter Name="dbID" DefaultValue="REPLICA_DB" Type="object" />
                                <asp:SessionParameter Name="NodeID" SessionField="DBID" Type="Int32" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                    <!-- HA/Sandeep Added this block Nov 27 Prescription History by Medication -->
                    <asp:ObjectDataSource ID="ObjDSPatientMedName" runat="server" SelectMethod="GetDefaultReportData"
                        ConvertNullToDBNull="False" TypeName="Allscripts.Impact.Reporting" OldValuesParameterFormatString="original_{0}">
                        <SelectParameters> 
                        <asp:SessionParameter Name="LicenseID" SessionField="LICENSEID" Type="String" />                              
			                <asp:SessionParameter Name="SiteID" SessionField="SITEID" Type="int32" />
			                <asp:Parameter Name="InventoryID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
			                <asp:SessionParameter Name="UserID" SessionField="USERID" Type="String" />
			                <asp:QueryStringParameter Name="ProviderID" QueryStringField="ProviderID" />
			                <asp:Parameter Name="POBID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
			                <asp:QueryStringParameter Type="String" Name="StartDate" QueryStringField="StartDate"></asp:QueryStringParameter>
			                <asp:QueryStringParameter Type="String" Name="EndDate" QueryStringField="EndDate"></asp:QueryStringParameter>
			                <asp:Parameter Name="PatientID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
			                <asp:QueryStringParameter Type="String" Name="DDI" QueryStringField="MedName"></asp:QueryStringParameter>
			                <asp:Parameter Name="Schedules" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
			                <asp:QueryStringParameter Name="ReportID" QueryStringField="ReportID" />
			                <asp:Parameter Name="UseReplica" Type="string" DefaultValue="N" ConvertEmptyStringToNull="false" />
                            <asp:SessionParameter Name="SessionUserID" SessionField="USERID" Type="String" />
                            <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                    <asp:ObjectDataSource ID="ObjDSPharmacyUtilizationSummary" runat="server" SelectMethod="GetDefaultReportData"
                        ConvertNullToDBNull="False" TypeName="Allscripts.Impact.Reporting" OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                            <asp:SessionParameter Name="LicenseID" SessionField="LICENSEID" Type="String" />
			                <asp:SessionParameter Name="SiteID" SessionField="SITEID" Type="int32" />
			                <asp:Parameter Name="InventoryID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
			                <asp:SessionParameter Name="UserID" SessionField="USERID" Type="String" />
			                <asp:QueryStringParameter Name="ProviderID" QueryStringField="ProviderID" />
			                <asp:Parameter Name="POBID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
			                <asp:QueryStringParameter Type="String" Name="StartDate" QueryStringField="StartDate"></asp:QueryStringParameter>
			                <asp:QueryStringParameter Type="String" Name="EndDate" QueryStringField="EndDate"></asp:QueryStringParameter>
			                <asp:Parameter Name="PatientID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
			                <asp:Parameter Name="DDI" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
			                <asp:Parameter Name="Schedules" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
			                <asp:QueryStringParameter Name="ReportID" QueryStringField="ReportID" />
			                <asp:Parameter Name="UseReplica" Type="string" DefaultValue="N" ConvertEmptyStringToNull="false" />
                            <asp:SessionParameter Name="SessionUserID" SessionField="USERID" Type="String" />
                            <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                    <asp:ObjectDataSource ID="ObjDSPrescriptionDetailCAS" runat="server" ConvertNullToDBNull="False"
                        SelectMethod="GetDefaultReportData" TypeName="Allscripts.Impact.Reporting" OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                            <asp:SessionParameter Name="LicenseID" SessionField="LICENSEID" Type="String" />
                            <asp:SessionParameter Name="SiteID" SessionField="SITEID" Type="int32" />
                            <asp:Parameter Name="InventoryID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:SessionParameter Name="UserID" SessionField="USERID" Type="String" />
                            <asp:Parameter Name="ProviderID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="POBID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="StartDate" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="EndDate" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="PatientID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="DDI" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="Schedules" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="ReportID" Type="string" DefaultValue="PrescriptionDetailOAS"
                                ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="UseReplica" Type="string" DefaultValue="N" ConvertEmptyStringToNull="false" />
                            <asp:SessionParameter Name="SessionUserID" SessionField="USERID" Type="String" />
                            <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                    <asp:ObjectDataSource ID="ObjDSPrescriptionDetailPOB" runat="server" ConvertNullToDBNull="False"
                        SelectMethod="GetDefaultReportData" TypeName="Allscripts.Impact.Reporting" OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                            <asp:SessionParameter Name="LicenseID" SessionField="LICENSEID" Type="String" />
                            <asp:SessionParameter Name="SiteID" SessionField="SITEID" Type="int32" />
                            <asp:Parameter Name="InventoryID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:SessionParameter Name="UserID" SessionField="USERID" Type="String" />
                            <asp:Parameter Name="ProviderID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="POBID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="StartDate" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="EndDate" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="PatientID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="DDI" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="Schedules" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:QueryStringParameter Name="ReportID" QueryStringField="ReportID" />
                            <asp:Parameter Name="UseReplica" Type="string" DefaultValue="N" ConvertEmptyStringToNull="false" />
                            <asp:SessionParameter Name="SessionUserID" SessionField="USERID" Type="String" />
                            <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                    <asp:ObjectDataSource ID="ObjDSPOBToProvider" runat="server" ConvertNullToDBNull="False"
                        SelectMethod="GetPOBToProviderAssociations" TypeName="Allscripts.Impact.RxUser">
                        <SelectParameters>
                            <asp:Parameter Name="pobID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="pobType" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="providerID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:SessionParameter Name="licenseID" SessionField="LICENSEID" Type="String" />
                            <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                    <asp:ObjectDataSource ID="ObjDSProviderToPOB" runat="server" ConvertNullToDBNull="False"
                        SelectMethod="GetProviderToPOBAssociations" TypeName="Allscripts.Impact.RxUser">
                        <SelectParameters>
                            <asp:Parameter Name="pobID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="pobType" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="providerID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:SessionParameter Name="licenseID" SessionField="LICENSEID" Type="String" />
                            <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                    <asp:ObjectDataSource ID="ObjectPatMedRec" runat="server" TypeName="Allscripts.Impact.Reporting"
                        SelectMethod="GetDefaultReportData" OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                            <asp:SessionParameter Name="LicenseID" SessionField="LICENSEID" Type="String" />
                            <asp:SessionParameter Name="SiteID" SessionField="SITEID" Type="Int32" />
                            <asp:Parameter Name="InventoryID" Type="String" DefaultValue="" ConvertEmptyStringToNull="False" />
                            <asp:SessionParameter Name="UserID" SessionField="USERID" Type="String" />
                            <asp:Parameter Name="ProviderID" Type="String" DefaultValue="" ConvertEmptyStringToNull="False" />
                            <asp:Parameter Name="POBID" Type="String" DefaultValue="" ConvertEmptyStringToNull="False" />
                            <asp:QueryStringParameter ConvertEmptyStringToNull="False" DefaultValue="NULL" Name="StartDate"
                                QueryStringField="StartDate" Type="String" />
                            <asp:QueryStringParameter ConvertEmptyStringToNull="False" DefaultValue="NULL" Name="EndDate"
                                QueryStringField="EndDate" Type="String" />
                            <asp:Parameter Name="PatientID" Type="String" DefaultValue="" ConvertEmptyStringToNull="False" />
                            <asp:Parameter Name="DDI" Type="String" DefaultValue="" ConvertEmptyStringToNull="False" />
                            <asp:Parameter Name="Schedules" Type="String" DefaultValue="" ConvertEmptyStringToNull="False" />
                            <asp:Parameter Name="ReportID" Type="string" DefaultValue="PatientMedReconciliation"
                                ConvertEmptyStringToNull="false" />
                            <asp:SessionParameter Name="SessionUserID" SessionField="USERID" Type="String" />
                            <asp:Parameter Name="UseReplica" Type="String" DefaultValue="N" ConvertEmptyStringToNull="False" />
                            <asp:SessionParameter Name="dbID" SessionField="DBID" Type="Object" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                    <asp:ObjectDataSource ID="ObjectPatMedRecDetail" runat="server" TypeName="Allscripts.Impact.Reporting"
                        SelectMethod="GetDefaultReportData" OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                            <asp:SessionParameter Name="LicenseID" SessionField="LICENSEID" Type="String" />
                            <asp:SessionParameter Name="SiteID" SessionField="SITEID" Type="Int32" />
                            <asp:Parameter Name="InventoryID" Type="String" DefaultValue="" ConvertEmptyStringToNull="False" />
                            <asp:SessionParameter Name="UserID" SessionField="USERID" Type="String" />
                            <asp:Parameter Name="ProviderID" Type="String" DefaultValue="" ConvertEmptyStringToNull="False" />
                            <asp:Parameter Name="POBID" Type="String" DefaultValue="" ConvertEmptyStringToNull="False" />
                            <asp:QueryStringParameter ConvertEmptyStringToNull="False" DefaultValue="NULL" Name="StartDate"
                                QueryStringField="StartDate" Type="String" />
                            <asp:QueryStringParameter ConvertEmptyStringToNull="False" DefaultValue="NULL" Name="EndDate"
                                QueryStringField="EndDate" Type="String" />
                            <asp:Parameter Name="PatientID" Type="String" DefaultValue="" ConvertEmptyStringToNull="False" />
                            <asp:Parameter Name="DDI" Type="String" DefaultValue="" ConvertEmptyStringToNull="False" />
                            <asp:Parameter Name="Schedules" Type="String" DefaultValue="" ConvertEmptyStringToNull="False" />
                            <asp:Parameter Name="ReportID" Type="string" DefaultValue="PatientMedReconciliationDetail"
                                ConvertEmptyStringToNull="false" />
                            <asp:SessionParameter Name="SessionUserID" SessionField="USERID" Type="String" />
                            <asp:Parameter Name="UseReplica" Type="String" DefaultValue="N" ConvertEmptyStringToNull="False" />
                            <asp:SessionParameter Name="dbID" SessionField="DBID" Type="Object" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                    <!-- HA/Sandeep added for Pharmacy Utilization Detail report-->
                    <asp:ObjectDataSource ID="ObjDSPharmacyUtilizationDetail" runat="server" SelectMethod="CHPharmacyUtilizationDetailReport"
                        ConvertNullToDBNull="False" TypeName="Allscripts.Impact.Pharmacy" OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                            <asp:SessionParameter Name="LicenseID" SessionField="LICENSEID" Type="String" />
                                <asp:QueryStringParameter Type="String" Name="dtStart" QueryStringField="StartDate"></asp:QueryStringParameter>
                                <asp:QueryStringParameter Type="String" Name="dtEnd" QueryStringField="EndDate"></asp:QueryStringParameter>
                            <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                    <!-- Pharmacy Refill Report -->
                    <asp:ObjectDataSource ID="ObjDSPharmacyRefillReport" runat="server" ConvertNullToDBNull="False"
                        SelectMethod="GetDefaultReportData" TypeName="Allscripts.Impact.Reporting" OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                            <asp:SessionParameter Name="LicenseID" SessionField="LICENSEID" Type="String" />
                            <asp:SessionParameter Name="SiteID" SessionField="SITEID" Type="int32" />
                            <asp:Parameter Name="InventoryID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:SessionParameter Name="UserID" SessionField="USERID" Type="String" />
                            <asp:QueryStringParameter Name="ProviderID" QueryStringField="ProviderID" />
                            <asp:Parameter Name="POBID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="StartDate" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="EndDate" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="PatientID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="DDI" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="Schedules" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:QueryStringParameter Name="ReportID" QueryStringField="ReportID" />
                            <asp:Parameter Name="UseReplica" Type="string" DefaultValue="N" ConvertEmptyStringToNull="false" />
                            <asp:SessionParameter Name="SessionUserID" SessionField="USERID" Type="String" />
                            <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                    <asp:ObjectDataSource ID="ObjDSPatientMedActiveInfo" runat="server" SelectMethod="GetPatientActiveMedication"
                        ConvertNullToDBNull="False" TypeName="Allscripts.Impact.Patient" OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                            <asp:Parameter Name="PatientID" DefaultValue="" Type="String" />
                            <asp:Parameter Name="LicenseID" DefaultValue="" Type="String" />
                            <asp:Parameter Name="UserID" DefaultValue="" Type="String" />
                            <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                    <asp:ObjectDataSource ID="ObjDSPatientPersonalInfo" runat="server" SelectMethod="GetPatientData"
                        ConvertNullToDBNull="False" TypeName="Allscripts.Impact.Patient" OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                            <asp:Parameter Name="PatientID" DefaultValue="" Type="String" />
                            <asp:Parameter Name="LicenseID" DefaultValue="" Type="String" />
                            <asp:Parameter Name="UserID" DefaultValue="" Type="String" />
                            <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                <asp:Parameter Name="NodeID" ConvertEmptyStringToNull="true" Type="object" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                    <asp:ObjectDataSource ID="ObjDSPatientAllergyInfo" runat="server" OldValuesParameterFormatString="original_{0}"
                            ConvertNullToDBNull="false" TypeName="Allscripts.Impact.Patient" SelectMethod="GetPatientAllergy">
                        <SelectParameters>
                            <asp:Parameter Name="PatientID" DefaultValue="" Type="String" />
                            <asp:Parameter Name="LicenseID" DefaultValue="" Type="String" />
                            <asp:Parameter Name="UserID" DefaultValue="" Type="String" />
                            <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                                <asp:SessionParameter Name="nodeID" ConvertEmptyStringToNull="true" Type="object" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                    <asp:ObjectDataSource ID="ObjDSPatientDiagnosisInfo" runat="server" OldValuesParameterFormatString="original_{0}"
                        ConvertNullToDBNull="False" TypeName="Allscripts.Impact.Patient" SelectMethod="PatientActiveDiagnosis">
                        <SelectParameters>
                            <asp:Parameter Name="PatientID" DefaultValue="" Type="String" />
                            <asp:Parameter Name="LicenseID" DefaultValue="" Type="String" />
                            <asp:Parameter Name="UserID" DefaultValue="" Type="String" />
                            <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                    <asp:ObjectDataSource ID="ObjDSPrescriptionCMS_GCodes" runat="server" ConvertNullToDBNull="False"
                        SelectMethod="GetDefaultReportData" TypeName="Allscripts.Impact.Reporting" OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                            <asp:SessionParameter Name="LicenseID" SessionField="LICENSEID" Type="String" />
                            <asp:SessionParameter Name="SiteID" SessionField="SITEID" Type="int32" />
                            <asp:Parameter Name="InventoryID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:SessionParameter Name="UserID" SessionField="USERID" Type="String" />
                            <asp:Parameter Name="ProviderID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="POBID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="StartDate" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="EndDate" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="PatientID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="DDI" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="Schedules" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:QueryStringParameter Name="ReportID" QueryStringField="ReportID" />
                            <asp:Parameter Name="UseReplica" Type="string" DefaultValue="N" ConvertEmptyStringToNull="false" />
                            <asp:SessionParameter Name="SessionUserID" SessionField="USERID" Type="String" />
                            <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                     <asp:ObjectDataSource ID="ObjInactivatedPARClass" runat="server" ConvertNullToDBNull="False"
                        SelectMethod="GetDefaultReportData" TypeName="Allscripts.Impact.Reporting" OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                            <asp:SessionParameter Name="LicenseID" SessionField="LICENSEID" Type="String" />
                            <asp:SessionParameter Name="SiteID" SessionField="SITEID" Type="int32" />
                            <asp:Parameter Name="InventoryID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:SessionParameter Name="UserID" SessionField="USERID" Type="String" />
                            <asp:Parameter Name="ProviderID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="POBID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="StartDate" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="EndDate" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="PatientID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="DDI" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="Schedules" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:QueryStringParameter Name="ReportID" QueryStringField="ReportID" />
                            <asp:Parameter Name="UseReplica" Type="string" DefaultValue="N" ConvertEmptyStringToNull="false" />
                            <asp:SessionParameter Name="SessionUserID" SessionField="USERID" Type="String" />
                            <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                        </SelectParameters>
                    </asp:ObjectDataSource>        
                    <asp:ObjectDataSource ID="ObjeRxProviderActivity" runat="server" ConvertNullToDBNull="False"
                        SelectMethod="GeteRxProviderActivity" TypeName="Allscripts.Impact.Reporting" OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                            <asp:SessionParameter Name="LicenseID" SessionField="LICENSEID" Type="String" />                            
                            <asp:Parameter Name="ProviderID" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="StartDate" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="EndDate" Type="string" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="IncludeCS" Type="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="ReportID" Type="string" DefaultValue="ProvidereRxActivityReport"
                                ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="UseReplica" Type="string" DefaultValue="N" ConvertEmptyStringToNull="false" />
                            <asp:SessionParameter Name="SessionUserID" SessionField="USERID" Type="String" />
                            <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                    <asp:ObjectDataSource ID="ObjEPCSRightsAssignment" runat="server" ConvertNullToDBNull="False"
                        SelectMethod="GetEPCSRightsAssignment" TypeName="Allscripts.Impact.Reporting" OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                            <asp:SessionParameter Name="LicenseID" SessionField="LICENSEID" Type="String" />                            
                            <asp:QueryStringParameter Name="ReportID" QueryStringField="ReportID" />
                            <asp:SessionParameter Name="SessionUserID" SessionField="USERID" Type="String" />
                            <asp:Parameter Name="UseReplica" Type="string" DefaultValue="N" ConvertEmptyStringToNull="false" />
                            <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                    <asp:ObjectDataSource ID="ObjRegistryChecked" runat="server" ConvertNullToDBNull="False"
                        SelectMethod="GetRegistryCheckedReport" TypeName="Allscripts.Impact.Reporting" OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                            <asp:QueryStringParameter ConvertEmptyStringToNull="False" DefaultValue="" Name="StartDate"
                                QueryStringField="StartDate" Type="String" />
                            <asp:QueryStringParameter ConvertEmptyStringToNull="False" DefaultValue="" Name="EndDate"
                                QueryStringField="EndDate" Type="String" />
                            <asp:SessionParameter Name="LicenseID" SessionField="LICENSEID" Type="String" />                            
                            <asp:Parameter Name="ReportID" Type="string" DefaultValue="RegistryCheckedReport"
                                ConvertEmptyStringToNull="false" />
                            <asp:Parameter Name="UseReplica" Type="string" DefaultValue="N" ConvertEmptyStringToNull="false" />
                            <asp:SessionParameter Name="SessionUserID" SessionField="USERID" Type="String" />
                            <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </div>
            </td>
        </tr>
        <tr class="h3title" align="center">
            <td colspan="4">
                <asp:Button ID="btnPrint" Width="1px" Height="1px" CssClass="btnstyle" runat="server"
                    Text="Print Report" OnClick="btnPrint_Click" meta:resourcekey="btnPrintResource1" />
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
