using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode.SpecialtyMedWorkflow
{
    public class DXCConfirmOffer
    {
        public virtual int ActivityID { get; set; }
        public virtual bool ConfirmOffer { get; set; }
        public virtual string PatientEmail { get; set; }
        public virtual string PatientMobilePhone { get; set; }
        public virtual DateTime TimeStamp { get; set; }
    }
}