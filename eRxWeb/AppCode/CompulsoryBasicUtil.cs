using Allscripts.ePrescribe.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eRxWeb.AppCode
{
    public static class CompulsoryBasicUtil
    {
        /// <summary>
        /// Check if CompulsoryBasic enforcement has started
        /// </summary>
        /// <param name="startDate"></param>
        /// <returns></returns>
        public static bool HasCompulsoryBasicStarted(DateTime startDate)
        {
            bool returnValue = false;
            if (DateTime.Compare(DateTime.Now, startDate) > 0) //Current date later than startdate 
            {
                returnValue = true;
            }
            return returnValue;
        }

        /// <summary>
        /// Compulsory Basic is enforced(time beyond start date) and user has not bought basic or deluxe
        /// </summary>
        /// <param name="isEnterpriseCompulsoryBasic"></param>
        /// <param name="isPricingStructureBasic"></param>
        /// <returns></returns>
        public static bool ForceCompulsoryRestrictions(bool isEnterpriseCompulsoryBasic, bool isPricingStructureBasic, Constants.DeluxeFeatureStatus LicenseDeluxeStatus, DateTime compulsoryBasicStartDate,
            bool enterpriseCompulsoryBasicForce, bool isLicenseInFreeTrial)
        {
            bool returnType = false;    //By default don't enforce restrictions

            //For EnterpriseDeluxeStatus Compulsory Basic Force, Offer 30 days(db config) free trial
            if (enterpriseCompulsoryBasicForce)
            {
                if (!isLicenseInFreeTrial &&
                    IsCompulsoryBasicOrDeluxeNotPurchased(isEnterpriseCompulsoryBasic, isPricingStructureBasic) //And nothing purchased
                    || IsCompulsoryBasicEnterpriseAndLicenseDisabled(isEnterpriseCompulsoryBasic, LicenseDeluxeStatus))
                {
                    returnType = true;
                }
            }
            else
            {
                if (HasCompulsoryBasicStarted(compulsoryBasicStartDate)    //In enforced period
                    && (IsCompulsoryBasicOrDeluxeNotPurchased(isEnterpriseCompulsoryBasic, isPricingStructureBasic) //And nothing purchased
                    || IsCompulsoryBasicEnterpriseAndLicenseDisabled(isEnterpriseCompulsoryBasic, LicenseDeluxeStatus))) //OR License Disabled (Billing Failure)
                {
                    returnType = true;  //Then enforce restrictions
                }
            }
            return returnType;
        }

        /// <summary>
        /// Whether to Display Compulsory Basic Promo Teaser Ad (For non purchased user before the start date)
        /// </summary>
        /// <param name="isEnterpriseCompulsoryBasic"></param>
        /// <param name="isPricingStructureBasic"></param>
        /// <returns></returns>
        public static bool DisplayPromoTeaserAd(bool isEnterpriseCompulsoryBasic, bool isPricingStructureBasic, DateTime compulsoryBasicStartDate)
        {
            bool returnType = false;    //By default don't display ad

            if (!HasCompulsoryBasicStarted(compulsoryBasicStartDate)    //Not in enforced period
                && IsCompulsoryBasicOrDeluxeNotPurchased(isEnterpriseCompulsoryBasic, isPricingStructureBasic)) //And nothing purchased
            {
                returnType = true;  //Then enforce restrictions
            }

            return returnType;
        }

        /// <summary>
        /// Check if user falls under the compulsory basic enterprise and has purchased the compulsory basic pricing structure
        /// </summary>
        /// <returns></returns>
        public static bool IsCompulsoryBasicPurchased(bool isEnterpriseCompulsoryBasic, bool isPricingStructureCompulsoryBasic)
        {
            bool returnType = false;

            if (isEnterpriseCompulsoryBasic && isPricingStructureCompulsoryBasic)
            {
                returnType = true;
            }

            return returnType;
        }

        /// <summary>
        /// Check if user falls under the compulsory basic enterprise and has not purchased compulsory basic or deluxe
        /// </summary>
        /// <returns></returns>
        public static bool IsCompulsoryBasicOrDeluxeNotPurchased(bool isEnterpriseCompulsoryBasic, bool isPricingStructureBasic)
        {
            bool returnType = false;

            if (isEnterpriseCompulsoryBasic && isPricingStructureBasic)
            {
                returnType = true;
            }

            return returnType;
        }

        /// <summary>
        /// Check if user falls under the compulsory basic enterprise and is disabled
        /// </summary>
        /// <returns></returns>
        public static bool IsCompulsoryBasicEnterpriseAndLicenseDisabled(bool isEnterpriseCompulsoryBasic, Constants.DeluxeFeatureStatus LicenseDeluxeStatus)
        {
            bool returnType = false;

            if (isEnterpriseCompulsoryBasic && LicenseDeluxeStatus == Constants.DeluxeFeatureStatus.Disabled)
            {
                returnType = true;
            }

            return returnType;
        }

        /// <summary>
        /// Is license in free trial period
        /// </summary>
        /// <returns></returns>
        public static bool IsLicenseInFreeTrialPeriod(DateTime licenseCreationDate, int totalTrialDays)
        {
            bool returnType = false;
            DateTime lastDayForTrial = licenseCreationDate.Date.AddDays(totalTrialDays);

            if (DateTime.Today < lastDayForTrial)
            {
                returnType = true;
            }

            return returnType;
        }
    }
}