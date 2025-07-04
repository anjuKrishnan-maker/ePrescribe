using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Response;
using Allscripts.Impact;
using eRxWeb.AppCode.DurBPL.RequestModel;
using eRxWeb.AppCode.DurBPL.ResponseModel;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Data;
using Telerik.Web.UI;
using static Allscripts.Impact.IgnoreReason;
using static Allscripts.ePrescribe.Common.Constants;

namespace eRxWeb.AppCode.DurBPL
{
    internal class DurSingleSelect : DurBase,IDur
    {
        const string RefillMsg = "Controlled substance meds being sent electronically could not be digitally signed.  Please try again or print.";
        const string To = "To";
        const string  SendToPharmacy= "SENTTOPHARMACY";
        const string MedicationName = "MedicationName";
        const string sucessMessage = "Rx for {0} approved and successfully sent to default printer for {1}.";
        public CSMedRefillNotAllowedContactResponse CSMedRefillRequestNotAllowedOnContactProvider(ICSMedRefillNotAllowedContactRequest contactRequest)
        {
            throw new NotImplementedException();
        }

        public CSMedRefillNotAllowedContactResponse CSMedRefillRequestNotAllowedOnContactProvider(ICSMedRefillNotAllowedContactRequest contactRequest, IImpactDurWraper impactDur)
        {
            throw new NotImplementedException();
        }

        public CSMedRefillNotAllowedPrintRefillResponse CSMedRefillRequestNotAllowedOnPrintRefillRequest(ICSMedRefillNotAllowedPrintRefillRequest requestOnPrintRefillRequest, IImpactDurWraper impactDur)
        {
            CSMedRefillNotAllowedPrintRefillResponse responseOnPrintRefillRequest = new CSMedRefillNotAllowedPrintRefillResponse();
            long taskID = Convert.ToInt64(PageState[SessionVariables.ApproveWorkflowRefillTaskId]);
            string rxID = responseOnPrintRefillRequest.RxId = Convert.ToString(PageState[SessionVariables.ApproveWorkflowRefillRxId]);

            //If the task type is to approve, then mark the rx to approved status (NEW)
            impactDur.ApprovePrescription(rxID, 1, PageState[SessionVariables.LicenseId].ToString(), PageState[SessionVariables.UserId].ToString(), base.DBID);
            //Update the task status
            impactDur.UpdateRxTask(taskID, PrescriptionTaskType.APPROVAL_REQUEST, PrescriptionTaskStatus.PROCESSED, Constants.PrescriptionStatus.NEW, null, string.Empty, PageState[SessionVariables.UserId].ToString(), base.DBID);
            SetPageStateVariables();
            impactDur.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS,SessionLicenseID,SessionUserID, SessionPatientID,rxID, requestOnPrintRefillRequest.UserHostAddress, base.DBID);
            responseOnPrintRefillRequest.Url = string.Concat(PageNames.CSS_DETECT, requestOnPrintRefillRequest.QueryString != null ? string.Concat("?", requestOnPrintRefillRequest.QueryString) : string.Empty);
            return responseOnPrintRefillRequest;
        }
        protected void setSuccessMessage(bool msgType, string rxID, EPCSDigitalSigningRequest signingRequest, EPCSDigitalSigningResponse signingResponse, IImpactDurWraper impactDurWraper)
        {
            if (PageState[SessionVariables.RxiD] != null)
            {
                if (msgType)
                {
                    DataSet dsMedName = impactDurWraper.ChGetRXDetails(rxID, base.DBID);

                    if (signingRequest.Request.QueryString[SessionVariables.Patient] != null)
                    {
                        PageState[SessionVariables.SuccessMsg] =String.Format( sucessMessage, Convert.ToString(dsMedName.Tables[0].Rows[0][MedicationName]), signingRequest.Request.QueryString[SessionVariables.Patient].ToString());
                    }
                    else if (PageState[SessionVariables.Patient] != null)
                    {
                        DataSet dsPat = CHPatient.PatientSearchById(PageState[SessionVariables.PatientId].ToString(), PageState[SessionVariables.LicenseId].ToString(), string.Empty, base.DBID);
                      //  PageState[SessionVariables.SuccessMsg] = string.Format(sucessMessage, dsMedName.Tables[0].Rows[0][MedicationName].ToString(), dsPat.Tables["Patient"].Rows[0]["LastName"].ToString() + ", " + dsPat.Tables["Patient"].Rows[0]["FirstName"].ToString() + ".";
                    }

                }
                else if (PageState[SessionVariables.SuccessMsg] == null)
                {
                    if (signingRequest.Request.QueryString[SessionVariables.Patient] != null)
                    {
                        DataSet ds = Prescription.Load(rxID, base.DBID);
                        //PageState[SessionVariables.SuccessMsg] = string "Rx for " + ds.Tables["RxDetail"].Rows[0]["MedicationName"].ToString() + " approved for " + signingRequest.Request.QueryString["Patient"].ToString() + ".";
                    }
                    else if (PageState[SessionVariables.PatientId] != null)
                    {
                        DataSet ds = Prescription.Load(rxID, base.DBID);
                        DataSet dsPat = CHPatient.PatientSearchById(PageState[SessionVariables.PatientId].ToString(), SessionLicenseID, string.Empty, base.DBID);
                        //PageState[SessionVariables.SuccessMsg] = "Rx for " + ds.Tables["RxDetail"].Rows[0]["MedicationName"].ToString() + " approved for " + dsPat.Tables["Patient"].Rows[0]["LastName"].ToString() + ", " + dsPat.Tables["Patient"].Rows[0]["FirstName"].ToString() + ".";
                    }
                }
            }
        }
        public EPCSDigitalSigningResponse EPCSDigitalSigningOnDigitalSigningComplete(EPCSDigitalSigningRequest signingRequest, IImpactDurWraper impactDur)
        {
            EPCSDigitalSigningResponse signingResponse = new EPCSDigitalSigningResponse();
            if (signingRequest.DsEventArgs.Success)
            {
                if (signingRequest.IsApprovalRequestWorkflow)
                {
                    //Copy and paste all the required code and just execute it
                    //Check the redirection logic.
                    if (PageState[SessionVariables.ApproveWorkflowRefillTaskId] != null)
                    {
                        long taskID = Convert.ToInt64(PageState[SessionVariables.ApproveWorkflowRefillTaskId]);
                        string rxID = PageState[SessionVariables.ApproveWorkflowRefillRxId].ToString();
                        //If the task type is to approve, then mark the rx to approved status (NEW)
                        impactDur.ApprovePrescription(rxID, 1, PageState[SessionVariables.LicenseId].ToString(), PageState[SessionVariables.UserId].ToString(), base.DBID);
                        long serviceTaskID = -1;
                        foreach (KeyValuePair<string, string> kvp in signingRequest.DsEventArgs.SignedMeds)
                        {
                            if (kvp.Key.Equals(rxID))
                            {
                                if (!string.IsNullOrEmpty(kvp.Value) && Convert.ToString(PageState[ SessionVariables.STANDING]) == "1")
                                {
                                    serviceTaskID = impactDur.SendThisEPCSMessage(kvp.Value, base.SessionLicenseID, base.SessionUserID, base.DBID);
                                }
                            }
                        }

                        //Update the task status
                        impactDur.UpdateRxTask(taskID, Constants.PrescriptionTaskType.APPROVAL_REQUEST, Constants.PrescriptionTaskStatus.PROCESSED, Constants.PrescriptionStatus.NEW, null, string.Empty, PageState[SessionVariables.UserId].ToString(), base.DBID);

                        impactDur.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, SessionLicenseID, SessionUserID, base.SessionPatientID, rxID, signingRequest.Request.UserIpAddress(), base.DBID);

                        string RxStatus = SendToPharmacy; //Setting the status of the Prescription April 3 2007..
                        impactDur.UpdateRxDetailStatus(SessionLicenseID, SessionUserID, rxID, RxStatus, base.DBID);

                        setSuccessMessage(false, rxID);
                        PageState[SessionVariables.RefillMsg] = null;

                        //RxUser user = new RxUser();

                        PageState[ SessionVariables.ApproveWorkflowRefillTaskId] = null;
                        PageState[SessionVariables.ApproveWorkflowRefillRxId] = null;

                        if (signingRequest.Request.QueryString[To] != null &&
                                (signingRequest.Request.QueryString[To].ToString().Contains(Constants.PageNames.APPROVE_REFILL_TASK)))
                        {
                            signingResponse.Url = string.Format("{0} ? Msg = {1}" , Constants.PageNames.APPROVE_REFILL_TASK ,  PageState[SessionVariables.SuccessMsg].ToString());
                        }

                        if (signingRequest.Request.QueryString[To] != null)
                        {
                            signingResponse.Url = signingRequest.Request.QueryString[To].ToString();
                        }
                        else
                        {
                            signingResponse.Url = Constants.PageNames.APPROVE_REFILL_TASK;
                        }

                    }
                }
            }
            else
            {
                if (signingRequest.DsEventArgs.ForceLogout)
                {
                    //force the user to log out if they've entered invalid credentials 3 times in a row
                    signingResponse.Url= Constants.PageNames.LOGOUT;
                }
                else if (string.IsNullOrEmpty(signingRequest.DsEventArgs.Message))
                {
                    PageState[SessionVariables.RefillMsg] = RefillMsg;
                }
                else
                {
                    PageState[SessionVariables.RefillMsg] = signingRequest.DsEventArgs.Message;
                }
            }
            return signingResponse;
        }

        private void setSuccessMessage(bool v, string rxID)
        {
            throw new NotImplementedException();
        }

        public string GetDEANumberToBeUsed(IStateContainer state)
        {
            throw new NotImplementedException();
        }

        public RxDurResponseModel GetDurRxList()
        {
            throw new NotImplementedException();
        }

        public DURCheckResponse GetDurWarnings()
        {
            throw new NotImplementedException();
        }

        public int getHeaderReason(RadGrid currentGrid)
        {
            throw new NotImplementedException();
        }

        public List<IgnoreReasonsResponse> GetIgnoreReasonsByCategoryAndUserType(IgnoreCategory category, IImpactDurWraper durWrapper)
        {
            throw new NotImplementedException();
        }

        public IStateContainer GetPageState()
        {
            return base.PageState;
        }

        public DurRedirectModel GetWarningRedirectDetails()
        {
            throw new NotImplementedException();
        }

        public GoBackResponse GoBack(GoBackRequest goBackRequest)
        {
            throw new NotImplementedException();
        }

        public SubmitDurResponse SaveDurForm(SubmitDurRequest submitDurRequest, IImpactDurWraper impactDur)
        {
            throw new NotImplementedException();
        }

        public bool SaveDurWarnings(ISaveDurRequest saveDurRequst, IImpactDurWraper impactDur)
        {
            throw new NotImplementedException();
        }

        public void SetDurWarnings(DURCheckResponse dURCheckResponse)
        {
            throw new NotImplementedException();
        }

        private void SetPageStateVariables()
        {
            PageState[SessionVariables.ApproveWorkflowRefillTaskId] = null;
            PageState[SessionVariables.ApproveWorkflowRefillRxId] = null;
            PageState[SessionVariables.RefillMsg] = Constants.Messages.CsRxPrinted;
        }

        public void SetPageState(IStateContainer pageState)
        {
            base.PageState = pageState;
        }

        public RxDurResponseModel GetDurRxList(IeRxWebAppCodeWrapper wrapper)
        {
            throw new NotImplementedException();
        }

        public DURCheckResponse GetDurWarnings(IeRxWebAppCodeWrapper wrapper)
        {
            throw new NotImplementedException();
        }

       
    }
}