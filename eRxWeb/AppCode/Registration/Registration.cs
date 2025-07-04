using Allscripts.ePrescribe.Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Allscripts.ePrescribe.Data.Registration;
using static Allscripts.ePrescribe.Common.Constants;
using Allscripts.Impact;
using Allscripts.ePrescribe.ExtensionMethods;
using Allscripts.Impact.Interfaces;

namespace eRxWeb.AppCode.Registration
{
    public class Registration
    {
        public RegistrationPricingStructure SetupRegistrationPricingStructures(IRegistrationData regData, IProductPrice productPrice, string enterpriseID)
        {
            var registrationPricingStructure = new RegistrationPricingStructure();
            var lstPricingStructureFlagged = getPricingStructureList(regData, enterpriseID);

            registrationPricingStructure.EnterprisePricing = lstPricingStructureFlagged.EnterprisePricing;
           
            registrationPricingStructure.EPCSSetupPrice = productPrice.EpcsRegistration;
            
          
            registrationPricingStructure.BasicPrice = getPriceFromPricingStructure(lstPricingStructureFlagged.pricingStructures, DeluxePricingStructure.CompulsoryBasic);
            registrationPricingStructure.DeluxePrice = getPriceFromPricingStructure(lstPricingStructureFlagged.pricingStructures, DeluxePricingStructure.DeluxeEpa);
            registrationPricingStructure.DeluxeLogRxPrice = getPriceFromPricingStructure(lstPricingStructureFlagged.pricingStructures, DeluxePricingStructure.DeluxeEpaLogRx);
            registrationPricingStructure.DeluxeEpcsPrice = getPriceFromPricingStructure(lstPricingStructureFlagged.pricingStructures, DeluxePricingStructure.DeluxeEpcsEpa2017);
            registrationPricingStructure.DeluxeEpcsLogRxPrice = getPriceFromPricingStructure(lstPricingStructureFlagged.pricingStructures, DeluxePricingStructure.DeluxeEpcsEpaLogRx2017);


            return registrationPricingStructure;
        }

        private PricingStructureFlagged getPricingStructureList(IRegistrationData regData, string enterpriseID)
        {
            PricingStructureFlagged pricingStructureFlagged = new PricingStructureFlagged();
            List<PricingStructure> lstPricingStructure = new List<PricingStructure>();
            DataSet dspricing = regData.GetPricingStructure(enterpriseID);
            DataTable dtpricing = dspricing.Tables[1];
            DataTable dtentpricing = dspricing.Tables[0];            
            if (dtpricing!= null && dtentpricing!= null)
            {
                int id; int price;
                foreach (DataRow row in dtpricing.Rows)
                {
                    id = price = 0;
                    int.TryParse(Convert.ToString(row["ID"]), out id);
                    int.TryParse(Convert.ToString(row["Price"]), out price);
                    lstPricingStructure.Add(new PricingStructure(id, Convert.ToString(row["PricingStructureDesc"]), price));
                }
                pricingStructureFlagged.EnterprisePricing = dtentpricing.Rows[0]["EnterprisePricing"].ToString() == "1" ? true : false;
                pricingStructureFlagged.pricingStructures = lstPricingStructure;
            }
            return pricingStructureFlagged;
        }

        private string getPriceFromPricingStructure(List<PricingStructure> psList, DeluxePricingStructure pricingStructure)
        {
            return psList.Find(x => x.Id == (int)pricingStructure) != null ? psList.Find(x => x.Id == (int)pricingStructure).Price.ToString() : string.Empty;
        }
    }    
}