using System;
using System.Security.Cryptography.X509Certificates;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Partner = eRxWeb.Partner;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SsoTests
{
    [TestClass]
    public class VerifyPartnerCertificateTests
    {
        [TestMethod]
        public void should_not_throw_exception_if_signiture_is_verified()
        {
            //arrange
            var ssoMock = MockRepository.GenerateMock<ISso>();
            ssoMock.Stub(x => x.VerifyResponseSigniture(null, null)).IgnoreArguments().Return(true);
            var partner = new Partner();
            partner.Certificate2 = new X509Certificate2();

            //act
            try
            {
                Sso.VerifyPartnerCertificate(partner, null, string.Empty, null, new Audit(), ssoMock, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void should_throw_exception_if_certificate2_is_null()
        {
            //arrange
            var ssoMock = MockRepository.GenerateMock<ISso>();
            ssoMock.Stub(x => x.VerifyResponseSigniture(null, null)).IgnoreArguments().Return(true);

            var auditMock = MockRepository.GenerateMock<IAudit>();
            auditMock.Stub(x => x.AddException(null, null, null, null, null, null, ConnectionStringPointer.SHARED_DB))
                .IgnoreArguments()
                .Return("");

            var partner = new Partner {Certificate2 = null};


            //act
            Sso.VerifyPartnerCertificate(partner, null, string.Empty, null, auditMock, ssoMock, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void should_throw_exception_if_cert_is_not_verified()
        {
            //arrange
            var ssoMock = MockRepository.GenerateMock<ISso>();
            ssoMock.Stub(x => x.VerifyResponseSigniture(null, null)).IgnoreArguments().Return(false);

            var auditMock = MockRepository.GenerateMock<IAudit>();
            auditMock.Stub(x => x.AddException(null, null, null, null, null, null, ConnectionStringPointer.SHARED_DB))
                .IgnoreArguments()
                .Return("");

            var stateMock = MockRepository.GenerateStub<IStateContainer>();
            stateMock["PartnerID"] = "9B467675-2DB4-42CB-87C0-6165DA6ED498";

            var partner = new Partner { Certificate2 = new X509Certificate2() };


            //act
            Sso.VerifyPartnerCertificate(partner, null, string.Empty, stateMock, auditMock, ssoMock, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1);
        }
    }
}
