<%@ Page Language="C#" MasterPageFile="~/Help/HelpMasterPage.master" AutoEventWireup="true" Inherits="eRxWeb.Help_Tutorial" Title="Tutorials" Codebehind="Tutorial.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<!--15.1.3 Changes- User Category tabs and links updated - Rags -12/15/10 -->
<p id="pTutorials" runat="server">
<%--<br />
     Click on the user categories below to view the links to Tutorials.<br />
    <br />
     Are you a: <br />
     <br />--%>
</p>

<%--<ajaxToolkit:Accordion  ID="sideAccordion" runat="server" ContentCssClass="accordionPanelContent" 
                        HeaderCssClass="accordionPanelHeader" FadeTransitions="true" TransitionDuration="250" FramesPerSecond="40"
                        RequireOpenedPane="false" SuppressHeaderPostbacks="true" SelectedIndex="-1" >
    <Panes>
    <ajaxToolkit:AccordionPane ID="AccordionPane0" runat="server"  >
            <Header><strong>Physician</strong></Header>
            <Content>
                <div style="left:20px; ">
                    <span class="copy">
                        <a href="/media/tutorials/newscriptdoc/newscriptdoc.htm">How to Write a New Prescription</a><br />
                        <br />
                        <a href="/media/tutorials/existingscriptdoc/existingscriptdoc.htm">How to Write a Single Prescription </a><br />
                        <br />
                        <a href="/media/tutorials/writingmultiplerxs/writingmultiplerxs.htm">How to Write Multiple Prescriptions </a><br />
                        <br />
                        <a href="/media/tutorials/scriptbydx/scriptbydx.html">How to Write a Prescription by Diagnosis </a><br />     
                        <br />
                        <a href="/media/tutorials/delsig/delsig.html">Creating a Sig </a><br />              
                        <br />
                        <a href="/media/tutorials/reconcilerefill/reconcilerefill.html">Processing a Refill </a><br />   
                        <br />
                        <a href="/media/tutorials/formularyalts/formularyalts.html">Formulary Indicators </a><br />
                        <br />
                        <a href="/media/tutorials/Coveragecopay/Coveragecopay.html">Coverage/Co-Pay </a><br />
                        <br />
                        <a href="/media/tutorials/preemptivedur/preemptivedur.html">Pre-emptive DUR </a><br />
                        <br />
                        <a href="/media/tutorials/addptreportedmed/addptreportedmed.html">Adding a Patient Reported Medication </a><br />
                        <br />
                        <a href="/media/tutorials/removerxfromscriptpad/removerxfromscriptpad.html">Removing a Prescription from the Script Pad </a> <br />
                        <br />
                        <a href="/media/tutorials/editrxfromscriptpad/editrxfromscriptpad.html">Editing a Prescription from the Script Pad </a><br />
                        <br />
                        <a href="/media/tutorials/changepharmfromscriptpad/changepharmfromscriptpad.html">Changing a Pharmacy on the Script Pad </a><br />
                        <br />
                        <a href="/media/tutorials/buttonsonscriptpad/buttonsonscriptpad.html">Buttons on the Script Pad </a><br />     
                        <br />
                        <a href="/media/tutorials/printmedlist/printmedlist.html">Printing a Current Medication List </a><br />
                        <br />
                        <a href="/media/tutorials/dceie/dceie.html">How to Inactivate a Prescription (D/C-EIE-Complete) </a><br />
                        <br />
                        <a href="/media/tutorials/processtaskrefilldoc/processtaskrefilldoc.htm">How to Process a Providers Task </a><br />
                        <br />
                        <a href="/media/tutorials/messagequeue/messagequeue.html">Confirming an Error in the Message Queue </a><br />
                        <br />
                        <a href="/media/tutorials/infoscript/infoscript.html">Info Script </a><br />
                        <br />
                        <a href="/media/tutorials/orderdeluxe/orderdeluxe.html">How to Order Deluxe </a><br />     
                        <br />
                        <a href="/media/tutorials/editrxfromscriptpad/editrxfromscriptpad.html">Editing a Prescription on the  Script Pad </a><br />
                        <br />
                        <a href="/media/tutorials/PatientReviewHxscreen/PatientReviewHxscreen.htm">Patient Review History Screen </a><br />
                        <br />
                        <a href="/media/tutorials/ePA/CreateEPARequest.htm">Create ePA Request </a><br />
                        <br />
                        <a href="/media/tutorials/ePA/AnswerEPATask.htm">Answer/Review ePA Task </a><br />
                        <br />
                        <a href="/media/tutorials/EpcsIntro/index.html">Sending Controlled Substances Electronically</a><br />
                        <br />
                    </span>
                </div>
            </Content>
        </ajaxToolkit:AccordionPane>
        <ajaxToolkit:AccordionPane ID="AccordionPane1" runat="server"  >
            <Header><strong>POB</strong></Header>
            <Content>
                <div style="left:20px; ">
                    <span class="copy">
                        <a href="/media/tutorials/addpatient/addpatient.html">Adding a Patient</a><br />
                        <br />
                        <a href="/media/tutorials/checkin/checkin.html">Checking a Patient In</a><br />
                        <br />
                        <a href="/media/tutorials/addeditallergy/addeditallergy.htm">Add/Edit an Allergy</a><br />
                        <br />
                        <a href="/media/tutorials/adddx/adddx.html">Adding a  Diagnosis</a><br />              
                        <br />
                        <a href="/media/tutorials/editdx/editdx.html">Editing a Diagnosis</a><br />   
                        <br />
                        <a href="/media/tutorials/cantfindpharm/cantfindpharm.htm">Add/Edit/Remove a Retail Pharmacy</a><br />
                        <br />
                        <a href="/media/tutorials/addmopharm/addmopharm.html">Adding a Mail Order Pharmacy</a><br />
                        <br />
                        <a href="/media/tutorials/existingscriptpob/existingscriptpob.htm">How to Write a Prescription as a POB</a><br />
                        <br />
                        <a href="/media/tutorials/tasktodr/tasktodr.html">How to Send a Task to a Doctor</a> <br />
                        <br />
                        <a href="/media/tutorials/processtaskrefillpob/processtaskrefillpob.htm">How to Process Assistant's Task</a><br />
                        <br />
                        <a href="/media/tutorials/reconcilerefill/reconcilerefill.html">Processing a Refill</a><br />
                        <br />
                        <a href="/media/tutorials/formularyalts/formularyalts.html">Formulary Alternatives</a><br />     
                        <br />
                        <a href="/media/tutorials/coveragecopay/coveragecopay.html">Coverage/Co-Pay</a><br />
                        <br />
                        <a href="/media/tutorials/preemptivedur/preemptivedur.html">Pre-emptive DUR</a><br />
                        <br />
                        <a href="/media/tutorials/addptreportedmed/addptreportedmed.html">Adding a Patient Reported Medication</a><br />
                        <br />
                        <a href="/media/tutorials/removerxfromscriptpad/removerxfromscriptpad.html">Removing a Prescription from the Script Pad</a><br />
                        <br />
                        <a href="/media/tutorials/editrxfromscriptpad/editrxfromscriptpad.html">Editing a Prescription on the Script Pad</a><br />
                        <br />
                        <a href="/media/tutorials/changepharmfromscriptpad/changepharmfromscriptpad.html">Changing a Pharmacy on the Script Pad</a><br />     
                        <br />
                        <a href="/media/tutorials/buttonsonscriptpad/buttonsonscriptpad.html">Buttons on the Script Pad</a><br />
                        <br />
                        <a href="/media/tutorials/printmedlist/printmedlist.html">Printing a Current Medication List</a><br />
                        <br />
                        <a href="/media/tutorials/dceie/dceie.html">How to Inactivate a Prescription (D/C-EIE-Complete)</a><br />
                        <br />
                        <a href="/media/tutorials/messagequeue/messagequeue.html">Confirming an Error in the Message Queue</a><br />
                        <br />
                        <a href="/media/tutorials/PatientReviewHxscreen/PatientReviewHxscreen.htm">Patient Review History Screen </a><br />
                        <br />
                        <a href="/media/tutorials/ePA/CreateEPARequest.htm">Create ePA Request </a><br />
                        <br />
                        <a href="/media/tutorials/ePA/AnswerEPATask.htm">Answer/Review ePA Task </a><br />
                        <br />

                    </span>
                </div>
            </Content>
        </ajaxToolkit:AccordionPane>
        <ajaxToolkit:AccordionPane ID="AccordionPane2" runat="server" >
            <Header><strong>Administrator</strong></Header>
            <Content>
                <div style="left:20px; ">
                    <span class="copy">
                        <a href="/media/tutorials/addedituser/index.html">Adding Users</a><br />
                        <br />
                        <a href="/media/tutorials/addedituser/index.html">Editing  Users</a><br />
                        <br />
                        <a href="/media/tutorials/addeditsiteinfo/addeditsiteinfo.htm">Add/Edit a Site Information</a><br />
                        <br />
                        <a href="/media/tutorials/durset/durset.html">DUR Settings</a><br />              
                        <br />
                        <a href="/media/tutorials/unlockuser/unlockuser.html">How to Unlock a User</a><br />   
                        <br />
                        <a href="/media/tutorials/Inactivateuser/Inactivateuser.html">How to Inactivate a User</a><br />
                        <br />
                        <a href="/media/tutorials/resetpw/resetpw.html">How to Reset a Password for a User</a><br />
                        <br />
                        <a href="/media/tutorials/orderdeluxe/orderdeluxe.html">How to Order Deluxe</a><br />
                        <br />
                        <a href="/media/tutorials/rxpaper/rxpaper.html">How to Order Prescription Paper</a><br />
                        <br />
                        <a href="/media/tutorials/mergepatients/mergepatients.html">How to Merge a Patient</a> <br />
                    </span>                   
                </div>
            </Content>
        </ajaxToolkit:AccordionPane>
        <ajaxToolkit:AccordionPane ID="AccordionPane3" runat="server" >
            <Header><strong>Staff Member</strong></Header>
            <Content>
                <div style="left:20px; ">
                    <span class="copy">
                        <a href="/media/tutorials/addpatient/addpatient.html">Adding a Patient</a><br />
                        <br />
                        <a href="/media/tutorials/addeditallergy/addeditallergy.htm">Add/Edit an Allergy</a><br />
                        <br />
                        <a href="/media/tutorials/adddx/adddx.html">Adding a Diagnosis</a><br />     
                        <br />
                        <a href="/media/tutorials/editdx/editdx.html">Editing a Diagnosis</a><br />              
                        <br />
                        <a href="/media/tutorials/cantfindpharm/cantfindpharm.htm">Add/Edit/Remove a Retail Pharmacy</a><br />
                        <br />
                        <a href="/media/tutorials/addmopharm/addmopharm.html">Adding a Mail Order Pharmacy</a><br />
                        <br />
                        <a href="/media/tutorials/checkin/checkin.html">Checking a Patient In</a><br />
                    </span>
                </div>
            </Content>
        </ajaxToolkit:AccordionPane>
        <ajaxToolkit:AccordionPane ID="AccordionPane4" runat="server" >
            <Header><strong>All</strong></Header>
            <Content>
                <div style="left:20px; ">
                    <span class="copy">
                        <a href="/media/tutorials/erxoverview15_2/erxoverview.htm">Welcome</a><br />
                        <br />
                        <a href="/media/tutorials/siteselection/siteselection.html">Selecting your Location from the Site Selection Page</a><br />
                        <br />
                        <a href="/media/tutorials/editownprofile/editownprofile.html">Editing your ePrescribe Profile</a><br />
                        <br />
                        <a href="/media/tutorials/reviewhx/reviewhx.html">Reviewing the Patients Medication History</a><br />     
                    </span>
                </div>
            </Content>
        </ajaxToolkit:AccordionPane>
    </Panes>                                             
</ajaxToolkit:Accordion>--%>
</asp:Content>
