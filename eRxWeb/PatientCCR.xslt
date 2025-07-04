<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns="urn:astm-org:CCR">
<!--  02/21/06 transforming AHS 10.1 Extract Schema  CCR_20051109 schema, the official v1.0 -->
	<!-- libraty function imports -->
	<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes" />
	<!-- ====================================GLOBAL VARIABLES ====  -->
	<xsl:variable name="ObjID">0</xsl:variable>
	<xsl:variable name="OID">1.3.6.1.4.1.21367.2005.1.1</xsl:variable>
	<xsl:variable name="emptyPhoneValue"><xsl:text>(0)0-0</xsl:text></xsl:variable>
	<xsl:variable name="noPhoneValue"><xsl:text>()-</xsl:text></xsl:variable>
	<xsl:variable name="lCase">abcdefghijklmnopqrstuvwxyz</xsl:variable>
	<xsl:variable name="uCase">ABCDEFGHIJKLMNOPQRSTUVWXYZ</xsl:variable>	
	<!-- the template to kick off everything -->
	<xsl:template match="/">
		<ContinuityOfCareRecord>
			<CCRDocumentObjectID>1.3.6.1.4.1.22812.1.<xsl:value-of select="Patient/Source/SITENUMBER" />.<xsl:value-of select="Patient/Document/SESSIONID" /></CCRDocumentObjectID>
			<Language><Text>English</Text></Language>
			<Version>V1.0</Version>
			<!-- basic date structure -->
			<xsl:call-template name="CCRDateTime" />
			<!-- the patient -->
			<xsl:call-template name="Patient" />

			<!-- where the CCR is from -->
			<xsl:call-template name="From"/>
			<!-- the Target(s):  To: -->
			<xsl:call-template name="To" />
			<!-- Purpose -->
			<xsl:call-template name="Purpose" />
			<!-- the Body consists of many optional sections -->
			<Body>
				<xsl:call-template name="Insurance" />
				<xsl:apply-templates select="Patient/Problems_Active" />
				<xsl:apply-templates select="Patient/Problems_PH" />
				<xsl:apply-templates select="Patient/Allergies" />
				<xsl:apply-templates select="Patient/Medications_Active" />
			</Body>
			<Actors>
				<xsl:call-template name="PatientActor"/>
				<xsl:apply-templates select="Patient/Provider_DEX"/>
				<xsl:apply-templates select="Patient/Provider_PCP"/>
				<xsl:variable name="fromUser"><xsl:call-template name="userIsUnique"/></xsl:variable>
				<xsl:call-template name="OrgActor"/>
				<xsl:call-template name="InfoSysActor"/>
				<xsl:apply-templates select="Patient/Target/REPOSITORY"/>
				<!-- if we output insurance then we need a payor actor -->
				<xsl:variable name="primaryInsurance" select="Patient/Demographics/PRIMARYINSURANCE" />
				<xsl:if test="string-length($primaryInsurance)>0">
					<xsl:call-template name="PayorActor"/>
				</xsl:if>				
			</Actors>
		</ContinuityOfCareRecord>
	</xsl:template>
	<!-- ======================= Header and helper templates ======================= -->
	<!-- the datetime node at the top of the ccr -->
	<xsl:template name="CCRDateTime">
		<DateTime>
			<Type><Text>CCR Creation DateTime</Text></Type>
			<ExactDateTime><xsl:value-of select="Patient/Document/CreateDTTMZ" /></ExactDateTime>
		</DateTime>
	</xsl:template>
	
	<!-- provide the value of the next object id -->
	<xsl:template name="NextObjectID"></xsl:template>
	
	<!-- Patient Node:  a pointer to a PatientActor node -->
	<xsl:template name="Patient">
		<Patient><ActorID>Patient.<xsl:value-of select="Patient/Demographics/ID"/></ActorID></Patient>
	</xsl:template>
	
	<!-- Patient Actor node -->
	<xsl:template name="PatientActor">
		<xsl:variable name="theNode" select="Patient/Demographics"/>
		<Actor>
			<!-- place holder nextDataObjectID -->
			<ActorObjectID>Patient.<xsl:value-of select="$theNode/ID"/></ActorObjectID>
			<Person>
				<Name>
					<CurrentName>
						<Given>
							<xsl:value-of select="$theNode/FIRSTNAME"/>
						</Given>
						<xsl:variable name="middleName" select="$theNode/MIDDLENAME"/>
						<xsl:if test="string-length($middleName)>0">
							<Middle>
								<xsl:value-of select="$middleName"/>
							</Middle>
						</xsl:if>
						<Family>
							<xsl:value-of select="$theNode/LASTNAME"/>
						</Family>
					</CurrentName>
					<DisplayName><xsl:value-of select="$theNode/FIRSTNAME"/><xsl:text> </xsl:text> <xsl:value-of select="$theNode/LASTNAME"/></DisplayName>
				</Name>
				<DateOfBirth>
					<xsl:variable name="dobString" select="$theNode/DATEOFBIRTH"/>
					<xsl:choose>
						<xsl:when test="string-length($dobString)>10">
						<ExactDateTime>
							<xsl:call-template name="formatDateyyyy-mm-dd"><xsl:with-param name="dateString"><xsl:value-of select="$theNode/DATEOFBIRTH"/></xsl:with-param></xsl:call-template>
						</ExactDateTime>
						</xsl:when>
						<xsl:otherwise>
							<ApproximateDateTime><Text><xsl:value-of select="$dobString"/></Text></ApproximateDateTime>
						</xsl:otherwise>
					</xsl:choose>
				</DateOfBirth>
				<Gender>
					<Text>
						<xsl:call-template name="ActorSex">
							<xsl:with-param name="sexCode" select="$theNode/SEX" />
						</xsl:call-template>
					</Text>
				</Gender>
			</Person>
			<!-- add the identifiers for the patient -->
			<xsl:call-template name="PatientIdentifiers"></xsl:call-template>
			<xsl:call-template name="mailAddressElements"><xsl:with-param name="theNode" select="Patient/Demographics"/></xsl:call-template>
			<xsl:call-template name="phoneElements"><xsl:with-param name="theNode" select="Patient/Demographics"/></xsl:call-template>
			<xsl:call-template name="emailElements"><xsl:with-param name="theNode" select="Patient/Demographics"/></xsl:call-template>
			<Source><Actor><ActorID>ePrescribe</ActorID></Actor></Source>
		</Actor>
	</xsl:template>
	
	<xsl:template name="ActorSex">
		<xsl:param name="sexCode" />
		<xsl:choose>
			<xsl:when test="substring($sexCode,1,1)='M'">Male</xsl:when>
			<xsl:when test="substring($sexCode,1,1)='F'">Female</xsl:when>
			<xsl:otherwise>Unknown</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
	<!-- the patient identifiers are the patient mrn plus any external identifiers we care to show -->
	<xsl:template name="PatientIdentifiers">
		<IDs>
			<Type>
				<Text>Patient ID</Text>
			</Type>
			<ID><xsl:value-of select="Patient/Demographics/ENTERPRISEMRN" /></ID>
			<Source>
				<Actor>
					<ActorID>ePrescribe</ActorID>
					<ActorRole>
						<Text>ERX</Text>
					</ActorRole>
				</Actor>
			</Source>
		</IDs>
	</xsl:template>

	<!-- From -->
	<xsl:template name="From">
		<From>
		<!-- from actor person ...the author is the DEX provider
				secondly, if the PCP is present and different then also include PCP -->
		<xsl:variable name="providerDESNode" select="Patient/Provider_DEX" />
		<xsl:variable name="providerPCPNode" select="Patient/Provider_PCP" />
		<ActorLink>
			<ActorID>Person.<xsl:value-of select="$providerDESNode/ID"/></ActorID> 
			<ActorRole>
			  <Text>Provider</Text> 
			</ActorRole>
		</ActorLink>
		
		<xsl:if test="providerPCPNode">
			<xsl:variable name="pcpID" select="$providerPCPNode/ID" />
			<xsl:variable name="authorID" select="$providerDESNode/ID" />
			<!-- if the PCP is not the author, then we'll emit the PCP as an informant as per 2.2.4.4
				but elsewhere, PCP's were described by example as participants...hmmmm -->
				<xsl:if test="$pcpID!=$authorID">
					<ActorLink>
						<ActorID>Person.<xsl:value-of select="$pcpID"/></ActorID> 
						<ActorRole>
						  <Text>Provider</Text> 
						</ActorRole>	
					</ActorLink>				
				</xsl:if>
		</xsl:if>
		
		<xsl:variable name="fromUser"><xsl:call-template name="userIsUnique"/></xsl:variable>
		<xsl:if test="$fromUser=1">
			<ActorLink>
				<ActorID>Person.<xsl:value-of select="Patient/User/ID"/></ActorID> 
				<ActorRole>
				  <Text>Provider</Text> 
				</ActorRole>
			</ActorLink>
		</xsl:if>
		
		<ActorLink>
			<ActorID>Site</ActorID> 
			<ActorRole>
			  <Text>Organization</Text> 
			</ActorRole>
		</ActorLink>
		
		<ActorLink>
			<ActorID>ePrescribe</ActorID> 
			<ActorRole>
			  <Text>InformationSystem</Text> 
			</ActorRole>
		</ActorLink>
		
		</From>
	</xsl:template>
	
<xsl:template match="Provider_DEX">
<xsl:call-template name="providerActor"><xsl:with-param name="providerModel" select="."/></xsl:call-template>
</xsl:template>

<xsl:template match="Provider_PCP">
	<!-- To represent a healthcare provider in a healthcare role without an assigned 
		 role known or representable to the author.  The example below represents a 
		 physician who was the patient's primary care provider.
	-->
	<xsl:variable name="pcpID" select="ID" />
	<xsl:variable name="authorID" select="./Provider_DEX/ID" />
	<!-- if the PCP is not the author, then we'll emit the PCP as an actor -->
		<xsl:if test="$pcpID!=$authorID">
			<xsl:call-template name="providerActor"><xsl:with-param name="providerModel" select="."/></xsl:call-template>
		</xsl:if>
	</xsl:template>
	
	<xsl:template match="Patient/User">
	<xsl:call-template name="providerActor"><xsl:with-param name="providerModel" select="."/></xsl:call-template>
	</xsl:template>
	
	<!-- emit a provider actor from data at the specified node -->
	<xsl:template name="providerActor">
		<xsl:param name="providerModel" />
		<Actor>
			<ActorObjectID>Person.<xsl:value-of select="$providerModel/ID"/></ActorObjectID>
			<Person>
				<Name>
					<CurrentName>
						<Given>
							<xsl:value-of select="$providerModel/FIRSTNAME" />
						</Given>
						<xsl:variable name="middleName" select="$providerModel/MIDDLENAME"/>
						<xsl:if test="string-length($middleName)>0">
							<Middle>
								<xsl:value-of select="$middleName" />
							</Middle>
						</xsl:if>
						<Family>
							<xsl:value-of select="$providerModel/LASTNAME" />
						</Family>
						<xsl:variable name="titleName" select="$providerModel/TITLENAME"/>
						<xsl:if test="string-length($titleName)>0">
							<Title>
								<xsl:value-of select="$titleName" />
							</Title>
						</xsl:if>
					</CurrentName>
				</Name>
				<Gender>
					<Text>
						<xsl:call-template name="ActorSex">
							<xsl:with-param name="sexCode" select="$providerModel/SEX" />
						</xsl:call-template>
					</Text>
				</Gender>
			</Person>
			<IDs>
				<ID><xsl:value-of select="ID"/></ID>
				<Source><Actor><ActorID>ePrescribe</ActorID></Actor></Source>
			</IDs>
			<xsl:variable name="specialtyText" select="$providerModel/SPECIALTY"/>
			<xsl:if test="string-length($specialtyText)>0">
				<Specialty>
					<Text>
						<xsl:value-of select="$specialtyText" />
					</Text>
				</Specialty>
			</xsl:if>			
			<xsl:call-template name="mailAddressElements"><xsl:with-param name="theNode" select="."/></xsl:call-template>
			<xsl:call-template name="phoneElements"><xsl:with-param name="theNode" select="."/></xsl:call-template>
			<xsl:call-template name="emailElements"><xsl:with-param name="theNode" select="."/></xsl:call-template>
			<Source><Actor><ActorID>ePrescribe</ActorID></Actor></Source>
		</Actor>
	</xsl:template>
	
	<!-- TO  -->
	<xsl:template name="To">
		<xsl:variable name="toPatient" select="Patient/Options/TO/PATIENT" />
		<xsl:variable name="toRhio" select="Patient/Target/REPOSITORY" />
		<!-- emit a To element if this is to patient or rhio -->
		<xsl:if test="($toPatient = 'True') or (string-length($toRhio)>0)">
			<To>
			<xsl:if test="$toPatient='True' ">
				<!-- to the patient -->
				<ActorLink>
					<ActorID>Patient.<xsl:value-of select="Patient/Demographics/ID"/></ActorID>
					<ActorRole><Text>Patient</Text></ActorRole>
				</ActorLink>
			</xsl:if>
			<xsl:if test="string-length($toRhio)>0">
				<!-- to the IS -->
				<ActorLink>
					<ActorID>Repository</ActorID>
					<ActorRole><Text>XDS Repository</Text></ActorRole>
				</ActorLink>
			</xsl:if>
			</To>
		</xsl:if>
	</xsl:template>

	<!-- Purpose -->
	<xsl:template name="Purpose">
		<!-- multiple purposes are possible -->
		<xsl:variable name="PatientPurpose" select="Patient/Options/PURPOSE/PATIENT" />
		<xsl:variable name="ReferralPurpose" select="Patient/Options/PURPOSE/REFERRAL" />
		<Purpose>
		<!-- patient purpose -->
		<xsl:if test="$PatientPurpose = 'True'">
			<xsl:variable name="patientPurposeText" select="Patient/Options/PURPOSE/PATIENTTEXT" />
			<Description>
				<Text>
					<xsl:choose>
						<xsl:when test="string-length($patientPurposeText)>0"><xsl:value-of select="$patientPurposeText"/></xsl:when>
						<xsl:otherwise>For the patient</xsl:otherwise>
					</xsl:choose>
				</Text>
			</Description>
		</xsl:if>
		<!-- referral purpose -->
		<xsl:variable name="referralPurposeText" select="Patient/Options/PURPOSE/REFERRALTEXT"/>
		<xsl:if test="$ReferralPurpose = 'True'">
			<xsl:choose>
				<xsl:when test="string-length($referralPurposeText)>0">
				<Description>
					<Text><xsl:value-of select="$referralPurposeText" /></Text>
				</Description>
				</xsl:when>
				<xsl:otherwise></xsl:otherwise>
			</xsl:choose>
		</xsl:if>		
		</Purpose>
	</xsl:template>

	<!--  ====================== Body Sections ====================== -->
	<!-- Insurance -->
	<xsl:template name="Insurance">
		<!-- a tw extract -->
		<xsl:variable name="primaryInsurance" select="Patient/Demographics/PRIMARYINSURANCE" />
		<xsl:if test="string-length($primaryInsurance)>0">
			<Payers>
				<Payer>
					<CCRDataObjectID>Payer</CCRDataObjectID>
					<Type><Text>Primary Health Insurance</Text></Type>
					<Description><Text><xsl:value-of select="$primaryInsurance" /></Text></Description>
					<Source><Actor><ActorID>ePrescribe</ActorID></Actor></Source>
				</Payer>
			</Payers>
		</xsl:if>
	</xsl:template>
	<!-- Advanced Directives -->
	<!-- Support -->
	<!-- Functional Status -->
	<!-- Problems -->
	<xsl:template match="Problems_Active">
		<Problems>
			<xsl:variable name="all_rows" select="ITEM" />
			<xsl:variable name="total_rows" select="count($all_rows)" />
			<xsl:for-each select="$all_rows[position()&lt;=ceiling($total_rows)]">
				<Problem>
					<xsl:call-template name="problemElements"><xsl:with-param name="theNode" select="."/></xsl:call-template>
					<!-- future :  add active problem specific elements:  Episodes, HealthStatus, PatientKnowledge -->
				</Problem>
			</xsl:for-each>
		</Problems>
	</xsl:template>
	
	<!-- Social HIstory -->
	<xsl:template match="Problems_PH">
		<SocialHistory>
			<xsl:variable name="all_rows" select="ITEM" />
			<xsl:variable name="total_rows" select="count($all_rows)" />
			<xsl:for-each select="$all_rows[position()&lt;=ceiling($total_rows)]">
				<SocialHistoryElement>
					<xsl:call-template name="problemElements"><xsl:with-param name="theNode" select="."/></xsl:call-template>
					<!-- future: add SocialHistory specifics: Episodes -->
				</SocialHistoryElement>
			</xsl:for-each>			
		</SocialHistory>
	</xsl:template>


<!-- output all info for the problem passed in as theNode -->
<xsl:template name="problemElements">
	<xsl:param name="theNode"/>
	<!-- some problem variables -->
	<xsl:variable name="icdCode" select="$theNode/ICD9DIAGNOSISCODE" />
	<xsl:variable name="categoryDET" select="$theNode/PROBLEMCATEGORYDET" />
	<xsl:variable name="onset" select="$theNode/ONSET" />
	<xsl:variable name="laterality" select="$theNode/LATERALITYTEXT"/>
	<xsl:variable name="severity" select="$theNode/CLINICALSEVERITY"/>
	<CCRDataObjectID>PR.<xsl:value-of select="$theNode/ID" /></CCRDataObjectID>
	<xsl:if test="string-length($onset)!=0">
		<DateTime>
			<Type><Text>Onset</Text></Type>
			<ApproximateDateTime>
				<Text>
					<xsl:value-of select="$onset" />
				</Text>
			</ApproximateDateTime>
		</DateTime>
	</xsl:if>
	<IDs>
		<ID><xsl:value-of select="$theNode/ID"/></ID>
		<Source><Actor><ActorID>ePrescribe</ActorID></Actor></Source>
	</IDs>
	<Type>
		<Text>
		<xsl:call-template name="problemTypeCode">
			<xsl:with-param name="icdValue" select="$icdCode" />
		</xsl:call-template>
		</Text>
	</Type>
	<Description>
		<Text>
      <xsl:value-of select="$theNode/PROBLEM"/>
    </Text>
		<!-- we want to attribute the description -->
		<xsl:if test="string-length($laterality)!=0">
			<ObjectAttribute>
				<Attribute>Laterality</Attribute>
				<AttributeValue><Value><xsl:value-of select="$laterality" /></Value></AttributeValue>
			</ObjectAttribute>
		</xsl:if>
		<xsl:if test="string-length($severity)!=0">
			<ObjectAttribute>		
				<Attribute>Severity</Attribute>
				<AttributeValue><Value><xsl:value-of select="$severity" /></Value></AttributeValue>
			</ObjectAttribute>
		</xsl:if>
		<xsl:if test="string-length($icdCode)!=0">
			<Code>
				<Value>
					<xsl:value-of select="$icdCode" />
				</Value>
				<CodingSystem>ICD9-CM</CodingSystem>
				<!-- future: check on adding version tag -->
			</Code>
		</xsl:if>		
	</Description>
	<Status><Text><xsl:value-of select="$theNode/PROBLEMSTATUS" /></Text></Status>
	<Source><Actor><ActorID>ePrescribe</ActorID></Actor></Source>	
</xsl:template>


	<!-- Alerts -->
	<xsl:template match="Allergies">
	<!-- VI#28493 only show Active status allergic reactions -->
	<xsl:variable name="all_active_rows" select="ITEM[ALLERGYSTATUS='Active']" />
	<xsl:variable name="active_row_count" select="count($all_active_rows)" />
		<xsl:if test="$active_row_count>0">
			<Alerts>
			<xsl:for-each select="$all_active_rows[position()&lt;=ceiling($active_row_count)]">			
				<xsl:variable name="onset" select="REACTIONFUZZYSORTAS" />
				<xsl:variable name="allergyCode" select="ALLERGYCODE" />
				<Alert>
				<CCRDataObjectID>AL.<xsl:value-of select="ID" /></CCRDataObjectID>
				<xsl:if test="string-length($onset)!=0">
					<DateTime>
						<ApproximateDateTime>
							<Text><xsl:value-of select="$onset" /></Text>
						</ApproximateDateTime>
					</DateTime>
				</xsl:if>	
				<IDs>
					<ID><xsl:value-of select="ID"/></ID>
					<Source><Actor><ActorID>ePrescribe</ActorID></Actor></Source>
				</IDs>					
				<Type><Text><xsl:call-template name="alertTypeCode"><xsl:with-param name="alertCategory" select="ALLERGYCATEGORY" /></xsl:call-template></Text></Type>
				<Description><Text><xsl:value-of select="ENTRYNAME"/></Text></Description>
				<Status><Text><xsl:value-of select="ALLERGYSTATUS" /></Text></Status>
				<Source><Actor><ActorID>ePrescribe</ActorID></Actor></Source>
				<!-- agent goes here -->
				<Agent>
					<xsl:choose>
						<xsl:when test="string-length($allergyCode)=0"><xsl:call-template name="environmentalAlert" /></xsl:when>
						<xsl:otherwise><xsl:call-template name="productAlert" /></xsl:otherwise>
					</xsl:choose>
				</Agent>
				<!-- reactions go here -->
				<xsl:variable name="r1" select="REACTION1" />
				<xsl:variable name="r2" select="REACTION2" />
				<xsl:variable name="r3" select="REACTION3" />
				<xsl:variable name="r4" select="REACTION4" />
				<xsl:variable name="r5" select="REACTION5" />
				<xsl:if test="string-length($r1)>0"><xsl:call-template name="reactionText"><xsl:with-param name="theText" select="$r1" /></xsl:call-template></xsl:if>
				<xsl:if test="string-length($r2)>0"><xsl:call-template name="reactionText"><xsl:with-param name="theText" select="$r2" /></xsl:call-template></xsl:if>
				<xsl:if test="string-length($r3)>0"><xsl:call-template name="reactionText"><xsl:with-param name="theText" select="$r3" /></xsl:call-template></xsl:if>
				<xsl:if test="string-length($r4)>0"><xsl:call-template name="reactionText"><xsl:with-param name="theText" select="$r4" /></xsl:call-template></xsl:if>
				<xsl:if test="string-length($r5)>0"><xsl:call-template name="reactionText"><xsl:with-param name="theText" select="$r5" /></xsl:call-template></xsl:if>			
				
				</Alert>
			</xsl:for-each>
			
			</Alerts>
		</xsl:if>	<!-- fi any active status rows -->
	</xsl:template>

	<!-- Medications -->
	<xsl:template match="Medications_Active">
		<Medications>
		<xsl:variable name="all_rows" select="ITEM" />
		<xsl:variable name="total_rows" select="count($all_rows)" />
		<xsl:for-each select="$all_rows[position()&lt;=ceiling($total_rows)]">
				<xsl:variable name="medicationStartDate"><xsl:call-template name="medStartDate" /></xsl:variable>
				<xsl:variable name="medicationTherapyDate"><xsl:call-template name="medTherapyCourse" /></xsl:variable>
				<xsl:variable name="ndcCode" select="NDCNUMBER" />
				<Medication>
				<CCRDataObjectID>ME.<xsl:value-of select="ID" /></CCRDataObjectID>
				<DateTime>
					<Type>
						<Text>Prescription Date</Text>
					</Type>
					<ExactDateTime><xsl:value-of select="STARTDATEFMT"/></ExactDateTime>
					<!--					<ApproximateDateTime>
						<Text><xsl:value-of select="$medicationStartDate" /><xsl:text> - </xsl:text><xsl:value-of select="$medicationTherapyDate" /></Text>
					</ApproximateDateTime>
-->
				</DateTime>
				<IDs>
					<ID><xsl:value-of select="ID"/></ID>
					<Source><Actor><ActorID>ePrescribe</ActorID></Actor></Source>
				</IDs>
				<Type><Text>Medication</Text></Type>
				<Description><Text><xsl:call-template name="formattedMedName" /></Text></Description>
				<Status><Text><xsl:value-of select="MEDICATIONSTATUS" /></Text></Status>
				<Source><Actor><ActorID>ePrescribe</ActorID></Actor></Source>	
				<Product>
					<!-- product name and code -->
					<ProductName><Text><xsl:value-of select="ENTRYNAME"/></Text>		<!-- 10.x does not have DRUGNAME <xsl:value-of select="DRUGNAME"/></Text> -->
					<xsl:if test="string-length($ndcCode)!=0">
						<Code>
							<Value><xsl:value-of select="$ndcCode" /></Value> 
							<CodingSystem>NDC</CodingSystem> 							
						</Code>
					</xsl:if>
					</ProductName>
					<!-- brandname -->
					<xsl:variable name="brandNameText" select="BRANDMEDNAME"/>
					<xsl:if test="string-length($brandNameText)>0">
						<BrandName><Text><xsl:value-of select="$brandNameText"/></Text></BrandName>					
					</xsl:if>
          <Strength>
            <Value>
              <xsl:value-of select="STRENGTH"/>
            </Value>
            <Units>
              <Unit><xsl:value-of select="STRENGTHUOM"/>
              </Unit>
            </Units>
          </Strength>
          <Form>
            <Text>
              <xsl:value-of select="DOSEUNITDESC"/>
            </Text>
          </Form>
				</Product>
				<!-- quantity and quantity units -->
					<!-- todo -->
				
				<!-- Directions - a possibly repeating Direction structure -->
				<Directions>
					<!-- todo:  how to determine if there are multiple direction elements ? -->
					<Direction>
						<!-- the description is the SIG -->
						<Description><Text><xsl:value-of select="SIG"/></Text></Description>
            <Dose>
              <Value>
                <xsl:value-of select="DOSE"/>
              </Value>
              <Units>
                <Unit>
                  <xsl:value-of select="DOSEUNIT"/>
                </Unit>
              </Units>
            </Dose>
            <!-- todo? - doseindicator, deliveryMethod -->
						<!-- Dose:  will this work withour DoseUnits ? Yes:  schema valid and aafp validator valid-->
						<!-- todo add DoseUnits -->
						<!-- 10.x does not have DOSE <Dose><Value><xsl:value-of select="DOSE"/></Value></Dose> -->
						<!-- skipping doseCalculation and vehicle -->
						<!-- Route of Admin -->
						<!-- 10.x does not have ROUTEOFADMINDISPLAY <Route><Text><xsl:value-of select="ROUTEOFADMINDISPLAY"/></Text></Route> -->
            <Route>
              <Text>
                <xsl:value-of select="ROUTEOFADMINDISPLAY"/>
              </Text>
            </Route>
						<!-- skipping more elements -->
						<!-- frequency -->
						<xsl:variable name="freqString" select="SIG"/>
						<xsl:if test="string-length($freqString)>0">
							<Frequency><Value><xsl:value-of select="$freqString"/></Value></Frequency>
						</xsl:if>
						<!-- skipping interval -->
						<!-- todo Duration and Duration Units -->
						<!-- skipping several other Direction elements -->
					</Direction>
				</Directions>
				<!-- todo Patient Instructions -->
				<!-- skipping fulfillment instructions -->
          <Refills>
            <Refill>
              <Number>
                <xsl:value-of select="REFILLS"/>
              </Number>
            </Refill>
          </Refills>
				<!-- todo for Immunizations:  series number -->
				</Medication>
			</xsl:for-each>		
		</Medications>
	</xsl:template>

	<!-- Procedures -->
	<!-- Encounters -->
	<!-- Plan of Care -->
	<!-- HealthCare Providers -->
	
<!--  ====================== other helpers for the Body Sections ============  -->
<xsl:template name="productAlert">
<xsl:variable name="allergyCode" select="ALLERGYCODE" />
<Products>
		<Product>
			<CCRDataObjectID>PR.<xsl:value-of select="ID"/>.1</CCRDataObjectID>
			<Type><Text>Medication</Text></Type>
			<Description><Text><xsl:value-of select="ENTRYNAME" /></Text>
				<Code>
						<Value><xsl:value-of select="$allergyCode" /></Value>			
						<CodingSystem>
							<xsl:choose>
								<xsl:when test="string-length($allergyCode)&gt;6">NDC</xsl:when>
								<xsl:otherwise>Medispan PAR ID</xsl:otherwise>
							</xsl:choose>
						</CodingSystem>
				</Code>
			</Description>
			<Source><Actor><ActorID>ePrescribe</ActorID></Actor></Source>	
			<!-- <InternalCCRLink><LinkID></LinkID></InternalCCRLink>		 -->
			<Product><ProductName><Text><xsl:value-of select="ENTRYNAME" /></Text></ProductName></Product>
		</Product>
</Products>		
</xsl:template>

<xsl:template name="environmentalAlert">
<EnvironmentalAgents>
	<EnvironmentalAgent>
			<CCRDataObjectID>PR.<xsl:value-of select="ID"/>.1</CCRDataObjectID>
			<Type><Text>non-medication</Text></Type>
			<Description>
					<Text><xsl:value-of select="ENTRYNAME" /></Text>
			</Description>
			<Source><Actor><ActorID>ePrescribe</ActorID></Actor></Source>	
	</EnvironmentalAgent>
</EnvironmentalAgents>
</xsl:template>
	
<!-- Reaction text -->
<xsl:template name="reactionText">
	<xsl:param name="theText" />
	<Reaction>
		<Description>
			<Text><xsl:value-of select="$theText" /></Text>
		</Description>
	</Reaction>
</xsl:template>

	
<!-- freetext for a problem consists of:  
	{No} category severity ProblemName onsetdt clinical progress; laterality; (icd-9); Resolved: date
-->	
<xsl:template name="problemText">
	<xsl:variable name="problemStatusEC" select="PROBLEMSTATUSEC" />
	<xsl:variable name="problemCategory" select="PROBLEMCATEGORYDET" />
	<xsl:variable name="problemSeverity" select="CLINICALSEVERITY" />
	<xsl:variable name="problem" select="PROBLEM" />
	<xsl:variable name="problemOnset" select="ONSET" />
	<xsl:variable name="problemMgt" select="MANAGEMENTEFFECTIVE" />
	<xsl:variable name="problemLaterality" select="LATERALITYTEXT" />
	<xsl:variable name="problemICD" select="ICD9DIAGNOSISCODE" />
	<xsl:variable name="problemResolved" select="RESOLVEDFUZZY" />

	<xsl:if test="$problemStatusEC = 'DENIED'"><xsl:text>No </xsl:text></xsl:if>
	<xsl:if test="string-length($problemCategory)!=0"><xsl:value-of select="$problemCategory" /><xsl:text> </xsl:text></xsl:if>
	<xsl:if test="string-length($problemSeverity)!=0"><xsl:value-of select="$problemSeverity" /><xsl:text> </xsl:text></xsl:if>
	<xsl:value-of select="$problem" />
	<xsl:if test="string-length($problemOnset)!=0"><xsl:text> </xsl:text><xsl:value-of select="$problemOnset" /></xsl:if>
	<xsl:if test="string-length($problemMgt)!=0"><xsl:text> </xsl:text><xsl:value-of select="$problemMgt" /></xsl:if>
	<xsl:if test="string-length($problemLaterality)!=0"><xsl:text>; </xsl:text><xsl:value-of select="$problemLaterality" /></xsl:if>
	<xsl:if test="string-length($problemICD)!=0"><xsl:text> (</xsl:text><xsl:value-of select="$problemICD" /><xsl:text>) </xsl:text></xsl:if>
	<xsl:if test="string-length($problemResolved)!=0"><xsl:text>; Resolved: </xsl:text><xsl:value-of select="$problemResolved" /></xsl:if>
</xsl:template>	

<!-- medication dates:  start and therapy course -->
<xsl:template name="medStartDate">
	<xsl:variable name="start2" select="STARTFUZZY2" />
	<xsl:variable name="startFuzzy" select="STARTFUZZYSORTAS" />
	<xsl:choose>
		<xsl:when test="string-length($start2)>0"><xsl:value-of select="$start2" /></xsl:when>
		<xsl:when test="string-length($startFuzzy)=0"></xsl:when>
		<xsl:when test="$startFuzzy = '1900-01-01 00:00:00.000'"></xsl:when>
		<xsl:otherwise><xsl:value-of select="$startFuzzy" /></xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="medTherapyCourse">
	<xsl:variable name="medEndDt"><xsl:value-of select="ENDDATE" /></xsl:variable>
	<xsl:variable name="medExpireDt"><xsl:value-of select="EXPIRATIONDT" /></xsl:variable>
	<xsl:variable name="medDisp"><xsl:value-of select="DISPOSITION" /></xsl:variable>
	<xsl:variable name="medStartDt"><xsl:value-of select="STARTFUZZYSORTAS" /></xsl:variable>
	<xsl:variable name="medPerformDt"><xsl:value-of select="PERFORMEDFUZZY2SORTAS" /></xsl:variable>
	<xsl:variable name="medStatus"><xsl:value-of select="MEDICATIONSTATUS" /></xsl:variable>

	<xsl:choose>
		<xsl:when test="$medStatus = 'Completed'"><xsl:value-of select="$medEndDt" /></xsl:when>
		<xsl:when test="$medStatus = 'Discontinued'"><xsl:value-of select="$medEndDt" /></xsl:when>
		<xsl:when test="$medStatus = 'Void'"><xsl:value-of select="$medEndDt" /></xsl:when>
		<xsl:when test="$medStatus = 'Entered in Error'"><xsl:value-of select="$medEndDt" /></xsl:when>
		<xsl:when test="$medStatus = 'Active'"><xsl:call-template name="medFinishTherapyCourse"><xsl:with-param name="startDt" select="$medStartDt" /><xsl:with-param name="expireDt" select="$medExpireDt" /><xsl:with-param name="endDt" select="$medEndDt" /><xsl:with-param name="performDt" select="$medPerformDt" /><xsl:with-param name="disp" select="$medDisp" /></xsl:call-template></xsl:when>
		<xsl:when test="$medStatus = 'Unauthorized'"><xsl:call-template name="medFinishTherapyCourse"><xsl:with-param name="startDt" select="$medStartDt" /><xsl:with-param name="expireDt" select="$medExpireDt" /><xsl:with-param name="endDt" select="$medEndDt" /><xsl:with-param name="performDt" select="$medPerformDt" /><xsl:with-param name="disp" select="$medDisp" /></xsl:call-template></xsl:when>
		<xsl:otherwise></xsl:otherwise>					
	</xsl:choose>
</xsl:template>

<xsl:template name="medFinishTherapyCourse">
	<xsl:param name="startDt" />
	<xsl:param name="expireDt" />
	<xsl:param name="endDt" />
	<xsl:param name="performDt" />
	<xsl:param name="disp" />

	<xsl:choose>
		<xsl:when test="string-length($expireDt)>0"><xsl:text>(Renew: </xsl:text><xsl:value-of select="$expireDt" /><xsl:text>)</xsl:text></xsl:when>
		<xsl:otherwise>
			<xsl:if test="translate($disp,$lCase,$uCase) = 'RX'">
					<xsl:if test="$startDt != $performDt">
						<xsl:text>(Last: </xsl:text><xsl:value-of select="$performDt" /><xsl:text>)</xsl:text>
					</xsl:if>
			</xsl:if>
		</xsl:otherwise>	
	</xsl:choose>
</xsl:template>

<xsl:template name="formattedMedName">
	<xsl:variable name="medName" select="ENTRYNAME" />
	<xsl:variable name="medSIG" select="SIG" />
	<xsl:variable name="medFreetextSIG" select="FREETEXTSIG" />
	<xsl:variable name="medDisp" select="DISPOSITION" />
	<xsl:variable name="medPRN" select="PRN" />

	<xsl:value-of select="$medName" />
	<xsl:choose>
		<xsl:when test="string-length($medSIG)!=0"><xsl:text>; </xsl:text><xsl:value-of select="$medSIG" /></xsl:when>
		<xsl:when test="string-length($medFreetextSIG)!=0"><xsl:text>; </xsl:text><xsl:value-of select="$medFreetextSIG" /></xsl:when>
		<xsl:otherwise></xsl:otherwise>
	</xsl:choose>
	<xsl:if test="translate($medDisp,$lCase,$uCase) != 'NONE'"><xsl:text> ; </xsl:text><xsl:value-of select="$medDisp" /></xsl:if>
	<xsl:if test="string-length($medPRN)!=0"><xsl:text> </xsl:text><xsl:value-of select="$medPRN" /></xsl:if>
</xsl:template>
	<!-- ============================== coded values =================== -->
<xsl:template name="problemTypeCode">
	<xsl:param name="icdValue" />
	<xsl:choose>
		<xsl:when test="string-length($icdValue)!=0">Diagnosis</xsl:when>
		<xsl:otherwise>Problem</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="alertTypeCode">
	<xsl:param name="alertCategory" />
	<xsl:choose>
		<xsl:when test="$alertCategory = 'Allergy'">Allergy</xsl:when>
		<xsl:when test="$alertCategory = 'Adverse Reaction'">AdverseReaction</xsl:when>
		<xsl:when test="$alertCategory = 'Suspect'">Alert</xsl:when>
		<xsl:when test="$alertCategory = 'Other'">Alert</xsl:when>
		<xsl:otherwise>Allergy</xsl:otherwise>					
	</xsl:choose>
</xsl:template>
<!-- =============   footer ================== -->
<xsl:template name="OrgActor">
<Actor>
	<ActorObjectID>Site</ActorObjectID>
	<Organization>
		<Name><xsl:value-of select="Patient/Source/SITENAME" /></Name>
	</Organization>
	<xsl:call-template name="mailAddressElements"><xsl:with-param name="theNode" select="Patient/Source"/></xsl:call-template>
	<xsl:call-template name="phoneElements"><xsl:with-param name="theNode" select="Patient/Source"/></xsl:call-template>
	<Source><Actor><ActorID>ePrescribe</ActorID></Actor></Source>		
</Actor>
</xsl:template>

<xsl:template name="InfoSysActor">
<Actor>
	<ActorObjectID>ePrescribe</ActorObjectID>
	<InformationSystem>
		<Name>ePrescribe by Allscripts, LLC.</Name>
		<Type>ERX</Type>
		<Version><xsl:value-of select="Patient/Source/ERXVERSION" /></Version>
	</InformationSystem>
	<Source><Actor><ActorID>ePrescribe</ActorID></Actor></Source>	
</Actor>
</xsl:template>

<!-- repository actor -->
<xsl:template match="Patient/Target/REPOSITORY">
<xsl:variable name="repositoryName" select="."/>
<xsl:if test="string-length($repositoryName)>0">
<Actor>
	<ActorObjectID>Repository</ActorObjectID>
	<InformationSystem>
		<Name><xsl:value-of select="."/></Name>
		<Type>XDS Repository</Type>
	</InformationSystem>
	<Source><Actor><ActorID>ePrescribe</ActorID></Actor></Source>	
</Actor>
</xsl:if>
</xsl:template>

<!-- payor org actor -->
<xsl:template name="PayorActor">
<Actor>
	<ActorObjectID>Payor</ActorObjectID>
	<Organization>
		<Name><xsl:value-of select="Patient/Demographics/PRIMARYINSURANCE" /></Name>
	</Organization>
	<Source><Actor><ActorID>ePrescribe</ActorID></Actor></Source>		
</Actor>
</xsl:template>

<!--     =============  helpers  ===============   -->
<xsl:template name="formatDateyyyy-mm-dd">
	<xsl:param name="dateString"/>
	<xsl:choose>
		<xsl:when test="string-length($dateString)=4">	<!-- YYYY -->
			<xsl:value-of select="$dateString" />
		</xsl:when>
		<xsl:when test="string-length($dateString)=8">	<!-- mon YYYY -->
			<xsl:variable name="monthString"><xsl:value-of select="substring($dateString,1,3)"/></xsl:variable>
			<xsl:variable name="year"><xsl:value-of select="substring($dateString,5,4)"/></xsl:variable>
			<xsl:value-of select="$year"/>-<xsl:call-template name="monthCode"><xsl:with-param name="monthName" select="$monthString"/></xsl:call-template>
		</xsl:when>
		<xsl:when test="string-length($dateString)>10">	<!-- at least dd mon YYYY -->
			<xsl:variable name="day"><xsl:value-of select="substring($dateString,1,2)"/></xsl:variable>
			<xsl:variable name="monthString"><xsl:value-of select="substring($dateString,4,3)"/></xsl:variable>
			<xsl:variable name="year"><xsl:value-of select="substring($dateString,8,4)"/></xsl:variable>
			<xsl:value-of select="$year"/>-<xsl:call-template name="monthCode"><xsl:with-param name="monthName" select="$monthString"/></xsl:call-template>-<xsl:value-of select="$day"/>		
		</xsl:when>
		<xsl:otherwise>	<!-- not sure what this is so just return it -->
			<xsl:value-of select="$dateString" />
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name="monthCode">
	<xsl:param name="monthName"/>
	<xsl:variable name="monthAbbrv"><xsl:value-of select="substring($monthName,1,3)"/></xsl:variable>
	<xsl:choose>
		<xsl:when test="$monthAbbrv='Jan'">01</xsl:when>
		<xsl:when test="$monthAbbrv='Feb'">02</xsl:when>
		<xsl:when test="$monthAbbrv='Mar'">03</xsl:when>
		<xsl:when test="$monthAbbrv='Apr'">04</xsl:when>
		<xsl:when test="$monthAbbrv='May'">05</xsl:when>
		<xsl:when test="$monthAbbrv='Jun'">06</xsl:when>
		<xsl:when test="$monthAbbrv='Jul'">07</xsl:when>
		<xsl:when test="$monthAbbrv='Aug'">08</xsl:when>
		<xsl:when test="$monthAbbrv='Sep'">09</xsl:when>
		<xsl:when test="$monthAbbrv='Oct'">10</xsl:when>
		<xsl:when test="$monthAbbrv='Nov'">11</xsl:when>
		<xsl:otherwise>12</xsl:otherwise>
	</xsl:choose>	
</xsl:template>

<xsl:template name="mailAddressElements">
	<xsl:param name="theNode"/>
	<Address>
		<Type><Text>Mail</Text></Type>
		<xsl:variable name="line1" select="$theNode/ADDRESSLINE1"/>
		<xsl:variable name="line2" select="$theNode/ADDRESSLINE2"/>
		<xsl:variable name="city" select="$theNode/CITY"/>
		<xsl:variable name="state" select="$theNode/STATE"/>
		<xsl:variable name="country" select="$theNode/COUNTRY"/>
		<xsl:variable name="zip" select="$theNode/ZIPCODE"/>
			<xsl:if test="string-length($line1)>0">
			<Line1>
				<xsl:value-of select="$line1" />
			</Line1>
			</xsl:if>
			<xsl:if test="string-length($line2)>0">
			<Line2>
				<xsl:value-of select="$line2" />
			</Line2>
			</xsl:if>
			<xsl:if test="string-length($city)>0">
			<City>
				<xsl:value-of select="$city" />
			</City>
			</xsl:if>
			<xsl:if test="string-length($state)>0">
			<State>
				<xsl:value-of select="$state" />
			</State>
			</xsl:if>
			<xsl:if test="string-length($country)>0">
			<Country>
				<xsl:value-of select="$country" />
			</Country>
			</xsl:if>
			<xsl:if test="string-length($zip)>0">
			<PostalCode>
				<xsl:value-of select="$zip" />
			</PostalCode>
			</xsl:if>
		</Address>
</xsl:template>

	<!-- phone numbers -->
	<xsl:template name="phoneElements">
		<xsl:param name="theNode"/>
		<!-- a primary number -->
		<xsl:variable name="primaryPhone">(<xsl:value-of select="$theNode/PHONEAREA" />)<xsl:value-of select="$theNode/PHONEEXCHANGE" />-<xsl:value-of select="$theNode/PHONELAST4" /></xsl:variable>
		<xsl:variable name="primaryPhString">
			<xsl:value-of select="$primaryPhone" />
		</xsl:variable>
		<xsl:if test="$primaryPhString!=$noPhoneValue">
			<xsl:if test="$primaryPhString != $emptyPhoneValue">
				<Telephone>
					<Value>
						<xsl:value-of select="$primaryPhone" />
					</Value>
					<Type><Text>Primary</Text></Type>
				</Telephone>
			</xsl:if>
		</xsl:if>
		<!-- do we have a home number ? -->
		<xsl:variable name="homePhone">(<xsl:value-of select="$theNode/HOMEPHONEAREA" />)<xsl:value-of select="$theNode/HOMEPHONEEXCHANGE" />-<xsl:value-of select="$theNode/HOMEPHONELAST4" /></xsl:variable>
		<xsl:variable name="homePhString">
			<xsl:value-of select="$homePhone" />
		</xsl:variable>
		<xsl:if test="$homePhString!=$noPhoneValue">
			<xsl:if test="$homePhString != $emptyPhoneValue">
				<Telephone>
					<Value>
						<xsl:value-of select="$homePhone" />
					</Value>
					<Type><Text>Home</Text></Type>
				</Telephone>
			</xsl:if>
		</xsl:if>
		<!-- do we have a work number ? -->
		<xsl:variable name="workPhone">(<xsl:value-of select="$theNode/WORKPHONEAREA" />)<xsl:value-of select="$theNode/WORKPHONEEXCHANGE" />-<xsl:value-of select="$theNode/WORKPHONELAST4" /></xsl:variable>
		<xsl:variable name="workPhString">
			<xsl:value-of select="$workPhone" />
		</xsl:variable>
		<xsl:if test="$workPhString!=$noPhoneValue">
			<xsl:if test="$workPhString != $emptyPhoneValue">
				<Telephone>
					<Value>
						<xsl:value-of select="$workPhone" />
					</Value>
					<Type><Text>Work</Text></Type>
				</Telephone>
			</xsl:if>
		</xsl:if>
	</xsl:template>

<xsl:template name="emailElements">
<xsl:param name="theNode"/>
<xsl:variable name="emailAddr" select="$theNode/EMAILADDRESS"/>
<xsl:if test="string-length($emailAddr)>0">
	<EMail>
	  <Value><xsl:value-of select="$emailAddr"/></Value> 
	  </EMail>
  </xsl:if>
</xsl:template>

	<!-- return 1 if the User.ID is different than both the PCP and the DEX's ID -->
	<xsl:template name="userIsUnique">
		<xsl:variable name="userNode" select="Patient/User"/>
		<xsl:variable name="providerDESNode" select="Patient/Provider_DEX" />
		<xsl:variable name="providerPCPNode" select="Patient/Provider_PCP" />
		<xsl:choose>
			<xsl:when test="count($userNode)=0">0</xsl:when>
			<xsl:otherwise>
				<xsl:choose>
					<xsl:when test="$userNode/ID=$providerDESNode/ID">0</xsl:when>
					<xsl:otherwise>
						<xsl:choose>
							<xsl:when test="$userNode/ID=$providerPCPNode/ID">0</xsl:when>
							<xsl:otherwise>1</xsl:otherwise>
						</xsl:choose>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
</xsl:stylesheet>
