<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:output method="html" indent="yes"/>
  <xsl:param name="purchaseModuleHeader" ></xsl:param>
  <xsl:param name="userCountString"></xsl:param>
  <xsl:param name="epcsString" ></xsl:param>
  <xsl:param name="amount" ></xsl:param>
  <xsl:param name="lastcardNo" ></xsl:param>
  <xsl:param name="transactionIdString" ></xsl:param>
  <xsl:param name="helpsiteurl"></xsl:param>

  <xsl:template match="/">
    <html>
      <head>
        <style type="text/css">
          .colour{
          color : #2F2257
          }
        </style>
      </head>
      <body style="font-family: Calibri">
        <p>
          <strong> </strong>
        </p>
        <p>
          <strong>
            <span class = "colour">Thank you for your purchase of Veradigm ePrescribe EPCS - the simple, safe and secure e-prescribing solution!</span>
          </strong>
        </p>
        <p>
          Your <xsl:value-of select="$purchaseModuleHeader"/> includes monthly access for <xsl:value-of select="$userCountString"/> user(s)<xsl:value-of select ="$epcsString"/>. 
        </p>
        <p>
          <br/>
          Total charges incurred:
          <xsl:value-of select="$amount"/>
          <br/>
          <br/>
          Credit Card: xxxx xxxx xxxx
          <xsl:value-of select="$lastcardNo"/>
          <br/>
          <br/>
          <xsl:if test="string-length($transactionIdString) > 0">
            Your order confirmation number is
            <xsl:value-of select="$transactionIdString"/>
          </xsl:if>
        </p>
        <p>
          This email will provide an overview of the steps required to complete the EPCS setup process.
        </p>

        <strong>Setting up EPCS will involve executing in sequence the following actions:</strong>
        <ol>
          <li>
            Administrator assigns provider(s) the rights to <strong>
              <span class = "colour"> Register for EPCS. </span>
            </strong>
          </li>
          <li>
            Provider goes through <strong>
              <span class = "colour">online identification </span>
            </strong>and authentication process.
          </li>
          <li>
            Administrator assigns selected DEA Registrant(s), the <strong>
              <span class = "colour">EPCS Approver </span>
            </strong>role.
          </li>
          <li>
            EPCS Approver <strong>
              <span class = "colour" >grants final approval </span>
            </strong>to provider(s) for EPCS.
          </li>
        </ol>
        <p>
          <strong>Note</strong>
          : For a single provider practice, the provider will complete both the provider and approver roles.
        </p>
        <p>
          Our
          <font size= "4">
            <strong>
              <a href="{$helpsiteurl}">
                <span class="colour"> Help Site </span>
              </a>

            </strong>
          </font>
          within the ePrescribe application, contains informative tutorials to introduce new users to    <strong>
            <span class="colour">ePrescribe </span>
          </strong>as well as further details on <strong>
            <span class="colour">EPCS</span>
          </strong>. The Help Site is accessed through the green icon located
          in the upper right hand side of ePrescribe once a user has logged in.
        </p>
        <p>
          The ID.me <strong>
            <span class="colour">Video Tutorials </span>
          </strong>can be found in our iLearn section:
        </p>
        <ol type="1">
          <li>
            <strong>Login into ePrescribe</strong>
          </li>
          <li>
            <strong>Click on the Graduation Cap icon</strong>
          </li>
          <li>
            <strong>Click on the Workflow Tutorials</strong>
          </li>
          <li>
            <strong>Type in Keyword EPCS</strong>
          </li>
          <li>
            <strong>All videos pertaining to EPCS registration will display</strong>
          </li>
        </ol>
        <p>
          <strong>
            <font color = "0431B4"> Please open the attachment linked to this email which contains the instructions on how to get started using EPCS!</font>
          </strong>
        </p>
        <p>
          <br/>
          We hope you enjoy the enhanced features of Veradigm ePrescribe Deluxe.
        </p>
        <p>
          If you have any questions or problems, please contact us!
        </p>
        Thank you,
        <br/>
        <strong>The ePrescribe SupportTeam</strong><br/>
        T 877-933-7274<br/>
        <a href="mailto:eprescribesupport@allscripts.com">
          <u>eprescribesupport@allscripts.com</u>
        </a>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
