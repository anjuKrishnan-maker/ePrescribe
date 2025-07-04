<%@ Control Language="C#" AutoEventWireup="true" Inherits="eRxWeb.Controls_AdControl" Codebehind="AdControl.ascx.cs" %>
<script language="javascript" type="text/javascript">

    var secondsPassed = -1;
    var timerID = window.setInterval('timerTick()', 1000);  
  
    function timerTick()
    {
        if (secondsPassed < 0)
        {
            var skipTime = document.getElementById("<%=iSkipTime.ClientID %>");
            secondsPassed = parseInt(skipTime.value);
        }
        
        if (secondsPassed >= 0)
        {
            secondsPassed = secondsPassed - 1;

            if (secondsPassed <= 0)
            {
                clearInterval(timerID);
                closeClicked();
            }
            else
            {
                var closeText = document.getElementById("<%=closeText.ClientID %>");
                if (closeText != null)
                {
                    setInnerText(closeText,"click here to skip or wait " + secondsPassed.toString() + " seconds");
                }
            }
        }
        else
        {
            clearInterval(timerID);
        }
    }
    
    function closeClicked()
    {
        var adPanel = document.getElementById("<%=adPanel.ClientID %>");
        if (adPanel != null)
        {
            adPanel.style.display = "none";
        }
        
        var closeButton = document.getElementById("<%=closeButton.ClientID %>");
        if (closeButton != null)
        {
            closeButton.click();
        }        
    }
    
    function setInnerText(ctl,content)
    {
        var isIE = (window.navigator.userAgent.indexOf("MSIE") > 0);

        if (!isIE) 
            ctl.textContent = content;
        else
            ctl.innerText = content;
    }	
    
    function adClicked()
    {        
        var adPanel = document.getElementById("<%=adPanel.ClientID %>");
        if (adPanel != null)
        {
            adPanel.style.display = "none";
        }
        
        var adButton = document.getElementById("<%=adButton.ClientID %>");
        if (adButton != null)
        {
            adButton.click();
        }
    }  
    
    function closePopupAndRedirectToDeluxe(LicenseDeluxeStatus) 
    {
            window.parent.CloseDeluxeAdPopup();

            if(LicenseDeluxeStatus == 'disabled') //Disabled
            {
                window.parent.LoadDeluxeAccountManagement();
            }
            else // 0 , Off
            {
                window.parent.LoadDeluxeFeatureSelection();
            }
    }
    
</script>
<div id="adPanel" runat="server" style="display:none; background-color:Transparent; border:none;">
    <div class="overlayTitle">
          <a id="featureModule" class="overlayTitleText" runat="server" />    
    </div>
    <table width="100%"  style="background-color:Transparent;">
        <tr style="height:5px;">
            <td align="left">    
            </td>
            <td align="right">
                <span id="closeSpan" runat="server" onclick="closeClicked()">
                    <a id="closeText" runat="server" class="interstitialAdSkip" />
                    <img id="closeAdImage" runat="server" src="~/Images/error.png" class="interstitialAdClose" />
                </span>
                <asp:Button ID="closeButton" runat="server" style="display:none;" OnClick="closeButton_Click" />
                <asp:Button ID="adButton" runat="server" style="display:none;" OnClick="adButton_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div id="adContent" runat="server">
                </div>    
                <input id="iSkipTime" runat="server" type="hidden" value="30" />                
            </td>
        </tr>
    </table>
</div>
