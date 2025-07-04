using Allscripts.ePrescribe.DatabaseSelector;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Common;
using System.Collections;
using Allscripts.Impact;
using eRxWeb.ServerModel;
using eRxWeb.AppCode;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.ServerModel.Request;
using Allscripts.Impact.Interfaces;

namespace eRxWeb.Controller
{
    public partial class FormularyAlternativesApiController : ApiController
    {
        private IStateContainer session;
        private IMedication medication;
        private static ILoggerEx logger = LoggerEx.GetLogger();
        public  FormularyAlternativesApiController()
        {
            this.session = new StateContainer(HttpContext.Current.Session);
            medication = new Medication();
        }

        public FormularyAlternativesApiController(IStateContainer session, IMedication medication)
        {
            this.session = session;
            this.medication = medication;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [HttpPost]
        public ApiResponse GetFormularyAlternatives(MedicationSelectedRequest Med)
        {
            if(!string.IsNullOrWhiteSpace(Med.taskScriptMessageId)) session[Constants.SessionVariables.TaskScriptMessageId] = Med.taskScriptMessageId;
            List<FormularyAlternative> data = new List<FormularyAlternative>();
            var response = new ApiResponse();
            try
            {
                using (var timer = logger.StartTimer("GetFormularyAlternatives"))
                {

                    data = getFormularyAlternatives(Med, session, medication);
                    response.Payload = data;
                    var FormularyAlternativesData = string.Join(",", data.Select(u => " DDI: " + Med.DDI + " FormularyStatus: " + Med.FormularyStatus + " MedName: " + Med.MedName + " taskScriptMessageId: " + Med.taskScriptMessageId + " Copay: " + u.Copay + " DrugName: " + u.DrugName + " ImageUrl: " + u.ImageUrl + " IsGeneric: " + u.IsGeneric + " LevelOfPreferedness: " + u.LevelOfPreferedness + " SearchDrugName: " + u.DrugName + " Status: " + u.Status + " ToolTip: " + u.ToolTip));
                    timer.Message = $"<FormularyAlternativesData>{FormularyAlternativesData}</FormularyAlternativesData>";
                }    
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                logger.Error("GetFormularyAlternatives Exception: " + ex.ToString());

                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;
        }

        internal static List<FormularyAlternative> GetFormularyAlternatives(MedicationSelectedRequest reqMed, IStateContainer requestState)
        {
            return getFormularyAlternatives(reqMed, requestState, new Medication());
        }
        internal static List<FormularyAlternative> getFormularyAlternatives(MedicationSelectedRequest reqMed, IStateContainer requestState, IMedication med)
        {
          
            List<FormularyAlternative> _FormularyAlternative = new List<FormularyAlternative>();
            DataSet ds = null;
            if (SatisfyVisibliltyCondition(requestState))
            {
                ds = InitFormularyAlternative(requestState,med, reqMed);
                requestState["FormularyAltData"] = ds;

                if (ds != null)
                {


                    DataTable prodTable = ds.Tables[3];

                    if (ds.Tables[3].Rows.Count > 0)
                    {

                        // var medPage = GetMedPageRedirect();

                        foreach (DataRow dr in ds.Tables[3].Rows)
                        {
                            FormularyAlternative row = new FormularyAlternative();

                            //let's add the formulary alternatives here
                            row.DrugName = dr["Name"].ToString().Trim();
                            row.Copay = dr["Copay"].ToString();
                            bool isOTC = false;
                            if (dr["IsOTC"] != Convert.DBNull)
                                isOTC = dr["IsOTC"].ToString() == "Y";
                            if (dr["IsGeneric"] != Convert.DBNull)
                                row.IsGeneric = dr["IsGeneric"].ToString() == "Y";

                            string imagePath = string.Empty;
                            string toolTip = string.Empty;

                            MedicationSearchDisplay.GetFormularyImagePathWithTooltip(Convert.ToInt32(dr["FormularyStatus"]), isOTC, out imagePath, out toolTip);
                            
                            row.ImageUrl = imagePath;
                            row.ToolTip = toolTip;

                            string levelOfPreferedness = string.Empty;
                            if (dr["LevelOfPreferedness"] != Convert.DBNull &&
                                dr["LevelOfPreferedness"].ToString() != string.Empty)
                            {
                                levelOfPreferedness = dr["LevelOfPreferedness"].ToString();
                            }

                            row.LevelOfPreferedness = levelOfPreferedness;
                            _FormularyAlternative.Add(row);
                        }
                    }

                }
            }
            return _FormularyAlternative;
        }

        [HttpPost]
        public ApiResponse GetFormularyAlternativesFromSession()
        {
            FormularyAlternative data = new FormularyAlternative();
            var response = new ApiResponse();
            try
            {
                using (var timer = logger.StartTimer("GetFormularyAlternativesFromSession"))
                {
                    data = getFormularyAlternativesFromSession(session);
                    response.Payload = data;
                    timer.Message = $"<FormularyAlternativeData>{data.ToLogString()}</FormularyAlternativeData>";
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                logger.Error("GetFormularyAlternativesFromSession Exception: " + ex.ToString());

                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;
        }
        internal static FormularyAlternative getFormularyAlternativesFromSession(IStateContainer requestState)
        {
            FormularyAlternative row = new FormularyAlternative();
            var rx = CurrentRx(requestState);
            if (rx != null)
            {
                bool isOTC = false;
                if (!string.IsNullOrEmpty(rx.IsOTC))
                {
                    isOTC = rx.IsOTC == "Y";
                }

                string imgPath = string.Empty;
                string toolTip = string.Empty;

                MedicationSearchDisplay.GetFormularyImagePathWithTooltip(Convert.ToInt32(rx.FormularyStatus), isOTC, out imgPath, out toolTip);

                row.ImageUrl = imgPath;
                row.ToolTip = toolTip;
                
                row.DrugName = getMedName(requestState);
                row.LevelOfPreferedness = rx.LevelOfPreferedness != null ? rx.LevelOfPreferedness : string.Empty;
            }
            return row;
        }
        private static string getMedName(IStateContainer pageState)
        {
            string med = string.Empty;

            if (pageState.Cast<ArrayList>("RxList",null) != null)
            {
                //there should only be 1 item in the list here
                Rx rx = CurrentRx(pageState);

                if (!string.IsNullOrEmpty(rx.MedicationName))
                {
                    med = rx.MedicationName;
                }
                if (!string.IsNullOrEmpty(rx.Strength))
                {
                    med = med + ' ' + rx.Strength;
                }
                if (!string.IsNullOrEmpty(rx.StrengthUOM))
                {
                    med = med + ' ' + rx.StrengthUOM;
                }

                if (!string.IsNullOrEmpty(rx.DosageFormDescription))
                {
                    med = med + ' ' + rx.DosageFormDescription;
                }
                if (!string.IsNullOrEmpty(rx.RouteOfAdminDescription))
                {
                    med = med + ' ' + rx.RouteOfAdminDescription;
                }
            }
            else
            {
                med = pageState.GetStringOrEmpty("MEDICATIONNAME");

                if (pageState["MEDICATIONNAME"] != null)
                {
                    med = pageState.GetStringOrEmpty("MEDICATIONNAME");
                }
                if (pageState["STRENGTH"] != null)
                {
                    med = med + ' ' + pageState.GetStringOrEmpty("STRENGTH");
                }
                if (pageState["STRENGTHUOM"] != null)
                {
                    med = med + ' ' + pageState.GetStringOrEmpty("STRENGTHUOM");
                }

                if (pageState["DosageForm"] != null)
                {
                    med = med + ' ' + pageState.GetStringOrEmpty("DosageForm");
                }
                if (pageState["RouteofAdmin"] != null)
                {
                    med = med + ' ' + pageState.GetStringOrEmpty("RouteofAdmin");
                }
            }

            return med;
        }

        private static DataSet InitFormularyAlternative(IStateContainer pageState,IMedication med, MedicationSelectedRequest reqMed)
        {
            return med.GetFormularyAlternatives(reqMed.DDI, pageState.GetStringOrEmpty("FORMULARYID"),
                 pageState.GetStringOrEmpty("PLANID"),
                 pageState.GetStringOrEmpty("ALTPLANID"),
                 reqMed.FormularyStatus,
                 pageState.GetStringOrEmpty("COPAYID"),
                 pageState.GetStringOrEmpty("LicenseID"),
                 pageState.Cast<Int32>("SiteID", 0),
                 pageState.GetStringOrEmpty("UserID"),
                 pageState.Cast<ConnectionStringPointer>("DBID", ConnectionStringPointer.ERXDB_SERVER_1)
                 );
        }
        private static bool SatisfyVisibliltyCondition(IStateContainer pageState)
        {
            var visible = true;
            if (string.IsNullOrWhiteSpace(pageState.GetStringOrEmpty("FORMULARYID")))
                visible = false;
            return visible;
        }
        private static bool ShowModelPopup { get; set; }

        private static Rx CurrentRx(IStateContainer pageState)
        {
            Rx rx = null;
            ArrayList rxList = pageState.Cast<ArrayList>("RxList", null);
            if (rxList != null && rxList.Count > 0)
                rx = (Rx)rxList[0];
            return rx;
        }
    }


}