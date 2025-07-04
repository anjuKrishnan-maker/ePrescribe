<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PptPlusCoupon.ascx.cs" Inherits="eRxWeb.Controls_PptPlusCoupon" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>
<script type="text/javascript">    
            function ToggleSendToPharmacy(ddldest, clientIdCheckSendOfferToPharmNotes, clientIdCheckSendToPharm) {
                var chkboxClientIdCheckSendOfferToPharmNotes = document.getElementById(clientIdCheckSendOfferToPharmNotes);
                if (chkboxClientIdCheckSendOfferToPharmNotes != null) {
                    if (ddldest.value !== '<%=Constants.ScriptPadDestinations.PHARM.ToString() %>' && ddldest.value !== '<%=Constants.ScriptPadDestinations.DOCPHARM.ToString()%>')
                    {
                        chkboxClientIdCheckSendOfferToPharmNotes.disabled = true;
                    }
                    else
                    {
                        chkboxClientIdCheckSendOfferToPharmNotes.removeAttribute('disabled');
                        chkboxClientIdCheckSendOfferToPharmNotes.parentNode.removeAttribute('disabled');
                    }
                    chkboxClientIdCheckSendOfferToPharmNotes.checked = false;
                }

                var chkboxClientIdCheckSendToPharm = document.getElementById(clientIdCheckSendToPharm);
                if (chkboxClientIdCheckSendToPharm != null) {
                    if (ddldest.value !== '<%=Constants.ScriptPadDestinations.PHARM.ToString() %>' && ddldest.value !== '<%=Constants.ScriptPadDestinations.DOCPHARM.ToString()%>')
                    {
                        chkboxClientIdCheckSendToPharm.checked = false;
                        chkboxClientIdCheckSendToPharm.disabled = true;
                    }
                    else
                    {
                        chkboxClientIdCheckSendToPharm.checked = true;
                        chkboxClientIdCheckSendToPharm.removeAttribute('disabled');
                        chkboxClientIdCheckSendToPharm.parentNode.removeAttribute('disabled');
                    }
                }
    }   
    function CheckGoodRx_Clicked(chkboxGoodRx, chkboxECoupon) {
        var chkboxECouponControl = document.getElementById(chkboxECoupon);
        var chkboxGoodRxControl = document.getElementById(chkboxGoodRx);
        if (chkboxECouponControl != null && chkboxGoodRxControl != null) {
            if (chkboxGoodRxControl.checked === true) {
                chkboxECouponControl.checked = false;
            }
        }

    }
</script>
<div id="divPptCoupons" runat="server" visible="false">
    <table style="width: 100%; margin-left: -12px;">
        <tr>
            <td class="couponCell1" style="border: none !important"></td>
            <td class="couponCell2" style="border: none !important"></td>
            <td class="couponCell3">
                <div>
                    <p>
                        <asp:LinkButton ID="lbViewOffer" runat="server" Text="Rx Cash Discount Offer" CssClass="couponLink"
                            ToolTip="Click here to view Prescription Price Transparency offer." OnClick="lbViewOffer_Click"></asp:LinkButton>
                    </p>
                </div>
            </td>
            <td class="couponCell4">
                <asp:CheckBox ID="chkPrintGoodRxOffers" Checked="true" runat="server" Text="Print Offer" CssClass="offerCB" />
                <br />
                <asp:CheckBox ID="chkSendOfferToPharmacy" Checked="false" runat="server" Text="Send offer to pharmacy" CssClass="offerCB"/>
                &nbsp;
                <asp:Image ID="imgStatusSendOfferToPharmacy" runat="server" ImageUrl="../images/info-act-16-x-16.png" ToolTip="Patient understands insurance will not be used."/>
            </td>
            <td style="border-bottom: none !important"></td>
            <td style="border: none !important"></td>
            <td style="border: none !important"></td>
        </tr>
    </table>
</div>


