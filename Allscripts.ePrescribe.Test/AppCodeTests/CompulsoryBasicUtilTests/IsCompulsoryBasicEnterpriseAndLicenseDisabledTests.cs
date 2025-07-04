using Allscripts.ePrescribe.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allscripts.ePrescribe.Test.AppCodeTests.CompulsoryBasicUtilTests
{
    public partial class CompulsoryBasicUtilTests
    {
        [TestClass]
        public class IsCompulsoryBasicEnterpriseAndLicenseDisabledTests
        {
            [TestMethod]
            public void should_return_true_when_enterprise_compulsorybasic_deluxefeaturestatus_disabled()
            {
                //arrange
                bool isEnterpriseCompulsoryBasic = true;
                Constants.DeluxeFeatureStatus LicenseDeluxeStatus = Constants.DeluxeFeatureStatus.Disabled;
                DateTime startDate = DateTime.Now.AddDays(1);

                //act
                bool result = eRxWeb.AppCode.CompulsoryBasicUtil.IsCompulsoryBasicEnterpriseAndLicenseDisabled(isEnterpriseCompulsoryBasic, LicenseDeluxeStatus);

                //assert
                bool expected = true;
                Assert.AreEqual(expected, result);
            }

            [TestMethod]
            public void should_return_false_when_enterprise_compulsorybasic_deluxefeaturestatus_notdisabled()
            {
                //arrange
                bool isEnterpriseCompulsoryBasic = true;
                Constants.DeluxeFeatureStatus LicenseDeluxeStatus = Constants.DeluxeFeatureStatus.Cancelled;
                DateTime startDate = DateTime.Now.AddDays(1);

                //act
                bool result = eRxWeb.AppCode.CompulsoryBasicUtil.IsCompulsoryBasicEnterpriseAndLicenseDisabled(isEnterpriseCompulsoryBasic, LicenseDeluxeStatus);

                //assert
                bool expected = false;
                Assert.AreEqual(expected, result);
            }

            [TestMethod]
            public void should_return_false_when_enterprise_notcompulsorybasic_deluxefeaturestatus_disabled()
            {
                //arrange
                bool isEnterpriseCompulsoryBasic = false;
                Constants.DeluxeFeatureStatus LicenseDeluxeStatus = Constants.DeluxeFeatureStatus.Disabled;
                DateTime startDate = DateTime.Now.AddDays(1);

                //act
                bool result = eRxWeb.AppCode.CompulsoryBasicUtil.IsCompulsoryBasicEnterpriseAndLicenseDisabled(isEnterpriseCompulsoryBasic, LicenseDeluxeStatus);

                //assert
                bool expected = false;
                Assert.AreEqual(expected, result);
            }

            [TestMethod]
            public void should_return_false_when_enterprise_notcompulsorybasic_deluxefeaturestatus_notdisabled()
            {
                //arrange
                bool isEnterpriseCompulsoryBasic = false;
                Constants.DeluxeFeatureStatus LicenseDeluxeStatus = Constants.DeluxeFeatureStatus.Cancelled;
                DateTime startDate = DateTime.Now.AddDays(1);

                //act
                bool result = eRxWeb.AppCode.CompulsoryBasicUtil.IsCompulsoryBasicEnterpriseAndLicenseDisabled(isEnterpriseCompulsoryBasic, LicenseDeluxeStatus);

                //assert
                bool expected = false;
                Assert.AreEqual(expected, result);
            }
        }
    }
}
