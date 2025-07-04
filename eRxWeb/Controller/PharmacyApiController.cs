using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.Impact.Utilities;
using Allscripts.ePrescribe.Shared.Logging;
using System.Web.Http;

namespace eRxWeb.Controllers
{
    public partial class PharmacyApiController : ApiController
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        const string MO_PHARMACY = "MOB_PHARMACY_ID";
        const string RETAIL_PHARMACY = "LASTPHARMACYID";

        [HttpPost]
        public ApiResponse GetPaharmacyByID(PharmacyRequestModel data)
        {
            IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
            var response = new ApiResponse();
            try
            {
                response.Payload = GetSelectedPharmacy(data.PharmacyID, pageState);
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                logger.Error("GetPaharmacyByID Exception: " + ex.ToString());

                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;
        }

        [HttpPost]
        public ApiResponse GetCurrentMOPharmacyDetails()
        {
            IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
            var response = new ApiResponse();
            try
            {
                response.Payload = GetSelectedPharmacy(pageState.GetStringOrEmpty(MO_PHARMACY), pageState);
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                logger.Error("GetCurrentMOPharmacyDetails Exception: " + ex.ToString());

                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;
        }


        [HttpPost]
        public ApiResponse GetCurrentRetailPharmacyDetails()
        {
            IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
            var response = new ApiResponse();
            try
            {
                response.Payload = GetSelectedPharmacy(pageState.GetStringOrEmpty(RETAIL_PHARMACY), pageState);
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                logger.Error("GetCurrentRetailPharmacyDetails Exception: " + ex.ToString());

                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;
        }


        [HttpPost]
        public ApiResponse NewPharmacyRequest(PharmacyRequestModel data)
        {
            using (var timer = logger.StartTimer("NewPharmacyRequest"))
            {
                IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
                var response = new ApiResponse();
                try
                {
                    response.Payload = SetPharmacyRequest(pageState, data.Name, data.Address1, data.Address2, data.City, data.State, data.ZipCode, data.Phone, data.Fax, data.Comment);
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                    logger.Error("NewPharmacyRequest Exception: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", $"<Address1>{data.Address1.ToLogString()}</Address1><Name>{data.Name.ToLogString()}</Name>,<Address2>{data.Address2.ToLogString()}</Address2>,<City>{data.City.ToLogString()}</City>,<State>{data.State.ToLogString()}</State>,<ZipCode>{data.ZipCode.ToLogString()}</ZipCode>,<Phone>{data.Phone.ToLogString()}</Phone>,<Fax>{data.Fax.ToLogString()}</Fax><Comment>{data.Comment.ToLogString()}</Comment>", response.ToLogString());
                return response;
            }
        }
        [HttpPost]
        public ApiResponse RemoveRetailPharmacyFromPatient()
        {
            IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
            var response = new ApiResponse();
            try
            {
                RemovePhermacyFromPatient(pageState, false);
                response.Payload = true;
                pageState.Remove(Constants.SessionVariables.PptPharmacyInfo);
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                logger.Error("RemoveRetailPharmacyFromPatient Exception: " + ex.ToString());

                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;
           
        }
        [HttpPost]
        public ApiResponse RemoveMOPharmacyFromPatient()
        {
            IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
            var response = new ApiResponse();
            try
            {
                RemovePhermacyFromPatient(pageState, true);
                response.Payload = true;
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                logger.Error("RemoveMOPharmacyFromPatient Exception: " + ex.ToString());

                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;
          
        }

        private static void RemovePhermacyFromPatient(IStateContainer pageState,bool isMailOrder)
        {

            Patient.UpdatePharmacyID(
                   pageState["PatientID"].ToString(),
                   System.Guid.Empty.ToString(),
                   isMailOrder,
                   AppCode.ApiHelper.GetDBID(pageState));

            EPSBroker.AuditLogPatientInsert(
             isMailOrder? ePrescribeSvc.AuditAction.PATIENT_MAIL_PHARMACY_DELETE:   ePrescribeSvc.AuditAction.PATIENT_RETAIL_PHARMACY_DELETE,
                AppCode.ApiHelper.GetSessionLicenseID(pageState),
                AppCode.ApiHelper.GetSessionUserID(pageState),
                ApiHelper.GetSessionPatientId(pageState),
                HttpContext.Current.Request.UserIpAddress(),
                AppCode.ApiHelper.GetDBID(pageState));
            if (!isMailOrder)
            {
                pageState["LASTPHARMACYNAME"] = null;
                pageState["PHARMACYID"] = null;
                pageState["LASTPHARMACYID"] = null;
            }
            else
            {
                pageState["MOB_NABP"] = null;
                pageState["PatientHasMOBCoverage"] = null;
            }
        }

        private static string SetPharmacyRequest(IStateContainer pageState, string pharmacyName, string Address, string Address2,
            string city, string state, string zip, string phone, string fax, string comment)
        {
            pageState.Remove("PracticeFavoritesExist");

            // use StringBuilder instead
            string msgbody = "";

            msgbody = pageState.GetStringOrEmpty("UserName");
            msgbody += " from " + pageState.GetStringOrEmpty("SiteName") + " (" + pageState.GetStringOrEmpty("AHSAccountID") + ") has made a request to add:\n\n";
            msgbody += "Pharmacy Name: " + pharmacyName + "\n";
            msgbody += "Address1: " + Address + "\n";

            if (Address2.Length > 0)
            {
                msgbody += "Address2: " + Address2 + "\n";
            }

            msgbody += "City: " + city + "\n";
            msgbody += "State: " + state + "\n";
            msgbody += "ZIP: " + zip + "\n";
            msgbody += "Phone: " + phone + "\n";

            if (fax.Length > 0)
            {
                msgbody += "Fax: " + fax + "\n";
            }

            if (comment.Length > 0)
            {
                msgbody += "Comment:" + comment + "\n";
            }

            SystemConfig.AddUserRequest(ApiHelper.GetSessionLicenseID(pageState), ApiHelper.GetSessionUserID(pageState), Constants.RequestType.NEW_PHARMACY, msgbody, ApiHelper.GetDBID(pageState));

            return "The following message has been sent:<br>\n" + msgbody.Replace("\n", "<BR>");

        }
        private static PharmacyModel GetSelectedPharmacy(string PharmacyID, IStateContainer pageState)
        {
            DataSet ds = LoadPharmacyData(PharmacyID, pageState);
            PharmacyModel data = new PharmacyModel();
            using (var timer = logger.StartTimer("GetSelectedPharmacy"))
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        string pharmacyID = dt.Rows[0].ItemArray[1].ToString();
                        dt.Columns.Add("EPCS", typeof(string));

                        if (dt.Rows[0]["EpcsEnabled"].ToString() == "1")
                        {
                            data.EPCS = "YES";
                        }
                        else
                        {
                            data.EPCS = "NO";
                        }

                        data.Name = Convert.ToString(dt.Rows[0]["Name"]);
                        data.Address1 = Convert.ToString(dt.Rows[0]["Address1"]);
                        data.State = Convert.ToString(dt.Rows[0]["State"]);
                        data.City = Convert.ToString(dt.Rows[0]["City"]);
                        data.ZipCode = Convert.ToString(dt.Rows[0]["Zip"]);
                        data.CrossStreet = Convert.ToString(dt.Rows[0]["CrossStreet"]);
                        data.Phone = StringHelper.FormatPhone(Convert.ToString(dt.Rows[0]["PhoneAreaCode"]), Convert.ToString(dt.Rows[0]["PhoneNumber"]));
                        data.Fax = StringHelper.FormatPhone(Convert.ToString(dt.Rows[0]["FaxAreaCode"]), Convert.ToString(dt.Rows[0]["FaxNumber"]));
                        data.NABP = Convert.ToString(dt.Rows[0]["NABP"]);



                        StringBuilder deliveryRate = new StringBuilder();
                        StringBuilder totalRx = new StringBuilder();
                        StringBuilder totalErrors = new StringBuilder();


                        try
                        {
                            if (dt.Rows[0]["SentScriptCnt"] != DBNull.Value && dt.Rows[0]["SentScriptCnt"].ToString().Trim() != "")
                            {
                                totalRx.Append(dt.Rows[0]["SentScriptCnt"].ToString());

                                if (totalRx.ToString() == "1")
                                {
                                    totalRx.Append(" total Rx");
                                }
                                else
                                {
                                    totalRx.Append(" total Rx's");
                                }

                                if (dt.Rows[0]["ErrorScriptCnt"] != DBNull.Value && dt.Rows[0]["ErrorScriptCnt"].ToString().Trim() != "")
                                {
                                    totalErrors.Append("(");
                                    totalErrors.Append(dt.Rows[0]["ErrorScriptCnt"].ToString());

                                    if (dt.Rows[0]["ErrorScriptCnt"].ToString() == "1")
                                    {
                                        totalErrors.Append(" error)");
                                    }
                                    else
                                    {
                                        totalErrors.Append(" errors)");
                                    }
                                }
                            }
                            else
                            {
                                totalRx.Append("N/A");
                            }

                            if (dt.Rows[0]["PercScriptSuccess"] != DBNull.Value && dt.Rows[0]["PercScriptSuccess"].ToString().Trim() != "")
                            {
                                deliveryRate.Append(decimal.Round(decimal.Parse(dt.Rows[0]["PercScriptSuccess"].ToString()), 0).ToString());
                                deliveryRate.Append("%");
                            }
                            else
                            {
                                deliveryRate.Append("N/A");
                            }
                        }
                        catch
                        {
                            totalRx.Remove(0, totalRx.Length);
                            totalErrors.Remove(0, totalRx.Length);
                            deliveryRate.Remove(0, totalRx.Length);

                            totalRx.Append("N/A"); ;
                            deliveryRate.Append("N/A");
                        }

                        data.TotalRx = totalRx.ToString();
                        data.TotalErrors = totalErrors.ToString();
                        data.DeliveryRate = deliveryRate.ToString();


                        if (dt.Rows[0]["TransMethod"] != DBNull.Value && dt.Rows[0]["TransMethod"].ToString().Trim() != "")
                        {
                            if (dt.Rows[0]["TransMethod"].ToString().ToUpper() == "FAX")
                            {
                                data.TransMethod = "Fax";
                            }
                            else
                            {
                                data.TransMethod = "Electronic";
                            }
                        }
                    }
                }
                timer.Message = string.Format("<Request>{0}</Request><Response>{1}</Response>", $"<Address1>{data.Address1.ToLogString()}</Address1><Name>{data.Name.ToLogString()}</Name>,<Address2>{data.Address2.ToLogString()}</Address2>,<City>{data.City.ToLogString()}</City>,<State>{data.State.ToLogString()}</State>,<ZipCode>{data.ZipCode.ToLogString()}</ZipCode>,<Phone>{data.Phone.ToLogString()}</Phone>,<Fax>{data.Fax.ToLogString()}</Fax><Comment>{data.Comment.ToLogString()}</Comment>", data.ToLogString());
            }
                
            return data;
        }

        private static DataSet LoadPharmacyData(string pharmacyId, IStateContainer pageState)
        {
            return Allscripts.Impact.Pharmacy.LoadPharmacy(pharmacyId, AppCode.ApiHelper.GetDBID(pageState));
        }
    }
    
}