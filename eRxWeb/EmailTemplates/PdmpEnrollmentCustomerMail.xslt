<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:output method="html" indent="yes"/>
  <xsl:param name="OrderID" ></xsl:param>
  <xsl:param name="Product" ></xsl:param>
  <xsl:param name="AccountName" ></xsl:param>
  <xsl:param name="AccountNumber" ></xsl:param>
  <xsl:param name="ContactNameAndTitle" ></xsl:param>
  <xsl:param name="OrderConfirmMessage" ></xsl:param>
  <xsl:param name="NumberOfProviders" ></xsl:param>

  <xsl:template match="@* | node()">
    <html>
      <head>
      </head>
      <body style="font-family: Calibri">
        Hello <xsl:value-of select="$ContactNameAndTitle"/>,
        <br/>
        <br/>
        <br/>
        Your order for PDMP Enrollment for Veradigm <xsl:value-of select="$Product"/> is confirmed.
        <br/>
        OrderID: <xsl:value-of select="$OrderID"/>
        <br/>
        Account Name: <xsl:value-of select="$AccountName"/>
        <br/>
        Account Number: <xsl:value-of select="$AccountNumber"/>
        <br/>
        Number of Providers: <xsl:value-of select="$NumberOfProviders"/> 
        <br/>
        <br/>
        <br/>
        <xsl:value-of select="$OrderConfirmMessage" disable-output-escaping="yes"/> 
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
