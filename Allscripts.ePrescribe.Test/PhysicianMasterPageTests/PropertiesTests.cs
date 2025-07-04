using System;
using eRxWeb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.PhysicianMasterPageTests
{
    [TestClass]
    public class PropertiesTests
    {
        [TestMethod]
        public void should_return_null_if_allowPatientEdit_is_false()
        {
            //arrange
            PhysicianMasterPage page = new PhysicianMasterPage();
            page.AllowPatientEdit = false;
            
            //act
            var redirect = page.editPatientRedirect;

            //assert
            Assert.AreEqual(null, redirect);
        }

        [TestMethod]
        public void should_return_redirect_if_allowPatientEdit_is_true()
        {
            //arrange
            PhysicianMasterPage page = new PhysicianMasterPage();
            page.AllowPatientEdit = true;

            //act
            var redirect = page.editPatientRedirect;

            //assert
            Assert.AreEqual(ePrescribe.Common.Constants.PageNames.ADD_PATIENT + "?Mode=Edit", redirect);
        }

        [TestMethod]
        public void should_return_null_if_allowAllergyEdit_is_false()
        {
            //arrange
            PhysicianMasterPage page = new PhysicianMasterPage();
            page.AllowAllergyEdit = false;

            //act
            var redirect = page.editAllergyRedirect;

            //assert
            Assert.AreEqual(null, redirect);
        }

        [TestMethod]
        public void should_return_redirect_if_allowAllergyEdit_is_true()
        {
            //arrange
            PhysicianMasterPage page = new PhysicianMasterPage();
            page.AllowAllergyEdit = true;

            //act
            var redirect = page.editAllergyRedirect;

            //assert
            Assert.AreEqual(ePrescribe.Common.Constants.PageNames.PATIENT_ALLERGY, redirect);
        }

        [TestMethod]
        public void should_return_null_if_allowDiagnosticsEdit_is_false()
        {
            //arrange
            PhysicianMasterPage page = new PhysicianMasterPage();
            page.AllowDiagnosisEdit = false;

            //act
            var redirect = page.editProblemRedirect;

            //assert
            Assert.AreEqual(null, redirect);
        }

        [TestMethod]
        public void should_return_redirect_if_allowDiagnosticsEdit_is_true()
        {
            //arrange
            PhysicianMasterPage page = new PhysicianMasterPage();
            page.AllowDiagnosisEdit = true;

            //act
            var redirect = page.editProblemRedirect;

            //assert
            Assert.AreEqual(ePrescribe.Common.Constants.PageNames.PATIENT_DIAGNOSIS, redirect);
        }

        [TestMethod]
        public void should_return_null_if_allowPharmacyEdit_is_false()
        {
            //arrange
            PhysicianMasterPage page = new PhysicianMasterPage();
            page.AllowPharmacyEdit = false;

            //act
            var redirect = page.EditRetailPharmRedirect;

            //assert
            Assert.AreEqual(null, redirect);
        }
    }
}
