using System;
using System.Web.UI.WebControls;
using Allscripts.Impact;
using Allscripts.Impact.Interfaces;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SpecialtyMedTests
{
    [TestClass]
    public class SetSpecialtyPaStatusLabelsTests
    {
        private const string _aTagFormat = @"<a title=""You are now leaving the Veradigm site to a site operated by a third party."" class=""linkButton"" onclick=""confirmOffer('{0}');connectToiFC('{1}', '{2}', '{0}', '{3}')"">{4}</a>";

        [TestMethod]
        public void should_not_set_literal_text_if_litSpecPaStatus_is_null()
        {
            //arrange
            var litSpecPAStatus = new Literal();

            var telerikMock = MockRepository.GenerateMock<ITelerik>();
            telerikMock.Stub(x => x.GetDataKeyValue("PriorAuthorizationStatus")).Return(SpecialtyMedPriorAuthorizationStatus.SAVED);
            telerikMock.Stub(x => x.GetDataKeyValue("litSpecPAStatus")).Return(litSpecPAStatus);
            telerikMock.Stub(x => x.GetDataKeyValue("SpecPAStatus")).Return(null);

            //act
            SpecialtyMed.SetSpecialtyPaStatusLabels(telerikMock, "url");

            //assert
            Assert.AreEqual("", litSpecPAStatus.Text);
        }

        [TestMethod]
        public void should_not_set_literal_text_if_url_is_null()
        {
            //arrange
            var litSpecPAStatus = new Literal();

            var telerikMock = MockRepository.GenerateMock<ITelerik>();
            telerikMock.Stub(x => x.GetDataKeyValue("PriorAuthorizationStatus")).Return(SpecialtyMedPriorAuthorizationStatus.SAVED);
            telerikMock.Stub(x => x.GetDataKeyValue("litSpecPAStatus")).Return(litSpecPAStatus);
            telerikMock.Stub(x => x.GetDataKeyValue("SpecPAStatus")).Return("Saved");

            //act
            SpecialtyMed.SetSpecialtyPaStatusLabels(telerikMock, null);

            //assert
            Assert.AreEqual("", litSpecPAStatus.Text);
        }

        [TestMethod]
        public void should_not_set_literal_text_if_url_is_empty_string()
        {
            //arrange
            var litSpecPAStatus = new Literal();

            var telerikMock = MockRepository.GenerateMock<ITelerik>();
            telerikMock.Stub(x => x.GetDataKeyValue("PriorAuthorizationStatus")).Return(SpecialtyMedPriorAuthorizationStatus.SAVED);
            telerikMock.Stub(x => x.GetDataKeyValue("litSpecPAStatus")).Return(litSpecPAStatus);
            telerikMock.Stub(x => x.GetDataKeyValue("SpecPAStatus")).Return("Saved");

            //act
            SpecialtyMed.SetSpecialtyPaStatusLabels(telerikMock, "");

            //assert
            Assert.AreEqual("", litSpecPAStatus.Text);
        }

        [TestMethod]
        public void should_set_literal_text_to_a_tag_if_status_is_saved()
        {
            //arrange
            var litSpecPAStatus = new Literal();
            var rxTaskID = "0";
            var url = "www.xMenRule.com";
            var specPaStatus = "Saved";
            var activityId = "123";
            var patientId = Guid.NewGuid();
            var telerikMock = MockRepository.GenerateMock<ITelerik>();
            telerikMock.Stub(x => x.GetDataKeyValue("PriorAuthorizationStatus")).Return(SpecialtyMedPriorAuthorizationStatus.SAVED);
            telerikMock.Stub(x => x.FindControl("litSpecPAStatus")).Return(litSpecPAStatus);
            telerikMock.Stub(x => x.GetDataKeyValue("SpecPAStatus")).Return(specPaStatus);
            telerikMock.Stub(x => x.GetDataKeyValue("ActivityID")).Return(activityId);
            telerikMock.Stub(x => x.GetDataKeyValue("PatientGUID")).Return(patientId);

            //act
            SpecialtyMed.SetSpecialtyPaStatusLabels(telerikMock, url);

            //assert
            Assert.AreEqual(string.Format(_aTagFormat, rxTaskID, url, activityId, patientId, specPaStatus), litSpecPAStatus.Text);
        }

        [TestMethod]
        public void should_set_literal_text_to_a_tag_if_status_is_ready()
        {
            //arrange
            var litSpecPAStatus = new Literal();
            var rxTaskID = "0";
            var url = "www.IAmBatman.com";
            var specPaStatus = "Ready";
            var activityId = "123";
            var patientId = Guid.NewGuid();
            var telerikMock = MockRepository.GenerateMock<ITelerik>();
            telerikMock.Stub(x => x.GetDataKeyValue("PriorAuthorizationStatus")).Return(SpecialtyMedPriorAuthorizationStatus.READY);
            telerikMock.Stub(x => x.FindControl("litSpecPAStatus")).Return(litSpecPAStatus);
            telerikMock.Stub(x => x.GetDataKeyValue("SpecPAStatus")).Return(specPaStatus);
            telerikMock.Stub(x => x.GetDataKeyValue("ActivityID")).Return(activityId);
            telerikMock.Stub(x => x.GetDataKeyValue("PatientGUID")).Return(patientId);

            //act
            SpecialtyMed.SetSpecialtyPaStatusLabels(telerikMock, url);

            //assert
            Assert.AreEqual(string.Format(_aTagFormat, rxTaskID, url, activityId, patientId, specPaStatus), litSpecPAStatus.Text);
        }
    }
}
