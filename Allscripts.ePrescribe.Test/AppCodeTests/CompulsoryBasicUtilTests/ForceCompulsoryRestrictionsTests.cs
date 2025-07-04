using Allscripts.ePrescribe.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Allscripts.ePrescribe.Test.AppCodeTests.CompulsoryBasicUtilTests
{
    public partial class CompulsoryBasicUtilTests
    {
        [TestClass]
        public class ForceCompulsoryRestrictionsTests
        {
            [TestMethod]
            public void should_return_true_when_enterprise_compulsorybasic_pricingstructure_basic_DeluxeFeatureStatus_Disabled_startdate_past_trialperiod_false()
            {
                //arrange
                bool isEnterpriseCompulsoryBasic = true;
                bool isPricingStructureBasic = true;
                DateTime startDate = DateTime.Now.AddDays(-1);
                Constants.DeluxeFeatureStatus deluxeFeatureStatus = Constants.DeluxeFeatureStatus.Disabled;
                bool isForceCompulsoryBasic = false;
                bool trialPeriod = false;

                //act
                bool result = eRxWeb.AppCode.CompulsoryBasicUtil.ForceCompulsoryRestrictions(isEnterpriseCompulsoryBasic,
                    isPricingStructureBasic, deluxeFeatureStatus, startDate, isForceCompulsoryBasic, trialPeriod);

                //assert
                bool expected = true;
                Assert.AreEqual(expected, result);
            }

            [TestMethod]
            public void should_return_true_when_EnterpriseCompulsoryBasic_true_DeluxeFeatureStatus_Disabled_startdate_past_trialperiod_false()
            {
                //arrange
                bool isEnterpriseCompulsoryBasic = true;   
                bool isPricingStructureBasic = false;   //Doesnt Matter
                DateTime startDate = DateTime.Now.AddDays(-1);
                Constants.DeluxeFeatureStatus deluxeFeatureStatus = Constants.DeluxeFeatureStatus.Disabled;
                bool isForceCompulsoryBasic = false;
                bool trialPeriod = false;

                //act
                bool result = eRxWeb.AppCode.CompulsoryBasicUtil.ForceCompulsoryRestrictions(isEnterpriseCompulsoryBasic,
                    isPricingStructureBasic, deluxeFeatureStatus, startDate, isForceCompulsoryBasic, trialPeriod);

                //assert
                bool expected = true;
                Assert.AreEqual(expected, result);
            }

            [TestMethod]
            public void should_return_false_when_EnterpriseCompulsoryBasic_false_DeluxeFeatureStatus_Disabled_startdate_past_trialperiod_false()
            {
                //arrange
                bool isEnterpriseCompulsoryBasic = false;  
                bool isPricingStructureBasic = false;   //Doesnt Matter
                DateTime startDate = DateTime.Now.AddDays(-1); 
                Constants.DeluxeFeatureStatus deluxeFeatureStatus = Constants.DeluxeFeatureStatus.Disabled;
                bool isForceCompulsoryBasic = false;
                bool trialPeriod = false;

                //act
                bool result = eRxWeb.AppCode.CompulsoryBasicUtil.ForceCompulsoryRestrictions(isEnterpriseCompulsoryBasic,
                    isPricingStructureBasic, deluxeFeatureStatus, startDate, isForceCompulsoryBasic, trialPeriod);

                //assert
                bool expected = false;
                Assert.AreEqual(expected, result);
            }

            [TestMethod]
            public void should_return_false_when_startdate_future_trialperiod_false()
            {
                //arrange
                bool isEnterpriseCompulsoryBasic = false;   //Doesnt Matter
                bool isPricingStructureBasic = true;    //Doesnt Matter
                DateTime startDate = DateTime.Now.AddDays(1); 
                Constants.DeluxeFeatureStatus deluxeFeatureStatus = Constants.DeluxeFeatureStatus.Force; //Doesnt Matter
                bool isForceCompulsoryBasic = false;
                bool trialPeriod = false;

                //act
                bool result = eRxWeb.AppCode.CompulsoryBasicUtil.ForceCompulsoryRestrictions(isEnterpriseCompulsoryBasic,
                    isPricingStructureBasic, deluxeFeatureStatus, startDate, isForceCompulsoryBasic, trialPeriod);

                //assert
                bool expected = false;
                Assert.AreEqual(expected, result);
            }

            [TestMethod]
            public void should_return_false_when_enterprise_notcompulsorybasic_pricingstructure_basic_DeluxeFeatureStatus_NotDisabled_startdate_past_trialperiod_false()
            {
                //arrange
                bool isEnterpriseCompulsoryBasic = false;
                bool isPricingStructureBasic = true;
                DateTime startDate = DateTime.Now.AddDays(-1);
                Constants.DeluxeFeatureStatus deluxeFeatureStatus = Constants.DeluxeFeatureStatus.Off; //Doesnt Matter
                bool isForceCompulsoryBasic = false;
                bool trialPeriod = false;

                //act
                bool result = eRxWeb.AppCode.CompulsoryBasicUtil.ForceCompulsoryRestrictions(isEnterpriseCompulsoryBasic,
                    isPricingStructureBasic, deluxeFeatureStatus, startDate, isForceCompulsoryBasic, trialPeriod);

                //assert
                bool expected = false;
                Assert.AreEqual(expected, result);
            }

            [TestMethod]
            public void should_return_false_when_enterprise_compulsorybasic_pricingstructure_notbasic_DeluxeFeatureStatus_NotDisabled_startdate_past_trialperiod_false()
            {
                //arrange
                bool isEnterpriseCompulsoryBasic = true;    //enterprise is compulsory basic
                bool isPricingStructureBasic = false;
                DateTime startDate = DateTime.Now.AddDays(-1);
                Constants.DeluxeFeatureStatus deluxeFeatureStatus = Constants.DeluxeFeatureStatus.Off; //Doesnt Matter
                bool isForceCompulsoryBasic = false;
                bool trialPeriod = false;

                //act
                bool result = eRxWeb.AppCode.CompulsoryBasicUtil.ForceCompulsoryRestrictions(isEnterpriseCompulsoryBasic,
                    isPricingStructureBasic, deluxeFeatureStatus, startDate, isForceCompulsoryBasic, trialPeriod);

                //assert
                bool expected = false;
                Assert.AreEqual(expected, result);
            }

            [TestMethod]
            public void should_return_true_when_enterprise_compulsorybasic_pricingstructure_basic_DeluxeFeatureStatus_NotDisabled_startdate_future_isForceCompulsoryBasic_true_trialperiod_false()
            {
                //arrange
                bool isEnterpriseCompulsoryBasic = true;    //enterprise is compulsory basic
                bool isPricingStructureBasic = true;
                DateTime startDate = DateTime.Now.AddDays(-1);
                Constants.DeluxeFeatureStatus deluxeFeatureStatus = Constants.DeluxeFeatureStatus.Off; //Doesnt Matter
                bool isForceCompulsoryBasic = true;
                bool trialPeriod = false;

                //act
                bool result = eRxWeb.AppCode.CompulsoryBasicUtil.ForceCompulsoryRestrictions(isEnterpriseCompulsoryBasic,
                    isPricingStructureBasic, deluxeFeatureStatus, startDate, isForceCompulsoryBasic, trialPeriod);

                //assert
                bool expected = true;
                Assert.AreEqual(expected, result);
            }

            [TestMethod]
            public void should_return_false_when_enterprise_compulsorybasic_pricingstructure_basic_DeluxeFeatureStatus_NotDisabled_startdate_future_isForceCompulsoryBasic_true_trialperiod_true()
            {
                //arrange
                bool isEnterpriseCompulsoryBasic = true;    //enterprise is compulsory basic
                bool isPricingStructureBasic = true;
                DateTime startDate = DateTime.Now.AddDays(-1);
                Constants.DeluxeFeatureStatus deluxeFeatureStatus = Constants.DeluxeFeatureStatus.Off; //Doesnt Matter
                bool isForceCompulsoryBasic = true;
                bool trialPeriod = true;

                //act
                bool result = eRxWeb.AppCode.CompulsoryBasicUtil.ForceCompulsoryRestrictions(isEnterpriseCompulsoryBasic,
                    isPricingStructureBasic, deluxeFeatureStatus, startDate, isForceCompulsoryBasic, trialPeriod);

                //assert
                bool expected = false;
                Assert.AreEqual(expected, result);
            }
        }
    }
}
