using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Medispan.Clinical.Model.Response;
using Allscripts.Impact;
using eRxWeb.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode.DurBPL.ResponseModel
{
    public class RxDurResponseModel
    {
        protected IStateContainer PageState => new StateContainer(HttpContext.Current.Session);
       
        public List<Rx> RxList { get; set; }
        
        public bool IsAnyDurSettingOn { get; set; }
        
        public bool FreeFormDrugsHasItems { get; set; }
        public bool SetAndShowDurWarnings { get; set; }
        
        public bool WarningsListHasItems { get; set; }
        public bool CustListFreeFormDrugs { get; set; }
        public bool FoodListInteractions { get; set; }
        public bool AlcoholListInteractions { get; set; }
        public bool HasPriorAdverseReactions { get; set; }
        public bool HasDuplicateTherapyItems { get; set; }
        public bool HasDrugInteractionsItems { get; set; }
        public bool HasDosageCheckMedicationsItems { get; set; }
    
    }
}