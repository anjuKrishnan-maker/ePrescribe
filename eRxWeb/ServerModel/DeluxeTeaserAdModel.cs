using Allscripts.ePrescribe.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel
{
    public class DeluxeTeaserAdModel
    {
        public List<ePrescribeSvc.TeaserAdResponse> TeaserAdContent { get; set; }
        public bool IsCompulsaryBasic { get; set; }
        public string AdType { get; set; }
        public string AdHtml { get; set; }
        public string Cookie { get; set; }
    }
}