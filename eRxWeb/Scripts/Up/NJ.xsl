<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:param name="MedicationQty" ></xsl:param>
  <xsl:param name="RefillQty"></xsl:param>
  <xsl:param name="IsCSMed"></xsl:param>
  <xsl:param name="IsWebPrint"></xsl:param>
  <xsl:template match="/">
    <html>
      <head>
        <title>Allscripts ePrescribe</title>
      </head>
      <body class="LeftMargin">
        <table width="100%" height="100%" style="overflow:hidden" border="0" cellspacing="0" cellpadding="0" bordercolor="blue">
          <xsl:if test ="$IsWebPrint != ''">
            <tr>
              <td height="14">
                <xsl:text> </xsl:text>
              </td>
            </tr>
          </xsl:if>
          <tr>
					  <td>
						  <table width="100%" border="0" cellspacing="0" cellpadding="0">
                <tr>
                  <td width="66%" class="headNameNJ">
                    <xsl:value-of select="NewDataSet/Patient/FirstName"/>
                    <xsl:text> </xsl:text>
                    <xsl:value-of select="NewDataSet/Patient/LastName"/>
                  </td>
                  <td width="20%">
                    <!-- Space for preprinted birthdate label -->
                    <xsl:text> </xsl:text>
                  </td>
                  <td width="14%" class="DOBNJ">
                    <xsl:value-of select="NewDataSet/Patient/DOB"/>
                  </td>
                </tr>
              </table>
            </td>
				</tr>
				<tr>
					<td height="12">
						<xsl:text> </xsl:text>
					</td>
				</tr>
				<tr>
					<td>
						<table width="100%" border="0" cellspacing="0" cellpadding="0" >
							<tr>
								<td width="66%" class="subheadingNJ">
									<xsl:value-of select="NewDataSet/Patient/Address"/>
									<xsl:value-of select="NewDataSet/Patient/State"/>
									<xsl:text> </xsl:text>
									<xsl:value-of select="NewDataSet/Patient/Zip"/>
								</td>
								<td width="20%">
									<xsl:text> </xsl:text>
								</td>
								<td width="14%" class="DOBNJ">
									<xsl:value-of select="NewDataSet/Prescription/Created"/>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td height="55">
						<xsl:text> </xsl:text>
					</td>
				</tr>
          <xsl:if test ="$IsWebPrint = ''">
            <tr>
              <td height="14">
                <xsl:text> </xsl:text>
              </td>
            </tr>
          </xsl:if>
				<tr>
					<td height="180">
						<table width="80%" cellspacing="0" cellpadding="0" border="1">
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
								<td  class="center">
									<xsl:value-of select="$MedicationQty"></xsl:value-of>
								</td>
							</tr>
							<tr>
								<td class="NormalCenterSmall">
									<xsl:value-of select="NewDataSet/Prescription/SIGText"/>
									
								</td>
							</tr>
							<!-- The following is for the CMS requirements for medicare reimbursement -->
							<tr height="7">
								<td>
								</td>
							</tr>
							<tr>
								<td class="footer">
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
              <!--  Pharmacy Notes is getting printed below -->
							<xsl:if test="NewDataSet/Prescription/PharmacyNotes !=''">
								<tr>
                  <td>
									  <table width="100%" border="0" cellspacing="0" cellpadding="0">
                      <tr>
                        <td class="Itallic">Pharmacy Comments:</td>
                        <td class="Normal">
                          <xsl:value-of select="NewDataSet/Prescription/PharmacyNotes"/>
                        </td>
                      </tr>
									  </table>
                  </td>
								</tr>
							</xsl:if>
						</table>
					</td>
				</tr>
        <tr>
          <xsl:if test ="$IsWebPrint = ''">
            <tr>
              <td height="60">
                <xsl:text> </xsl:text>
              </td>
            </tr>
          </xsl:if>
        </tr>
          <xsl:if test ="$IsCSMed != ''">
            <xsl:if test="$RefillQty != ''">
              <xsl:if test="$RefillQty = '0'">
                <tr>
                  <td>
                    <table width="100%" border="0" cellspacing="0" cellpadding="0">
                      <tr>
                        <td width="5%" class="headNameNJ">
                          <!--Space for do not refill -->
                        </td>
                        <td width="95%" class="leftalign">
                          <xsl:text> X </xsl:text>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </xsl:if>
              <tr>
                <td height="17">
                  <xsl:text> </xsl:text>
                </td>
              </tr>
              <xsl:if test="$RefillQty != '0'">
                <tr>
                  <td>
                    <table width="100%" border="0" cellspacing="0" cellpadding="0">
                      <tr>
                        <td width="99%" class="leftalignNJ">
                          <xsl:value-of select="$RefillQty"></xsl:value-of>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </xsl:if>
            </xsl:if>
          </xsl:if>
			</table>
		</body>
    </html>
  </xsl:template>
</xsl:stylesheet>

