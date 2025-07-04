<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:param name="MedicationQty" ></xsl:param>
  <xsl:template match="/">
    <html>
      <head>
        <title>Allscripts ePrescribe</title>
      </head>
      <body class="LeftMargin">
        <table width="340" border="0" cellspacing="0" cellpadding="0"  bordercolor="blue">
          <tr>
            <td height="130">
              <table border="0" cellpadding="0" cellspacing="0" width="100%">
                <tr>
                  <td>&#xA0;</td>
                </tr>
                <tr>
                  <td align="center" class="CenterBold">
                    PRESCRIPTION BLANK
                  </td>
                </tr>
                <tr>
                  <td>
                    <hr noshade="noshade" style="color:#000;"></hr>
                  </td>
                </tr>
                <tr>
                  <td>
                    <table width="100%">
                      <tr>
                        <td class="NormalCenterCA2" style="text-align:center;text-transform:uppercase;font-weight:normal;">
                          <xsl:value-of select="NewDataSet/Provider/Title"/>
                          <xsl:text> </xsl:text>
                          <xsl:value-of select="NewDataSet/Provider/FirstName"/>
                          <xsl:text> </xsl:text>
                          <xsl:value-of select="NewDataSet/Provider/LastName"/>
                          <xsl:text> </xsl:text>
                          <xsl:value-of select="NewDataSet/Provider/Suffix"/>
                          <xsl:value-of select="NewDataSet/PhysicianAssistant/Title"/>
                          <xsl:text> </xsl:text>
                          <xsl:value-of select="NewDataSet/PhysicianAssistant/FirstName"/>
                          <xsl:text> </xsl:text>
                          <xsl:value-of select="NewDataSet/PhysicianAssistant/LastName"/>
                          <xsl:text> </xsl:text>
                          <xsl:value-of select="NewDataSet/PhysicianAssistant/Suffix"/>
                          <br></br>
                          <xsl:value-of select="NewDataSet/PhysicianAssistant/State"/>
                          <xsl:text> </xsl:text>
                          <xsl:value-of select="NewDataSet/PhysicianAssistant/LicenseNumber"/>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
                <tr>
                  <td class="NormalCenterCA2" style="text-align:center;text-transform:uppercase;font-weight:normal;">
                    <xsl:value-of select="NewDataSet/Practice/Name"/>
                  </td>
                </tr>
                <tr>
                  <tr>
                    <td class="NormalCenterCA2" style="text-align:center;text-transform:uppercase;font-weight:normal;">
                      <xsl:value-of select="NewDataSet/Practice/Address1"/>
                      <xsl:value-of select="NewDataSet/Practice/Address2"/>
                    </td>
                  </tr>
                  <tr>
                    <td class="NormalCenterCA2" style="text-align:center;text-transform:uppercase;font-weight:normal;">
                      <xsl:value-of select="NewDataSet/Practice/City"/>,
                      <xsl:value-of select="NewDataSet/Practice/State"/>
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="NewDataSet/Practice/Zip"/>
                    </td>
                  </tr>
                </tr>
                <tr>
                  <td class="small">
                    <table width="100%"  border="0" cellpadding="0" cellspacing="0">
                      <tr>
                        <td class="small" width="33%">
                          <xsl:if test="NewDataSet/Provider/UPIN !=''">NPI: </xsl:if>
                          <xsl:value-of select="NewDataSet/Provider/UPIN"/>
                          <xsl:text> </xsl:text>
                          <xsl:text> </xsl:text>
                          <xsl:if test="NewDataSet/Provider/DEANumber !=''">DEA: </xsl:if>
                          <xsl:value-of select="NewDataSet/Provider/DEANumber"/>
                          <xsl:text> </xsl:text>
                          <xsl:text> </xsl:text>
                          <xsl:if test="NewDataSet/PhysicianAssistant/UPIN !=''">NPI: </xsl:if>
                          <xsl:value-of select="NewDataSet/PhysicianAssistant/UPIN"/>
                          <xsl:text> </xsl:text>
                          <xsl:text> </xsl:text>
                          <xsl:if test="NewDataSet/PhysicianAssistant/DEANumber !=''">DEA: </xsl:if>
                          <xsl:value-of select="NewDataSet/PhysicianAssistant/DEANumber"/>
                        </td>
                        <td class="small" align="center"  width="34%">
                          <xsl:value-of select="substring(NewDataSet/Practice/Phone,1,5)"/>
                          <xsl:value-of select="substring(NewDataSet/Practice/Phone,6,3)"/>-<xsl:value-of select="substring(NewDataSet/Practice/Phone,9,4)"/>
                        </td>
                        <td class="small" width="33%" align="right">
                          <xsl:value-of select="NewDataSet/Provider/State"/>
                          <xsl:text> </xsl:text>
                          <xsl:value-of select="NewDataSet/Provider/LicenseNumber"/>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
                <tr>
                  <td class="centerjustify" style="text-align:center;">
                    IF PRESCRIPTION IS WRITTEN AT ALTERNATE PRACTICE SITE CHECK HERE <input type="checkbox"></input><BR></BR>
                    AND PRINT ALTERNATE ADDRESS AND TELEPHONE NUMBER ON REVERSE SITE
                  </td>
                </tr>
                <tr>
                  <td>
                    <hr noshade="noshade" style="color:#000;"></hr>
                  </td>
                </tr>
              </table>
            </td>
            <!-- 03/09/2007 MEW mods to support NJ format -->
          </tr>
          <tr>
            <td>
              <table width="340" border="0" cellspacing="0" cellpadding="0">
                <td width="55" class="Normal">
                  <!-- Space for preprinted "Patient" heading l-->
                  <xsl:text>PATIENT</xsl:text>
                </td>
                <td width="350" class="headName">
                  <table width="100%"  border="0" cellpadding="0" cellspacing="0" class="table">
                    <tr>
                      <td class="Normal" style="text-align:left;">
                        &#xA0;&#xA0;
                        <xsl:value-of select="NewDataSet/Patient/FirstName"/>
                        <xsl:text> </xsl:text>
                        <xsl:value-of select="NewDataSet/Patient/LastName"/>
                      </td>
                    </tr>
                  </table>&#xA0;
                </td>
                <td width="50" class="Normal">
                  <!-- Space for preprinted birthdate label -->
                  <xsl:text>D.O.B.</xsl:text>
                </td>
                <td width="40">
                  <table width="100%"  border="0" cellpadding="0" cellspacing="0" class="table">
                    <tr>
                      <td class="Normal">
                        <xsl:value-of select="NewDataSet/Patient/DOB"/>
                      </td>
                    </tr>
                  </table>

                </td>
              </table>
            </td>
          </tr>
          <tr>
            <td height="5">
              <xsl:text> </xsl:text>
            </td>
          </tr>
          <tr>
            <td>
              <table width="340" border="0" cellspacing="0" cellpadding="0" >
                <tr>
                  <td width="40" class="Normal">
                    ADDRESS
                  </td>
                  <td width="370" class="subheading">
                    <table width="100%"  border="0" cellpadding="0" cellspacing="0" class="table">
                      <tr>
                        <td class="Normal">
                          <xsl:value-of select="NewDataSet/Patient/Address"/>
                          <xsl:value-of select="NewDataSet/Patient/State"/>
                          <xsl:text> </xsl:text>
                          <xsl:value-of select="NewDataSet/Patient/Zip"/>
                        </td>
                      </tr>
                    </table>
                  </td>
                  <td width="50" class="Normal">
                    DATE
                  </td>
                  <td width="40" class="DOB">
                    <table width="100%"  border="0" cellpadding="0" cellspacing="0" class="table">
                      <tr>
                        <td class="Normal">
                          <!-- Print the current prescription date.-->
                          <xsl:value-of select="NewDataSet/Prescription/Created"/>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          <tr>
            <td height="70">
              <br></br>
              <table border="0" cellpadding="0" cellspacing="0" width="100%">
                <tr valign="middle">
                  <td>
                    <table border="3" cellpadding="0"  style="border-collapse:collapse;border-color:black;"  width="70">
                      <tr valign="middle">
                        <td align="center"  style="font-size:24pt;font-weight:bold;">Rx</td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          <tr>
            <td>
              <table width="340" cellspacing="0" cellpadding="0" border="0">
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
                  <td height="17" class="center">
                    <xsl:value-of select="$MedicationQty"></xsl:value-of>
                  </td>
                </tr>
                <tr>
                  <td height="17" class="NormalCenterSmall">
                    <xsl:value-of select="NewDataSet/Prescription/SIGText"/>

                  </td>
                </tr>
                <tr>
                  <td>
                    <table border="0" cellpadding="0" cellspacing="0" width="100%">
                      <tr>
                        <td class="leftboldsmall" style="vertical-align:bottom;font-size:8.5px;font-weight:normal;" width="210">
                          SUBSTITUTION PERMISSIBLE
                        </td>
                        <td width="110" align="left">
                          <table class="table" width="100%">
                            <tr>
                              <td>
                                &#xA0;
                              </td>
                            </tr>
                          </table>
                        </td>
                        <td width="20">
                          &#xA0;
                        </td>
                        <td class="leftboldsmall" style="vertical-align:bottom;font-size:8.5px;font-weight:normal;" width="150">
                          DO NOT SUBSTITUTE
                        </td>
                        <td width="110" align="left">
                          <table class="table" width="100%">
                            <tr>
                              <td>
                                &#xA0;
                              </td>
                            </tr>
                          </table>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
                <tr>
                  <td>
                    &#xA0;
                  </td>
                </tr>
                <tr>
                  <td>
                    <table border="0" cellpadding="5" cellspacing="0" width="100%">
                      <tr>
                        <td style="border:solid black 2px;" class="small" width="38%">
                          <table border="0" cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                              <td>
                                <table border="0" cellpadding="0"  cellspacing="0" width="100%">
                                  <tr>
                                    <td class="leftboldsmall" style="font-weight:normal;">DO NOT REFILL</td>
                                    <td width="30">
                                      <table border="0" cellpadding="0"  cellspacing="0" width="100%" class="table">
                                        <tr>
                                          <td align="center">
                                            <xsl:if test="NewDataSet/Prescription/OriginalRefillQuantity = 0">
                                              <span class="CenterBold">X</span>
                                            </xsl:if>
                                          </td>
                                        </tr>
                                      </table>
                                    </td>
                                  </tr>
                                </table>
                              </td>
                            </tr>
                            <tr>
                              <td class="leftsmall">
                                <table border="0" cellpadding="0" cellspacing="0" width="100%">
                                  <tr>
                                    <td class="leftboldsmall" style="font-weight:normal;">REFILL</td>
                                    <td width="30">
                                      <table border="0" cellpadding="0"  cellspacing="0" width="100%" class="table">
                                        <tr>
                                          <td align="center" class="CenterBold">
                                            <xsl:if test="NewDataSet/Prescription/OriginalRefillQuantity > 0">
                                              <xsl:value-of select="NewDataSet/Prescription/RefillQuantity"/>
                                            </xsl:if>&#xA0;
                                          </td>
                                        </tr>
                                      </table>
                                    </td>
                                    <td class="leftboldsmall" style="font-weight:normal;">TIMES</td>
                                  </tr>
                                </table>
                              </td>
                            </tr>
                          </table>
                        </td>
                        <td style="border:solid black 2px;font-weight:normal;vertical-align:top;" class="leftboldsmall" >SIGNATURE OF PRESCRIBER</td>
                      </tr>
                    </table>

                  </td>
                </tr>
                <!-- The following is for the CMS requirements for medicare reimbursement -->
                <tr height="7">
                  <td>
                  </td>
                </tr>
                <tr>
                  <td class="footer" align="center">
                    Security Features: 1. (**) bordered and spelled quantities; <br></br>2. (**) bordered number of refills; <br></br>3. This list of security features
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
                <!--  Pharmacy Notes is getting printed below -->
                <xsl:if test="NewDataSet/Prescription/PharmacyNotes !=''">
                  <tr>
                    <td  height="215">
                    </td>
                  </tr>
                  <tr>
                    <table width="340" border="0" cellspacing="0" cellpadding="0">
                      <td class="Itallic">Pharmacy Comments:</td>
                      <td class="Normal">
                        <xsl:value-of select="NewDataSet/Prescription/PharmacyNotes"/>
                      </td>
                    </table>
                  </tr>
                </xsl:if>
              </table>
            </td>
          </tr>
        </table>
      </body>
    </html>
  </xsl:template>

</xsl:stylesheet>

