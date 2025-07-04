using Allscripts.ePrescribe.Objects.PPTPlus;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.AppCode.Rtps;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class RtpsHelperTests
    {
        [TestMethod]
        public void should_get_valid_transaction_ID()
        {
            //This is a placeholder, when the actual method is completed, test will fail and needs to be fixed
            //var assembly = this.GetType().GetTypeInfo().Assembly;
            //var textStreamReader = new StreamReader(assembly.GetManifestResourceStream("Allscripts.ePrescribe.Test.SampleJsons.Rtps_Display_Disposition.json"));
            //string fhirJsonFromTxHub = textStreamReader.ReadToEnd();
            //var stateContainerMock = MockRepository.GenerateStub<IStateContainer>();
            //IRtpsDisposition rtpsDispositonMock = MockRepository.GenerateStub<IRtpsDisposition>();
            //rtpsDispositonMock.Stub(_ => _.GetCrxTransactionID(null)).IgnoreArguments().Return("b05fa2d0-7882-11e9-83b9-d1deff5d2d6e");
            //var epsBrokerMock = MockRepository.GenerateStub<IEPSBroker>();
            //string crxID = new RtpsHelper().CheckForRtpsAndSendDispositionMessage(stateContainerMock, fhirJsonFromTxHub, rtpsDispositonMock, epsBrokerMock);
            //Assert.IsTrue(string.Compare("b05fa2d0-7882-11e9-83b9-d1deff5d2d6e", crxID) == 0);            
        }
        [TestMethod]
        public void should_send_display_disposition()
        {
            //This is a placeholder, when the actual method is completed, test will fail and needs to be fixed
            //var assembly = this.GetType().GetTypeInfo().Assembly;
            //var textStreamReader = new StreamReader(assembly.GetManifestResourceStream("Allscripts.ePrescribe.Test.SampleJsons.Rtps_Display_Disposition.json"));
            //string fhirJsonFromTxHub = textStreamReader.ReadToEnd();
            //var stateContainerMock = MockRepository.GenerateStub<IStateContainer>();
            //IRtpsDisposition rtpsDispositonMock = MockRepository.GenerateStub<IRtpsDisposition>();
            //rtpsDispositonMock.Stub(_ => _.GetCrxTransactionID(null)).IgnoreArguments().Return("b05fa2d0-7882-11e9-83b9-d1deff5d2d6e");
            //var epsBrokerMock = MockRepository.GenerateStub<IEPSBroker>();
            //rtpsDispositonMock.Stub(_ => _.SendDisplayedDisposition(stateContainerMock,"b05fa2d0-7882-11e9-83b9-d1deff5d2d6e", epsBrokerMock)).IgnoreArguments().Return(new eRxWeb.ePrescribeSvc.RtpsCRxDispositionResponse {StatusCode=200 });         
            //string crxID = new RtpsHelper().CheckForRtpsAndSendDispositionMessage(stateContainerMock, fhirJsonFromTxHub, rtpsDispositonMock, epsBrokerMock);
            //Assert.IsTrue(string.Compare("b05fa2d0-7882-11e9-83b9-d1deff5d2d6e", crxID) == 0);

            
        }
    }
}
