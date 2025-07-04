using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Data.Registration;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.ePrescribe.Objects;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace eRxWeb.Controller
{
    public class DeluxeFeatureSelectionApiController : ApiController
    {
        public DeluxeFeatureSelectionApiController()
        {

        }
        private static ILoggerEx logger = LoggerEx.GetLogger();
        IStateContainer pageState;
        IStateContainer session = new StateContainer(HttpContext.Current.Session);

        public IStateContainer PageState
        {
            get
            {
                if (pageState == null)
                    pageState = new StateContainer(HttpContext.Current.Session);
                return pageState;
            }
            set
            {
                pageState = value;
            }
        }

        private ApplicationLicense SessionLicense
        {
            get
            {
                if (PageState["SessionLicense"] == null)
                {
                    if (PageState["DBID"] == null)
                    {
                        PageState["SessionLicense"] = new ApplicationLicense(string.Empty, 0, string.Empty, ConnectionStringPointer.ERXDB_DEFAULT);
                    }
                    else
                    {
                        PageState["SessionLicense"] = new ApplicationLicense(string.Empty, 0, string.Empty, (ConnectionStringPointer)PageState["DBID"]);
                    }
                }

                return (ApplicationLicense)PageState["SessionLicense"];
            }
        }

        [HttpPost]
        public ApiResponse GetDeluxeFeatureSelectionInitialData()
        {
            using (var timer = logger.StartTimer("DeluxeFeatureSelectionApi - GetDeluxeFeatureSelectionInitialData"))
            {
                IStateContainer session = new StateContainer(HttpContext.Current.Session);
                var response = new ApiResponse();

                try
                {
                    DeluxeFeature deluxeFeature = new DeluxeFeature();
                    deluxeFeature.deluxeFeatureSelection = GetDeluxeModulesWithPrice();
                    deluxeFeature.subscribedModule = GetModuleSubscribed();
                    deluxeFeature.forceCompulsoryRestrictions = GetForceCompulsoryRestrictions();
                    deluxeFeature.product = GetProductsDetails();
                    response.Payload = deluxeFeature;
                }
                catch (Exception ex)
                {
                    var errorMessage = ApiHelper.AuditException(ex.ToString(), session);
                    logger.Error("DeluxeFeatureSelectionApi - GetDeluxeFeatureSelectionInitialData: Exception while fetching data: " + ex.ToString());

                    response.ErrorContext = new ErrorContextModel()
                    {
                        Error = ErrorTypeEnum.ServerError,
                        Message = errorMessage
                    };
                    timer.Message = string.Format($"<ErrorContext>{response.ErrorContext.ToLogString()}</ErrorContext>");
                }
                return response;
            }
        }

        private DeluxeFeatureSelection GetDeluxeModulesWithPrice()
        {
            session["SelectedFeature"] = null;
            session["AdsFreeChecked"] = null;           
            var dbId = ApiHelper.GetDBID(PageState);
            var sessionLicense = ApiHelper.SessionLicense(session);
            DeluxeFeatureSelection deluxeFeatureSelection = new DeluxeFeatureSelection();
            List<DeluxeFeaturePricing> deluxeFeaturePricing = new List<DeluxeFeaturePricing>();
            DataSet dataSetFetureSelection = Allscripts.ePrescribe.Data.DeluxePurchase.GetDeluxePricingStructures(dbId, sessionLicense.EnterpriseClient.ID);
            bool EnterprisePricing = dataSetFetureSelection.Tables[0].Rows[0]["EnterprisePricing"].ToString() == "1" ? true : false;
            deluxeFeaturePricing = (from DataRow dataRow in dataSetFetureSelection.Tables[1].Rows
                                    select new DeluxeFeaturePricing()
                                    {
                                        PricingStructureDesc = Convert.ToString(dataRow["PricingStructureDesc"]),
                                        Price = Convert.ToInt32(dataRow["Price"]),
                                        Id = Convert.ToInt32(dataRow["Id"]),
                                    }).ToList();
            if (!SessionLicense.IsEnterpriseCompulsoryBasicOrForceCompulsoryBasic())
            {
                //Only compulsory basic enterprises should have basic purchase option
                deluxeFeaturePricing.Remove(deluxeFeaturePricing.Find(p=> p.Id == 12));
            }
            deluxeFeatureSelection.deluxeFeaturePricing = deluxeFeaturePricing;
            deluxeFeatureSelection.EnterprisePricing = EnterprisePricing;            
            return deluxeFeatureSelection;
        }
        
        private string GetModuleSubscribed()
        {           
            var licenseID = ApiHelper.GetSessionLicenseID(PageState);
            var dbId = ApiHelper.GetDBID(PageState);
            string ModuleSubscribed = string.Empty;
            DataTable dtDeluxeAccountInfo = DeluxePurchaseManager.GetDeluxeAccountInfo(licenseID, dbId).Tables[0];
            if (dtDeluxeAccountInfo.Rows.Count > 0 && dtDeluxeAccountInfo.Rows[0]["ModuleSubscribed"] != DBNull.Value)
            {
                ModuleSubscribed = Convert.ToString(dtDeluxeAccountInfo.Rows[0]["ModuleSubscribed"]);
            }
            else
            {
                ModuleSubscribed = new RegistrationData().GetRegistrantUserPricingStructure(ApiHelper.GetSessionUserID(PageState));
                ModuleSubscribed = string.IsNullOrWhiteSpace(ModuleSubscribed) ? "CompulsoryBasic" : ModuleSubscribed;
            }

            session["CurrentModule"] = ModuleSubscribed;            
            return ModuleSubscribed;
        }
       
        private ForceCompulsoryRestrictions GetForceCompulsoryRestrictions()
        {           
            ForceCompulsoryRestrictions forceCompulsoryRestriction = new ForceCompulsoryRestrictions();
            forceCompulsoryRestriction.IsForcePaymentState = BillingUtil.ShouldUserFinishAPayment(session);
            forceCompulsoryRestriction.IsEnterpriseCompulsoryBasic = SessionLicense.IsEnterpriseCompulsoryBasicOrForceCompulsoryBasic();
            
            return forceCompulsoryRestriction;
        }
       
        private List<Products> GetProductsDetails()
        {           
            List<Products> products = new List<Products>();
            DataTable dataTableProducts = DeluxePurchaseManager.GetProductDetails();
            products = (from DataRow dataRow in dataTableProducts.Rows
                        select new Products()
                        {
                            ProductName = Convert.ToString(dataRow["ProductName"]),
                            ProductPrice = Convert.ToInt32(dataRow["ProductPrice"])
                        }).ToList();           
            return products;
        }

        [HttpPost]
        public ApiResponse SetSelectedDeluxeFeature([FromBody]string SelectedDeluxeFeature)
        {
            var response = new ApiResponse();
            IStateContainer session = new StateContainer(HttpContext.Current.Session);
            session["SelectedFeature"] = SelectedDeluxeFeature;
            response.Payload = true;
            return response;
        }

    }
}
