using eRxWeb.ePrescribeSvc;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;

namespace eRxWeb.AppCode.Interfaces
{
    public interface IRtpsDisposition
    {
        ePrescribeSvcResponse RtpsSendDisposition(
           string userId,
           string licenseId,
           string patientId,
           string transactionId,
           string relatesToTransactionId,
           Disposition disposition,
           ConnectionStringPointer dbid);
    }
}
