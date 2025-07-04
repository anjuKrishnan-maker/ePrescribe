using System;
using System.Web.UI.HtmlControls;
using Allscripts.Impact.Interfaces;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.SpecialtyMedTests
{
    [TestClass]
    public class SetRadioButtonTests
    {
        [TestMethod]
        public void should_not_throw_exception_if_rbSelectedRow_is_null()
        {
            //arrange
            HtmlInputRadioButton rbSelectedTask = null;

            var telerikMock = MockRepository.GenerateMock<ITelerik>();
            telerikMock.Stub(x => x.FindControl("rbSelectedTask")).Return(rbSelectedTask);

            //act
            SpecialtyMed.SetRadioButton(telerikMock);
        }

        [TestMethod]
        public void should_add_RxTaskID_attribute_with_correct_value()
        {
            //arrange
            var rbSelectedTask = new HtmlInputRadioButton();

            var rxTaskId = 12349384;

            var telerikMock = MockRepository.GenerateMock<ITelerik>();
            telerikMock.Stub(x => x.FindControl("rbSelectedTask")).Return(rbSelectedTask);
            telerikMock.Stub(x => x.GetDataKeyValue("RxTaskID")).Return(rxTaskId);

            //act
            SpecialtyMed.SetRadioButton(telerikMock);

            //Assert
            Assert.AreEqual(rbSelectedTask.Attributes["RxTaskID"], rxTaskId.ToString());
        }
        [TestMethod]
        public void should_add_onclick_attribute_with_correct_value()
        {
            //arrange
            var rbSelectedTask = new HtmlInputRadioButton();

            var rxTaskId = 12349384;
            var patientGuid = Guid.NewGuid();

            var telerikMock = MockRepository.GenerateMock<ITelerik>();
            telerikMock.Stub(x => x.FindControl("rbSelectedTask")).Return(rbSelectedTask);
            telerikMock.Stub(x => x.GetDataKeyValue("RxTaskID")).Return(rxTaskId);
            telerikMock.Stub(x => x.GetDataKeyValue("PatientGUID")).Return(patientGuid);

            //act
            SpecialtyMed.SetRadioButton(telerikMock);

            //Assert
            Assert.AreEqual(rbSelectedTask.Attributes["onclick"], $"taskSelectedRadio('{rxTaskId}', '{patientGuid}')");
        }
    }
}
