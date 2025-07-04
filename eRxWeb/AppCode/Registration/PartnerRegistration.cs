using Allscripts.ePrescribe.Data.Registration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode.Registration
{
    public class PartnerRegistration
    {
        public string StylePath { get; set; }
        public string ProductName { get; set; }
        public PartnerRegistration GetPartnerEnterpriseInfo(string clientID, IRegistrationData regData)
        {
            var enterpriseInfo = regData.GetEnterpriseClientInfo(clientID);
            if (enterpriseInfo.Tables[0].Rows.Count > 0)
            {
                if (enterpriseInfo.Tables[0].Rows[0]["Active"].ToString() == "Y")
                {
                    if (enterpriseInfo.Tables[0].Rows[0]["StyleSheetReg"] != null)
                    {
                        StylePath = enterpriseInfo.Tables[0].Rows[0]["StyleSheetReg"].ToString();
                    }

                    if (enterpriseInfo.Tables[0].Rows[0]["ProductNameReg"] != null)
                    {
                        ProductName = enterpriseInfo.Tables[0].Rows[0]["ProductNameReg"].ToString();
                    }                    
                }
            }

            return this;
        }
    }
}