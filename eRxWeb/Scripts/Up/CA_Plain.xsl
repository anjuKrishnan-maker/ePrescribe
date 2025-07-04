<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:param name="MedicationQty" ></xsl:param>
  <xsl:template match="/">
    <html>
      <head>
        <title>Allscripts ePrescribe</title>
      </head>
      <body class="LeftMargin">
        <table width="100%"  border="0" cellspacing="0" cellpadding="0" bordercolor="blue">
          <!-- Header Area - in CA the header is preprinted, so nothing goes here -->
          <tr>
            <td width="100%" height="20"></td>
          </tr>
          <tr>
            <td class="NormalCenter">
              <xsl:value-of select="NewDataSet/Practice/Name"/>
            </td>
          </tr>          
          <tr>
            <td class="NormalCenterSmall">
              <xsl:value-of select="NewDataSet/Practice/Address1"/>
              <xsl:value-of select="NewDataSet/Practice/Address2"/>
            </td>
          </tr>
          <tr>
            <td class="NormalCenterSmall">
              <xsl:value-of select="NewDataSet/Practice/City"/>,
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
          <tr height="10">
            <td class="headName"></td>
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
                  <td class="headName" width="50%">
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
                  <td class="centerright" width="50%">
                    <xsl:if test="NewDataSet/Provider/State !=''">Lic: </xsl:if>
                    <xsl:value-of select="NewDataSet/Provider/State"/>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Provider/LicenseNumber"/>
                  </td>
                  <td class="centerright" width="50%">
                    <xsl:if test="NewDataSet/PhysicianAssistant/State !=''">Lic: </xsl:if>
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
                  <td class="centerright" width="50%">
                    <xsl:if test="NewDataSet/Provider/UPIN !=''">NPI: </xsl:if>
                    <xsl:value-of select="NewDataSet/Provider/UPIN"/>                    
                  </td>
                  <td class="centerright" width="50%">
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
                  <td class="centerright" width="50%">
                    <xsl:if test="NewDataSet/Provider/DEANumber !=''">DEA: </xsl:if>
                    <xsl:value-of select="NewDataSet/Provider/DEANumber"/>                    
                  </td>
                  <td class="centerright" width="50%">
                    <xsl:if test="NewDataSet/PhysicianAssistant/DEANumber !=''">DEA: </xsl:if>
                    <xsl:value-of select="NewDataSet/PhysicianAssistant/DEANumber"/>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          <tr height="10">
            <td class="headName"></td>
          </tr>
          <tr>
            <td>
              <table width="100%"  border="0" cellspacing="0" cellpadding="0" bordercolor="pink">
                <tr>
                  <td width="13%" height="29">
                    <xsl:text>Name:</xsl:text>
                  </td>
                  <td colspan="3" width="87%" class="headName">
                    <table width="100%"  border="0" cellpadding="0" cellspacing="0" class="table">
                      <tr>
                        <td>
                          <xsl:value-of select="NewDataSet/Patient/FirstName"/>
                          <xsl:text> </xsl:text>
                          <xsl:value-of select="NewDataSet/Patient/LastName"/>
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
              <table width="100%" height="29" border="0" cellpadding="0" cellspacing="0">
                <tr>
                  <td width="13%" height="29">
                    <!-- Space for Name heading -->
                    <xsl:text>Address:</xsl:text>
                  </td>
                  <td width="59%" class="subheading">
                    <table width="100%"  border="0" cellpadding="0" cellspacing="0" class="table">
                      <tr>
                        <td>
                          <xsl:value-of select="NewDataSet/Patient/Address"/>
                          <xsl:value-of select="NewDataSet/Patient/State"/>
                          <xsl:text> </xsl:text>
                          <xsl:value-of select="NewDataSet/Patient/Zip"/>
                        </td>
                      </tr>
                    </table>
                  </td>
                  <td width ="11%">
                    <xsl:text>Date:</xsl:text>
                  </td>
                  <td width="16%" height="29" class="DOB">
                    <table width="100%"  border="0" cellpadding="0" cellspacing="0" class="table">
                      <tr>
                        <td>
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
          <tr height="30">
            <!-- Space between patient demographic information and med info -->
            <td></td>
          </tr>
          <tr>
            <td>
              <table width="100%" height="34" border="0" cellpadding="0" cellspacing="0">
                <tr>
                  <td width="79%" class="NormalCenter">
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
                  <td width="21%"></td>
                </tr>
                <tr>
                  <td width="79%" height="17" class="center">
                    <xsl:value-of select="$MedicationQty"></xsl:value-of>
                  </td>
                  <td width="21%"></td>
                </tr>
                <tr>
                  <td width="79%" height="17" class="NormalCenterSmall">
                    <xsl:value-of select="NewDataSet/Prescription/SIGText"/>
                  </td>
                  <td width="21%">
                    <table border="0" cellpadding="0" cellspacing="0" width="100%">
                      <tr>
                        <td>
                          <xsl:if test="NewDataSet/Prescription/Quantity &lt; 25">
                            <input type="checkbox" checked="checked"  disabled="disabled"></input>1-24
                          </xsl:if>
                          <xsl:if test="NewDataSet/Prescription/Quantity &gt; 25">
                            <input type="checkbox" disabled="disabled"></input>1-24
                          </xsl:if>
                        </td>
                      </tr>
                      <tr>
                        <td>
                          <xsl:if test="(NewDataSet/Prescription/Quantity &gt; 24) and (NewDataSet/Prescription/Quantity &lt; 50)">
                            <input type="checkbox" checked="checked"   disabled="disabled"></input>25-49
                          </xsl:if>
                          <xsl:if test="(NewDataSet/Prescription/Quantity &lt; 25)">
                            <input type="checkbox" disabled="disabled"></input>25-49
                          </xsl:if>
                          <xsl:if test="(NewDataSet/Prescription/Quantity &gt; 49)">
                            <input type="checkbox"  disabled="disabled"></input>25-49
                          </xsl:if>
                        </td>
                      </tr>
                      <tr>
                        <td>
                          <xsl:if test="(NewDataSet/Prescription/Quantity &gt; 49) and (NewDataSet/Prescription/Quantity &lt; 75)">
                            <input type="checkbox" checked="checked"   disabled="disabled"></input>50-74
                          </xsl:if>
                          <xsl:if test="(NewDataSet/Prescription/Quantity &lt; 50)">
                            <input type="checkbox"  disabled="disabled"></input>50-74
                          </xsl:if>
                          <xsl:if test="(NewDataSet/Prescription/Quantity &gt; 74)">
                            <input type="checkbox"  disabled="disabled"></input>50-74
                          </xsl:if>
                        </td>
                      </tr>
                      <tr>
                        <td>
                          <xsl:if test="(NewDataSet/Prescription/Quantity &gt; 74) and (NewDataSet/Prescription/Quantity &lt; 101)">
                            <input type="checkbox" checked="checked"   disabled="disabled"></input>75-100
                          </xsl:if>
                          <xsl:if test="(NewDataSet/Prescription/Quantity &lt; 75)">
                            <input type="checkbox"  disabled="disabled"></input>75-100
                          </xsl:if>
                          <xsl:if test="(NewDataSet/Prescription/Quantity &gt; 100)">
                            <input type="checkbox"  disabled="disabled"></input>75-100
                          </xsl:if>
                        </td>
                      </tr>
                      <tr>
                        <td>
                          <xsl:if test="(NewDataSet/Prescription/Quantity &gt; 100) and (NewDataSet/Prescription/Quantity &lt; 151)">
                            <input type="checkbox" checked="checked"   disabled="disabled"></input>101-150
                          </xsl:if>
                          <xsl:if test="(NewDataSet/Prescription/Quantity &lt; 101)">
                            <input type="checkbox"  disabled="disabled"></input>101-150
                          </xsl:if>
                          <xsl:if test="(NewDataSet/Prescription/Quantity &gt; 150)">
                            <input type="checkbox"  disabled="disabled"></input>101-150
                          </xsl:if>
                        </td>
                      </tr>
                      <tr>
                        <td>
                          <xsl:if test="(NewDataSet/Prescription/Quantity &gt; 150)">
                            <input type="checkbox" checked="checked"   disabled="disabled"></input>151 and over
                          </xsl:if>
                          <xsl:if test="(NewDataSet/Prescription/Quantity &lt; 151)">
                            <input type="checkbox" disabled="disabled"></input>151 and over
                          </xsl:if>
                        </td>
                      </tr>
                      <tr>
                        <td>
                          <table border="0" cellpadding="0" cellspacing="0" width="100%">
                            <tr>
                              <td>
                                <table border="0" cellpadding="0" cellspacing="0" width="100%" class="table">
                                  <tr>
                                    <td align="center">
                                      <xsl:value-of select="NewDataSet/Prescription/DosageDescription"/>
                                    </td>
                                  </tr>
                                </table>
                              </td>
                              <td>Units</td>
                            </tr>
                          </table>
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
              <table border="0" cellpadding="0" cellspacing="0" width="100%">
                <tr>
                  <td>
                    <table width="28%" border="0" cellpadding="0" cellspacing="0">
                      <tr valign="middle">
                        <td class="small" align="left"  width="35">Refill</td>
                        <xsl:if test="NewDataSet/Prescription/OriginalRefillQuantity = 0">
                          <td class="small" align="center" style="border:1px solid #000;"  width="25">**NR**</td>
                          <td class="small" align="center" width="15">1</td>
                          <td class="small" align="center" width="15">2</td>
                          <td class="small" align="center" width="15">3</td>
                          <td class="small" align="center" width="15">4</td>
                          <td class="small" align="center" width="15">5</td>
                        </xsl:if>
                        <xsl:if test="NewDataSet/Prescription/OriginalRefillQuantity = 1">
                          <td class="small" align="center" width="25">NR</td>
                          <td class="small" align="center" style="border:1px solid #000;"  width="15">**1**</td>
                          <td class="small" align="center" width="15">2</td>
                          <td class="small" align="center" width="15">3</td>
                          <td class="small" align="center" width="15">4</td>
                          <td class="small" align="center" width="15">5</td>
                        </xsl:if>
                        <xsl:if test="NewDataSet/Prescription/OriginalRefillQuantity = 2">
                          <td class="small" align="center" width="25">NR</td>
                          <td class="small" align="center" width="15">1</td>
                          <td class="small" align="center" style="border:1px solid #000;"  width="15">**2**</td>
                          <td class="small" align="center" width="15">3</td>
                          <td class="small" align="center" width="15">4</td>
                          <td class="small" align="center" width="15">5</td>
                        </xsl:if>
                        <xsl:if test="NewDataSet/Prescription/OriginalRefillQuantity = 3">
                          <td class="small" align="center" width="25">NR</td>
                          <td class="small" align="center" width="15">1</td>
                          <td class="small" align="center" width="15">2</td>
                          <td class="small" align="center" style="border:1px solid #000;" width="15">**3**</td>
                          <td class="small" align="center" width="15">4</td>
                          <td class="small" align="center" width="15">5</td>
                        </xsl:if>
                        <xsl:if test="NewDataSet/Prescription/OriginalRefillQuantity = 4">
                          <td class="small" align="center" width="25">NR</td>
                          <td class="small" align="center" width="15">1</td>
                          <td class="small" align="center" width="15">2</td>
                          <td class="small" align="center" width="15">3</td>
                          <td class="small" align="center" style="border:1px solid #000;" width="15">**4**</td>
                          <td class="small" align="center" width="15">5</td>
                        </xsl:if>
                        <xsl:if test="NewDataSet/Prescription/OriginalRefillQuantity = 5">
                          <td class="small" align="center" width="25">NR</td>
                          <td class="small" align="center" width="15">1</td>
                          <td class="small" align="center" width="15">2</td>
                          <td class="small" align="center" width="15">3</td>
                          <td class="small" align="center" width="15">4</td>
                          <td class="small" align="center" style="border:1px solid #000;" width="15">**5**</td>
                        </xsl:if>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          <tr>
            <td>
              <table border="0" cellpadding="0" cellspacing="0" width="100%">
                <tr>
                  <td class="small" width="17%">
                  </td>
                  <td align="left" width="34%">
                    <table border="0" cellpadding="0" cellspacing="0" width="100%" class="table">
                      <tr>
                        <td class="small" align="center">                           
                        </td>
                      </tr>
                    </table>
                  </td>
                  <td width="4%"></td>
                  <td>
                    <table border="0" cellpadding="0" cellspacing="0" width="100%" class="table">
                      <tr>
                        <td class="small">
                          <!-- this is the signature line-->&#xA0;
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
              <table border="0" cellpadding="0" cellspacing="0">
                <tr>
                  <td width ="72%" align="left" class="small">
                    <xsl:if test="NewDataSet/Prescription/DAW = 'Y'">
                      <input type="checkbox" checked="checked"  disabled="disabled"></input>
                    </xsl:if>
                    <xsl:if test="NewDataSet/Prescription/DAW = 'N'">
                      <input type="checkbox" disabled="disabled"></input>
                    </xsl:if>Do Not Substitute - Dispense As Written
                  </td>
                  <td class="small">Signature</td>
                </tr>
              </table>
            </td>
          </tr>
          <tr>
            <td align="center">
              <table border="0" cellpadding="0" cellspacing="0">
                <tr>
                  <td width="6%">
                    <table border="0" cellpadding="0" cellspacing="0" width="100%" class="table">
                      <tr>
                        <td class="small" align="center">1</td>
                      </tr>
                    </table>
                  </td>
                  <td class="small">Prescription is void if the number of drugs prescribed is not noted.</td>
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
              Security Features: 1. (**) bordered and spelled quantities; 2. (**) bordered number of refills; <br></br>3. This list of security features
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
          <!-- Space between the sigtext and the where the Pharmacy Notes is getting printed below -->
          <xsl:if test="NewDataSet/Prescription/PharmacyNotes !=''">
            <tr height="120">
              <td>
              </td>
            </tr>
            <tr>
              <table width="100%" border="0" cellspacing="0" cellpadding="0">
                <td class="Itallic">Pharmacy Comments:</td>
                <td class="Normal">
                  <xsl:value-of select="NewDataSet/Prescription/PharmacyNotes"/>
                </td>
              </table>
            </tr>
          </xsl:if>
        </table>
      </body>
    </html>
  </xsl:template>

</xsl:stylesheet>

