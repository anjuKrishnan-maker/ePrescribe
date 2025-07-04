using eRxWeb.ServerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode
{
    public class CommonUiSetup
    {
        internal static CommonUiIInitialPayload RetrieveUrls()
        {
            return new CommonUiIInitialPayload
            {
                PdmpUrl = Allscripts.Impact.ConfigKeys.PdmpCommonUiEndpoint,
                PptUrl = Allscripts.Impact.ConfigKeys.PptPlusCommonUIEndpoint
            };
        }
    }
}