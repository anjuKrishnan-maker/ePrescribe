using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.ServerModel
{
    public class PharmacyInfo
    {
        public object LastPharmacyName { get; internal set; }
        public bool IsMoEpcs { get; internal set; }
        public bool MoreMailOrderPharmVisible { get; internal set; }
        public bool MoreRetailPharm { get; internal set; }
        public bool IsRetailEpcs { get; internal set; }
        public string PrefMOP { get; internal set; }
        public bool RemMOPharmVisible { get; internal set; }
        public bool RemPharmacyVisible { get; internal set; }
    }
}