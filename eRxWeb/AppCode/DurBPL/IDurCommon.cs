using eRxWeb.ePrescribeSvc;

namespace eRxWeb.AppCode.DurBPL
{
    public interface IDurCommon
    {
        AuditLogPatientRxResponse AuditLogPatientRxInsert(AuditAction auditAction, string patientID, string rxID, string UserHostAddress, string createdUTC = null);
    }
}