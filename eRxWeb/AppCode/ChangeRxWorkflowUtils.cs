using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eRxWeb.AppCode.Interfaces;
using System.Data;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Common;
using Rx = Allscripts.Impact.Rx;
using ScriptMessage = Allscripts.Impact.ScriptMessage;
using RequestedRx = Allscripts.ePrescribe.Objects.RequestedRx;
using ChangeRxWorkflow = Allscripts.ePrescribe.Objects.ChangeRxWorkflow;
using Prescription = Allscripts.Impact.Prescription;
using eRxWeb.State;

namespace eRxWeb.AppCode
{
    public class RxChangeBaseParameters
    {
        #region privateMembers
        private Rx m_currentRx = null;
        private ConnectionStringPointer m_dbID;
        private string m_sSessionLicenseID = string.Empty;
        private string m_sSessionUserID = string.Empty;
        private string m_sShieldSecurityToken = string.Empty;
        private int m_nSessionSiteID = 0;
        private string m_sSessionPatientID = string.Empty;
        private string m_sExtFacilityCd = string.Empty;
        private string m_sExtGroupID = string.Empty;
        #endregion

        #region properties
        public Rx CurrentRx
        {
            get
            {
                return m_currentRx;
            }
            set
            {
                m_currentRx = value;
            }
        }
        public ConnectionStringPointer DBID
        {
            get
            {
                return m_dbID;
            }
            set
            {
                m_dbID = value;
            }
        }
        public string SessionLicenseID
        {
            get
            {
                return m_sSessionLicenseID;
            }
            set
            {
                m_sSessionLicenseID = value;
            }
        }
        public string SessionUserID
        {
            get
            {
                return m_sSessionUserID;
            }
            set
            {
                m_sSessionUserID = value;
            }
        }
        public string ShieldSecurityToken
        {
            get
            {
                return m_sShieldSecurityToken;
            }
            set
            {
                m_sShieldSecurityToken = value;
            }
        }

        public int SessionSiteID
        {
            get
            {
                return m_nSessionSiteID;
            }
            set
            {
                m_nSessionSiteID = value;
            }
        }

        public string SessionPatientID
        {
            get
            {
                return m_sSessionPatientID;
            }
            set
            {
                m_sSessionPatientID = value;
            }

        }
        public string ExtFacilityCd
        {
            get
            {
                return m_sExtFacilityCd;
            }
            set
            {
                m_sExtFacilityCd = value;
            }

        }
        public string ExtGroupID
        {
            get
            {
                return m_sExtGroupID;
            }
            set
            {
                m_sExtGroupID = value;
            }

        }
        #endregion


    }
    public class RxChangeUIParameters
    {
        #region privateMembers
        private string txtQuantityText = string.Empty;
        private string txtRefillText = string.Empty;
        private string txtDaysSupplyText = string.Empty;
        private string txtFreeTextSigText = string.Empty;
        private bool chkRegistryCheckedVisible = false;
        private bool chkRegistryCheckedChecked = false;
        private string strReqRxQueryString = string.Empty;
        private string ipAddress = string.Empty;
        #endregion

        #region properties
        public string TxtQuantity
        {
            get
            {
                return txtQuantityText;
            }
            set
            {
                txtQuantityText = value;
            }
        }

        public string TxtRefill
        {
            get
            {
                return txtRefillText;
            }
            set
            {
                txtRefillText = value;
            }
        }

        public string TxtDaysSupply
        {
            get
            {
                return txtDaysSupplyText;
            }
            set
            {
                txtDaysSupplyText = value;
            }
        }

        public string TxtFreeTextSig
        {
            get
            {
                return txtFreeTextSigText;
            }
            set
            {
                txtFreeTextSigText = value;
            }
        }

        public bool IschkRegistryCheckedVisible
        {
            get
            {
                return chkRegistryCheckedVisible;
            }
            set
            {
                chkRegistryCheckedVisible = value;
            }
        }

        public bool IschkRegistryCheckedChecked
        {
            get
            {
                return chkRegistryCheckedChecked;
            }
            set
            {
                chkRegistryCheckedChecked = value;
            }
        }

        public string ReqRxQueryString
        {
            get
            {
                return strReqRxQueryString;
            }
            set
            {
                strReqRxQueryString = value;
            }
        }

        
        #endregion

    }

   
    public class ChangeRxWorkflowUtils: IChangeRxWorkflow
    {
        private static IStateContainer _pageState;
        public static IStateContainer PageState
        {
            get { return _pageState ?? (_pageState = new StateContainer(HttpContext.Current.Session)); }
        }

        /// <summary>
        /// Create the appropriate ApproveRXCHG AMEMOM message that is forwarded to hub and logged internally
        /// </summary>
        /// <param name="rxChgIn"></param>
        /// <param name="rxChgUiParams"></param>
        /// <param name="rxChgBaseParams"></param>
        public void ApproveAndLogRxChangeWorkflowMessage(ChangeRxWorkflow rxChgIn, RxChangeUIParameters rxChgUiParams, RxChangeBaseParameters rxChgBaseParams)
        {
            var rx = rxChgIn.Rx as Rx;
            var sm = rxChgIn.ScriptMessage as ScriptMessage;
            ScriptMessage.ApproveRXCHGMessage(rxChgIn, rxChgBaseParams.SessionUserID, Constants.PrescriptionTransmissionMethod.SENT,
                rxChgBaseParams.SessionLicenseID, rxChgBaseParams.SessionUserID, rxChgBaseParams.SessionSiteID, rxChgBaseParams.ShieldSecurityToken,
                rxChgBaseParams.SessionUserID, rxChgBaseParams.DBID);          
        }

        /// <summary>
        /// ChangeRx requires original med to be discontinued
        /// </summary>
        /// <param name="rxChgIn"></param>
        /// <param name="rxChgBaseParams"></param>
        public void DiscontinuePriorMedBeforeCHGRX(ChangeRxWorkflow rxChgIn, RxChangeBaseParameters rxChgBaseParams)
        {
            var sm = rxChgIn.ScriptMessage as ScriptMessage;
            string extFacilityCode = rxChgBaseParams.ExtFacilityCd != null ? rxChgBaseParams.ExtFacilityCd : null;
            string extGroupID = rxChgBaseParams.ExtGroupID != null ? rxChgBaseParams.ExtGroupID : null;
            Prescription.Discontinue(sm.DBRxID, "1", DateTime.Today.ToShortDateString(), string.Empty, string.Empty,
            rxChgBaseParams.SessionUserID, rxChgBaseParams.SessionLicenseID, extFacilityCode, extGroupID, rxChgBaseParams.DBID);
        }

        /// <summary>
        /// User input like DaysSupply, Quantity, Refills, comments and SigText need to be captured
        /// </summary>
        /// <param name="changeRxWorkflow"></param>
        /// <param name="rxReq"></param>
        /// <param name="rxChgUiParams"></param>
        /// <param name="rxChgBaseParams"></param>
        /// <returns></returns>
        public ChangeRxWorkflow UserInputUpdatedRxChangeWorkflow(ChangeRxWorkflow changeRxWorkflow, RequestedRx rxReq, RxChangeUIParameters rxChgUiParams, RxChangeBaseParameters rxChgBaseParams)
        {
            ChangeRxWorkflow retChangeRxWorkflow = changeRxWorkflow;
            Rx rx = retChangeRxWorkflow.Rx as Rx;
            rx.Quantity = System.Convert.ToDecimal(rxChgUiParams.TxtQuantity);
            rx.Refills = System.Convert.ToInt32(rxChgUiParams.TxtRefill);
            rx.DaysSupply = System.Convert.ToInt32(rxChgUiParams.TxtDaysSupply);
            rx.SigText = rxChgUiParams.TxtFreeTextSig;
            //Coming from approve task screen wanting to change a ChangeRxMed
            if (string.IsNullOrEmpty(rx.SigText))
            {
                rx.SigText = SigAspx.GetEnglishDescriptionFromSigID(rx.SigID, rxChgBaseParams.DBID);

            }
            retChangeRxWorkflow.RequestedRx = rxReq;
            retChangeRxWorkflow.Rx = rx;
            return retChangeRxWorkflow;
        }

    }
}