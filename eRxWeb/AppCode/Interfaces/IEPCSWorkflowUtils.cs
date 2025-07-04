using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Rx = Allscripts.Impact.Rx;
using Allscripts.Impact;

namespace eRxWeb.AppCode.Interfaces
{
    
    public interface IEPCSWorkflowUtils
    {
        bool IsEPCSWorkflowExpected(EPCSParameters epcsParams);
        bool IsControlledSubstanceMed(string reconciledControlledSubstanceCode, bool IsFreeFormMedControlSubstance);
        List<Rx> UpdateAndReturnEpcsMedList(List<Rx> epcsMedList, Rx rx);
        bool ShouldLaunchEPCSWorkflow(bool canStartEpcs, List<Rx> epcsMedList);
        EPCSParameters BuildEpcsWorkflowParameterObject(RxTaskModel currentTask);
    }
}
