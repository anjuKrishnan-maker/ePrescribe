using System;
using System.Data;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact.Interfaces;
using eRxWeb.AppCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.ApproveRefillTaskTests
{
    [TestClass]
    public class IsCsMedTests
    {
        [TestMethod]
        public void should_return_true_if_csMedCode_is_not_empty_and_csMedCode_is_not_U()
        {
            //arrange
            var medDs = new DataSet();
            var medDt = new DataTable();
            medDt.Columns.Add("ControlledSubstanceCode");
            medDt.Columns.Add("DDI");
            var medRow = medDt.NewRow();
            medRow["ControlledSubstanceCode"] = 2;
            medRow["DDI"] = "123";
            medDt.Rows.Add(medRow);
            medDs.Tables.Add(medDt);

            var medMock = MockRepository.GenerateMock<IMedication>();
            medMock.Stub(x => x.LoadByNDC(null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(medDs);

            var presMock = MockRepository.GenerateMock<IPrescription>();
            presMock.Stub(x => x.GetStateControlledSubstanceCode(null, null, null, ConnectionStringPointer.SHARED_DB));

            var reqRx = new RequestedRx
            {
                NDCNumber = "123"
            };

            //act
            var result = ApproveRefillTask.IsCsMed(reqRx, "NY", "NY", medMock, presMock, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1);

            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void should_return_false_if_csMedCode_is_empty_and_csMedCode_is_not_U()
        {
            //arrange
            var medDs = new DataSet();
            var medDt = new DataTable();
            medDt.Columns.Add("ControlledSubstanceCode");
            medDt.Columns.Add("DDI");
            var medRow = medDt.NewRow();
            medRow["ControlledSubstanceCode"] = "";
            medRow["DDI"] = "123";
            medDt.Rows.Add(medRow);
            medDs.Tables.Add(medDt);

            var medMock = MockRepository.GenerateMock<IMedication>();
            medMock.Stub(x => x.LoadByNDC(null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(medDs);

            var presMock = MockRepository.GenerateMock<IPrescription>();
            presMock.Stub(x => x.GetStateControlledSubstanceCode(null, null, null, ConnectionStringPointer.SHARED_DB));

            var reqRx = new RequestedRx
            {
                NDCNumber = "123"
            };

            //act
            var result = ApproveRefillTask.IsCsMed(reqRx, "NY", "NY", medMock, presMock, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_false_if_csMedCode_is_not_empty_and_csMedCode_is_U()
        {
            //arrange
            var medDs = new DataSet();
            var medDt = new DataTable();
            medDt.Columns.Add("ControlledSubstanceCode");
            medDt.Columns.Add("DDI");
            var medRow = medDt.NewRow();
            medRow["ControlledSubstanceCode"] = "U";
            medRow["DDI"] = "123";
            medDt.Rows.Add(medRow);
            medDs.Tables.Add(medDt);

            var medMock = MockRepository.GenerateMock<IMedication>();
            medMock.Stub(x => x.LoadByNDC(null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(medDs);

            var presMock = MockRepository.GenerateMock<IPrescription>();
            presMock.Stub(x => x.GetStateControlledSubstanceCode(null, null, null, ConnectionStringPointer.SHARED_DB));

            var reqRx = new RequestedRx
            {
                NDCNumber = "123"
            };

            //act
            var result = ApproveRefillTask.IsCsMed(reqRx, "NY", "NY", medMock, presMock, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1);

            //assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void should_return_true_if_csStateCode_is_not_empty_and_csStateCode_is_not_U()
        {
            //arrange
            var medDs = new DataSet();
            var medDt = new DataTable();
            medDt.Columns.Add("ControlledSubstanceCode");
            medDt.Columns.Add("DDI");
            var medRow = medDt.NewRow();
            medRow["ControlledSubstanceCode"] = "U";
            medRow["DDI"] = "123";
            medDt.Rows.Add(medRow);
            medDs.Tables.Add(medDt);

            var medMock = MockRepository.GenerateMock<IMedication>();
            medMock.Stub(x => x.LoadByNDC(null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(medDs);

            var presMock = MockRepository.GenerateMock<IPrescription>();
            presMock.Stub(x => x.GetStateControlledSubstanceCode(null, null, null, ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return("2");

            var reqRx = new RequestedRx
            {
                NDCNumber = "123"
            };

            //act
            var result = ApproveRefillTask.IsCsMed(reqRx, "NY", "NY", medMock, presMock, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1);

            //assert
            Assert.IsTrue(result);
        }
    }
}
