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
          <!-- There will be a minumum margin on the printer -->
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
					<tr height="5">
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
					<tr height="10">
						<td></td>
					</tr>
					<tr>
						<td>
              <table width="100%"  border="0" cellspacing="0" cellpadding="0" bordercolor="pink">
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
                  <td width="60%" class="subheading">
										<xsl:value-of select="NewDataSet/Patient/Address"/>
										<xsl:value-of select="NewDataSet/Patient/State"/>
										<xsl:text> </xsl:text>
										<xsl:value-of select="NewDataSet/Patient/Zip"/>
									</td>
                  <td width ="7%">
										<xsl:text> </xsl:text>
									</td>
                  <td width="20%" class="DOB">
										<!-- Print the current prescription date.-->
										<xsl:value-of select="NewDataSet/Prescription/Created"/>
									</td>
								</tr>
							</table>
						</td>
					</tr>
					<tr height="20">
						<!-- Space between patient demographic information and med info -->
						<td></td>
					</tr>
					<tr>
						<td>
              <table width="100%" height="34" border="0" cellpadding="0" cellspacing="0">
								<tr>
                  <td width="77%"  class="NormalCenter">
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
                  <td width="23%"></td>
								</tr>
								<tr>
                  <td width="77%" height="17" class="center">
										<xsl:value-of select="$MedicationQty"></xsl:value-of>
									</td>
                  <td width="23%"></td>
								</tr>
								<tr>
                  <td width="77%" height="17" class="NormalCenterSmall">
										<xsl:value-of select="NewDataSet/Prescription/SIGText"/>
									</td>
                  <td width="23%"></td>
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
						<tr height="100">
							<!-- Space adjustment should be done in the height above -->
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


