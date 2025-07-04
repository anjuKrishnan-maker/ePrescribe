using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.ePrescribeSvc;
using eRxWeb.State;

namespace eRxWeb.AppCode
{
    public class WeightUtil
    {
        public static void LogIfModified(string weight, string weightOnLoad, string patientId, string userHostAddress, IEPSBroker iEpsBroker, IStateContainer session)
        {
            if (!weight.Trim().Equals(weightOnLoad.Trim()))
            {

                iEpsBroker.AuditLogPatientInsert(AuditAction.PATIENT_WEIGHT_MODIFIED, session.GetStringOrEmpty("LICENSEID"), session.GetStringOrEmpty("USERID"), patientId, userHostAddress, session.Cast("DBID", Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer.ERXDB_DEFAULT));
            }
        }
    }
}