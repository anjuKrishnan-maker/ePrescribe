using System;
using System.Xml;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.Objects;
using eRxWeb.AppCode.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.TaskDisplayTests
{
    [TestClass]
    public class CreateRxDetailTests
    {
        [TestMethod]
        public void should_set_all_values_to_empty_string()
        {
            //arrange

            var doc = new XmlDocument();
            doc.LoadXml("<root></root>");

            var smNodes = new ScriptMessageNodes
            {
                Prescription = doc.FirstChild,
                Provider = doc.FirstChild,
                SupervisingProvider = doc.FirstChild
            };

            //act
            var result = TaskDisplay.CreateRxDetail(smNodes, "", "");


            //assert
            Assert.AreEqual(CommonTerms.NotSpecified, result.DrugDescription);
            Assert.AreEqual(CommonTerms.NotSpecified, result.Quantity);
            Assert.AreEqual(string.Empty, result.DaysSupply);
            Assert.AreEqual(CommonTerms.NotSpecified, result.Refills);
            Assert.AreEqual(CommonTerms.NotSpecified, result.SigText);
            Assert.AreEqual(string.Empty, result.RxNotes);
            Assert.AreEqual(CommonTerms.NotSpecified, result.CreatedDate);
            Assert.AreEqual(CommonTerms.NotSpecified, result.LastFillDate);
            Assert.AreEqual(string.Empty, result.Daw);
            Assert.AreEqual(", ", result.ProviderOfRecord);
            Assert.AreEqual("", result.PharmacyDetails);
        }

        [TestMethod]
        public void should_set_all_values()
        {
            //arrange

            var doc = new XmlDocument();
            doc.LoadXml("<root><PrescriberOrderNo>00f18c340463471dadc67047a20619eb</PrescriberOrderNo><NDCNumber>00074434190</NDCNumber><DrugDescription>Synthroid 25 MCG</DrugDescription><Quantity>84</Quantity><QuantityQualifier>TABS</QuantityQualifier><Created>2013-04-01T00:00:00</Created><LastFillDate>2013-04-01T00:00:00</LastFillDate><DosageFormCode /><StrengthUOM /><SIGText>Take one tablet daily.</SIGText><RefillQuantity>1</RefillQuantity><RefillQuantityQual>P</RefillQuantityQual><DaysSupply>90</DaysSupply><DAW>0</DAW><DAWDetail>0</DAWDetail><PharmacyNotes>Please remind patient that smoking is not recommended while taking this product.</PharmacyNotes><RxNotes>Please remind patient that smoking is not recommended while taking this product.</RxNotes><RxNormCUI>749158</RxNormCUI><RxNormQual>BPK</RxNormQual><NCPDPQuantityUOM>C48542</NCPDPQuantityUOM></root>");

            var smNodes = new ScriptMessageNodes
            {
                Prescription = doc.Clone().FirstChild
            };

            //act
            var result = TaskDisplay.CreateRxDetail(smNodes, "", "Pharmacy 123 Lane");


            //assert
            Assert.AreEqual("Synthroid 25 MCG", result.DrugDescription);
            Assert.AreEqual("84", result.Quantity);
            Assert.AreEqual("90", result.DaysSupply);
            Assert.AreEqual("1", result.Refills);
            Assert.AreEqual("Take one tablet daily.", result.SigText);
            Assert.AreEqual("Please remind patient that smoking is not recommended while taking this product.", result.RxNotes);
            Assert.AreEqual("4/1/2013", result.CreatedDate);
            Assert.AreEqual("4/1/2013", result.LastFillDate);
            Assert.AreEqual("0", result.Daw);
            Assert.AreEqual(null, result.ProviderOfRecord);
            Assert.AreEqual("Pharmacy 123 Lane", result.PharmacyDetails);
        }

        [TestMethod]
        public void should_add_form_Description_if_not_already_contained_in_description()
        {
            //arrange

            var doc = new XmlDocument();
            doc.LoadXml("<root><PrescriberOrderNo>00f18c340463471dadc67047a20619eb</PrescriberOrderNo><NDCNumber>00074434190</NDCNumber><DrugDescription>Synthroid 25 MCG</DrugDescription><Quantity>84</Quantity><QuantityQualifier>TABS</QuantityQualifier><Created>2013-04-01T00:00:00</Created><DosageFormCode /><StrengthUOM /><SIGText>Take one tablet daily.</SIGText><RefillQuantity>1</RefillQuantity><RefillQuantityQual>P</RefillQuantityQual><DaysSupply>90</DaysSupply><DAW>0</DAW><DAWDetail>0</DAWDetail><PharmacyNotes>Please remind patient that smoking is not recommended while taking this product.</PharmacyNotes><RxNotes>Please remind patient that smoking is not recommended while taking this product.</RxNotes><RxNormCUI>749158</RxNormCUI><RxNormQual>BPK</RxNormQual><NCPDPQuantityUOM>C48542</NCPDPQuantityUOM></root>");

            var smNodes = new ScriptMessageNodes
            {
                Prescription = doc.Clone().FirstChild
            };

            //act
            var result = TaskDisplay.CreateRxDetail(smNodes, "Tablet", null);


            //assert
            Assert.AreEqual("Synthroid 25 MCG Tablet", result.DrugDescription);
        }

        [TestMethod]
        public void should_not_add_form_Description_if_already_contained_in_description()
        {
            //arrange

            var doc = new XmlDocument();
            doc.LoadXml("<root><PrescriberOrderNo>00f18c340463471dadc67047a20619eb</PrescriberOrderNo><NDCNumber>00074434190</NDCNumber><DrugDescription>Synthroid 25 MCG Tablet</DrugDescription><Quantity>84</Quantity><QuantityQualifier>TABS</QuantityQualifier><Created>2013-04-01T00:00:00</Created><DosageFormCode /><StrengthUOM /><SIGText>Take one tablet daily.</SIGText><RefillQuantity>1</RefillQuantity><RefillQuantityQual>P</RefillQuantityQual><DaysSupply>90</DaysSupply><DAW>0</DAW><DAWDetail>0</DAWDetail><PharmacyNotes>Please remind patient that smoking is not recommended while taking this product.</PharmacyNotes><RxNotes>Please remind patient that smoking is not recommended while taking this product.</RxNotes><RxNormCUI>749158</RxNormCUI><RxNormQual>BPK</RxNormQual><NCPDPQuantityUOM>C48542</NCPDPQuantityUOM></root>");
            var smNodes = new ScriptMessageNodes
            {
                Prescription = doc.Clone().FirstChild
            };

            //act
            var result = TaskDisplay.CreateRxDetail(smNodes, "Tablet", null);


            //assert
            Assert.AreEqual("Synthroid 25 MCG Tablet", result.DrugDescription);
        }

        [TestMethod]
        public void should_providerOfRecord_should_be_supProvider()
        {
            //arrange

            var providerXml = new XmlDocument();
            providerXml.LoadXml("<root><LastName>Corn</LastName><FirstName>Joe</FirstName></root>");

            var supProviderXml = new XmlDocument();
            supProviderXml.LoadXml("<root><LastName>Jones</LastName><FirstName>Jim</FirstName></root>");

            var smNodes = new ScriptMessageNodes
            {
                Provider = providerXml.Clone().FirstChild,
                SupervisingProvider = supProviderXml.Clone().FirstChild,
                Prescription = new XmlDocument()
            };
            
            //act
            var result = TaskDisplay.CreateRxDetail(smNodes, null, null);


            //assert
            Assert.AreEqual("Jones, Jim", result.ProviderOfRecord);
        }

        [TestMethod]
        public void should_providerOfRecord_should_be_Provider()
        {
            //arrange

            var providerXml = new XmlDocument();
            providerXml.LoadXml("<root><LastName>Corn</LastName><FirstName>Joe</FirstName></root>");

            var smNodes = new ScriptMessageNodes
            {
                Provider = providerXml.Clone().FirstChild,
                Prescription = new XmlDocument()
            };

            //act
            var result = TaskDisplay.CreateRxDetail(smNodes, null, null);


            //assert
            Assert.AreEqual("Corn, Joe", result.ProviderOfRecord);
        }

        [TestMethod]
        public void should_not_add_form_description_if_its_null()
        {
            //arrange
            var doc = new XmlDocument();
            doc.LoadXml("<root><PrescriberOrderNo>00f18c340463471dadc67047a20619eb</PrescriberOrderNo><NDCNumber>00074434190</NDCNumber><DrugDescription>Synthroid 25 MCG</DrugDescription><Quantity>84</Quantity><QuantityQualifier>TABS</QuantityQualifier><Created>2013-04-01T00:00:00</Created><DosageFormCode /><StrengthUOM /><SIGText>Take one tablet daily.</SIGText><RefillQuantity>1</RefillQuantity><RefillQuantityQual>P</RefillQuantityQual><DaysSupply>90</DaysSupply><DAW>0</DAW><DAWDetail>0</DAWDetail><PharmacyNotes>Please remind patient that smoking is not recommended while taking this product.</PharmacyNotes><RxNotes>Please remind patient that smoking is not recommended while taking this product.</RxNotes><RxNormCUI>749158</RxNormCUI><RxNormQual>BPK</RxNormQual><NCPDPQuantityUOM>C48542</NCPDPQuantityUOM></root>");

            var smNodes = new ScriptMessageNodes
            {
                Prescription = doc.Clone().FirstChild
            };

            //act
            var result = TaskDisplay.CreateRxDetail(smNodes, null, null);


            //assert
            Assert.AreEqual("Synthroid 25 MCG", result.DrugDescription);
        }
    }
}
