<%@ Page Language="C#" MasterPageFile="~/PhysicianMasterPageBlank.master" AutoEventWireup="true" Inherits="eRxWeb.MultipleViewCSS" Title="Script Print Preview" CodeBehind="MultipleViewCSS.aspx.cs" %>

<%@ Register Src="Controls/Message.ascx" TagName="Message" TagPrefix="ePrescribe" %>

<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script language="javascript" type="text/javascript">

        function printScripts_Click() {
            var btnPrint = document.getElementById("<%=btnPrint.ClientID %>");
            if (btnPrint != null) {
                btnPrint.disabled = "true";
                btnPrint.value = "Processing...";
            }

            var hiddenPrintTrigger = document.getElementById("<%=hiddenPrintTrigger.ClientID %>");
            var browser = detectBrowser();
            //
            // Code reference from: www.eggheadcafe.com/PrintSearchContent.asp?LINKID=449
            //
            getIFrameDocument('<%=pdfDocumentFrame.ClientID %>').focus();

            document.getElementById("PrintInd").value = "Y";
            var bMS_IE = false;
            // Verify whether the browser is Internet Explorer (IE8,IE9,IE10 -> "msie", but for IE11 the name is changed into "trident").
            var userAgent = window.navigator.userAgent.toLowerCase();
            bMS_IE = ((userAgent.indexOf('msie ') > -1) || (userAgent.indexOf("trident/") > -1)) ? true : false;


            if (hiddenPrintTrigger != null) {
                if (browser[0] == 'Safari') {

                    setTimeout(__doPostBack(hiddenPrintTrigger.name, ''), 1000);
                    getIFrameDocument('<%=pdfDocumentFrame.ClientID %>').print();
                }
                else if (bMS_IE) {
                    try {
                        document.getElementById('<%=pdfDocumentFrame.ClientID %>').contentWindow.document.execCommand('print', false, null);
                    }
                    catch (e) {
                        getIFrameDocument('<%=pdfDocumentFrame.ClientID %>').print();
                    }

                    __doPostBack(hiddenPrintTrigger.name, '');
                }
                else {
                    getIFrameDocument('<%=pdfDocumentFrame.ClientID %>').print();
                    __doPostBack(hiddenPrintTrigger.name, '');
                }
            }
        }

        function printCoupons_Click() {
            var btnPrintCoupons = document.getElementById("<%=btnPrintCoupons.ClientID %>");
            if (btnPrintCoupons != null) {
                btnPrintCoupons.disabled = "true";
                btnPrintCoupons.value = "Processing...";
            }

            getIFrameDocument('<%=couponFrame.ClientID %>').focus();
            getIFrameDocument('<%=couponFrame.ClientID %>').print();

            var btnPrintCouponsTrigger = document.getElementById("<%=btnPrintCouponsTrigger.ClientID %>");
            if (btnPrintCouponsTrigger != null) {
                // btnPrintCouponsTrigger.click()
                __doPostBack(btnPrintCouponsTrigger.name, '')
            }
        }
        function btnPrintGoodRxCoupons_Click() {
            var btnPrintGoodRxCoupons = document.getElementById("<%=btnPrintGoodRxCoupons.ClientID %>");
            if (btnPrintGoodRxCoupons != null) {
                btnPrintGoodRxCoupons.disabled = "true";
                btnPrintGoodRxCoupons.value = "Processing...";
            }

            getIFrameDocument('<%=goodRxCouponsFrame.ClientID %>').focus();
            getIFrameDocument('<%=goodRxCouponsFrame.ClientID %>').print();

            var btnGoodRxCoupontrigger = document.getElementById("<%=btnGoodRxCoupontrigger.ClientID %>");
            if (btnGoodRxCoupontrigger != null) {
                __doPostBack(btnGoodRxCoupontrigger.name, '')
            }
        }

        function printGoodRxCoupon_Click(btPrint, frameID) {
            if (btPrint != null) {
                btPrint.disabled = "true";
                btPrint.value = "Processing...";
            }
            var browser = detectBrowser();
            if (browser[0] == 'Safari') {
                frameID.window.print();
            }
            else {
                frameID.focus();
                frameID.contentWindow.focus();
                frameID.contentWindow.print();
            }
            var btnGoodRxCoupontrigger = document.getElementById("<%=btnGoodRxCoupontrigger.ClientID %>");
            if (btnGoodRxCoupontrigger != null) {
                __doPostBack(btnGoodRxCoupontrigger.name, '')
            }
        }

        function printGoodRxCouponForIE_Click(btPrint, frameID) {
            if (btPrint != null) {
                btPrint.disabled = "true";
                btPrint.value = "Processing...";
            }

            document.getElementById(frameID.id).Print();
            var btnGoodRxCoupontrigger = document.getElementById("<%=btnGoodRxCoupontrigger.ClientID %>");
            if (btnGoodRxCoupontrigger != null) {
                __doPostBack(btnGoodRxCoupontrigger.name, '')
            }
        }

        function printPatientReceipts_Click() {
            var btnPatientReceiptPrint = document.getElementById("<%=btnPatientReceiptPrint.ClientID %>");
            if (btnPatientReceiptPrint != null) {
                btnPatientReceiptPrint.disabled = "true";
                btnPatientReceiptPrint.value = "Processing...";
            }

            getIFrameDocument('<%=patientReceiptFrame.ClientID %>').focus();
            getIFrameDocument('<%=patientReceiptFrame.ClientID %>').print();

            var hiddenReceiptPrintTrigger = document.getElementById("<%=hiddenReceiptPrintTrigger.ClientID %>");
            if (hiddenReceiptPrintTrigger != null) {
                // hiddenReceiptPrintTrigger.click()
                __doPostBack(hiddenReceiptPrintTrigger.name, '')
            }
        }


        function printRxInfo_Click() {
            var btnRxInfoPrint = document.getElementById("<%=btnRxInfoPrint.ClientID %>");
            if (btnRxInfoPrint != null) {
                btnRxInfoPrint.disabled = "true";
                btnRxInfoPrint.value = "Processing...";
            }

            getIFrameDocument('<%=rxInfoFrame.ClientID %>').focus();
            getIFrameDocument('<%=rxInfoFrame.ClientID %>').print();


            var btnRxInfoPrintTrigger = document.getElementById("<%=btnRxInfoPrintTrigger.ClientID %>");
            if (btnRxInfoPrintTrigger != null) {
                __doPostBack(btnRxInfoPrintTrigger.name, '')
            }
        }

        function AutoOpenRxInfoPrintDialog(sender, e) {
            var tabContainer = document.getElementById('<%=printTabs.ClientID %>').control;
            if (tabContainer !== null && tabContainer !== undefined &&
                tabContainer.get_activeTab().get_headerText().startsWith('RxInfo')) {
                var frame = getIFrameDocument('<%=rxInfoFrame.ClientID %>');
                if (frame !== null && frame !== undefined) {
                    frame.focus();
                    frame.print();

                    var btnRxInfoPrintTrigger = document.getElementById("<%=btnRxInfoPrintTrigger.ClientID %>");
                    if (btnRxInfoPrintTrigger != null) {
                        __doPostBack(btnRxInfoPrintTrigger.name, '')
                    }
                }
            }
        }
 
            function getIFrameDocument(aID) {
                var iFrame = document.getElementById(aID);
                if (iFrame !== null && iFrame !== undefined) {
                    if (iFrame.contentDocument) {
                        return iFrame.contentWindow;
                    }
                    else {
                        return document.frames[aID];
                    }
                }
            }

        function SetStatus(controlId) {
            document.getElementById(controlId).value = "PrintPreferenceChanged";
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
        window.onload = function () {
            AutoOpenRxInfoPrintDialog(null, null);
        }
    </script>
    <table border="0" cellpadding="0" cellspacing="0" width="100%">
        <tr class="h1title">
            <td colspan="2">
                <span class="Phead indnt">Print Preview</span>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <ePrescribe:Message ID="ucMessage" runat="server" Visible="false" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <ajaxToolkit:TabContainer ID="printTabs" runat="server" OnClientActiveTabChanged="AutoOpenRxInfoPrintDialog" >
                    <ajaxToolkit:TabPanel ID="scriptsTab" runat="server" HeaderText="Scripts">
                        <ContentTemplate>
                            <table border="1" bordercolor="#b5c4c4" cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td>
                                        <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                            <tr class="h4title">
                                                <td colspan="4">
                                                    <asp:Button ID="btnCancel" runat="server" CssClass="btnstyle" OnClick="btnCancel_Click"
                                                        Text="Cancel" />&nbsp;
                                                    <asp:Button ID="btnPrint" runat="server" Text="Print" OnClientClick="printScripts_Click()"
                                                        CssClass="btnstyle" />
                                                    <asp:Button ID="hiddenPrintTrigger" runat="server" Style="display: none;" OnClick="hiddenPrintTrigger_Click" />
                                                    <asp:RadioButtonList ID="rdoPrintingOption" runat="server" AutoPostBack="True" Height="13px"
                                                        OnSelectedIndexChanged="rdoPrintingOption_SelectedIndexChanged" RepeatDirection="Horizontal"
                                                        RepeatLayout="Flow" Width="101px">
                                                    </asp:RadioButtonList>
                                                    <input type="hidden" id="PrintInd" name="PrintInd" value="N" />
                                                    <input type="hidden" id="rdoStatus" name="rdoStatus" value="" runat="server" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" class="indnt">
                                        <iframe id="pdfDocumentFrame" runat="server" name="pdfDocumentFrame" align="top"
                                            width="100%" style="height: 79vh" frameborder="0" title="Print Script"></iframe>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel ID="goodRxCouponTab" runat="server" HeaderText="GoodRx Coupon">
                        <ContentTemplate>
                            <table border="1" bordercolor="#b5c4c4" cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td>
                                        <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                            <tr class="h4title">
                                                <td colspan="4">
                                                    <asp:Button ID="btnGoodRxCouponCancel" runat="server" CssClass="btnstyle" Text="Cancel" OnClick="btnGoodRxCouponCancel_Click" />
                                                    <asp:Button ID="btnPrintGoodRxCoupons" runat="server" Text="Print" CssClass="btnstyle" OnClientClick="btnPrintGoodRxCoupons_Click()" />
                                                    <asp:Button ID="btnGoodRxCoupontrigger" runat="server" Style="display: none;" OnClick="btnGoodRxCoupontrigger_Click" />
                                                    <input type="hidden" id="hfPrintGoodRxCouponsInd" name="printGoodRxCouponsInd" value="N" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" class="indnt">
                                        <iframe id="goodRxCouponsFrame" runat="server" name="goodRxCouponsFrame" src="#" align="top"
                                            width="100%" style="height: 79vh" title="Print GoodRx Coupons"></iframe>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel ID="couponsTab" runat="server" HeaderText="Coupons">
                        <ContentTemplate>
                            <table border="1" bordercolor="#b5c4c4" cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td>
                                        <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                            <tr class="h4title">
                                                <td colspan="4">
                                                    <asp:Button ID="btnPrintCouponsCancel" runat="server" CssClass="btnstyle" Text="Cancel"
                                                        OnClick="btnPrintCouponsCancel_Click" />
                                                    <asp:Button ID="btnPrintCoupons" runat="server" Text="Print" CssClass="btnstyle"
                                                        OnClientClick="printCoupons_Click()" />
                                                    <asp:Button ID="btnPrintCouponsTrigger" runat="server" Style="display: none;" OnClick="printCouponsTrigger_Click" />
                                                    <input type="hidden" id="hfPrintCouponsInd" name="printCouponsInd" value="N" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" class="indnt">
                                        <iframe id="couponFrame" runat="server" name="couponFrame" src="#" align="top" width="100%"
                                            height="<%=getTableHeight() %>" frameborder="0" title="Print Coupons"></iframe>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel ID="patientReceiptsTab" runat="server" HeaderText="Patient Informational Copy">
                        <ContentTemplate>
                            <table border="1" bordercolor="#b5c4c4" cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td>
                                        <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                            <tr class="h4title">
                                                <td colspan="4">
                                                    <asp:Button ID="btnPatientReceiptCancel" runat="server" CssClass="btnstyle" Text="Cancel"
                                                        OnClick="btnPatientReceiptCancel_Click" Visible="false" />&nbsp;
                                                    <asp:Button ID="btnPatientReceiptPrint" runat="server" Text="Print" OnClientClick="printPatientReceipts_Click()"
                                                        CssClass="btnstyle" Visible="false" />
                                                    <asp:Button ID="hiddenReceiptPrintTrigger" runat="server" Style="display: none;"
                                                        OnClick="hiddenReceiptPrintTrigger_Click" />
                                                    <input type="hidden" id="hfPatReceiptPrintInd" name="patReceiptPrintInd" value="N" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" class="indnt">
                                        <iframe id="patientReceiptFrame" runat="server" name="patientReceiptFrame" align="top"
                                            width="100%" height="<%=getTableHeight() %>" frameborder="0" title="Print Patient Receipts"></iframe>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>
                    <ajaxToolkit:TabPanel ID="rxInfoTab" runat="server" HeaderText="Rx Info">
                        <ContentTemplate>
                            <table border="1" bordercolor="#b5c4c4" cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td>
                                        <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                            <tr class="h4title">
                                                <td colspan="4">
                                                    <asp:Button ID="btnRxInfoCancel" runat="server" CssClass="btnstyle" Text="Cancel"
                                                        OnClick="btnRxInfoCancel_Click" />
                                                    <asp:Button ID="btnRxInfoPrint" runat="server" Text="Print" CssClass="btnstyle"
                                                        OnClientClick="printRxInfo_Click()" />
                                                    <asp:Button ID="btnRxInfoPrintTrigger" runat="server" Style="display: none;" OnClick="printRxInfoTrigger_Click" />
                                                    <%--<input type="hidden" id="hfPrintRxInfoInd" name="printRxInfoInd" value="N" />--%>
                                                    <%--<asp:Button ID="btnRxInfoPrint" runat="server" Text="Print" CssClass="btnstyle" />--%>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" class="indnt">
                                        <iframe id="rxInfoFrame" runat="server" name="rxInfoFrame" src="#" align="top" width="100%"
                                            height="<%=getTableHeight() %>" frameborder="0" title="Print RxInfo"></iframe>
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </ajaxToolkit:TabPanel>

                </ajaxToolkit:TabContainer>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content2" runat="Server" ContentPlaceHolderID="ContentPlaceHolder2">
    <%--    <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" ChildrenAsTriggers="False">
        <ContentTemplate>
            <asp:Panel ID="panelHelpHeader" CssClass="accordionHeader" runat="server" Width="100%">
                <table cellspacing="0" cellpadding="0" width="100%" border="0">
                    <tbody>
                        <tr>
                            <td align="left" width="140">
                                <div id="Header" class="accordionHeaderText">
                                    Help With This Screen</div>
                            </td>
                            <td align="right" width="14">
                                <asp:Image ID="hlpclpsimg" runat="server" ImageUrl="~/images/chevrondown-nor-light-12-x-12.png"></asp:Image>&nbsp;&nbsp;
                            </td>
                        </tr>
                    </tbody>
                </table>
            </asp:Panel>
            <asp:Panel ID="panelHelp" CssClass="accordionContent" runat="server" Width="95%">
            </asp:Panel>
            <ajaxToolkit:CollapsiblePanelExtender ID="cpeHelp" runat="server" Collapsed="true"
                CollapsedSize="0" TargetControlID="panelHelp" ExpandControlID="panelHelpHeader"
                CollapseControlID="panelHelpHeader" ExpandDirection="Vertical" CollapsedImage="images/chevrondown-nor-light-12-x-12.png"
                ExpandedImage="images/chevronup-nor-light-16-x-16.png" ImageControlID="hlpclpsimg" SuppressPostBack="true">
            </ajaxToolkit:CollapsiblePanelExtender>
            <asp:Panel ID="panelMessageHeader" CssClass="accordionHeader" runat="server" Width="100%">
                <table cellspacing="0" cellpadding="0" width="100%" border="0">
                    <tbody>
                        <tr valign="baseline" height="16">
                            <td align="left" width="140">
                                <asp:Label CssClass="accordionHeaderText" ID="lblPrintingInfo" runat="server">Printing Information</asp:Label><br />
                            </td>
                            <td align="right" width="14">
                                <asp:Image ID="msgclpsimg" runat="server" ImageUrl="~/images/chevrondown-nor-light-12-x-12.png"></asp:Image>&nbsp;&nbsp;
                            </td>
                        </tr>
                    </tbody>
                </table>
            </asp:Panel>
            <asp:Panel ID="panelMessage" CssClass="accordionContent" runat="server" Width="95%">
                Prescriptions printed in this state do not require special security paper. If you
                feel more secure in printing your prescriptions on security style paper, please
                visit our paper vendor, <a target="_paper" href="http://www.rxpaper.com/ePrescribe/">
                    http://www.rxpaper.com/ePrescribe/</a>. Select the state you are practicing
                in and follow the link for instructions on ordering.
            </asp:Panel>
            <ajaxToolkit:CollapsiblePanelExtender ID="cpeMessage" runat="server" Collapsed="false"
                CollapsedSize="0" TargetControlID="panelMessage" ExpandControlID="panelMessageHeader"
                CollapseControlID="panelMessageHeader" ExpandDirection="Vertical" CollapsedImage="images/chevrondown-nor-light-12-x-12.png"
                ExpandedImage="images/chevronup-nor-light-16-x-16.png" ImageControlID="msgclpsimg" SuppressPostBack="true">
            </ajaxToolkit:CollapsiblePanelExtender>
        </ContentTemplate>
    </asp:UpdatePanel>--%>
</asp:Content>
