using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eRxWeb.AppCode.Interfaces
{
    public interface IAppConfig
    {
        string GetAppSettings(string key);
    }
}
