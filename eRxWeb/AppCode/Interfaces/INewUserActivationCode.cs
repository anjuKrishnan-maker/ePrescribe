using eRxWeb.ServerModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eRxWeb.AppCode.Interfaces
{
    public interface INewUserActivationCode
    {        
        bool EmailActivationCode(IEmail sendEmail, string userName, UserActivationInfoModel userActivationInfo);
        
    }
}
