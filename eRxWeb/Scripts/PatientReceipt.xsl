<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <html>
      <body class="LeftMargin">
        <div style="height:92%;width:90%">
          <table width="100%">
            <tr>
              
              <td class="center">
                <table style="border:solid 1px black;width:auto;">
                  <tr>
                    <td class="NormalCenter">
                      <xsl:value-of select="NewDataSet/Practice/Name"/>
                    </td>
                  </tr>
                  <tr>
                    <td class="NormalCenter">
                      <xsl:value-of select="NewDataSet/Practice/Address"/>
                      <xsl:value-of select="NewDataSet/Practice/State"/>
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="NewDataSet/Practice/Zip"/>
                    </td>
                  </tr>
                  <tr>
                    <td class="NormalCenter">
                      <areacode>
                        <xsl:value-of select="substring(NewDataSet/Practice/Phone,1,5)"/>
                      </areacode>
                      <phonenumber>
                        <xsl:value-of select="substring(NewDataSet/Practice/Phone,6,3)"/>-<xsl:value-of select="substring(NewDataSet/Practice/Phone,9,4)"/>
                      </phonenumber>
                    </td>                    
                  </tr>
                </table>
              </td>
              <td style="font-weight:bold;width:50%;font-size:16px;">
                Copy Only - not valid for dispensing.
              </td>
            </tr>
          </table>
          <table width="100%">
            <tr>
              <td style="font-weight:bold;width:50%;font-size:16px;">
                Patient Informational Copy
              </td>
              <td style="font-weight:bold;width:50%;">
                Printed By :
                <xsl:value-of select="NewDataSet/Practice/LoginUser"/>
                <xsl:text> </xsl:text>
                <xsl:value-of select="NewDataSet/Practice/CurrentDate"/>
              </td>
            </tr>
          </table>

          <table style="width:100%">
            <tr>
              <td>
                <hr />
              </td>
            </tr>
          </table>
          <table width="100%">
            <tr>
              <td width="50%">
                <table>
                  <tr>
                    <td>Patient Name :</td>
                    <td>
                      <xsl:value-of select="NewDataSet/Patient/FirstName"/>
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="NewDataSet/Patient/LastName"/>
                    </td>
                  </tr>
                </table>
              </td>
              <td width="50%">
                <table>
                  <tr>
                    <td>
                      Provider Name :
                    </td>
                    <td>
                      <xsl:value-of select="NewDataSet/Provider/Title"/>
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="NewDataSet/Provider/FirstName"/>
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="NewDataSet/Provider/LastName"/>
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="NewDataSet/Provider/Suffix"/>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
          </table>

          <xsl:if test="NewDataSet/RetailPharmacy">
            <table width="100%">
              <tr>
                <td style="font-size:10px; font-weight:bold;font-family:Verdana;">
                  The following medications were sent to:
                </td>
              </tr>
              <tr>
                <td>
                  <xsl:value-of select="NewDataSet/RetailPharmacy/Name"/>
                  <xsl:text> </xsl:text>
                  <xsl:value-of select="NewDataSet/RetailPharmacy/Address"/>
                  <xsl:text> </xsl:text>
                  <xsl:value-of select="NewDataSet/RetailPharmacy/State"/>
                  <xsl:text> </xsl:text>
                  <xsl:value-of select="NewDataSet/RetailPharmacy/Zip"/>
                </td>
              </tr>
            </table>
            <table width="100%">
              <tr style="background-color:#BBBBBB">
                <td style="color:black;width:35%;font-weight:bold;font-size:9px;font-family:Verdana;background-color:#BBBBBB;">
                  Medication
                </td>
                <td style="color:black;width:35%;font-weight:bold;font-size:9px;font-family:Verdana;background-color:#BBBBBB;">
                  Directions
                </td>
                <td style="color:black;width:35%;font-weight:bold;font-size:9px;font-family:Verdana;background-color:#BBBBBB;">
                  Quantity
                </td>
                <td style="color:black;width:35%;font-weight:bold;font-size:9px;font-family:Verdana;background-color:#BBBBBB;">
                  Refills
                </td>
              </tr>
              <xsl:for-each select="NewDataSet/RetailPrescription">
                <tr>
                  <td>
                    <xsl:value-of select="MedName"/>
                  </td>
                  <td>
                    <xsl:value-of select="SIGText"/>
                  </td>

                  <td>
                    <xsl:value-of select="Quantity"/>
                  </td>
                  <td>
                    <xsl:value-of select="RefillQuantity1"/>
                  </td>

                </tr>
              </xsl:for-each>
            </table>
          </xsl:if>
          <xsl:if test="NewDataSet/MailOrderPharmacy">
            <table width="100%">
              <tr>
                <td style="font-size:12px; font-weight:bold;font-family:Verdana;">
                  The following medications were sent to:
                </td>
              </tr>
              <tr>
                <td>
                  <xsl:value-of select="NewDataSet/MailOrderPharmacy/Name"/>
                  <xsl:text> </xsl:text>
                  <xsl:value-of select="NewDataSet/MailOrderPharmacy/Address"/>
                  <xsl:text> </xsl:text>
                  <xsl:value-of select="NewDataSet/MailOrderPharmacy/State"/>
                  <xsl:text> </xsl:text>
                  <xsl:value-of select="NewDataSet/MailOrderPharmacy/Zip"/>
                </td>
              </tr>
            </table>
            <table width="100%">
              <tr style="background-color:#BBBBBB">
                <td style="color:black;width:35%;font-weight:bold;font-size:10px;font-family:Verdana;">
                  Medication
                </td>
                <td style="color:black;width:35%;font-weight:bold;font-size:10px;font-family:Verdana;">
                  Directions
                </td>
                <td style="color:black;width:35%;font-weight:bold;font-size:10px;font-family:Verdana;">
                  Quantity
                </td>
                <td style="color:black;width:35%;font-weight:bold;font-size:10px;font-family:Verdana;">
                  Refills
                </td>
              </tr>
              <xsl:for-each select="NewDataSet/MailOrderPrescription">
                <tr>
                  <td>
                    <xsl:value-of select="MedName"/>
                  </td>
                  <td>
                    <xsl:value-of select="SIGText"/>
                  </td>

                  <td>
                    <xsl:value-of select="Quantity"/>
                  </td>
                  <td>
                    <xsl:value-of select="RefillQuantity1"/>
                  </td>

                </tr>
              </xsl:for-each>
            </table>
          </xsl:if>
          <xsl:if test="NewDataSet/PrintedPrescription">
            <table width="100%">
              <tr>
                <td style="font-size:10px; font-weight:bold;font-family:Verdana;">
                  The following medications were printed:
                </td>
              </tr>
            </table>
            <table width="100%">
              <tr style="background-color:#BBBBBB;">
                <td style="color:black;width:35%;font-weight:bold;font-size:9px;font-family:Verdana;">
                  Medication
                </td>
                <td style="color:black;width:35%;font-weight:bold;font-size:9px;font-family:Verdana;">
                  Directions
                </td>
                <td style="color:black;width:35%;font-weight:bold;font-size:9px;font-family:Verdana;">
                  Quantity
                </td>
                <td style="color:black;width:35%;font-weight:bold;font-size:9px;font-family:Verdana;">
                  Refills
                </td>
              </tr>
              <xsl:for-each select="NewDataSet/PrintedPrescription">
                <tr>
                  <td>
                    <xsl:value-of select="MedName"/>
                  </td>
                  <td>
                    <xsl:value-of select="SIGText"/>
                  </td>

                  <td>
                    <xsl:value-of select="Quantity"/>
                  </td>
                  <td>
                    <xsl:value-of select="RefillQuantity1"/>
                  </td>

                </tr>
              </xsl:for-each>
            </table>
          </xsl:if>
          <table style="width:100%">
            <tr>
              <td>
                <hr />
              </td>
            </tr>
          </table>
          <table width="100%">
            <tr>
              <td style="color:black;font-weight:bold;">
                Disclaimer:
                <xsl:text> </xsl:text>
              </td>
              <td style="color:black;">
                This is not a prescription. This is an informational copy of medications that have been prescribed and/or sent electronically to the pharmacy during the patient's visit with the doctor.
              </td>
            </tr>
            </table>

        </div>
      </body>
    </html>
  </xsl:template>

</xsl:stylesheet>

