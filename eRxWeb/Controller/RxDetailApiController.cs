using Allscripts.ePrescribe.Shared.Logging;
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
using System.Web.Http;

namespace eRxWeb.Controllers
{
    public partial class RxDetailApiController : ApiController
    {
        private static readonly ILoggerEx logger = LoggerEx.GetLogger();

        [HttpPost]
        public ApiResponse GetRxDetail([FromBody]string rxId)
        {
            using (var timer = logger.StartTimer("GetRxDetail"))
            {
                IStateContainer pageState = new StateContainer(HttpContext.Current.Session);
                var response = new ApiResponse();
                try
                {
                    response.Payload = GetRxDetail(rxId, pageState);
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), pageState);
                    logger.Error("GetRxDetail Exception: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format("<ErrorContext>{0}</ErrorContext>", response.ErrorContext.ToLogString());
                }
                timer.Message = string.Format("<Request><RxId>{0}</RxId></Request><Response>{1}</Response>", rxId, response.ToLogString());
                return response;
            }
        }
        private static RxDetailModel GetRxDetail(string rxID,IStateContainer pageState)
        {

            DataSet ds = Prescription.LoadExtraDetails(rxID, ApiHelper.GetDBID(pageState));
            DataTable dt = ds.Tables[0];
            DataRow dr = dt.Rows[0];
            RxDetailModel model = new RxDetailModel();
            bool pharmacyInfoAvailalbe = (dr["PharmacyInfoAvailable"].ToString() == "Y");
            bool additionalDetailsAvailable = (dr["ExtraDetailAvailable"].ToString() == "Y");

            if (pharmacyInfoAvailalbe)
            {

                model.Pharmacy.PharmacyNameDetail = dr["PharmacyName"].ToString();

                if (dr["PharmacyAddress1"] != DBNull.Value)
                {
                    model.Pharmacy.PharmacyAddressDetail = dr["PharmacyAddress1"].ToString();
                }

                if (dr["PharmacyAddress2"] != DBNull.Value && dr["PharmacyAddress2"].ToString().Trim() != "")
                {
                    model.Pharmacy.PharmacyAddressDetail = model.Pharmacy.PharmacyAddressDetail + ", " + dr["PharmacyAddress2"].ToString();
                }

                StringBuilder pharmDetail = new StringBuilder();

                if (dr["PharmacyCity"] != DBNull.Value)
                {
                    pharmDetail.Append(dr["PharmacyCity"].ToString());
                    pharmDetail.Append(", ");
                }

                if (dr["PharmacyState"] != DBNull.Value)
                {
                    pharmDetail.Append(dr["PharmacyState"].ToString());
                    pharmDetail.Append(" ");
                }

                if (dr["PharmacyZip"] != DBNull.Value)
                {
                    pharmDetail.Append(dr["PharmacyZip"].ToString());
                }

                model.Pharmacy.PharmacyCityStateZipDetail = pharmDetail.ToString();

                if (dr["PharmacyPhone"] != DBNull.Value && dr["PharmacyPhone"].ToString().Trim() != "")
                {
                    model.Pharmacy.PharmacyPhoneDetail = dr["PharmacyPhone"].ToString();
                }
                else
                {
                    model.Pharmacy.PharmacyPhoneDetail = "N/A";
                }

                if (dr["PharmacyFax"] != DBNull.Value && dr["PharmacyFax"].ToString().Trim() != "")
                {
                    model.Pharmacy.PharmacyFaxDetail = dr["PharmacyFax"].ToString();
                }
                else
                {
                    model.Pharmacy.PharmacyFaxDetail = "N/A";
                }
            }
            else
            {
                model.Pharmacy = null;
            }
            

            if (additionalDetailsAvailable)
            {

                model.History.ProviderName = getText(dr.ItemArray[1].ToString() + " " + dr.ItemArray[0].ToString());
                model.History.PracticeName = getText(dr.ItemArray[2].ToString());
                model.History.ProviderAddress = formatAddress(dr.ItemArray[3].ToString(), dr.ItemArray[4].ToString());
                model.History.ProviderCityStateZip = formatCityStateZip(
                    dr.ItemArray[5].ToString(), dr.ItemArray[6].ToString(), dr.ItemArray[7].ToString());
                model.History.lProviderPhone = getText(dr.ItemArray[9].ToString());
                model.History.ProviderFax = getText(dr.ItemArray[10].ToString());
                model.History.ProviderEmail = getText(dr.ItemArray[11].ToString());
                model.History.PharmacistName = getText(dr.ItemArray[13].ToString() + " " + dr.ItemArray[12].ToString());
                model.History.PharmacyName = getText(dr.ItemArray[14].ToString());
                model.History.PharmacyAddress = formatAddress(dr.ItemArray[15].ToString(), dr.ItemArray[16].ToString());
                model.History.PharmacyCityStateZip = formatCityStateZip(
                    dr.ItemArray[17].ToString(), dr.ItemArray[18].ToString(), dr.ItemArray[19].ToString());
                model.History.PharmacyPhone = getText(dr.ItemArray[21].ToString());
                model.History.PharmacyFax = getText(dr.ItemArray[22].ToString());
                model.History.PharmacyEmail = getText(dr.ItemArray[23].ToString());
            }
            else
            {
                model.History = null;
            }
            return model;
        }
        private static string getText(string text)
        {
            if (text.Trim() != "")
                return text;
            else
                return "N/A";
        }
        private static string formatAddress(string address1, string address2)
        {
            if (address1.Trim() == "" && address2.Trim() == "")
                return "";
            else
                return address1.Trim() + " " + address2.Trim();
        }

        private static string formatCityStateZip(string city, string state, string zip)
        {
            if (city.Trim() == "" && state.Trim() == "" && zip.Trim() == "")
                return "";
            else if (city.Trim() == "")
                return state.Trim() + " " + zip.Trim();
            else
                return city.Trim() + ", " + state.Trim() + " " + zip.Trim();
        }

    }

}