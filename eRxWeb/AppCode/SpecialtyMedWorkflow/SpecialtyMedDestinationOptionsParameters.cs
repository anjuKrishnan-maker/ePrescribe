using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Allscripts.ePrescribe.Common;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact.Interfaces;

namespace eRxWeb.AppCode.SpecialtyMedWorkflow
{
    public class SpecialtyMedDestinationOptionsParameters: ISpecialtyMedDestinationOptionsParameters
    {
        #region Properties
        public string ProviderSPI { get; set; }
        public string RxID { get; set; }
        public ConnectionStringPointer DBID { get; set; }
        public bool IsLimitedDistributionMedication { get; set; }
        public bool IsControlledSubstanceMedication { get; set; }
        public bool HasValidMobPharmacy { get; set; }
        public bool HasLastPharmacy { get; set; }
        public int RX_DETAIL_status { get; set; }       
        #endregion

        #region  Methods
        public bool IsSendToPharmacyOptionAvailable()
        {
            return (!string.IsNullOrEmpty(ProviderSPI) && !IsControlledSubstanceMedication 
                    && HasLastPharmacy && (RX_DETAIL_status == Constants.RxDetailProcessPending));
        }

        public bool IsSendToLimitedDistributionPharmacyOptionAvailable()
        {
            return (!string.IsNullOrEmpty(ProviderSPI) && !IsControlledSubstanceMedication 
                    && IsLimitedDistributionMedication && (RX_DETAIL_status == Constants.RxDetailProcessPending));
        }

        public bool IsSendToMailOrderOptionAvailable()
        {
            return (!string.IsNullOrEmpty(ProviderSPI) && !IsControlledSubstanceMedication 
                    && HasValidMobPharmacy && (RX_DETAIL_status == Constants.RxDetailProcessPending));
        }

        public bool IsPrintOptionAvailable()
        {
            return (RX_DETAIL_status == Constants.RxDetailProcessPending);
        }

        public bool IsEIEOptionAvailable()
        {
            return (RX_DETAIL_status == Constants.RxDetailProcessPending);
        }
        #endregion
    }
}