using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.AppCode;
using eRxWeb.ServerModel;
using eRxWeb.ServerModel.Request;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eRxWeb.Controller
{
    public partial class GenericAlternativeApiController : ApiController
    {
        private static ILoggerEx logger = LoggerEx.GetLogger();
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        [HttpPost]
        public ApiResponse GetGenericAlternatives(MedicationSelectedRequest Med)
        {
            IStateContainer requestState = new StateContainer(HttpContext.Current.Session);
            if (!string.IsNullOrWhiteSpace(Med.taskScriptMessageId)) requestState[Constants.SessionVariables.TaskScriptMessageId] = Med.taskScriptMessageId;

            var response = new ApiResponse();
            try
            {
                List<GenericAlternative> _GenericAlternative = new List<GenericAlternative>();
                using (var timer = logger.StartTimer("GetGenericAlternatives"))
                {
                    _GenericAlternative = getGenericAlternatives(Med, requestState);
                    var GenericAlternativeData = string.Join(",", _GenericAlternative.Select(u => " DDI: " + Med.DDI + " FormularyStatus: " + Med.FormularyStatus + " MedName: " + Med.MedName + " taskScriptMessageId: " + Med.taskScriptMessageId + " DrugName: " + u.DrugName));
                    timer.Message = $"<GenericAlternativeData>{GenericAlternativeData}</GenericAlternativeData>";
                }
                response.Payload = _GenericAlternative;
            }
            catch (Exception ex)
            {
                var errorMessage = ApiHelper.AuditException(ex.ToString(), requestState);
                logger.Error("GetGenericAlternatives Exception: " + ex.ToString());

                response.ErrorContext = new ErrorContextModel()
                {
                    Error = ErrorTypeEnum.ServerError,
                    Message = errorMessage
                };
            }
            return response;
           
        }

        internal static List<GenericAlternative> getGenericAlternatives(MedicationSelectedRequest Med, IStateContainer requestState)
        {
            DataSet ds = null;
            ds = InitGenericAlternative(requestState, Med.DDI);

            DataTable prodTable = ds.Tables["Generics"];

            var generics = new List<GenericAlternative>();
            if (ds.Tables["Generics"].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables["Generics"].Rows)
                {
                    GenericAlternative row = new GenericAlternative();
                    row.DrugName = dr["Name"].ToString();
                    generics.Add(row);
                }
            }

            return generics;
        }

        private static DataSet InitGenericAlternative(IStateContainer pageState,string ddi)
        {
            return Allscripts.Impact.Medication.GetGenericAlternatives(ddi,pageState.Cast<ConnectionStringPointer>("DBID", ConnectionStringPointer.ERXDB_SERVER_1));
        }       
    }
}