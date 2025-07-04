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
            <td height="85">
              <xsl:text> </xsl:text>
            </td>
          </tr>
          <tr>
            <td>
              <table width="100%">
						<tr>
							<td width="13%" height="29">
								<xsl:text> </xsl:text>
							</td>
                  <td colspan="3" width="87%" class="headName">
								<xsl:value-of select="NewDataSet/Patient/FirstName"/>
								<xsl:text> </xsl:text>
								<xsl:value-of select="NewDataSet/Patient/LastName"/>
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
								<xsl:text> </xsl:text>
							</td>
                  <td width="59%" class="subheading">
								<xsl:value-of select="NewDataSet/Patient/Address"/>
								<xsl:value-of select="NewDataSet/Patient/State"/>
								<xsl:text> </xsl:text>
								<xsl:value-of select="NewDataSet/Patient/Zip"/>
							</td>
                  <td width ="11%">
								<xsl:text> </xsl:text>
							</td>
                  <td width="16%" height="29" class="DOB">
								<!-- Print the current prescription date.-->
								<!-- <xsl:value-of select="NewDataSet/Prescription/Created"/> -->
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
							<td width="21%"></td>
						</tr>
					</table>
				</td>
			</tr>
      <tr>
        <td>
          <table width="100%" height="7" border="0" cellpadding="0" cellspacing="0">
            <tr>
              <td width="79%" class="NormalRight">
                Date : <xsl:value-of select="NewDataSet/Prescription/Created"/>
              </td>
              <td width="21%"></td>
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
				<td class="footer">
					Security Features: 1. (**) bordered and spelled quantities; <br></br>2. (**) bordered number of refills; 3. This list of security features
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
					<table width="475" border="0" cellspacing="0" cellpadding="0">
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

