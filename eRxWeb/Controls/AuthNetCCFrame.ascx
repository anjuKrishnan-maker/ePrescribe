<%@ Control Language="C#" AutoEventWireup="true" Inherits="eRxWeb.Controls_AuthNetCCFrame" Codebehind="AuthNetCCFrame.ascx.cs" %>

 <link href="contentx/manage.css" rel="stylesheet" type="text/css" />
 <script type="text/javascript" src="contentx/popup.js"></script>

  <script type="text/javascript">
  //<![CDATA[
              // Uncomment this line if eCheck is enabled. This does not affect functionality, only the initial sizing of the popup page for add payment.
              //AuthorizeNetPopup.options.eCheckEnabled = true;

              // Uncomment these lines to define a function that will be called when the popup is closed.
              // For example, you may want to refresh your page and/or call the GetCustomerProfile API method from your server.
             AuthorizeNetPopup.options.onPopupClosed = function () {
                document.forms.form1.submit();
              };

              // Uncomment this line if you do not have absolutely positioned elements on your page that can obstruct the view of the popup.
              // This can speed up the processing of the page slightly.
              //AuthorizeNetPopup.options.skipZIndexCheck = true;

              // Uncomment this line to use test.authorize.net instead of secure.authorize.net.

              var testMode =  <% Response.Write(Session["PaymentGateWayTestMode"]); %>;
              if(testMode == "1")
                AuthorizeNetPopup.options.useTestEnvironment = true;
            else
                AuthorizeNetPopup.options.useTestEnvironment = false;
  //]]>
  </script>


<!-- 
INSTRUCTIONS:
Put this hidden <form> anywhere on your page with the token from the GetHostedProfilePage API call.
-->
<form method="post" action="https://test.authorize.net/profile/manage" id="formAuthorizeNetPopup" name="formAuthorizeNetPopup" target="iframeAuthorizeNet" style="display:none;">
  <input type="hidden" name="Token" value="<% Response.Write(Session["Token"]); %>" />
  <input type="hidden" name="PaymentProfileId" value="<% Response.Write(Session["editProfileID"]); %>" />
</form>

<form method="post" action="" id="form1" name="form1" style="display:none;">
      <input type="hidden" name="ProfileCreated" id="ProfileCreated" value="True" />
      <input type="hidden" name="editProfileID" value="<% Response.Write(Session["editProfileID"]); %>" />
</form>

<!-- 
INSTRUCTIONS:
Put this button wherever you want on your page.
-->
<button style="display:none;" id="ButtonPopUp" onclick="<%
    if(Session["editProfileID"]!=null)
    {
        string str = Session["editProfileID"].ToString();
        if(String.IsNullOrEmpty(str)) 
            Response.Write("AuthorizeNetPopup.openAddPaymentPopup()");
        else
             Response.Write("AuthorizeNetPopup.openEditPaymentPopup("+str+")");
    }
    else
    {
        Response.Write("AuthorizeNetPopup.openAddPaymentPopup()");
    }
  %>">Manage my payment and shipping information</button>

<%--<button style="display:none;" id="ButtonPopUp" onclick="AuthorizeNetPopup.openEditPaymentPopup()">Manage my payment information</button>--%>
<!-- 
INSTRUCTIONS:
Put this divAuthorizeNetPopup section right before the closing </body> tag. The popup will be centered inside the whole browser window. 
If you want the popup to be centered inside some other element such as a div, put it inside that element.
-->
<div id="divAuthorizeNetPopup" style="display:none;" class="AuthorizeNetPopupGrayFrameTheme">
  <div class="AuthorizeNetPopupOuter">
    <div class="AuthorizeNetPopupTop">
      <div class="AuthorizeNetPopupClose">
        <a href="javascript:;" onclick="AuthorizeNetPopup.closePopup();" title="Close"> </a>
      </div>
    </div>
    <div class="AuthorizeNetPopupInner">
      <iframe name="iframeAuthorizeNet" id="iframeAuthorizeNet" src="contentx/empty.html" frameborder="0" scrolling="auto"></iframe>
    </div>
    <div class="AuthorizeNetPopupBottom">
      <div class="AuthorizeNetPopupLogo" title="Powered by Authorize.Net"></div>
    </div>
  </div>
  <div class="AuthorizeNetShadow AuthorizeNetShadowT"></div>
  <div class="AuthorizeNetShadow AuthorizeNetShadowR"></div>
  <div class="AuthorizeNetShadow AuthorizeNetShadowB"></div>
  <div class="AuthorizeNetShadow AuthorizeNetShadowL"></div>
  <div class="AuthorizeNetShadow AuthorizeNetShadowTR"></div>
  <div class="AuthorizeNetShadow AuthorizeNetShadowBR"></div>
  <div class="AuthorizeNetShadow AuthorizeNetShadowBL"></div>
  <div class="AuthorizeNetShadow AuthorizeNetShadowTL"></div>
</div>


<!-- 
INSTRUCTIONS:
Put this divAuthorizeNetPopupScreen section right before the closing </body> tag.
-->
<div id="divAuthorizeNetPopupScreen" style="display:none;"></div>

<script type="text/javascript">
    document.getElementById("ButtonPopUp").click();
</script>
     