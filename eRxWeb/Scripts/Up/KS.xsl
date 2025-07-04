<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:param name="MedicationQty" ></xsl:param>
  <xsl:template match="/">
    <html>
      <head>
        <title>Allscripts ePrescribe</title>
      </head>
      <body class="LeftMargin">
        <table width="100%" height="98%"  border="0" cellspacing="0" cellpadding="0" bordercolor="blue">
          <tr>
            <td width="100%"></td>
          </tr>
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
					  <td class="headName"></td>
				  </tr>
				  <tr>
					  <td class="headName">
						  <xsl:value-of select="NewDataSet/Provider/Title"/>
						  <xsl:text> </xsl:text>
						  <xsl:value-of select="NewDataSet/Provider/FirstName"/>
						  <xsl:text> </xsl:text>
						  <xsl:value-of select="NewDataSet/Provider/LastName"/>
						  <xsl:text> </xsl:text>
						  <xsl:value-of select="NewDataSet/Provider/Suffix"/>
					  </td>
				  </tr>
				  <tr>
					  <td class="subheading">
						  <xsl:value-of select="NewDataSet/Provider/DEANumber"/>
					  </td>
				  </tr>
				  <tr>
					  <!-- Leave space between the header and the script contents -->
					  <td></td>
				  </tr>
				  <tr>
					  <!--Patient Name, DOB, Address -->
					  <td>
						  <table width="100%" border="0" cellspacing="0" cellpadding="0">
							  <tr>
								  <td width="18%">Name</td>
								  <td width="49%">
									  <table width="49%" border="0" cellpadding="0" cellspacing="0" class="table"  >
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
								  <td width="18%" align ="right">
									  <table width="100%" border="0" cellpadding="0" cellspacing="0" class="table">
										  <tr>
											  <td class="DOB">
												  <xsl:value-of select="NewDataSet/Patient/DOB"/>
											  </td>
										  </tr>
									  </table>
								  </td>
							  </tr>
							  <tr>
								  <td width="18%">Address</td>
								  <td colspan="3">
									  <table width="100%" border="0" cellpadding="0" cellspacing="0" class="table"  bordercolor="red">
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
					  <!-- Space between patient demographic information and med info -->
					  <td></td>
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
					  <!-- Space between sig and signature lines -->
					  <td>
					  </td>
				  </tr>
				  <tr>
			      <td width="50%">
			        <xsl:if test="NewDataSet/Prescription/CSCode=''">
			          <img  width="100%" height="38">
				          <xsl:attribute name="src" >
					          signature.aspx?ProviderId=<xsl:value-of select= "NewDataSet/Provider/ProviderID" />
				          </xsl:attribute>
			          </img>
			        </xsl:if>
			      </td>
            <td width="50%">
              <xsl:if test="NewDataSet/Prescription/CSCode=''">
                <xsl:if test="NewDataSet/Prescription/DAW='N'">
                  <img   width="100%" height="38">
                    <xsl:attribute name="src" >
                      signature.aspx?ProviderId=<xsl:value-of select= "NewDataSet/Provider/ProviderID" />
                    </xsl:attribute>
                  </img>
                </xsl:if>
              </xsl:if>
            </td>
          </tr>
        <tr>
          <td class="center">Dispense as written</td>
          <td class="center">Brand exchange permissible</td>
        </tr>
        <tr>
          <td></td>
        </tr>
        <tr>
          <td></td>
        </tr>
          <tr>
            <td>
              <table width="100%" border="0" cellspacing="0" cellpadding="0">
                <tr>
                  <td width="12%" class="Itallic">Written:</td>
                  <td width="15%" align="left" class="Itallic">
                    <xsl:value-of select="NewDataSet/Prescription/Created"/>
                  </td>
                  <td width="47%" align="center" class="Itallic">
                    Refills Authorized <xsl:value-of select="NewDataSet/Prescription/RefillQuantity"/> Time(s)
                  </td>
                  <td width="6%" class="Itallic">Rx:</td>
                  <td width="12%">
                    <table width="100%"  border="0" cellpadding="0" cellspacing="0" class="table">
                      <tr>
                        <td width="100%" class="center">
                          <xsl:value-of select="NewDataSet/Prescription/RxNumber"/>
                        </td>
                      </tr>
                    </table>
                  </td>
                  <td width="9%" class="Itallic">
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
          <tr>
            <td></td>
          </tr>
          <tr>
            <td></td>
          </tr>
        </table>
    </body>
    </html>
</xsl:template>

</xsl:stylesheet> 

