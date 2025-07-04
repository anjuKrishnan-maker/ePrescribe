using System;
using System.Collections;
using System.Collections.Generic;
using Allscripts.ePrescribe.Objects;

namespace eRxWeb.AppCode.DurBPL.RequestModel
{
    public interface ISaveDurRequest
    {
        List<Guid> DurList { get; set; }
        ArrayList RxToProviderList { get; set; }
        List<DurWarningSaveRecord> Warnings { get; set; }
    }
}