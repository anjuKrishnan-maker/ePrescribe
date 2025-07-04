using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Allscripts.Impact.Interfaces;

namespace eRxWeb.AppCode.Interfaces
{
    public interface ISpecialtyMedDestinationOptionsParameters
    {
        bool IsSendToPharmacyOptionAvailable();
        bool IsSendToLimitedDistributionPharmacyOptionAvailable();
        bool IsSendToMailOrderOptionAvailable();
        bool IsPrintOptionAvailable();
        bool IsEIEOptionAvailable();       
    }
}
