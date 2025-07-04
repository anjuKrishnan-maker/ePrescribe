using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Web.Script.Services;
using Allscripts.Impact;
using Allscripts.ePrescribe.DatabaseSelector;
using System.Collections.Generic;
using System.Data;


namespace eRxWeb
{
    /// <summary>
    /// Summary description for erxnowmed
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.Web.Script.Services.ScriptService]
    public class erxnowmed : System.Web.Services.WebService
    {

        public erxnowmed()
        {

            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }

        [WebMethod(EnableSession = true)]
        public string[] queryMeds(string prefixText, int count)
        {
            ConnectionStringPointer dbID = ConnectionStringPointer.ERXDB_DEFAULT;

            if (Session["DBID"] != null)
            {
                dbID = (ConnectionStringPointer) Session["DBID"];
            }

            return Medication.QueryMeds(prefixText, count, "N", dbID);
        }

        [WebMethod(EnableSession = true)]
        public string[] queryMedsWithGenerics(string prefixText, int count)
        {
            ConnectionStringPointer dbID = ConnectionStringPointer.ERXDB_DEFAULT;

            if (Session["DBID"] != null)
            {
                dbID = (ConnectionStringPointer) Session["DBID"];
            }

            return Medication.QueryMeds(prefixText, count, "Y", dbID);
        }

        [WebMethod(EnableSession = true)]
        public string[] queryClassMeds(string prefixText, int count, string contextKey)
        {
            ConnectionStringPointer dbID = ConnectionStringPointer.ERXDB_DEFAULT;

            if (Session["DBID"] != null)
            {
                dbID = (ConnectionStringPointer) Session["DBID"];
            }

            return Medication.QueryClassMeds(prefixText, count, contextKey, dbID);
        }

    }

}