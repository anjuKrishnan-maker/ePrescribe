<%@ Page Language="C#" AutoEventWireup="true" Inherits="eRxWeb.ManualNPICheckForm" CodeBehind="ManualNPICheckForm.aspx.cs" %>
<%@ Import Namespace="Allscripts.ePrescribe.Common" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta name="robots" content="noindex" />
    <title></title>
    <link id="lnkDefaultStyleSheet" href="Style/Default.css" runat="server" rel="stylesheet" type="text/css" />
   <%if (Session["Theme"] != null)
      { %>
    <link href="<%=Session["Theme"]%>" rel="stylesheet" type="text/css" />
    <%}%>
    <style type="text/css" media="screen">
        .PrintButton
        {
            display: block;
        }
    </style>
    <style type="text/css" media="print">
        .PrintButton
        {
            display: none;
        }
    </style>
</head>
<%if (PlacementResponse != null)
  { %>
<%=PlacementResponse.Header%>
<%} %>
<body>
    <form id="form1" runat="server">
    <table id="Header" width="100%" cellspacing="0" cellpadding="0" style="height: 126px; page-break-after:always">
        <tr>
            <td class="appStripe" rowspan="6" style="height: 8%; vertical-align: top; width: 100px">
                <img id="mainLogo" src="images/shim.gif" width="120" height="65" />
                <img src="images/powered_by.gif" id="poweredByImage" class="sponsored" />
                <center>
                    <img src="images/spinner.gif" alt="" id="loading" style="display: none" /></center>
            </td>
            <td colspan="7" class="appStripe" style="height: 5px">
            </td>
        </tr>
        <tr style="height: 20px">
            <td class="head valgn nowrap" style="text-align: right; padding-right: 10px">
                <asp:Label ID="lblSiteName" runat="server" Font-Bold="true"></asp:Label>
            </td>
        </tr>
        <tr style="height: 20px">
            <td class="head valgn" style="text-align: right; padding-right: 10px">
                <asp:Label ID="lblUser" runat="server" Font-Bold="true"></asp:Label>
            </td>
        </tr>
        <tr style="height: 20px">
            <td class="head valgn" style="text-align: right; padding-right: 10px">
                <%  if (base.SessionLicense.EnterpriseClient.ShowLogoutIcon)
                    { %><a id="lnkLogout" href="logout.aspx" class="PrintButton">Logout</a> &nbsp;&nbsp;<% } %>
            </td>
        </tr>
        <tr style="height: 20px">
            <td class="valgn" style="text-align: right; padding-right: 10px">
                &nbsp;
            </td>
        </tr>
        <tr style="height: 20px">
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <table width="100%" border="0" cellspacing="0" cellpadding="0">
                    <tr>
                        <td class="h1title">
                            <span class="indnt Phead" style="padding-left: 15px;">Manual NPI Check Form</span>
                        </td>
                    </tr>
                    <tr class="h2title">
                        <td class="Phead indnt">
                            <asp:Button ID="btnPrint" runat="server" Text="Print" OnClientClick="javascript:window.print();"
                                CssClass="PrintButton" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div>
                                <table width="100%" cellpadding="5">
                                    <tr>
                                        <td align="center">
                                            <h2>
                                                The Time for ePrescribing is Now</h2>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Welcome to the Manual NPI Validation process for Veradigm ePrescribe™. Fax-In
                                            the form. You must be a prescriber to register.
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            There are three steps to the process:
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <b>Step 1: Complete this form</b>
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <b>Step 2: Fax us valid copies (not web copies please) of the following:</b>
                                            <ul>
                                                <li>This completed form</li>
                                                <li>DEA Number with Expiration date if available</li>
                                                <li>NPI Number</li>
                                                <li>State License Number with Expiration date</li>
                                                <li>Government issued ID (such as drivers license)</li>
                                                <li>Provide a faxed notarized form showing Applicant presented to Notary:<br />
                                                    <br />
                                                    <ul>
                                                        <li>(1) Form of identification from National Government Photo IDs<br />
                                                            <b>OR</b></li>
                                                        <li>(2) Forms of identification from Non-National Government Photo IDs<br />
                                                            <b>OR</b></li>
                                                        <li>(1) Form of identification from Non-National Government Photo IDs <b><u>AND</u></b>
                                                            (1) from Non-Photo IDs</li>
                                                    </ul>
                                                </li>
                                            </ul>
                                        </td>
                                    </tr>                                    
                                    <tr>
                                        <td>
                                            <b>Step 3: Confirm identity and finish registration</b>
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Once your identification is verified, ePrescribe support will contact your practice.
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            We are confident that you and your patients will enjoy the safety and advantages
                                            offered by electronic prescribing with ePrescribe by Veradigm Healthcare, LLC.
                                            If you have any questions or concerns please contact eprescribesupport@allscripts.com
                                            .
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Sincerely,<br />
                                            ePrescribe Support Team
                                            <br />
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <b>Please fax this form and requisite documents to 919.800.6001</b>
                                        </td>
                                    </tr>
                                </table>
                                <table width="100%" cellpadding="10">
                                    <tr>
                                        <td align="center" colspan="2">
                                            <h3>
                                                Practice Information</h3>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 117px">
                                            Practice Name:
                                        </td>
                                        <td>
                                            __________________________________________________
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 117px">
                                            Practice Address:
                                        </td>
                                        <td>
                                            __________________________________________________
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 117px">
                                            Practice City:
                                        </td>
                                        <td>
                                            _________________________________&nbsp&nbsp&nbsp&nbsp State: ________&nbsp&nbsp&nbsp&nbsp
                                            ZIP: ______________
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 117px">
                                            Practice Phone:
                                        </td>
                                        <td>
                                            (________)_________________________________________
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 117px">
                                            Practice Fax:
                                        </td>
                                        <td>
                                            (________)_________________________________________
                                        </td>
                                    </tr>
                                </table>
                                <table cellpadding="10">
                                    <tr>
                                        <td align="center" colspan="2">
                                            <h3>
                                                Provider Information</h3>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            First Name: ___________________________________ &nbsp&nbsp&nbsp Last Name: _______________________________________
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Title:
                                        </td>
                                        <td>
                                            _________________________________________________
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Primary Specialty:
                                        </td>
                                        <td>
                                            _________________________________________________
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Secondary Specialty:
                                        </td>
                                        <td>
                                            _________________________________________________
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Phone:
                                        </td>
                                        <td>
                                            (________)________________________________________
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Birth year:
                                        </td>
                                        <td>
                                            ________
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Last 4 of SSN:
                                        </td>
                                        <td>
                                            ________
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            Email:
                                        </td>
                                        <td>
                                            _________________________________________________
                                            <br />
                                            <span class="smalltext">Please print clearly and legibly. You will receive a confirmation
                                                email to this address once registration is complete.</span>
                                        </td>
                                    </tr>
                                </table>
                                <table cellpadding="10px" style="background-color: #D8D8D8">
                                    <tr>
                                        <td colspan="4">
                                            <i>Either DEA information or NPI is required to register. You will be required to provide
                                                proof of the license that you provide.</i>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            NPI:
                                        </td>
                                        <td>
                                            __________________________
                                        </td>
                                        <td align="right">
                                        </td>
                                        <td>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            DEA Number:
                                        </td>
                                        <td>
                                            __________________________
                                        </td>
                                        <td align="right">
                                            DEA Expiration Date:
                                        </td>
                                        <td>
                                            __________________________
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4">
                                            DEA Schedule:&nbsp&nbsp&nbsp&nbsp II&nbsp&nbsp&nbsp&nbsp III&nbsp&nbsp&nbsp&nbsp
                                            IV&nbsp&nbsp&nbsp&nbsp V&nbsp&nbsp&nbsp&nbsp <span class="smalltext">Circle all that
                                                apply</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            State License Number:
                                        </td>
                                        <td>
                                            __________________________
                                        </td>
                                        <td align="right">
                                            State License Expiration Date:
                                        </td>
                                        <td>
                                            __________________________
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="4">
                                            Issue state: __________________________
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <br />
                                <br />
                                <br />
                                <br />
                                <b>Required Document 1 for Notary:</b>
                                <br />
                                <br />
                                <br />
                                <table width="100%" cellpadding="10px" border="1" style="border-collapse: collapse">
                                    <tr>
                                        <td style="width: 20%">
                                            Document Type
                                        </td>
                                        <td style="width: 50%">
                                        </td>
                                        <td style="width: 30%">
                                            Photo ID: Y/N
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 20%">
                                            Issued By
                                        </td>
                                        <td colspan="2">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 20%">
                                            Serial Number
                                        </td>
                                        <td style="width: 50%">
                                        </td>
                                        <td style="width: 30%">
                                            Expiration Date:
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 20%">
                                            Name appearing on Document
                                        </td>
                                        <td colspan="2">
                                        </td>
                                    </tr>
                                </table>
                                <br />
                                <br />
                                <br />
                                <br />
                                <br />
                                <br />
                                <b>Required Document 2 for Notary:</b>
                                <br />
                                <br />
                                <br />                                 
                                <table width="100%" cellpadding="10px" border="1" style="border-collapse: collapse">
                                    <tr>
                                        <td style="width: 20%">
                                            Document Type
                                        </td>
                                        <td style="width: 50%">
                                        </td>
                                        <td style="width: 30%">
                                            Photo ID: Y/N
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 20%">
                                            Issued By
                                        </td>
                                        <td colspan="2">
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 20%">
                                            Serial Number
                                        </td>
                                        <td style="width: 50%">
                                        </td>
                                        <td style="width: 30%">
                                            Expiration Date:
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width: 20%">
                                            Name appearing on Document
                                        </td>
                                        <td colspan="2">
                                        </td>
                                    </tr>
                                </table>
                                <table cellpadding="10px">
                                    <tr>
                                        <td colspan="2">
                                            <b>For security purposes, please provide your mother's maiden name. We will not be able
                                                to process your application without this information.</b>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2">
                                            Mother's maiden name: ____________________________________________
                                        </td>
                                    </tr>                                    
                                </table>
                                <table width="100%" cellpadding="10px">
                                    <tr>
                                        <td>
                                            Please fax us valid copies (not web copies please) of your State License, DEA certificate,
                                            government issued ID (such as a drivers license), along with this completed document.
                                            <br />
                                            <br />
                                            <b>Please fax this form in its entirety and requisite documents to 919.800.6001</b>
                                            <br />
                                        </td>
                                    </tr>
                                </table>
                                <table width="100%" cellpadding="10">
                                    <tr>
                                        <td>
                                            Veradigm ePrescribe (TM) AUTHORIZATION                                            
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            THIS IS A LEGAL AGREEMENT BETWEEN YOU (DEFINED BELOW) AND Veradigm HEALTHCARE,
                                            LLC (“ALLSCRIPTS”). BEFORE SUBMITTING ANY DATA OR INFORMATION TO Veradigm AS PART
                                            OF THE ePrescribe SERVICE ENROLLMENT PROCESS, YOU MUST CAREFULLY READ AND AGREE
                                            TO THE TERMS AND CONDITIONS CONTAINED IN THIS ePrescribe AUTHORIZATION (THIS AGREEMENT).
                                            BY SIGNING BELOW, YOU REPRESENT THAT YOU ARE ACTING ON BEHALF OF YOURSELF, AS AN
                                            INDIVIDUAL, AND YOUR EMPLOYER (COLLECTIVELY; YOU), AND THAT YOU AGREE TO BE BOUND
                                            BY THIS AGREEMENT.
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            1. DATA SUBMISSION, COLLECTION AND USE. You represent and warrant that (a) all of
                                            the information and data submitted to Veradigm Healthcare, LLC as part of the
                                            ePrescribe enrollment process (collectively, the “Data”) is accurate and complete;
                                            and (b) You have the authority to submit all such Data. You acknowledge and agree
                                            that Veradigm Healthcare, LLC has the right to (i) take all steps necessary to
                                            confirm Your identity and otherwise verify such Data, including without limitation,
                                            the right to submit Data to third parties; (ii) otherwise use such Data in the course
                                            of the ePrescribe enrollment process or the ePrescribe service for any legal purpose
                                            (including without limitation, the right to share such Data with third parties);
                                            and (iii) use such Data to contact you regarding the ePrescribe enrollment process,
                                            the ePrescribe service, or any other product or service that we believe might be
                                            of interest to You. You specifically consent to the foregoing uses of such Data.
                                            You agree to defend, indemnify, and hold Veradigm Healthcare, LLC, its officers,
                                            directors, employees, agents, licensors, and suppliers, harmless from and against
                                            any claims, actions or demands, liabilities and settlements including without limitation,
                                            reasonable legal and accounting fees, resulting from, or alleged to result from,
                                            the provision, receipt or use of any Data submitted to Veradigm Healthcare, LLC
                                            hereunder. Veradigm HEALTHCARE, LLC RESERVES THE RIGHT, IN ITS SOLE DISCRETION,
                                            TO DETERMINE ELIGIBILITY FOR THE ePrescribe SERVICE. YOU UNDERSTAND AND AGREE THAT
                                            ENTERING INTO THIS AGREEMENT OR SUBMITTING DATA HEREUNDER IS NO GUARANTEE THAT YOU
                                            WILL BE APPROVED FOR PARTICIPATION IN THE ePrescribe SERVICE.
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            2. NO WARRANTIES; DISCLAIMER: Veradigm HEALTHCARE, LLC, ON ITS OWN BEHALF AND
                                            ON BEHALF OF ITS LICENSORS, CONTRACTORS, SUPPLIERS AND ANY OTHER PARTIES WHO MAY
                                            BE ASSOCIATED WITH THE ePrescribe SERVICE, TO THE MAXIMUM EXTENT PERMITTED BY LAW,
                                            DISCLAIM ALL WARRANTIES HEREUNDER. TO THE MAXIMUM EXTENT PERMITTED BY APPLICABLE
                                            LAW, NOTWITHSTANDING ANYTHING TO THE CONTRARY CONTAINED IN THIS AGREEMENT, IN NO
                                            EVENT SHALL Veradigm HEALTHCARE, LLC, ITS LICENSORS, SUPPLIERS OR ANY THIRD PARTIES
                                            BE LIABLE FOR ANY INDIRECT, CONSEQUENTIAL, SPECIAL, PUNITIVE OR INCIDENTAL DAMAGES
                                            (INCLUDING WITHOUT LIMITATION, DAMAGES FOR PERSONAL INJURY, SICKNESS, DEATH, BUSINESS
                                            INTERRUPTION, OR LOSS OF BUSINESS INFORMATION) OR DAMAGES FOR LOSS OF PROFITS OR
                                            REVENUES THAT MAY RESULT FROM OR IN CONNECTION WITH THE SUBMISSION, RECEIPT OR USE
                                            OF DATA HEREUNDER, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGES, OR EVEN IF
                                            SUCH POSSIBILITY WAS REASONABLY FORESEEABLE, WHETHER BASED ON WARRANTY, CONTRACT,
                                            TORT OR ANY OTHER LEGAL THEORY. Veradigm HEALTHCARE, LLC SHALL BE LIABLE ONLY
                                            TO THE EXTENT OF ACTUAL DAMAGES INCURRED BY YOU, NOT TO EXCEED ONE HUNDRED DOLLARS
                                            ($100). Remedies under this Agreement are exclusive and are limited to those expressly
                                            provided for in this Agreement.
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            3. GENERAL: Unless and until You enter into a Participation Agreement governing
                                            the ePrescribe Service, this Agreement constitutes the entire agreement between
                                            you and Veradigm Healthcare, LLC with respect to ePrescribe. You expressly agree
                                            that exclusive jurisdiction for any dispute with Veradigm Healthcare, LLC, or
                                            in any way relating to your use of the ePrescribe service, resides in the courts
                                            of the State of Illinois and you further agree and expressly consent to the exercise
                                            of personal jurisdiction in the courts of the State of Illinois in connection with
                                            any such dispute including any claim involving Veradigm Healthcare, LLC or its
                                            affiliates, subsidiaries, employees, contractors, officers, directors, telecommunication
                                            providers, and content provides. This Agreement is governed by the internal substantive
                                            laws of the State of Illinois, without respect to its conflict of laws principles.
                                            If any provision of this Agreement is found to be invalid by any court having competent
                                            jurisdiction, the invalidity of such provision shall not affect the validity of
                                            the remaining provisions of this Agreement, which shall remain in full force and
                                            effect. No waiver of any of this Agreement shall be deemed a further or continuing
                                            waiver of such term or condition or any other term or condition.                                            
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            February 2014
                                            <br />
                                            Confidential and Proprietary, Veradigm Healthcare, LLC
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <b>Provider signature:</b> _______________________________________ <b>Date:</b>
                                            __________________________
                                            <br />
                                            <br />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            I hereby confirm that on the above date, the Applicant personally appeared before
                                            me, signed the document in my presence, and presented identity documents (s), one
                                            of which was a government issued photo.                                            
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <b>Notary Public</b>                                            
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div>
                                                <table  width="100%" border="1" style="border-collapse: collapse">
                                                    <tr>
                                                        <td>                                                            
                                                            <br />
                                                            State of ____________________________, &nbsp County of ____________________________________
                                                            <br />
                                                            <br />
                                                            <br />
                                                            Subscribed and sworn to before me this________&nbsp day of _____________&nbsp 20_____
                                                            <br />
                                                            <br />
                                                            <br />
                                                            By________________________________________________________________________
                                                            <br />
                                                            &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp(name
                                                            of Applicant)
                                                            <br />
                                                            <br />
                                                            Proved to me on the basis of satisfactory evidence to be the person who appeared
                                                            before me.
                                                            <br />
                                                            <br />
                                                            <br />
                                                            ________________________________________________________________________
                                                            <br />
                                                            Printed Name of Notary
                                                            <br />
                                                            <br />
                                                            <br />
                                                            ________________________________________________________________________
                                                            <br />
                                                            Signature of Notary
                                                            <br />
                                                            <br />
                                                            <br />
                                                            <br />
                                                            <br />
                                                            Seal
                                                            <br />
                                                            <br />
                                                            <br />
                                                            <br />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center">
                                            <%=Copyright.ShortText %>
                                            <br />
                                            222 Merchandise Mart #2024, Chicago, Illinois 60654
                                            <br />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <table width="100%" border="0" cellspacing="0" cellpadding="10">
        <tr>
            <td align="center">
                <h5>
                    Listing of Acceptable Identity Proofing Documents</h5>
            </td>
        </tr>
        <tr>
            <td>
                For identity proofing, applicants must present:
                <ul>
                    <li>(1) form of identification from <b>Group A:</b> National Government Photo IDs<br />
                        <b>OR</b></li>
                    <li>(2) forms of identification from <b>Group B:</b> Non-National Government Photo IDs<br />
                        <b>OR</b></li>
                    <li>(1) form of identification from <b>Group B:</b> Non-National Government Photo IDs <b><u>AND</u></b>
                        (1) from <b>Group C:</b> Non-Photo IDs</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td>
                Group A IDs must be issued by a national government agency and must include a photo
                of the applicant. Group B and C IDs can be issued by a government agency (federal,
                state or local) or other reputable organization and may not include a photo of the
                applicant.
            </td>
        </tr>
        <tr>
            <td>
                <b><u>Group A:</u></b>
            </td>
        </tr>
        <tr>
            <td>
                <b>The following is an <u>example</u> list of national government agency IDs which generally
                    include a picture and qualify as the sole form of identification:</b>
                <ul>
                    <li>U.S. Passport (unexpired or expired)</li>
                    <li>Unexpired foreign passport, with I-551 stamp or attached Form I-94 indicating unexpired
                        employment authorization</li>
                    <li>Driver's license issued by a non-US national (state, regional, or provincial driver's
                        licenses do not apply) government authority provided it contains a photograph and
                        information such as name, date of birth, gender, height, eye color and address</li>
                    <li>Permanent Resident Card or Allen Registration Receipt Card with photograph (Form
                        I-151 or I-551)</li>
                    <li>U.S. Military ID Card</li>
                    <li>Military dependent's ID card</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td>
                <b><u>Group B:</u></b>
            </td>
        </tr>
        <tr>
            <td>
                <b>The following is an <u>example</u> list of acceptable non-national government forms
                    of identification that include a photo of the applicant. The applicant may present
                    any two forms of ID from this list, or optionally one from this list and one from
                    Group C below:</b>
                <ul>
                    <li>Driver's license or ID card issued by a state or outlying possession of the United
                        States provided it contains a photograph and information such as name, date of birth,
                        gender, height, eye color and address</li>
                    <li>Driver's license issued by a non-US government authority (state, regional, or provincial)
                        provided it contains a photograph and information such as name, date of birth, gender,
                        height, eye color and address</li>
                    <li>ID card issued by a state or local government agencies or entities,provided it contains
                        a photograph and information such as name, date of birth, gender, height, eye color
                        and address</li>
                    <li>A corporate ID card with photograph</li>
                    <li>School ID card with a photograph</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td>
                <b><u>Group C:</u></b>
            </td>
        </tr>
        <tr>
            <td>
                <b>The following is an <u>example</u> list of acceptable forms of identification that
                    do not have a photo of the applicant. The applicant may only present one of the
                    forms of identification ID from this list in conjunction with a single form of identification
                    from Group B above:</b>
                <ul>
                    <li>Certificate of U.S. Citizenship (Form N-560 or N-561)</li>
                    <li>Certificate of Naturalization (Form N-550 or N-570)</li>
                    <li>Unexpired Temporary Resident Card(Form I-688)</li>
                    <li>Unexpired Employment Authorization Card(Form I-688A)</li>
                    <li>Unexpired Reentry Permit(Form I-327)</li>
                    <li>Unexpired Refugee Travel Document(Form I-571)</li>
                    <li>Unexpired Employment Authorization Document issued by DHS that contains a photograph(Form
                        I-688B)</li>
                    <li>Voter registration card</li>
                    <li>U.S. Coast Guard Merchant Mariner Card</li>
                    <li>U.S. social security card issued by the Social Security Administration (other than
                        a card stating it is not valid for employment)</li>
                    <li>Original or certified copy of birth certificate issued by a state, county, municipal
                        authority or outlying possession of the United States bearing an official seal</li>
                    <li>Certificate of Birth Abroad issued by the Department of State (FormFS-545 or FormDS-1350)</li>
                </ul>
            </td>
        </tr>
    </table>
    </form>
    <%if ( new eRxWeb.AppCode.AppConfig().GetAppSettings<bool>(eRxWeb.AppCode.AppConfig.K_IS_GA_ENABLED) == true)
          { %>
                <script type="text/javascript">var gaAccountId = '<%= new eRxWeb.AppCode.AppConfig().GetAppSettings(eRxWeb.AppCode.AppConfig.K_GA_ACCOUNT_ID) %>'</script>
                <script src="js/googleAnalyticsInit.js" type="text/javascript"> </script>
                <script type="text/javascript"> ga('send', 'pageview');</script>
        <%} %>
</body>
</html>
