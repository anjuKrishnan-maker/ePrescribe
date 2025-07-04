using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Allscripts.Impact.PreBuildPrescription;
using System.Data;
using Allscripts.ePrescribe.DatabaseSelector;

namespace eRxWeb
{
    /// <summary>
    /// Summary description for eRxNowPreBuiltPrescriptionGroup
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class eRxNowPreBuiltPrescriptionGroup : System.Web.Services.WebService
    {


        public eRxNowPreBuiltPrescriptionGroup()
        {

            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }

        [WebMethod(EnableSession = true)]
        public List<string> queryPrebuiltPrescriptionGroup(string prefixText, int count)
        {
            List<string> listTerm = new List<string>();
            Allscripts.Impact.PreBuildPrescription.PreBuildPrescription preBuilteRx = new Allscripts.Impact.PreBuildPrescription.PreBuildPrescription();
            if (Session["LICENSEID"] != null && Session["LICENSEID"].ToString() != string.Empty)
            {
                ConnectionStringPointer dbID = ConnectionStringPointer.ERXDB_DEFAULT;

                if (Session["DBID"] != null)
                {
                    dbID = (ConnectionStringPointer)Session["DBID"];
                }
                var groupsDataTable = preBuilteRx.GetPreBuiltPrescriptionGroup(Session["LICENSEID"].ToString(), true, null, dbID);
                
                var query = from p in groupsDataTable
                            where p.Name.StartsWith((prefixText.Length == 0) ? p.Name : prefixText,StringComparison.OrdinalIgnoreCase)
                            select new
                            {
                                Name = p.Name
                            };
                foreach (var grp in query)
                {
                    listTerm.Add(grp.Name);
                }

            }

            return listTerm;

        }
    }
}
