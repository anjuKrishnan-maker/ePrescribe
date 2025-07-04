using System;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using eRxWeb.ServerModel.Request;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using PatientCoverage = eRxWeb.AppCode.PatientCoverage;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.PatientCoverageTests
{
    [TestClass]
    public class PatientCoverageTests
    {
        private IStateContainer _session;
        private IPatientCoverage _patientCoverage;
        private IPptPlus _pptPlus;

        [TestInitialize]
        public void Init()
        {
            _session = MockRepository.GenerateMock<IStateContainer>();
            _session.Stub(_ => _.Cast("SessionLicense", default(ApplicationLicense)))
                .Return(new ApplicationLicense { EnterpriseClient = new EnterpriseClient("") });

            _patientCoverage = MockRepository.GenerateMock<IPatientCoverage>();
            _pptPlus = MockRepository.GenerateMock<IPptPlus>();
        }

        [TestMethod]
        public void should_call_get_coverage_with_coverageId_and_dbId()
        {
            //arrange
            _session.Stub(_ => _.Cast(null, default(ConnectionStringPointer)))
                .IgnoreArguments()
                .Return(ConnectionStringPointer.AUDIT_ERXDB_SERVER_2);

            _patientCoverage.Stub(_ => _.GetCoverage(0, ConnectionStringPointer.SHARED_DB)).IgnoreArguments()
                .Return(new Objects.PatientCoverage());

            var coverageId = "4898493948";
            var apiRequest = new UpdatePatientSelectedCoverageRequest {CoverageId = coverageId};

            //act
            PatientCoverage.UpdatePatientCoverage(apiRequest, _session, _patientCoverage, _pptPlus);

            //assert
            _patientCoverage.AssertWasCalled(_ => _.GetCoverage(Arg<long>.Is.Equal(Convert.ToInt64(coverageId)), Arg<ConnectionStringPointer>.Is.Equal(ConnectionStringPointer.AUDIT_ERXDB_SERVER_2)));
        }

        [TestMethod]
        public void should_set_session_coverage_info()
        {
            //arrange
            _session.Stub(_ => _.Cast(null, default(ConnectionStringPointer)))
                .IgnoreArguments()
                .Return(ConnectionStringPointer.AUDIT_ERXDB_SERVER_2);

            var coverageId = "4898493948";
            var planId = "dkksdss";
            var fid = "dkskdskd";
            var fa = "Y";
            var otc = "otdkksd";
            var gdp = "dgkdkksoldss";
            var uldp = "dkskdks";
            var copay = "dkksdlsd";
            var altplan = "dksdir883293";
            var infoSource = "disd883kdskds";

            var patCoverage = new Objects.PatientCoverage
            {
                CoverageId = coverageId,
                PlanId = planId,
                FormularyId = fid,
                FormularyActive = fa,
                OtcCoverage = otc,
                GenericDrugPolicy = gdp,
                UnListedDrugPolicy = uldp,
                CopayId = copay,
                AltPlanId = altplan,
                InfoSourcePayerId = infoSource
            };

            _patientCoverage.Stub(_ => _.GetCoverage(0, ConnectionStringPointer.SHARED_DB)).IgnoreArguments()
                .Return(patCoverage);

            var apiRequest = new UpdatePatientSelectedCoverageRequest { CoverageId = coverageId };

            _session.Expect(_ => _["SelectedCoverageID"] = coverageId);
            _session.Expect(_ => _["PlanID"] = planId);
            _session.Expect(_ => _["FormularyID"] = fid);
            _session.Expect(_ => _["FormularyActive"] = fa);
            _session.Expect(_ => _["OTCCoverage"] = otc);
            _session.Expect(_ => _["GenericDrugPolicy"] = gdp);
            _session.Expect(_ => _["UnListedDrugPolicy"] = uldp);
            _session.Expect(_ => _["CoverageID"] = coverageId);
            _session.Expect(_ => _["CopayID"] = copay);
            _session.Expect(_ => _["AltPlanID"] = altplan);
            _session.Expect(_ => _["InfoSourcePayerID"] = infoSource);

            //act
            PatientCoverage.UpdatePatientCoverage(apiRequest, _session, _patientCoverage, _pptPlus);

            //assert
            _session.VerifyAllExpectations();
        }
    }
}
