using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel.Request
{
    public class WelcomeTourRequest
    {
    }

    public class WelcomeTourDoNotShowAgainRequest
    {
        public int TourType { get; set; }
    }
}