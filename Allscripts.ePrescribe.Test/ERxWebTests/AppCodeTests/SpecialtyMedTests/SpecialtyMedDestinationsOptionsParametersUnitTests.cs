using System;
using Allscripts.ePrescribe.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using eRxWeb.AppCode.SpecialtyMedWorkflow;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SpecialtyMedTests
{
    [TestClass]
    public class SpecialtyMedDestinationsOptionsParametersUnitTests
    {
        [TestMethod]
        public void should_always_return_true_if_nonCSMed_And_Valid_SPI_AND_HasLastPharmacy_For_Method_IsSendToPharmacyOptionAvailable()
        {
            //Arrange
            SpecialtyMedDestinationOptionsParameters specDest = new SpecialtyMedDestinationOptionsParameters();
            specDest.IsControlledSubstanceMedication = false;
            specDest.ProviderSPI = "1234567";
            specDest.HasLastPharmacy = true;
            specDest.RX_DETAIL_status = Constants.RxDetailProcessPending;


            //Act
            bool isSendToPharmacyOptionAvailable = specDest.IsSendToPharmacyOptionAvailable();

            //Assert
            Assert.AreEqual(true, isSendToPharmacyOptionAvailable);
        }

        [TestMethod]
        public void should_always_return_false_if_RX_DETAIL_status_notPending_For_Method_IsSendToPharmacyOptionAvailable()
        {
            //Arrange
            SpecialtyMedDestinationOptionsParameters specDest = new SpecialtyMedDestinationOptionsParameters();
            specDest.IsControlledSubstanceMedication = false;
            specDest.ProviderSPI = "1234567";
            specDest.HasLastPharmacy = true;
            specDest.RX_DETAIL_status = 0; // pending would be 3


            //Act
            bool isSendToPharmacyOptionAvailable = specDest.IsSendToPharmacyOptionAvailable();

            //Assert
            Assert.AreEqual(false, isSendToPharmacyOptionAvailable);
        }

        [TestMethod]
        public void should_always_return_false_if_RX_DETAIL_status_notPending_For_Method_IsPrintOptionAvailable()
        {
            //Arrange
            SpecialtyMedDestinationOptionsParameters specDest = new SpecialtyMedDestinationOptionsParameters();
            specDest.RX_DETAIL_status = 0; // pending would be 3


            //Act
            bool isPrintOptionAvailable = specDest.IsPrintOptionAvailable();

            //Assert
            Assert.AreEqual(false, isPrintOptionAvailable);
        }

        [TestMethod]
        public void should_always_return_false_if_RX_DETAIL_status_notPending_For_Method_IsEIEOptionAvailable()
        {
            //Arrange
            SpecialtyMedDestinationOptionsParameters specDest = new SpecialtyMedDestinationOptionsParameters();
            specDest.RX_DETAIL_status = 0; // pending would be 3

            //Act
            bool isEIEOptionAvailable = specDest.IsEIEOptionAvailable();

            //Assert
            Assert.AreEqual(false, isEIEOptionAvailable);
        }


        [TestMethod]
        public void should_always_return_false_if_either_CSMed_or_Invalid_SPI_For_Method_IsSendToPharmacyOptionAvailable()
        {
            //Arrange
            SpecialtyMedDestinationOptionsParameters specDest = new SpecialtyMedDestinationOptionsParameters();
            specDest.IsControlledSubstanceMedication = true;
            specDest.ProviderSPI = "1234567";
            specDest.HasLastPharmacy = true;

            //Act
            bool isSendToPharmacyOptionAvailable = specDest.IsSendToPharmacyOptionAvailable();

            //Assert
            Assert.AreEqual(false, isSendToPharmacyOptionAvailable);

            //Arrange
            specDest.IsControlledSubstanceMedication = false;
            specDest.ProviderSPI = string.Empty;

            //Act
            isSendToPharmacyOptionAvailable = specDest.IsSendToPharmacyOptionAvailable();

            //Assert
            Assert.AreEqual(false, isSendToPharmacyOptionAvailable);
        }

        [TestMethod]
        public void should_always_return_true_if_nonCSMed_And_Valid_SPI_For_Method_IsSendToMailOrderOptionAvailable()
        {
            //Arrange
            SpecialtyMedDestinationOptionsParameters specDest = new SpecialtyMedDestinationOptionsParameters();
            specDest.IsControlledSubstanceMedication = false;
            specDest.ProviderSPI = "1234567";
            specDest.HasValidMobPharmacy = true;
            specDest.RX_DETAIL_status = Constants.RxDetailProcessPending;


            //Act
            bool isSendToMOOptionAvailable = specDest.IsSendToMailOrderOptionAvailable();

            //Assert
            Assert.AreEqual(true, isSendToMOOptionAvailable);
        }

        [TestMethod]
        public void should_always_return_false_if_either_CSMed_or_Invalid_SPI_For_Method_IsSendToMailOrderOptionAvailable()
        {
            //Arrange
            SpecialtyMedDestinationOptionsParameters specDest = new SpecialtyMedDestinationOptionsParameters();
            specDest.IsControlledSubstanceMedication = true;
            specDest.ProviderSPI = "1234567";
            specDest.HasValidMobPharmacy = true;
            specDest.RX_DETAIL_status = Constants.RxDetailProcessPending;


            //Act
            bool isSendToMOOptionAvailable = specDest.IsSendToMailOrderOptionAvailable();

            //Assert
            Assert.AreEqual(false, isSendToMOOptionAvailable);

            //Arrange
            specDest.IsControlledSubstanceMedication = false;
            specDest.ProviderSPI = string.Empty;

            //Act
            isSendToMOOptionAvailable = specDest.IsSendToMailOrderOptionAvailable();

            //Assert
            Assert.AreEqual(false, isSendToMOOptionAvailable);
        }

        [TestMethod]
        public void should_always_return_true_if_nonCSMed_And_Valid_SPI_And_WhiteListedMed_For_Method_IsSendToLDPLOptionAvailable()
        {
            //Arrange
            SpecialtyMedDestinationOptionsParameters specDest = new SpecialtyMedDestinationOptionsParameters();
            specDest.IsControlledSubstanceMedication = false;
            specDest.ProviderSPI = "1234567";
            specDest.IsLimitedDistributionMedication = true;
            specDest.RX_DETAIL_status = Constants.RxDetailProcessPending;


            //Act
            bool isSendToLDPLOptionAvailable = specDest.IsSendToLimitedDistributionPharmacyOptionAvailable();

            //Assert
            Assert.AreEqual(true, isSendToLDPLOptionAvailable);
        }

        [TestMethod]
        public void should_always_return_false_if_either_CSMed_or_Invalid_SPI_or_non_whitelisted_med_For_Method_IsSendToLDPLOptionAvailable()
        {
            //Arrange
            SpecialtyMedDestinationOptionsParameters specDest = new SpecialtyMedDestinationOptionsParameters();
            specDest.IsControlledSubstanceMedication = true;
            specDest.ProviderSPI = "1234567";


            //Act
            bool isSendToLDPLOptionAvailable = specDest.IsSendToLimitedDistributionPharmacyOptionAvailable();

            //Assert
            Assert.AreEqual(false, isSendToLDPLOptionAvailable);

            //Arrange
            specDest.IsControlledSubstanceMedication = false;
            specDest.ProviderSPI = string.Empty;

            //Act
            isSendToLDPLOptionAvailable = specDest.IsSendToLimitedDistributionPharmacyOptionAvailable();

            //Assert
            Assert.AreEqual(false, isSendToLDPLOptionAvailable);

            //Arrange
            specDest.IsControlledSubstanceMedication = false;
            specDest.ProviderSPI = "1234567";
            specDest.IsLimitedDistributionMedication = false;

            //Act
            isSendToLDPLOptionAvailable = specDest.IsSendToLimitedDistributionPharmacyOptionAvailable();

            //Assert
            Assert.AreEqual(false, isSendToLDPLOptionAvailable);
        }
    }
}
