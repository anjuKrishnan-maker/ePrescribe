<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:param name="MedicationQty" ></xsl:param>
  <xsl:param name="WrittenDate" ></xsl:param>
	<xsl:template match="/">
		<html>
			<head>
				<title>Allscripts ePrescribe</title>
			</head>
			<body class="LeftMargin">
				<table width="640"  border="0" cellspacing="0" cellpadding="0" bordercolor="blue">
					<tr>
						<td height="40"></td>
					</tr>
					<tr>
						<td class="ROW_NormalCenter">
                <xsl:value-of select="NewDataSet/Practice/Name"/>
            </td>
					</tr>
					<tr>
						<td class="ROW_NormalCenterSmall">
                <xsl:value-of select="NewDataSet/Practice/Address"/>
                <xsl:value-of select="NewDataSet/Practice/State"/>
                <xsl:text> </xsl:text>
                <xsl:value-of select="NewDataSet/Practice/Zip"/>
						</td>
					</tr>
					<tr>
						<td class="ROW_NormalCenterSmall">
                <areacode>
                  <xsl:value-of select="substring(NewDataSet/Practice/Phone,1,5)"/>
                </areacode>
                <phonenumber>
                  <xsl:value-of select="substring(NewDataSet/Practice/Phone,6,3)"/>-<xsl:value-of select="substring(NewDataSet/Practice/Phone,9,4)"/>
                </phonenumber>
						</td>
					</tr>
					<tr height="10">
						<td class="ROW_headName"></td>
					</tr>
          <tr>
            <td>
              <table width="100%">
                <tr>
                  <td class="ROW_headName" width="50%">
                    <xsl:value-of select="NewDataSet/Provider/Title"/>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Provider/FirstName"/>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Provider/LastName"/>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Provider/Suffix"/>
                  </td>
                  <td class="ROW_leftboldsmall"  width="50%">
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
                  <td class="ROW_Normal" width="50%" style="text-align:left">
                    <xsl:value-of select="NewDataSet/Provider/State"/>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Provider/LicenseNumber"/>
                  </td>
                  <td class="ROW_leftboldsmall" width="50%">
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
                  <td class="ROW_Normal" width="50%" style="text-align:left">
                    <xsl:if test="NewDataSet/Provider/DEANumber !=''">DEA: </xsl:if>
                    <xsl:value-of select="NewDataSet/Provider/DEANumber"/>
                  </td>
                  <td class="ROW_leftboldsmall" width="50%">
                    <xsl:if test="NewDataSet/PhysicianAssistant/DEANumber !=''">DEA: </xsl:if>
                    <xsl:value-of select="NewDataSet/PhysicianAssistant/DEANumber"/>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          <tr>
            <td>
              <table width="100%">
                <tr>
                  <td class="ROW_Normal" width="50%" style="text-align:left">
                    <xsl:if test="NewDataSet/Provider/UPIN !=''">NPI: </xsl:if>
                    <xsl:value-of select="NewDataSet/Provider/UPIN"/>
                  </td>
                  <td class="ROW_leftboldsmall" width="50%">
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
                  <td class="ROW_Normal" width="50%" style="text-align:left">
                    <xsl:if test="NewDataSet/Provider/DPS !=''">ASSMCA: </xsl:if>
                    <xsl:value-of select="NewDataSet/Provider/DPS"/>
                  </td>
                  <td class="ROW_leftboldsmall" width="50%">
                    <xsl:if test="NewDataSet/PhysicianAssistant/DPS !=''">ASSMCA: </xsl:if>
                    <xsl:value-of select="NewDataSet/PhysicianAssistant/DPS"/>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          <tr>
						<!-- Leave space between the header and the script contents -->
						<td height="25"></td>
					</tr>
					<tr>
						<!--Patient Name, DOB, Address -->
            <td class="ROW_Normal" style="font-size:16px">
              <table width="100%"  border="0" cellspacing="0" cellpadding="0">
                <tr>
                  <td width="70px" height="30">Name:</td>
                  <td width="10px"></td>
                  <td width="475px">
                    <table width="100%" height="18"  border="0" cellpadding="0" cellspacing="0">
                      <tr>
                        <td width="100%" style="border-bottom:solid 1px Black; font-style:italic; font-weight:bold;">
                          <xsl:value-of select="NewDataSet/Patient/FirstName"/>
                          <xsl:text> </xsl:text>
                          <xsl:value-of select="NewDataSet/Patient/LastName"/>
                        </td>
                      </tr>
                    </table>
                  </td>
                  <td width="40px">DOB:</td>
                  <td width="10px"></td>
                  <td width="55px">
                    <table width="100%"  border="0" cellpadding="0" cellspacing="0">
                      <tr>
                        <td width="100%" style="border-bottom:solid 1px Black">
													<xsl:value-of select="NewDataSet/Patient/DOB"/>
												</td>
											</tr>
										</table>
									</td>
								</tr>
								<tr>
									<td width="70px">Address:</td>
                  <td width="10px"></td>
									<td colspan="4">
										<table width="575" height="18"  border="0" cellpadding="0" cellspacing="0">
											<tr>
                        <td style="border-bottom:solid 1px Black; font-style:italic;">
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
					<tr height="30">
						<!-- Space between patient demographic information and med info -->
						<td></td>
					</tr>
          <tr>
            <td>
              <table width="100%" border="0" cellpadding="0" cellspacing="0">
                <xsl:for-each select="NewDataSet/Prescription">
                  <tr>
                    <td class="ROW_Normal">
                      <xsl:value-of select="MedicationName"/>
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="Strength"/>
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="StrengthUOM"/>
                      <xsl:text> </xsl:text>
                      <xsl:value-of select="DosageDescription"/>
                    </td>
                  </tr>
                  <tr>
                    <td height="17" class="ROW_NormalSmall">
                      <xsl:value-of select="MedicationQuantityDisplay"></xsl:value-of>
                    </td>
                  </tr>
                  <tr>
                    <td class="ROW_NormalItallic">
                      <xsl:value-of select="SIGText"/>
                      <xsl:if test="PharmacyNotes !=''">
                        <br/>
                        <table width="100%" border="0" cellspacing="0" cellpadding="0">
                          <td class="ROW_Itallic">Pharmacy Comments:</td>
                          <td class="ROW_small">
                            <xsl:value-of select="PharmacyNotes"/>
                          </td>
                        </table>
                      </xsl:if>
                    </td>
                  </tr>
                  <tr>
                    <td>
                      <table width="100%" border="0" cellspacing="0" cellpadding="0" bordercolor="red">
                        <tr>
                          <td width="230" align="left" class="ROW_Itallic">
                            Refills Authorized <xsl:value-of select="RefillQuantity"/> Time(s)
                          </td>
                          <td width="40" class="ROW_Itallic">Rx:</td>
                          <td width="40" class="ROW_Itallic">
                            <table width="100%" border="0" cellpadding="0" cellspacing="0" class="table">
                              <tr>
                                <td width="100%" height="18" class="ROW_NormalSmall">
                                  <xsl:text> </xsl:text>
                                  <xsl:value-of select="RxNumber"/>
                                </td>
                              </tr>
                            </table>
                          </td>
                          <td width="50" class="ROW_Itallic">
                            <xsl:value-of select="CSCode"/>
                          </td>
                          <td width="280" height="40px">
                            <table width="100%"  border="0" cellspacing="0" cellpadding="0" bordercolor="red">
                              <xsl:if test="DAW='Y'">
                                <tr>
                                  <td width="50%" style="text-align:left">
                                    <img src="images/checkbox_unchecked.gif"></img>  May Substitute
                                  </td>
                                  <td width="50%" style="text-align:left">
                                    <img src="images/checkbox_checked.gif"></img> May Not Substitute
                                  </td>
                                </tr>
                              </xsl:if>
                              <xsl:if test="DAW='N'">
                                <tr>
                                  <td width="50%" style="text-align:left">
                                    <img src="images/checkbox_checked.gif"></img> May Substitute
                                    </td>
                                  <td width="50%" style="text-align:left">
                                    <img src="images/checkbox_unchecked.gif"></img>  May Not Substitute
                                  </td>
                                </tr>
                              </xsl:if>
                            </table>
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>
                  <tr height="20">
                    <td>
                      
                    </td>
                  </tr>
                </xsl:for-each>
              </table>
            </td>
          </tr>
          <!-- The following is for the CMS requirements for medicare reimbursement -->
					<tr height="20">
						<td>
						</td>
					</tr>
          <tr>
            <td>
              <table width="100%">
                <tr >
                  <td align="left" class="ROW_NormalBoldItallic">
                    Written: <xsl:value-of select="$WrittenDate"></xsl:value-of>
                  </td>
                  <td align="right" class="ROW_NormalBoldItallic">
                    Prescriber Signature: _______________________
                  </td>
                </tr>
              </table>
            </td>            
          </tr>
          <tr height="20">
            <td>

            </td>
          </tr>          
					<tr>
						<td class="ROW_footer" style="vertical-align:bottom;">
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
				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>

