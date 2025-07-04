using Allscripts.ePrescribe.Objects.PPTPlus;
using eRxWeb.ePrescribeSvc;
using ConnectionStringPointer = Allscripts.ePrescribe.DatabaseSelector.ConnectionStringPointer;

namespace eRxWeb.AppCode.Interfaces
{
    public interface IRtpsHelper
    {
        void CheckAndSendDisposition(
           string userId,
           string licenseId,
           string patientId,
           string relatesToTxId,
           Disposition disposition,
           IRtpsDisposition rtpsDisposition,
           ConnectionStringPointer dbid);
    }
}
