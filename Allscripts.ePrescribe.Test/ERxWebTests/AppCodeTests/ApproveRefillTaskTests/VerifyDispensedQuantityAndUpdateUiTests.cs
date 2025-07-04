using System.Web.UI;
using System.Web.UI.WebControls;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact.Interfaces;
using Allscripts.Impact.Tasks.Interfaces;
using eRxWeb.AppCode.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using ApproveRefillTask = eRxWeb.AppCode.ApproveRefillTask;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.ApproveRefillTaskTests
{
    [TestClass]
    public class VerifyDispensedQuantityAndUpdateUiTests
    {
        [TestMethod]
        public void should_not_disable_approve_if_dispensed_rx_is_null()
        {
            //arrange
            var controls = new ControlCollection(new Control());
            var rdbApprove = new RadioButton();
            var tempDataItemMock = MockRepository.GenerateStub<ITelerik>();
            var approveRefillTaskMock = MockRepository.GenerateStub<IApproveRefillTask>();
            approveRefillTaskMock.Stub(x => x.DoesChangeRequestLinkExist(null)).IgnoreArguments().Return(true);

            //act
            ApproveRefillTask.VerifyDispensedQuantityAndUpdateUi(null, controls, rdbApprove, tempDataItemMock, approveRefillTaskMock);

            //assert
            Assert.IsTrue(rdbApprove.Enabled);
        }

        [TestMethod]
        public void should_not_disable_approve_if_dispensed_rx_quantity_is_greater_than_0()
        {
            //arrange
            var controls = new ControlCollection(new Control());
            var rdbApprove = new RadioButton();
            var tempDataItemMock = MockRepository.GenerateStub<ITelerik>();
            var approveRefillTaskMock = MockRepository.GenerateStub<IApproveRefillTask>();
            var dispensedRx = new DispensedRx {Quantity = 2};
            approveRefillTaskMock.Stub(x => x.DoesChangeRequestLinkExist(null)).IgnoreArguments().Return(true);

            //act
            ApproveRefillTask.VerifyDispensedQuantityAndUpdateUi(dispensedRx, controls, rdbApprove, tempDataItemMock, approveRefillTaskMock);

            //assert
            Assert.IsTrue(rdbApprove.Enabled);
        }

        [TestMethod]
        public void should_not_disable_approve_if_dispensed_rx_quantity_is_less_than_0_and_DoesChangeRequestLinkExist_is_false()
        {
            //arrange
            var controls = new ControlCollection(new Control());
            var rdbApprove = new RadioButton();
            var tempDataItemMock = MockRepository.GenerateStub<ITelerik>();
            var approveRefillTaskMock = MockRepository.GenerateStub<IApproveRefillTask>();
            var dispensedRx = new DispensedRx { Quantity = -2 };
            approveRefillTaskMock.Stub(x => x.DoesChangeRequestLinkExist(null)).IgnoreArguments().Return(false);

            //act
            ApproveRefillTask.VerifyDispensedQuantityAndUpdateUi(dispensedRx, controls, rdbApprove, tempDataItemMock, approveRefillTaskMock);

            //assert
            Assert.IsTrue(rdbApprove.Enabled);
        }

        [TestMethod]
        public void should_disable_approve_and_show_label_if_dispensed_rx_quantity_is_less_than_0_and_DoesChangeRequestLinkExist_is_true_and_approve_rb_is_not_null()
        {
            //arrange
            var controls = new ControlCollection(new Control());
            var rdbApprove = new RadioButton();
            var lblQuantityError = new Label { Visible = false };
            var tempDataItemMock = MockRepository.GenerateStub<ITelerik>();
            tempDataItemMock.Stub(x => x.FindControl("lblQuantityError")).Return(lblQuantityError);
            var approveRefillTaskMock = MockRepository.GenerateStub<IApproveRefillTask>();
            var dispensedRx = new DispensedRx { Quantity = 0 };
            approveRefillTaskMock.Stub(x => x.DoesChangeRequestLinkExist(null)).IgnoreArguments().Return(true);


            //act
            ApproveRefillTask.VerifyDispensedQuantityAndUpdateUi(dispensedRx, controls, rdbApprove, tempDataItemMock, approveRefillTaskMock);

            //assert
            Assert.IsFalse(rdbApprove.Enabled);
            Assert.IsTrue(lblQuantityError.Visible);
        }
    }
}
