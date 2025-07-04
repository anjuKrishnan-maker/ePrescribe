<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="html" indent="yes" version="4.0"/>
  <xsl:param name="name" ></xsl:param>
  <xsl:param name="shieldActivationCodeURL"></xsl:param>
  <xsl:param name="activationCode" ></xsl:param>
  <xsl:param name="shieldHelpURL" ></xsl:param>
  <xsl:template match="/">
    <html>
      <head>
        <title></title>
      </head>
      <body style="font-family:Arial">
        <div style="background-color:#2F2257; width:100%; padding:10px">
          <span style="font-size:x-large; color:White; font-weight:bold">Your New User Activation Code</span>
        </div>
        <div style="width:100%">
          <div style="float:left; width:70%; padding:10px">
            <p>
              <xsl:value-of select="$name"/>,
            </p>
            <p>
              Your new user account setup is nearly complete. You must now follow the instructions below to complete the setup of your new account.
            </p>
            <p>
              Instructions for completing your new user account setup:
            </p>
            <ol>
              <li style="padding-bottom:10px">
                Navigate to <span style="text-decoration:underline; color:Blue">
                  <xsl:value-of select="$shieldActivationCodeURL"/>
                </span>
              </li>
              <li>
                <p>Enter your personal Activation Code.</p>
                <p style="padding-left:30px; font-weight: bold; color:Maroon">
                  Activation Code: <xsl:value-of select="$activationCode"/>
                </p>
              </li>
              <li style="padding-bottom:10px">
                Follow the onscreen instructions to complete the activation process.
              </li>
            </ol>
            <p>
              During this process you will be asked to create an ePrescribe Login ID and password.
            </p>
            <p>

              <b>What are the password requirements?</b>
              <ul>
                <li>At least 8 characters</li>
                <li>At least 3 of the 4:</li>
                <ul>
                  <li style="list-style: square inside">1 upper case letter</li>
                  <li style="list-style: square inside">1 lower case letter</li>
                  <li style="list-style: square inside">1 numeric character</li>
                  <li style="list-style: square inside">1 punctuation character – Examples: !#$.?</li>
                </ul>
                <li>Example: ILoveePrescribe!</li>
              </ul>
            </p>

            <p>
              <b>Security Questions:</b>
            </p>

            <p>You will be asked to answer 3 security questions.  These questions are used to identify you if you need to use the Forgot Password functionality.</p>

            <p>Please take note of the questions you chose and your answers.  Answers are case sensitive.</p>

            <p>Need help? See below for help resources.</p>

            <ul>
              <li>
                Email support available at <a href="mailto:eprescribesupport@allscripts.com">eprescribesupport@allscripts.com</a>
              </li>
            </ul>

            <p>After you register be sure to contact support to learn more about our “NEW” mobile app for Android and iOS devices!</p>

          </div>

        </div>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
