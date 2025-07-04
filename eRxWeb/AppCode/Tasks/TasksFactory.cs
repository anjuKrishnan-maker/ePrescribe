using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Allscripts.ePrescribe.Common;
using Allscripts.Impact;
using Allscripts.Impact.Tasks;
using Allscripts.Impact.Tasks.Interfaces;

namespace eRxWeb.AppCode.Tasks
{
    public static class TasksFactory
    {
        public static IRxTask GetTask(RxTaskModel task)
        {
            switch (task.TaskType)
            {
                case Constants.PrescriptionTaskType.APPROVAL_REQUEST:
                    return new ApproveRxTask(task);
                case Constants.PrescriptionTaskType.REFREQ:
                    return new RefillRequestRxTask(task);
                case Constants.PrescriptionTaskType.RXCHG:
                    return new ChangeRxTask(task);
                case Constants.PrescriptionTaskType.RXCHG_PRIORAUTH:
                    return new ChangeRxTask(task);
                default:
                    return null;
            }
        }
    }
}