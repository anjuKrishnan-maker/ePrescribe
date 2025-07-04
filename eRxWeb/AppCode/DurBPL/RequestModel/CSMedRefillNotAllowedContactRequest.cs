
namespace eRxWeb.AppCode.DurBPL.RequestModel
{
    public class CSMedRefillNotAllowedContactRequest : ICSMedRefillNotAllowedContactRequest
    {
        public CSMedRefillNotAllowedContactRequest(string shieldSecurityToken)
        {
            ShieldSecurityToken = shieldSecurityToken;
        }
        public string ShieldSecurityToken { get; set; }
    }
}