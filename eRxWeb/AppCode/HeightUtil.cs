using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.ePrescribeSvc;
using eRxWeb.State;

namespace eRxWeb.AppCode
    {
        public class HeightUtil
        {
            public static void LogIfModified(string height, string heightOnLoad, string patientId, string userHostAddress, IEPSBroker iEpsBroker, IStateContainer session)
            {
                if (!height.Trim().Equals(heightOnLoad.Trim()))
                {

                    iEpsBroker.AuditLogPatientInsert(AuditAction.PATIENT_HEIGHT_MODIFIED, session.GetStringOrEmpty("LICENSEID"), session.GetStringOrEmpty("USERID"), patientId, userHostAddress, session.Cast("DBID", Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer.ERXDB_DEFAULT));
                }
            }
        }
    }