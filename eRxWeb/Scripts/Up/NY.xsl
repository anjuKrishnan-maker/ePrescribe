<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:param name="MedicationQty" ></xsl:param>
  <xsl:template match="/">
    <html>
      <head>
        <title>Allscripts ePrescribe</title>
      </head>
      <body class="LeftMargin">
        <table width="100%"  border="0" cellspacing="0" cellpadding="0" style="overflow:hidden">
          <tr>
            <td class="NormalCenter">
              <xsl:value-of select="NewDataSet/Practice/Name"/>
            </td>
          </tr>
          <tr>
            <td class="NormalCenterSmall">
              <xsl:value-of select="NewDataSet/Practice/Address"/>
              <xsl:value-of select="NewDataSet/Practice/State"/>
              <xsl:text> </xsl:text>
              <xsl:value-of select="NewDataSet/Practice/Zip"/>
            </td>
          </tr>
          <tr>
            <td class="NormalCenterSmall">
              <areacode>
                <xsl:value-of select="substring(NewDataSet/Practice/Phone,1,5)"/>
              </areacode>
              <phonenumber>
                <xsl:value-of select="substring(NewDataSet/Practice/Phone,6,3)"/>-<xsl:value-of select="substring(NewDataSet/Practice/Phone,9,4)"/>
              </phonenumber>
            </td>
          </tr>
          <tr>
            <td>
              <table width="100%">
                <tr>
                  <td class="headName" width="50%">
                    <xsl:value-of select="NewDataSet/Provider/Title"/>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Provider/FirstName"/>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Provider/LastName"/>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Provider/Suffix"/>
                  </td>
                  <td class="leftboldsmall"  width="50%">
                    <xsl:value-of select="NewDataSet/PhysicianAssistant/Title"/>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/PhysicianAssistant/FirstName"/>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/PhysicianAssistant/LastName"/>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/PhysicianAssistant/Suffix"/>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          <tr>
            <td>
              <table width="100%">
                <tr>
                  <td class="leftboldsmall" width="50%">
                    <xsl:value-of select="NewDataSet/Provider/State"/>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Provider/LicenseNumber"/>
                  </td>
                  <td class="leftboldsmall" width="50%">
                    <xsl:value-of select="NewDataSet/PhysicianAssistant/State"/>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/PhysicianAssistant/LicenseNumber"/>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          <tr>
            <td>
              <table width="100%">
                <tr>
                  <td class="leftboldsmall" width="50%">
                    <xsl:if test="NewDataSet/Provider/UPIN !=''">NPI: </xsl:if>
                    <xsl:value-of select="NewDataSet/Provider/UPIN"/>
                  </td>
                  <td class="leftboldsmall" width="50%">
                    <xsl:if test="NewDataSet/PhysicianAssistant/UPIN !=''">NPI: </xsl:if>
                    <xsl:value-of select="NewDataSet/PhysicianAssistant/UPIN"/>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          <tr>
            <td>
              <table width="100%">
                <tr>
                  <td class="leftboldsmall" width="50%">
                    <xsl:if test="NewDataSet/Provider/DEANumber !=''">DEA: </xsl:if>
                    <xsl:value-of select="NewDataSet/Provider/DEANumber"/>
                  </td>
                  <td class="leftboldsmall" width="50%">
                    <xsl:if test="NewDataSet/PhysicianAssistant/DEANumber !=''">DEA: </xsl:if>
                    <xsl:value-of select="NewDataSet/PhysicianAssistant/DEANumber"/>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          <tr>
            <td>
              <table width="100%" border="0" cellspacing="0" cellpadding="0">
                <tr>
                  <td width="18%">Name</td>
                  <td width="49%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="0" class="table">
                      <tr>
                        <td width="100%" class="headName">
                          <xsl:value-of select="NewDataSet/Patient/FirstName"/>
                          <xsl:text> </xsl:text>
                          <xsl:value-of select="NewDataSet/Patient/LastName"/>
                        </td>
                      </tr>
                    </table>
                  </td>
                  <td width="15%">DOB</td>
                  <td width="18%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="0" class="table">
                      <tr>
                        <td width="100%" class="DOB">
                          <xsl:value-of select="NewDataSet/Patient/DOB"/>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
                <tr>
                  <td width="18%">Address</td>
                  <td colspan="3">
                    <table width="100%" border="0" cellpadding="0" cellspacing="0" class="table">
                      <tr>
                        <td width="100%" class="subheading">
                          <xsl:value-of select="NewDataSet/Patient/Address"/>
                          <xsl:value-of select="NewDataSet/Patient/State"/>
                          <xsl:text> </xsl:text>
                          <xsl:value-of select="NewDataSet/Patient/Zip"/>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          <tr>
            <td class="NormalCenter">
              <xsl:value-of select="NewDataSet/Prescription/MedicationName"/>
              <xsl:text> </xsl:text>
              <xsl:value-of select="NewDataSet/Prescription/Strength"/>
              <xsl:text> </xsl:text>
              <xsl:value-of select="NewDataSet/Prescription/StrengthUOM"/>
              <xsl:text> </xsl:text>
              <xsl:value-of select="NewDataSet/Prescription/RouteOfAdministration"/>
              <xsl:text> </xsl:text>
              <xsl:value-of select="NewDataSet/Prescription/DosageDescription"/>
            </td>
          </tr>
          <tr>
            <td class="center">
              <xsl:value-of select="$MedicationQty"></xsl:value-of>
            </td>
          </tr>
          <tr>
            <td class="NormalCenterSmall">
              <xsl:value-of select="NewDataSet/Prescription/SIGText"/>
            </td>
          </tr>
          <tr>
            <!-- Space between sig and signature line -->
            <td height="40">
            </td>
          </tr>
          <tr>
            <td>
              <table width="100%"  border="0" cellspacing="0" cellpadding="0">
                <tr>
                  <td width="20%">
                    <xsl:text> </xsl:text>
                  </td>
                  <td width="60%">
                    <table width="100%" border="0" cellspacing="0" cellpadding="0" class="table">
                      <tr>
                        <td>
                          <xsl:text> </xsl:text>
                        </td>
                      </tr>
                    </table>
                  </td>
                    <td width="20%">
                      <xsl:text> </xsl:text>
                    </td>
                </tr>
                <tr>
                  <td width="20%"></td>
                  <td width="60%" class="NormalCenterSmall">Signature of Prescriber</td>
                  <td width="20%"></td>
                </tr>
                <tr>
                  <td width="100%" colspan="3" class="NYDAW">
                    THIS PRESCRIPTION WILL BE FILLED GENERICALLY UNLESS PRESCRIBER WRITES 'd.a.w.' IN THE BOX BELOW.
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          <tr>
            <td>
              <table width="100%" border="0" cellspacing="0" cellpadding="0">
                <tr>
                  <td width="20%"></td>
                  <td width="25%">
                    <table width="100%" height="50"  border="0" cellpadding="0" cellspacing="0" class="tableAll">
                      <xsl:if test="NewDataSet/Prescription/DAW='Y'">
                        <tr>
                          <td width="100%" class="middle">d.a.w.</td>
                        </tr>
                      </xsl:if>
                      <xsl:if test="NewDataSet/Prescription/DAW='N'">
                        <tr>
                          <td width="100%"></td>
                        </tr>
                      </xsl:if>
                    </table>
                  </td>
                  <td width="10%"></td>
                  <td width="25%">
                    <table width="100%" height="50"  border="0" cellpadding="0" cellspacing="0" class="tableAll">
                      <tr>
                        <td width="100%"></td>
                      </tr>
                    </table>
                  </td>
                  <td width="20%"></td>
                </tr>
                <tr>
                  <td width="20%"></td>
                  <td width="25%" class="NYDAW2">Dispense As Written</td>
                  <td width="10%"></td>
                  <td width="25%" class="NYDAW2">MDD</td>
                  <td width="20%"></td>
                </tr>
              </table>
            </td>
          </tr>
          <tr>
            <td>
              <table width="100%"  border="0" cellspacing="0" cellpadding="0">
                <tr>
                  <td width="12%" class="BottomItallic">Written:</td>
                  <td width="14%" class="BottomItallic">
                    <xsl:value-of select="NewDataSet/Prescription/Created"/>
                  </td>
                  <td width="50%" align="center" class="BottomItallic">
                    Refills Authorized <xsl:value-of select="NewDataSet/Prescription/RefillQuantity"/> Time(s)
                  </td>
                  <td width="6%" class="BottomItallic">Rx:</td>
                  <td width="10%">
                    <table width="100%" border="0" cellpadding="0" cellspacing="0" class="table">
                      <tr style="vertical-align:bottom">
                        <td width="100%" class="BottomCenter">
                          <xsl:value-of select="NewDataSet/Prescription/RxNumber"/>
                        </td>
                      </tr>
                    </table>
                  </td>
                  <td width="9%" class="BottomItallic">
                    <xsl:value-of select="NewDataSet/Prescription/CSCode"/>
                  </td>
                </tr>
              </table>
              <table width="100%"   border="0" cellspacing="0" cellpadding="0" bordercolor="red">
                <tr>
                  <td width="12%" class="Itallic" align="left">
                    <xsl:choose>
                      <xsl:when test="NewDataSet/Prescription/WorkFlowType=2">
                        Do Not Fill Before: <xsl:value-of select="NewDataSet/Prescription/StartDate"/>
                      </xsl:when>
                      <xsl:otherwise>
                      </xsl:otherwise>
                    </xsl:choose>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          <!--  Pharmacy Notes is getting printed below -->
          <!--  MEW - Need for force pharmacy comments onto the 2nd "page" of the script paper to avoid 
					      writing over the bar code on the bottom of the script. -->
          <tr>
            <td height="20">
            </td>
          </tr>
          <!-- The following is for the CMS requirements for medicare reimbursement -->
          <tr>
            <td class="footer" align="center">
              Security Features: 1. (**) bordered and spelled quantities; 2. (**) bordered number of refills; 3. This list of security features
            </td>
          </tr>
          <!--  diagnosis code and diagnosis description -->
          <xsl:if test="NewDataSet/Prescription/ICD10Code !=''">
            <tr>
              <td class="centerBold">
                <xsl:value-of select="NewDataSet/Prescription/ICD10Code"/>
                <xsl:if test="NewDataSet/Prescription/ICD10Description !=''"> - </xsl:if>
                <xsl:value-of select="NewDataSet/Prescription/ICD10Description"/>
              </td>
            </tr>
          </xsl:if>
          <xsl:if test="NewDataSet/Prescription/PharmacyNotes !=''">
            <tr>
              <td width="100%">
                <table width="100%" border="0" cellspacing="0" cellpadding="0">
                  <td class="Itallic" width="15%">Pharmacy Comments:</td>
                  <td class="Normal" width="85%" style="text-align:left; overflow:hidden;">
                    <xsl:value-of select="NewDataSet/Prescription/PharmacyNotes"/>
                  </td>
                </table>
              </td>
            </tr>
          </xsl:if>
        </table>
      </body>
    </html>
</xsl:template>
</xsl:stylesheet> 

