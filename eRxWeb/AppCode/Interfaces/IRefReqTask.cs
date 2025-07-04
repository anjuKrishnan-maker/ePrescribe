using Allscripts.Impact;
using Allscripts.Impact.Interfaces;

namespace eRxWeb.AppCode.Interfaces
{
    public interface IRefReqTask
    {
        void DenyMessageWithNoRxId(IScriptMessage iScriptMessage);
    }
}