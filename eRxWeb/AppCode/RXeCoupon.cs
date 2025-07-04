using Allscripts.Impact;
using eRxWeb.AppCode.StateUtils;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode
{
    public class RXeCoupon
    {
        public static void requestECoupon(object rx,IStateContainer PageState)
        {
            EPSBroker.RequestECoupon(((Rx)rx).RxID, Allscripts.ePrescribe.Common.Constants.ECouponWorkFlowType.NEWRX, UserInfo.GetSessionLicenseID(PageState), UserInfo.GetSessionUserID(PageState), ApiHelper.GetDBID(PageState));
        }
    }
}