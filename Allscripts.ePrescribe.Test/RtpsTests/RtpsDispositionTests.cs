using eRxWeb.AppCode.Interfaces;
using eRxWeb.AppCode.Rtps;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Allscripts.ePrescribe.Test.RtpsTests
{
    [TestClass]
    public class RtpsDispositionTests
    {
        //[TestMethod]
        //public void should_get_valid_transaction_ID()
        //{            
        //    string partnerResponseId = @"{""url"": ""https://fhir.allscripts.com/pptx/messageHeader-partnerResponseId"",
        //                  ""valueString"": ""b05fa2d0-7882-11e9-83b9-d1deff5d2d6e"" }";
        //    JToken token = JToken.Parse(partnerResponseId);
        //    string crxID = new RtpsDisposition().GetCrxTransactionID(token);
        //    Assert.IsTrue(string.Compare("b05fa2d0-7882-11e9-83b9-d1deff5d2d6e", crxID) == 0);
            
        //}

        //[TestMethod]
        //public void should_get_empty_transaction_ID_when_partner_responseid_missing()
        //{           
        //    string partnerResponseId = @"{""url"": ""https://fhir.allscripts.com/pptx/messageHeader-hasrtps"",
        //                  ""valueString"": ""b05fa2d0-7882-11e9-83b9-d1deff5d2d6e"" }";
        //    JToken token = JToken.Parse(partnerResponseId);           
        //    string crxID = new RtpsDisposition().GetCrxTransactionID(token);
        //    Assert.IsTrue(string.IsNullOrEmpty(crxID));
        //}
        //[TestMethod]
        //public void should_get_failed_status_code_when_partner_responseid_empty()
        //{           
        //    var stateContainerMock = MockRepository.GenerateStub<IStateContainer>();
        //    string partnerTxId = string.Empty;           
        //    var epsBrokerMock = MockRepository.GenerateStub<IEPSBroker>();
        //    epsBrokerMock.Stub(_ => _.RtpsCrxMedDisposition(new DatabaseSelector.ConnectionStringPointer(),null)).IgnoreArguments().Return(new eRxWeb.ePrescribeSvc.RtpsCRxDispositionResponse { StatusCode = 500 });
        //    eRxWeb.ePrescribeSvc.RtpsCRxDispositionResponse rtpsCRxDispositionResponse = new RtpsDisposition().SendDisplayedDisposition(stateContainerMock, partnerTxId, epsBrokerMock);
        //    Assert.AreEqual(rtpsCRxDispositionResponse.StatusCode, 500);          
        //}
    }
}
