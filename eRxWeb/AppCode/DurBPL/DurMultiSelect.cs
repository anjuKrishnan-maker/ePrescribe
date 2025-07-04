using System;
using System.Collections.Generic;
using Telerik.Web.UI;
using Constants = Allscripts.ePrescribe.Common.Constants;
using Allscripts.ePrescribe.DatabaseSelector;
using eRxWeb.State;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Response;
using System.Collections;
using System.Web;
using ScriptMessage = Allscripts.Impact.ScriptMsg.ScriptMessage;
using System.Text;
using System.Data;
using Prescription = Allscripts.Impact.Prescription;
using Allscripts.Impact;
using Allscripts.ePrescribe.ExtensionMethods;
using eRxWeb.AppCode.DurBPL.ResponseModel;
using eRxWeb.ServerModel;
using eRxWeb.AppCode.DurBPL;
using eRxWeb.AppCode.DurBPL.RequestModel;
using System.Linq;
using static Allscripts.Impact.IgnoreReason;
using Allscripts.Impact.Interfaces;
using static Allscripts.ePrescribe.Common.Constants;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Settings;

namespace eRxWeb.AppCode.DurBPL
{
    public class DurMultiSelect : DurBase, IDur
    {
        #region
        const string EsignCompleteRefillMessage = "ChangeRx for {0} approved for {1} .";
        const string Pharm = "PHARM";
        const string EpcsEnabled = "EpcsEnabled";
        const string Message = "Contact provider by alternate methods regarding controlled medications";
        const string State = "State";
        const string epcsDigitalSigningeMessage = "Controlled substance meds being sent electronically could not be digitally signed.Please try again or print.";
        const string RefillMessage = "Controlled substance refill denied.";
        const string ReasonID = "ReasonID";
        const string ReasonDescription = "ReasonDescription";
        const string Category = "Category";
        const string Active = "Active";
        const string Zero = "0";
        const string InActive = "Y";
        const string Created = "Created";
        const string Modified = "Modified";
        #endregion
        RxDurResponseModel rxDurResponseModel = new RxDurResponseModel();
        public DURCheckResponse DURWarnings
        {
            set { PageState[Constants.SessionVariables.DURCheckResponse] = value; }
        }

        public string GetDEANumberToBeUsed(IStateContainer state)
        {
            return StateUtils.UserInfo.GetDEANumberToBeUsed(state);
        }

        public IStateContainer GetPageState()
        {
            return PageState;
        }

        public void SetPageState(IStateContainer state)
        {
            PageState = state;
        }

        public int getHeaderReason(RadGrid currentGrid)
        {
            throw new NotImplementedException();
        }

        public CSMedRefillNotAllowedPrintRefillResponse CSMedRefillRequestNotAllowedOnPrintRefillRequest(ICSMedRefillNotAllowedPrintRefillRequest requestOnPrintRefillRequest, IImpactDurWraper impactDur)
        {
            CSMedRefillNotAllowedPrintRefillResponse responseOnPrintRefillRequest = new CSMedRefillNotAllowedPrintRefillResponse();
            string taskScriptMessageId = this.PageState[SessionVariables.TaskScriptMessageId].ToString();
            string sessionLicenseID = this.SessionLicenseID;
            string sessionUserID = this.SessionUserID;
            ConnectionStringPointer dbId = this.DBID;
            string rxID = Guid.NewGuid().ToString();
            DataSet pharmDS = new DataSet();
            ScriptMessage sm = null;
            StringBuilder pharmComments = new StringBuilder();
            Prescription rx = new Prescription();
            string deaNumber = GetDEANumberToBeUsed(this.PageState);
            PageState[SessionVariables.IsCSRefillNotAllowed] = true;
            string statePharmacy = impactDur.LoadPharamcyDetail(taskScriptMessageId, sessionLicenseID, sessionUserID, ref pharmDS, ref sm, State, dbId);
            pharmComments.Append(Convert.ToString(this.CurrentRx.Notes));
            string stateCSCodeForPharmacy = impactDur.GetStateControlledSubstanceCode(this.CurrentRx.DDI, statePharmacy, this.DBID);
            string providerID = GetProviderInfo(sessionUserID);
            impactDur.SetHeaderInfo(sessionUserID, rxID, sm, rx, providerID, Convert.ToInt32(this.PageState[SessionVariables.SiteId].ToString()), dbId);
            impactDur.AddMedication(sessionUserID, this.CurrentRx, this.PageState[SessionVariables.PerformFormulary].ToString(), this.DURSettings, pharmComments, deaNumber, sm, rx, stateCSCodeForPharmacy, providerID,
                            this.PageState[SessionVariables.RxWorkFlow] != null ? ((PrescriptionWorkFlow)Convert.ToInt32(this.PageState[SessionVariables.RxWorkFlow])) : PrescriptionWorkFlow.STANDARD,
                            this.PageState[SessionVariables.ExternalFacilityCd] != null ? Convert.ToString(this.PageState[SessionVariables.ExternalFacilityCd].ToString()) : null,
                            this.PageState[SessionVariables.ExternalGroupID] != null ? Convert.ToString(this.PageState[SessionVariables.ExternalGroupID]) : null
                                                        );
            impactDur.SaveRxInfo(Convert.ToInt32(this.PageState[SessionVariables.SiteId].ToString()), sessionLicenseID, sessionUserID, dbId, rxID, rx, deaNumber, GetIsCsRegistryChecked(), this.CurrentRx);
            SetTaskid(rxID);
            responseOnPrintRefillRequest.RxId = rxID;
            responseOnPrintRefillRequest.Url = GetReturnUrl();
            return responseOnPrintRefillRequest;
        }

        private ScriptMessage GetScriptMessage(IImpactDurWraper impactDur)
        {
            if (string.IsNullOrWhiteSpace(PageState.GetStringOrEmpty(SessionVariables.TaskScriptMessageId)))
            {
                return base.RxTask.ScriptMessage as ScriptMessage;
            }
            else
            {
                return impactDur.CreateNewScriptMessage(PageState.GetStringOrEmpty(SessionVariables.TaskScriptMessageId), SessionLicenseID, SessionUserID, DBID);
            }
        }

        public SubmitDurResponse SaveDurForm(SubmitDurRequest submitDurRequest, IImpactDurWraper impactDur)
        {
            SubmitDurResponse durResponse = new SubmitDurResponse();
            if (submitDurRequest.IsCapturedReasons)
            {
                durResponse.DurGoNext = this.PageState.GetString(SessionVariables.DUR_GO_NEXT, null);
                if (PageState[SessionVariables.TaskScriptMessageId] != null || this.RxTask != null)
                {
                    PageState.Remove(SessionVariables.CameFrom);
                    string reconciledControlledSubstanceCode = string.Empty;
                    string stateCSCodeForPractice = string.Empty;
                    string stateCSCodeForPharmacy = string.Empty;
                    ScriptMessage sm = GetScriptMessage(impactDur);
                    DataSet dsPharmacy = new DataSet();
                    string fedCSCode = this.CurrentRx.ControlledSubstanceCode;
                    string statePharmacy = impactDur.LoadPharamcyDetail(this.DBID, ref dsPharmacy, ref sm, State);
                    if (string.IsNullOrWhiteSpace(this.CurrentRx.DDI))
                    {
                        reconciledControlledSubstanceCode = this.CurrentRx.ScheduleUsed.ToString();
                    }
                    else
                    {
                        stateCSCodeForPharmacy = impactDur.GetStateControlledSubstanceCode(this.CurrentRx.DDI, null, statePharmacy, this.DBID);
                        reconciledControlledSubstanceCode = impactDur.ReconcileControlledSubstanceCodes(fedCSCode, stateCSCodeForPharmacy);
                    }

                    // Check if med is a CS med
                    if (
                            (!string.IsNullOrWhiteSpace(reconciledControlledSubstanceCode) &&
                            reconciledControlledSubstanceCode.ToUpper() != "U" &&
                            reconciledControlledSubstanceCode != "0")
                            ||
                            (this.CurrentRx.IsFreeFormMedControlSubstance)
                        )
                    {
                        // check if stars align for EPCS
                        //
                        // "CanTryEPCS" is true so that means the physician is EPCS authorized and the 
                        // Enterprise Client associated with this license is EPCS enabled
                        //
                        if (this.CanTryEPCS)
                        {
                            //
                            // check if Med is Federal controlled substance (schedule 2,3,4,5) OR
                            // Med is a state controlled substance in the state the provider's practice is in AND 
                            // Med is a state controlled substance in the state of the pharmacy where the script is being sent
                            //
                            if (!this.CurrentRx.IsFreeFormMedControlSubstance)
                            {
                                stateCSCodeForPractice = impactDur.GetStateControlledSubstanceCode(this.CurrentRx.DDI, Convert.ToString(this.PageState[SessionVariables.PracticeState]), this.DBID);
                                reconciledControlledSubstanceCode = impactDur.ReconcileControlledSubstanceCodes(fedCSCode, stateCSCodeForPharmacy, stateCSCodeForPractice);
                            }

                            if (impactDur.IsCSMedEPCSEligible(fedCSCode, stateCSCodeForPharmacy, stateCSCodeForPractice) || this.CurrentRx.IsFreeFormMedControlSubstance)
                            {
                                //
                                // check if the state in which the site's practice is in, is EPCS authorized for the 
                                // CS schedule of the selected med
                                //
                                if (this.SiteEPCSAuthorizedSchedules.Contains(reconciledControlledSubstanceCode))
                                {
                                    //
                                    // check if pharmacy is EPCS enabled
                                    //
                                    impactDur.GetEPCSAuthorizedSchedulesForPharmacy(reconciledControlledSubstanceCode, dsPharmacy, EpcsEnabled, submitDurRequest, this.DBID);
                                }
                            }
                        }

                        if (submitDurRequest.starsAlign)
                        {
                            durResponse.IsstarsAlign = true;
                            // show EPCS Signing popup
                            this.CurrentRx.EffectiveDate = DateTime.Now.Date;
                            this.CurrentRx.Destination = Pharm;

                            if (reconciledControlledSubstanceCode == ReconciledControlledSubstanceCode.Two)
                            {
                                this.CurrentRx.CanEditEffectiveDate = true;
                            }

                            this.CurrentRx.ScheduleUsed = Convert.ToInt32(reconciledControlledSubstanceCode);

                            List<Rx> epcsMedList = new List<Rx>();
                            //Explicitly set the ScriptMessageID when ChangeRequest REFREQ or ReconcilePatient REFREQ case
                            if (this.PageState.ContainsKey(SessionVariables.TaskScriptMessageId))
                            {
                                this.CurrentRx.ScriptMessageID = this.PageState.GetStringOrEmpty(SessionVariables.TaskScriptMessageId);
                            }
                            epcsMedList.Add(this.CurrentRx);

                            //If CS refreq or rxchg, move on to script pad page
                            if (this.PageState[SessionVariables.IsCsRefReqWorkflow] != null || this.ChangeRxRequestedMedCs != null)
                            {
                                durResponse.IsCsMedWorkFlow = true;
                                durResponse.Url = PageNames.SCRIPT_PAD;
                                durResponse.IsGatherWarningsAndInfoToWriteToDB = true;
                                durResponse.RxID = null;
                                PageState[SessionVariables.CheckDur] = null;
                                PageState[SessionVariables.DUR_GO_NEXT] = null;
                                DURWarnings = null;
                                durResponse.Url = PageNames.SCRIPT_PAD;
                            }
                            durResponse.EpcsMedList = epcsMedList;
                            durResponse.RefreshUcEPCSDigitalSigning = true;
                        }
                        else
                        {
                            durResponse.IsstarsAlign = false;
                            durResponse.IsCSMedRefillRequestNotAllowed = true;
                        }
                    }
                    else
                    {
                        impactDur.GetRefillRequest(submitDurRequest, sm);
                        // refill - not CS...
                        SaveRefillRequest(submitDurRequest, durResponse, impactDur);
                        //todo : get values to dur response
                    }
                }
                else if (durResponse.DurGoNext != null)
                {
                    durResponse.IsGatherWarningsAndInfoToWriteToDB = true;
                    durResponse.RxID = null;
                    PageState[SessionVariables.CheckDur] = null;
                    PageState[SessionVariables.DUR_GO_NEXT] = null;
                    DURWarnings = null;
                    durResponse.Url = durResponse.DurGoNext;
                }
                else
                {
                    durResponse.IsGatherWarningsAndInfoToWriteToDB = true;
                    durResponse.RxID = null;
                    SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
                    {
                        PatientId = PageState.GetStringOrEmpty(SessionVariables.PatientId)
                    };
                    durResponse.selectPatientComponentParameters = selectPatientComponentParameters;
                    durResponse.Url = AngularStringUtil.CreateInitComponentUrl(PageNames.SELECT_PATIENT);
                }
            }
            else
            {
                if (SessionUserType == UserCategory.POB_LIMITED)
                {
                    if (PageState[SessionVariables.DUR_GO_NEXT] != null)
                    {
                        durResponse.IsGatherWarningsAndInfoToWriteToDB = true;
                        durResponse.RxID = null;
                        durResponse.Url = Convert.ToString(PageState[SessionVariables.DUR_GO_NEXT]);
                        PageState[SessionVariables.DUR_GO_NEXT] = null;
                        PageState[SessionVariables.CheckDur] = null;
                        DURWarnings = null;
                    }
                    else
                    {
                        durResponse.IsGatherWarningsAndInfoToWriteToDB = true;
                        durResponse.RxID = null;
                        durResponse.Url = PageNames.SELECT_PATIENT;
                    }
                }
                else
                {
                    durResponse.IsIgnoreReasonsNotSelected = true;
                }
            }
            return durResponse;
        }




        public void SaveRefillRequest(SubmitDurRequest durRequest, SubmitDurResponse durResponse, IImpactDurWraper impactDur)
        {
            decimal metricQuantity = this.CurrentRx.Quantity;
            string delegateProviderID = string.Empty;
            string rxID = string.Empty;
            if ((this.CurrentRx.PackageSize * this.CurrentRx.PackageQuantity) > 0)
            {
                metricQuantity = this.CurrentRx.Quantity * this.CurrentRx.PackageSize * this.CurrentRx.PackageQuantity;
            }
            if (this.IsPOBUser || this.SessionUserType == UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED)
            {
                ProcessForPobOrPhysicianReq(durRequest, metricQuantity, out delegateProviderID, out rxID, impactDur);
            }
            else
            {
                // workflow for Provider and PA no supervision users
                rxID = ProcessForNoSupervisionUsers(durRequest, metricQuantity, delegateProviderID, impactDur);
            }
            impactDur.AuditLogPatientRxInsert(ePrescribeSvc.AuditAction.PATIENT_TASK_PROCESS, SessionLicenseID, SessionUserID, SessionPatientID, rxID, durRequest.UserHostAddress, base.DBID);
            SetRefillMsg();
            impactDur.SendOutboundInfoScriptMessage(rxID, 1, SessionLicenseID, SessionUserID, string.Empty, Convert.ToString(PageState[SessionVariables.STANDING]), this.DBID);
            SetResponseObj(durResponse, rxID);
            //make sure we close out the refreq
            PageState.Remove(SessionVariables.TaskScriptMessageId);
            ClearMedicationInfo(false);
        }

        public void SaveRefillRequest(SubmitDurRequest durRequest, IImpactDurWraper impactDur)
        {
            SaveRefillRequest(durRequest, new SubmitDurResponse(), impactDur);
        }

        public void ProcessForPobOrPhysicianReq(SubmitDurRequest durRequest, decimal metricQuantity, out string delegateProviderID, out string rxID, IImpactDurWraper impactDur)
        {
            if (this.PageState[SessionVariables.SupervisingProviderId] != null)
            {
                // get the correct delegate provider for refill request for PA with supervision when refill request is being processed by a POB user
                delegateProviderID = this.PageState.GetStringOrEmpty(SessionVariables.SupervisingProviderId);
            }
            else
            {
                // get the correct delegate provider for refill request for PA with supervision when refill request is being processed by a POB user
                delegateProviderID = this.PageState[SessionVariables.DelegateProviderId] != null ? this.PageState.GetStringOrEmpty(SessionVariables.DelegateProviderId) : this.SessionUserID;
            }

            if ((this.PageState[SessionVariables.SupervisingProviderId] != null) && (!string.IsNullOrEmpty(Convert.ToString(this.PageState[SessionVariables.TaskType])) && (Constants.PrescriptionTaskType)this.PageState[SessionVariables.TaskType] == Constants.PrescriptionTaskType.RXCHG))
            {
                //
                // pass in the correct provider for POB user when processing refill request for PA with supervision (and selected supervising provider)
                //
                rxID = impactDur.ApproveRXCHGMessage(this.RxTask, durRequest.ProviderId, Constants.PrescriptionTransmissionMethod.SENT);
            }
            else if ((this.PageState[SessionVariables.SupervisingProviderId] == null) && (this.RxTask != null && this.RxTask.TaskType == Constants.PrescriptionTaskType.RXCHG))
            {
                rxID = impactDur.ApproveRXCHGMessage(this.RxTask, delegateProviderID, Constants.PrescriptionTransmissionMethod.SENT);
            }
            else
            {
                rxID = impactDur.ApproveMessage(this.PageState.GetStringOrEmpty(SessionVariables.TaskScriptMessageId), this.SessionLicenseID, this.SessionUserID, this.CurrentRx, metricQuantity, durRequest.ProviderId, Constants.PrescriptionTransmissionMethod.SENT,
                            delegateProviderID, Convert.ToInt32(this.PageState.GetStringOrEmpty(SessionVariables.SiteId)), this.ShieldSecurityToken, this.DBID);
            }
        }

        private void SetResponseObj(SubmitDurResponse durResponse, string rxID)
        {
            durResponse.RxID = rxID;
            durResponse.IsGatherWarningsAndInfoToWriteToDB = true;
            durResponse.IsRefreshActiveMeds = true;
            durResponse.Url = Convert.ToString(this.PageState[SessionVariables.CameFrom]);
            this.PageState.Remove(SessionVariables.CameFrom);
            if (string.IsNullOrEmpty(durResponse.Url))
            {
                if (this.IsUserAPrescribingUserWithCredentials)
                {
                    durResponse.IsUserAPrescribingUserWithCredentials = true;
                    durResponse.Url = PageNames.APPROVE_REFILL_TASK;
                }
                else
                {
                    durResponse.Url = PageNames.PHARMACY_REFILL_SUMMARY;
                }
            }
        }

        public void SetRefillMsg()
        {
            if (PageState[SessionVariables.ChangeRxRequestedMedCs] == null)
            {
                if (RxTask != null && RxTask.TaskType == PrescriptionTaskType.RXCHG)
                {
                    // ChangeRx Request workflow...
                    PageState[SessionVariables.RefillMsg] = string.Format("RxChange for {0} approved for {1}.", CurrentRx.MedicationName, Convert.ToString(PageState["PATIENTNAME"]).Trim());
                }
                else
                {
                    PageState[SessionVariables.RefillMsg] = string.Format("Refill for {0} approved for {1}.", CurrentRx.MedicationName, Convert.ToString(PageState["PATIENTNAME"]).Trim());
                }
            }
        }

        private string ProcessForNoSupervisionUsers(SubmitDurRequest saveDurRequest, decimal metricQuantity, string delegateProviderID, IImpactDurWraper impactDur)
        {
            string rxID;
            if (RxTask != null && RxTask.TaskType == PrescriptionTaskType.RXCHG)
            {
                rxID = impactDur.ApproveRXCHGMessage(RxTask, saveDurRequest.ProviderId, PrescriptionTransmissionMethod.SENT);
            }
            else if (ChangeRxRequestedMedCs != null)
            {
                var sm = impactDur.CreateNewScriptMessage(PageState.GetStringOrEmpty(SessionVariables.TaskScriptMessageId), SessionLicenseID, SessionUserID, DBID);
                Guid delegateProviderId = DelegateProvider != null
                    ? DelegateProvider.UserID.ToGuidOr0x0()
                    : Guid.Empty;
                var rxTask = new RxTaskModel
                {
                    ScriptMessage = sm,
                    ScriptMessageGUID = PageState.GetStringOrEmpty(SessionVariables.TaskScriptMessageId),
                    Rx = CurrentRx,
                    UserId = SessionUserID.ToGuid(),
                    DbId = DBID,
                    LicenseId = SessionLicenseID.ToGuid(),
                    SiteId = SessionSiteID,
                    ShieldSecurityToken = ShieldSecurityToken,
                    ExternalFacilityCd = PageState.GetStringOrEmpty(SessionVariables.ExternalFacilityCd),
                    ExternalGroupID = PageState.GetStringOrEmpty(SessionVariables.ExternalGroupID),
                    UserType = SessionUserType,
                    DelegateProviderId = delegateProviderId
                };
                rxTask.RxRequestType = Allscripts.Impact.RequestType.APPROVE_WITH_CHANGE;
                rxID = impactDur.ApproveRXCHGMessage(rxTask, SessionUserID, PrescriptionTransmissionMethod.SENT);
                PageState[SessionVariables.RefillMsg] = string.Format(EsignCompleteRefillMessage, CurrentRx.MedicationName, Convert.ToString(PageState[SessionVariables.PatientName]).Trim());
            }
            else
            {
                rxID = impactDur.ApproveMessage(Convert.ToString(PageState[SessionVariables.TaskScriptMessageId]), SessionLicenseID, SessionUserID, CurrentRx, metricQuantity, saveDurRequest.ProviderId, PrescriptionTransmissionMethod.SENT, delegateProviderID, Convert.ToInt32(PageState[SessionVariables.SiteId]), impactDur.GetShieldSecurityToken(PageState), DBID);
            }
            return rxID;
        }

        public string GetReturnUrl()
        {
            string returnUrl;
            if (IsUserAPrescribingUserWithCredentials)
            {
                returnUrl = string.Format("{0}?To={1}", PageNames.CSS_DETECT, HttpUtility.HtmlEncode(PageNames.APPROVE_REFILL_TASK));
            }
            else
            {
                returnUrl = string.Format("{0}?To={1}", PageNames.CSS_DETECT, HttpUtility.HtmlEncode(PageNames.PHARMACY_REFILL_SUMMARY));
            }
            return returnUrl;
        }

        public void SetTaskid(string rxID)
        {
            Hashtable htTaskRxID = new Hashtable();
            htTaskRxID.Add(Convert.ToInt64(PageState[SessionVariables.TaskId]), rxID);
            PageState[SessionVariables.HTTaskRxID] = htTaskRxID;
        }

        private bool? GetIsCsRegistryChecked()
        {
            bool? isCSRegistryChecked = null;
            if (PageState[SessionVariables.isCSRegistryChecked] != null)
                isCSRegistryChecked = bool.Parse(PageState[SessionVariables.isCSRegistryChecked].ToString());
            PageState.Remove(SessionVariables.isCSRegistryChecked);
            return isCSRegistryChecked;
        }

        private string GetProviderInfo(string sessionUserID)
        {
            string providerID = sessionUserID;
            if (IsPOBUser && PageState[SessionVariables.DelegateProviderId] != null)
            {
                providerID = Convert.ToString(PageState[SessionVariables.DelegateProviderId]);
            }
            return providerID;
        }

        public DURCheckResponse GetDurWarnings(IeRxWebAppCodeWrapper eRWwrapper)
        {
            var durWarnings = PageState[SessionVariables.DURCheckResponse];
            if (durWarnings == null)
            {
                DURResponse durResponse = eRWwrapper.GetDurWarnings(PageState.GetStringOrEmpty(SessionVariables.PatientDOB),
                                                                            PageState.GetStringOrEmpty(SessionVariables.Gender),
                                                                            ScriptPadMeds,
                                                                            PageState.Cast(SessionVariables.ACTIVEMEDDDILIST, new List<string>()),
                                                                            DurPatientAllergies,
                                                                            DURSettings);
                durWarnings = durResponse.DurCheckResponse;
                if (durResponse.MedispanCopyright != null)
                    PageState[SessionVariables.MedispanCopyright] = durResponse.MedispanCopyright;
                PageState[SessionVariables.DURCheckResponse] = durResponse.DurCheckResponse;
            }
            return (DURCheckResponse)durWarnings;
        }
        public void SetDurWarnings(DURCheckResponse dURCheckResponse)
        {
            DURWarnings = dURCheckResponse;
        }

        public RxDurResponseModel GetDurRxList(IeRxWebAppCodeWrapper eRxWrapper)
        {
            DURCheckResponse durwarning = GetDurWarnings(eRxWrapper);
            var taskType = eRxWrapper.RetrieveTaskType(RxTask, PageState.Cast(SessionVariables.TaskType, PrescriptionTaskType.DEFAULT));
            var rxList = eRxWrapper.RetrieveDrugsListBasedOnWorkflowType(
                PageState.Cast(SessionVariables.RxList, new ArrayList()).ToList<Rx>(),
                ScriptPadMeds,
                taskType);

            rxDurResponseModel.RxList = rxList;
            rxDurResponseModel.IsAnyDurSettingOn = eRxWrapper.IsAnyDurSettingOn(DURSettings);
            rxDurResponseModel.FreeFormDrugsHasItems = eRxWrapper.RetrieveFreeFormDrugsHasItems(rxList);
            if (rxDurResponseModel.IsAnyDurSettingOn && rxDurResponseModel.FreeFormDrugsHasItems || durwarning != null)
                rxDurResponseModel.SetAndShowDurWarnings = true;
            if (eRxWrapper.DurWarningsHasItems(durwarning) && rxDurResponseModel.IsAnyDurSettingOn)
                rxDurResponseModel.WarningsListHasItems = true;
            if (rxDurResponseModel.FreeFormDrugsHasItems && rxDurResponseModel.IsAnyDurSettingOn)
                rxDurResponseModel.CustListFreeFormDrugs = true;
            if (durwarning.FoodInteractions != null && eRxWrapper.FoodInteractionsHasItems(durwarning) && DURSettings.CheckFoodInteraction == YesNoSetting.Yes)
                rxDurResponseModel.FoodListInteractions = true;
            if (durwarning.AlcoholInteractions != null && eRxWrapper.AlcoholInteractionsHasItems(durwarning) && DURSettings.CheckAlcoholInteraction == YesNoSetting.Yes)
                rxDurResponseModel.AlcoholListInteractions = true;
            if (durwarning.PriorAdverseReactions != null && eRxWrapper.PriorAdverseReactionsHasItems(durwarning) && DURSettings.CheckPar == YesNoSetting.Yes)
                rxDurResponseModel.HasPriorAdverseReactions = true;
            if (durwarning.DuplicateTherapy != null && eRxWrapper.DuplicateTherapyHasItems(durwarning) && DURSettings.CheckDuplicateTherapy == YesNoSetting.Yes)
                rxDurResponseModel.HasDuplicateTherapyItems = true;
            if (durwarning.DrugInteractions != null && eRxWrapper.DrugInteractionsHasItems(durwarning) && DURSettings.CheckDrugToDrugInteraction == YesNoSetting.Yes)
                rxDurResponseModel.HasDrugInteractionsItems = true;
            if (durwarning.Dosage != null && eRxWrapper.DosageCheckMedicationsHasItems(durwarning) && DURSettings.CheckPerformDose == YesNoSetting.Yes)
                rxDurResponseModel.HasDosageCheckMedicationsItems = true;
            return rxDurResponseModel;
        }
        public DurRedirectModel GetWarningRedirectDetails()
        {
            DurRedirectModel durRedirectModel = new DurRedirectModel();
            string go_next = string.Empty;
            var durGoNext = this.PageState.GetStringOrEmpty(SessionVariables.DUR_GO_NEXT);
            var cameFrom = this.PageState.GetStringOrEmpty(SessionVariables.CameFrom);
            this.PageState[SessionVariables.CheckDur] = null;
            if (!string.IsNullOrWhiteSpace(durGoNext))
            {
                go_next = durGoNext;
                this.PageState[SessionVariables.DUR_GO_NEXT] = null;
            }
            else if (!string.IsNullOrWhiteSpace(cameFrom))
            {
                go_next = cameFrom;
            }

            DURWarnings = null;
            if (!string.IsNullOrWhiteSpace(go_next))
            {
                durRedirectModel.RedirectUrl = go_next;
            }
            else
            {
                SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
                {
                    PatientId = PageState.GetStringOrEmpty(SessionVariables.PatientId)
                };
                durRedirectModel.RedirectUrl = RedirectToSelectPatient(null, selectPatientComponentParameters);
            }
            return durRedirectModel;

        }
        public bool SaveDurWarnings(ISaveDurRequest durRequest, IImpactDurWraper impactDur)
        {
            if (durRequest.Warnings.Any())
            {
                impactDur.SaveDurWarnings(durRequest.Warnings.ToDataTable(), this.DBID);
            }
            MedsWithDURs = durRequest.DurList != null ? durRequest.DurList.Select(x => x.ToString()).ToList() : new List<string>();
            PageState[SessionVariables.SuperPobToProvider] = durRequest.RxToProviderList;
            return true;
        }
        public CSMedRefillNotAllowedContactResponse CSMedRefillRequestNotAllowedOnContactProvider(ICSMedRefillNotAllowedContactRequest contactRequest, IImpactDurWraper impactDur)
        {
            CSMedRefillNotAllowedContactResponse response = new CSMedRefillNotAllowedContactResponse();
            impactDur.RejectScriptMessage(
                PageState[SessionVariables.TaskScriptMessageId].ToString(),
                string.Empty,
                Message,
                SessionUserID,
                SessionLicenseID,
                Guid.Empty.ToString(),
                contactRequest.ShieldSecurityToken,
                SessionSiteID,
                DBID);
            PageState[SessionVariables.RefillMsg] = RefillMessage;
            if (IsUserAPrescribingUserWithCredentials)
            {
                response.RedirectUrl = PageNames.APPROVE_REFILL_TASK;
            }
            else
            {
                response.RedirectUrl = PageNames.PHARMACY_REFILL_SUMMARY;
            }
            return response;
        }
        public List<IgnoreReasonsResponse> GetIgnoreReasonsByCategoryAndUserType(IgnoreCategory category, IImpactDurWraper durWrapper)
        {
            UserCategory usertype = (UserCategory)PageState[Constants.SessionVariables.UserType];
            DataTable dtIgnoreReasons = durWrapper.GetIgnoreReasons(category, usertype, DBID);
            List<IgnoreReasonsResponse> lstIgnoreReasonsResponse = new List<IgnoreReasonsResponse>();
            foreach (DataRow dr in dtIgnoreReasons.Rows)
            {
                IgnoreReasonsResponse ignoreReasonsResponse = new IgnoreReasonsResponse();
                ignoreReasonsResponse.ReasonID = Convert.ToInt32(dr[ReasonID].ToString());
                ignoreReasonsResponse.ReasonDescription = dr[ReasonDescription].ToString();
                ignoreReasonsResponse.Category = Convert.ToInt32(Convert.ToString(dr[Category]) == string.Empty ? Zero : Convert.ToString(dr[Category]));
                ignoreReasonsResponse.Active = Convert.ToChar(Convert.ToString(dr[Active]) == string.Empty ? InActive : Convert.ToString(dr[Active]));
                ignoreReasonsResponse.Created = Convert.ToString(dr[Created]);
                ignoreReasonsResponse.Modified = Convert.ToString(dr[Modified]);
                lstIgnoreReasonsResponse.Add(ignoreReasonsResponse);
            }
            return lstIgnoreReasonsResponse;
        }
        public GoBackResponse GoBack(GoBackRequest goBackRequest)
        {
            GoBackResponse goBackResponse = new GoBackResponse();
            if (PageState[SessionVariables.IsCsRefReqWorkflow] != null || goBackRequest.ChangeRxRequestedMedCs != null)
            {
                if (goBackRequest.ChangeRxRequestedMedCs != null)
                {
                    ScriptPadMeds.Clear();
                }
                goBackResponse.Url = Constants.PageNames.SIG;
            }
            if (PageState[SessionVariables.DUR_GO_PREVIOUS] != null)
            {
                PageState.Remove(SessionVariables.RefillMsg); // Remove the refill message from session, if the user wishes to cancel/ hits Back button.
                goBackResponse.Url = Convert.ToString(PageState[SessionVariables.DUR_GO_PREVIOUS]);
                PageState[SessionVariables.DUR_GO_PREVIOUS] = null;
            }
            else
            {
                SelectPatientComponentParameters selectPatientComponentParameters = new SelectPatientComponentParameters
                {
                    PatientId = PageState.GetStringOrEmpty(SessionVariables.PatientId)
                };
                goBackResponse.Url = RedirectToSelectPatient(null, selectPatientComponentParameters);
            }
            return goBackResponse;
        }
        public void saveRxChangeRequestForEPCS(string scriptMessageID, string rxID, EPCSDigitalSigningResponse signingResponse, IImpactDurWraper impactDur)
        {
            PerformChangeRx(rxID, signingResponse, impactDur);
            impactDur.SendCHGRESScriptMessageForEPCS(scriptMessageID, rxID, SessionLicenseID, SessionUserID, RxTask.ScriptMessageGUID, CurrentRx.Notes, DBID);
            PageState.Remove(SessionVariables.Notes);
        }
        private void CompleteChangeRxCsMedWorkflow(string scriptMessageID, string rxID, EPCSDigitalSigningResponse signingResponse, IImpactDurWraper impactDur)
        {
            PerformChangeRx(rxID, signingResponse, impactDur);
            PageState.Remove(SessionVariables.TaskScriptMessageId); //make sure we close out the pharm task
            PageState.Remove(SessionVariables.Notes);
        }

        public void saveRefillRequestForEPCS(string scriptMessageID, string rxID, EPCSDigitalSigningResponse signingResponse, IImpactDurWraper impactDur)
        {
            PerformChangeRx(rxID, signingResponse, impactDur);
            impactDur.SendRenewalScriptMessageForEPCS(scriptMessageID, rxID, base.SessionLicenseID, base.SessionUserID, Convert.ToString(PageState[SessionVariables.TaskScriptMessageId]), base.CurrentRx.Notes, base.DBID);
            PageState.Remove(SessionVariables.TaskScriptMessageId); //make sure we close out the pharm task
            PageState.Remove(SessionVariables.Notes);
        }

        private void PerformChangeRx(string rxID, EPCSDigitalSigningResponse signingResponse, IImpactDurWraper impactDur)
        {
            signingResponse.IsGatherWarningsAndInfoToWriteToDB = true;
            signingResponse.RxId = rxID;
            string smid = impactDur.SendOutboundInfoScriptMessage(rxID, 1, base.SessionLicenseID, base.SessionUserID, string.Empty, Convert.ToString(PageState[SessionVariables.STANDING]), base.DBID);
            PageState[SessionVariables.RefillMsg] = string.Format(EsignCompleteRefillMessage, base.CurrentRx.MedicationName, Convert.ToString(PageState[SessionVariables.PatientName]).Trim());
            signingResponse.UpdatePatientActiveMeds = true;
            if (!string.IsNullOrEmpty(Convert.ToString(PageState[SessionVariables.CameFrom])))
            {
                signingResponse.Url = Convert.ToString(PageState[SessionVariables.CameFrom]);
            }
            else
            {
                signingResponse.Url = PageNames.APPROVE_REFILL_TASK;
            }
        }

        public EPCSDigitalSigningResponse EPCSDigitalSigningOnDigitalSigningComplete(EPCSDigitalSigningRequest signingRequest, IImpactDurWraper impactDur)
        {
            EPCSDigitalSigningResponse signingResponse = new EPCSDigitalSigningResponse();
            if (signingRequest.DsEventArgs.Success)
            {
                signingResponse.IsEventSuccess = true;
                foreach (KeyValuePair<string, string> signedMed in signingRequest.DsEventArgs.SignedMeds)
                {
                    if (PageState[SessionVariables.ChangeRxRequestedMedCs] == null)
                    {
                        if (base.RxTask != null && base.RxTask.TaskType == PrescriptionTaskType.RXCHG)
                        {
                            saveRxChangeRequestForEPCS(signedMed.Value, signedMed.Key, signingResponse, impactDur);
                        }
                        else
                        {
                            saveRefillRequestForEPCS(signedMed.Value, signedMed.Key, signingResponse, impactDur);
                        }
                    }
                    else
                    {
                        CompleteChangeRxCsMedWorkflow(signedMed.Value, signedMed.Key, signingResponse, impactDur);
                    }
                }
            }
            else
            {
                if (signingRequest.DsEventArgs.ForceLogout)
                {
                    //force the user to log out if they've entered invalid credentials 3 times in a row
                    signingResponse.Url = Constants.PageNames.LOGOUT;
                }
                else if (string.IsNullOrEmpty(signingRequest.DsEventArgs.Message))
                {
                    signingResponse.MessageText = epcsDigitalSigningeMessage;
                }
                else
                {
                    signingResponse.MessageText = signingRequest.DsEventArgs.Message;
                }
                signingResponse.MessageIconType = Controls_Message.MessageType.ERROR;
                signingResponse.DisplayMainMessage = true;
            }
            return signingResponse;
        }

        public DURCheckResponse GetDurWarnings(IeRxWebAppCodeWrapper codeWarper, IImpactDurWraper impactDur)
        {
            throw new NotImplementedException();
        }


    }
}