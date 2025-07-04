using System;
using System.Data;
using Allscripts.ePrescribe.Data.Registration;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using eRxWeb.AppCode.Registration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.AppCodeTests.RegistrationTests
{
    [TestClass]
    public class SetupRegistrationPricingStructuresTests
    {
        string pricingStructureData = "{\"Table0\":[{\"EnterprisePricing\":\"0\"}],\"Table1\":[{   \"ID\": 1,   \"PricingStructureDesc\": \"Deluxe\",   \"Price\": 20 }, {   \"ID\": 2,   \"PricingStructureDesc\": \"DeluxeEPCS\",   \"Price\": 39 }, {   \"ID\": 3,   \"PricingStructureDesc\": \"Legacy DeluxeEPCSLogRx\",   \"Price\": 20 }, {   \"ID\": 4,   \"PricingStructureDesc\": \"DeluxeEpa\",   \"Price\": 20 }, {   \"ID\": 5,   \"PricingStructureDesc\": \"Legacy DeluxeEpaLogRx\",   \"Price\": 20 }, {   \"ID\": 6,   \"PricingStructureDesc\": \"DeluxeEPCSEpa\",   \"Price\": 49 }, {   \"ID\": 7,   \"PricingStructureDesc\": \"DeluxeEPCSEpaLogRx\",   \"Price\": 30 }, {   \"ID\": 8,   \"PricingStructureDesc\": \"Basic\",   \"Price\": 0 }, {   \"ID\": 9,   \"PricingStructureDesc\": \"Legacy DeluxeEPCS\",   \"Price\": 25 }, {   \"ID\": 10,   \"PricingStructureDesc\": \"DeluxeEPCSLogRx\",   \"Price\": 25 }, {   \"ID\": 11,   \"PricingStructureDesc\": \"DeluxeEpaLogRx\",   \"Price\": 18 }, {   \"ID\": 12,   \"PricingStructureDesc\": \"CompulsoryBasic\",   \"Price\": 9 }, {   \"ID\": 13,   \"PricingStructureDesc\": \"DeluxeEPCSEpa2017\",   \"Price\": 39 }, {   \"ID\": 14,   \"PricingStructureDesc\": \"DeluxeEPCSEpaLogRx2017\",   \"Price\": 33 }]}";
        [TestMethod]
        public void Should_return_basic_deluxe_epcs_minimum_price()
        {
            var reg = new Registration();
        
            DataSet psData = (DataSet)JsonConvert.DeserializeObject(pricingStructureData, (typeof(DataSet)));
            var regDataMock = MockRepository.GenerateMock<IRegistrationData>();
            string emptyEnterpriseID = "";
            regDataMock.Stub(_ => _.GetPricingStructure(emptyEnterpriseID)).Return(psData);

            var productPriceMock = MockRepository.GenerateMock<IProductPrice>();
            productPriceMock.Stub(_ => _.EpcsRegistration).Return("30");
            var result = reg.SetupRegistrationPricingStructures(regDataMock, productPriceMock, emptyEnterpriseID);

            Assert.AreEqual("9", result.BasicPrice);
            Assert.AreEqual("20", result.DeluxePrice);
            Assert.AreEqual("18", result.DeluxeLogRxPrice);
            Assert.AreEqual("39", result.DeluxeEpcsPrice);
            Assert.AreEqual("33", result.DeluxeEpcsLogRxPrice);
        }
    }
}
