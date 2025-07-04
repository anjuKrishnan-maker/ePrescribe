using Allscripts.ePrescribe.Objects;
using Allscripts.Impact.ScriptMsg;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode.DurBPL.RequestModel
{
    public class SaveDurRequest : ISaveDurRequest
    {
        public SaveDurRequest(ArrayList rxToProviderList, List<Guid> durList, List<DurWarningSaveRecord> warnings)
        {
            RxToProviderList = rxToProviderList;
            DurList = durList;
            Warnings = warnings;
        }

        public ArrayList RxToProviderList { get; set; }
        public List<Guid> DurList { get; set; }
        public List<DurWarningSaveRecord> Warnings { get; set; }

    }
}