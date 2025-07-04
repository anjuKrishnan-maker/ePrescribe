<%@ Control Language="C#" AutoEventWireup="true" Inherits="eRxWeb.Controls_DeluxeEPCSCommonPanels" Codebehind="DeluxeEPCSCommonPanels.ascx.cs" %>
  

    <asp:Panel style="DISPLAY: none" id="panelProviders" runat="server">    
        <div id="div6" runat="server" class="overlaymain">
            <div class="overlayTitle">
                Provider List
            </div>
            <div id="div8" runat="server" class="overlaysub1">
                <table class="PopUpTable">
                    <tr>
                        <td align="center">
                            <table border="0" width="80%">
                                <tr>
                                    <td align="left" class="PopUpTd">
                                        Pricing for Veradigm ePrescribe™ <span id="spnFeatureInPopUpTd" style="font-weight: normal; font-size: 10px; color: #404040;">Deluxe</span> is based on the number of enrolled providers.
                                        <br />
                                        Sharing user IDs is a violation of the terms of service and can result in immediate 
                                        deactivation.
                                    </td>
                                </tr>
                                <tr>
                                    <td align="left" class="PopUpTd">
                                        <asp:Panel ID="panelProvidersScroll" runat="server" ScrollBars="Vertical" Width="90%" Height="200px">
                                            <center>                                     
                                                <asp:GridView ID="deluxeProviderListGrid" runat="server"
                                                GridLines="Both" DataKeyNames="Name,AdminCode" DataSourceID="GetDeluxeProviderList" 
                                                AllowPaging="false" AutoGenerateColumns="false" PageSize="10000">
                                                    <Columns>
                                                        <asp:BoundField DataField="Name" HeaderText="Name"  HeaderStyle-HorizontalAlign="Left">
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="AdminCode" HeaderText="Admin" HeaderStyle-HorizontalAlign="Left">
                                                            <ItemStyle HorizontalAlign="Left" />
                                                        </asp:BoundField>
                                                    </Columns>
                                                </asp:GridView>
                                                <asp:ObjectDataSource ID="GetDeluxeProviderList" runat="server" SelectMethod="GetDeluxeUsers"
                                                    TypeName="Allscripts.Impact.DeluxePurchaseManager" OldValuesParameterFormatString="original_{0}">
                                                    <SelectParameters>
                                                        <asp:SessionParameter Name="licenseID" SessionField="LICENSEID" Type="String" />
                                                        <asp:SessionParameter Name="dbID" SessionField="DBID" Type="object" />                                            
                                                    </SelectParameters>
                                                </asp:ObjectDataSource>
                                            </center>
                                        </asp:Panel>                                                
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>    
            </div>
            <div class="overlayFooter">
                <asp:Button ID="btnProviderListClose" runat="server" Text="Close" CssClass="btnstyle btnStyleAction" />
            </div>
        </div>
    </asp:Panel>
    <asp:Button ID="hiddenProviders" runat="server" style="display:none;" />
    <ajaxToolkit:ModalPopupExtender id="mpeProviders" runat="server" BehaviorID="mpeProviders" DropShadow="false" BackgroundCssClass="modalBackground"  TargetControlID="hiddenProviders"  PopupControlID="panelProviders" CancelControlID="btnProviderListClose"></ajaxToolkit:ModalPopupExtender>
   
   <asp:Panel id="panelTC" style="DISPLAY: none" runat="server">    
        <div id="div4" runat="server" class="overlaymain" >
            <div id="div7" runat="server" class="overlaysub1" >
                <div style="height:400px; overflow:scroll"> 
                    <div id="eulaContent" runat="server"></div>
                </div>
                <br />
            </div>
            <div class="overlayFooter">
                <input type="button" value="Print" class="btnstyle btnStyleAction" onclick="window.open('pdf.aspx?GeneralDoc=DeluxeTermsConditions.aspx')"/>
                    <asp:Button ID="btnTCClose" runat="server" Text="Close" CssClass="btnstyle" />
            </div>
        </div>
    </asp:Panel>
    <asp:Button ID="hiddenTC" runat="server" style="display:none;" />
    <ajaxToolkit:ModalPopupExtender id="mpeTC" runat="server" BehaviorID="mpeTC" DropShadow="false" BackgroundCssClass="modalBackground" TargetControlID="hiddenTC" PopupControlID="panelTC" CancelControlID="btnTCClose"></ajaxToolkit:ModalPopupExtender>                                               
   

    <!-- temperary fix for relogon issue - have user logoff and back on -->
    <asp:Panel ID="pnlCancelLogOff" runat="server" Style="text-align: center; display:none;">
        <table style="width: 100%; background-color:White;" cellpadding="10px">
            <tr>
                <td>
                    <span style="font-size: 14pt; text-align: center;">These changes will take effect once you log out and log back in.</span>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btnCancelLogOff" runat="server" Text="OK"  CssClass="btnstyle" />
                </td>
            </tr>
        </table>
    </asp:Panel>
        
    <ajaxToolkit:ModalPopupExtender ID="mpeCancelLogOff" runat="server" BehaviorID="mpeLogOff"
        DropShadow="false" BackgroundCssClass="modalBackground" TargetControlID="btnCancelContinueLogOff"
        PopupControlID="pnlCancelLogOff" Drag="false" PopupDragHandleControlID="pnlLogOff" />