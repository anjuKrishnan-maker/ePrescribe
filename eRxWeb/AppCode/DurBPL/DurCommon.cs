using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode.DurBPL
{
    public class DurCommon : DurBase, IDurCommon
    {
        public ePrescribeSvc.AuditLogPatientRxResponse AuditLogPatientRxInsert(ePrescribeSvc.AuditAction auditAction, string patientID, string rxID, string UserHostAddress, string createdUTC = null)
        {
            return EPSBroker.AuditLogPatientRxInsert(
                auditAction,
                SessionLicenseID,
                SessionUserID,
                patientID,
                UserHostAddress,
                rxID,
                DBID,
                createdUTC);
        }
    }
}