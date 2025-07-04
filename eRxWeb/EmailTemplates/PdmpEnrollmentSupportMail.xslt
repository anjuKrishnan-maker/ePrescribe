<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:output method="html" indent="yes"/>
  <xsl:param name="OrderID" ></xsl:param>
  <xsl:param name="Product" ></xsl:param>
  <xsl:param name="AccountName" ></xsl:param>
  <xsl:param name="AccountNumber" ></xsl:param>
  <xsl:param name="Address1" ></xsl:param>
  <xsl:param name="Address2" ></xsl:param>
  <xsl:param name="City" ></xsl:param>
  <xsl:param name="State" ></xsl:param>
  <xsl:param name="ZipCode" ></xsl:param>
  <xsl:param name="ContactNameAndTitle" ></xsl:param>
  <xsl:param name="ContactNumber" ></xsl:param>
  <xsl:param name="ContactFax" ></xsl:param>
  <xsl:param name="ContactEmail" ></xsl:param>
  <xsl:param name="NumberOfProviders" ></xsl:param>

  <xsl:template match="@* | node()">
    <html>
      <head>
      </head>
      <body style="font-family: Calibri">
        <h1>PDMP Enrollment Details</h1>
        Order ID: <xsl:value-of select="$OrderID"/>
        <br/><br/>
        Veradigm Product: <xsl:value-of select="$Product"/>
        <br/><br/>
        Account Name: <xsl:value-of select="$AccountName"/>
        <br/><br/>
        Account Number: <xsl:value-of select="$AccountNumber"/>
        <br/><br/>
        Address: <xsl:value-of select="$Address1"/>,
        <xsl:if test="string-length($Address2) > 0">
          <br/><xsl:value-of select="$Address2"/>,
        </xsl:if>
              <br/><xsl:value-of select="$City"/>,
              <br/><xsl:value-of select="$State"/> - <xsl:value-of select="$ZipCode"/>
        <br/><br/>
        Contact Name: <xsl:value-of select="$ContactNameAndTitle"/>
        <br/><br/>
        Contact Number: <xsl:value-of select="$ContactNumber"/>
        <br/><br/>
        Contact Fax: <xsl:value-of select="$ContactFax"/>
        <br/><br/>
        Contact Email: <xsl:value-of select="$ContactEmail"/>
        <br/><br/>
        Number of Providers: <xsl:value-of select="$NumberOfProviders"/>           
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
