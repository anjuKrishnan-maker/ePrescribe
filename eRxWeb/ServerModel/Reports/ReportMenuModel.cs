using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel.Reports
{
    public class ReportMenuModel
    {
        public List<ReportGroup> ReportGroupList { get; set; }
    }

    public class ReportGroup
    {
        public string Name { get; set; }
        public List<ReportLink> ReportLinkList { get; set; }

        public ReportGroup() { }

        public ReportGroup(string name)
        {
            Name = name;
        }
    }

    public class ReportLink
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Page { get; set; }
        public bool ShowDeluxeAd { get; set; } = false;
    }
}