using System;
using System.Data;
using Allscripts.ePrescribe.Common;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Objects;
using Allscripts.Impact.Interfaces;
using Allscripts.Impact.Tasks;
using eRxWeb.AppCode;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using eRxWeb.AppCode.Tasks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.ChangeRxTests
{
    [TestClass]
    public class CheckForDelegateProviderTests
    {
        [TestMethod]
        public void should_return_same_userId_as_sent_in_when_user_is_not_pob_or_pa_with_sup()
        {
            //arrange
            var userId = new Guid("AD8F7EB5-B1EF-4FD0-B944-937F4E75BBBD");

            var rxUserMock = MockRepository.GenerateMock<IRxUser>();
            
            //act
            var result = new ChangeRxTask().CheckForDelegateProvider(userId, Guid.Empty, Guid.Empty, Constants.UserCategory.PROVIDER,  rxUserMock, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1);

            //assert
            Assert.AreEqual(userId, result);
        }

        [TestMethod]
        public void should_return_value_from_delegate_provider_session_var_if_user_is_pa_with_sup()
        {
            //arrange
            var userId = new Guid("AD8F7EB5-B1EF-4FD0-B944-937F4E75BBBD");
            var delegateId = new Guid("72F258BC-78DB-47F6-8DAF-08EC4AD2F398");

            var rxUserMock = MockRepository.GenerateMock<IRxUser>();
            
            //act
            var result = new ChangeRxTask().CheckForDelegateProvider(userId, Guid.Empty, delegateId, Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED,  rxUserMock, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1);

            //assert
            Assert.AreEqual(delegateId, result);
        }

        [TestMethod]
        public void should_return_delegate_provider_from_session_if_loggedIn_user_is_pob_and_provider_from_sm_is_pa_with_sup()
        {
            //arrange
            var userId = new Guid("AD8F7EB5-B1EF-4FD0-B944-937F4E75BBBD");
            var delegateId = new Guid("72F258BC-78DB-47F6-8DAF-08EC4AD2F398");
            
            var dt = new DataTable();
            dt.Columns.Add("UserType");
            var row = dt.NewRow();
            row["UserType"] = Constants.UserCategory.PHYSICIAN_ASSISTANT_SUPERVISED;

            var rxUserMock = MockRepository.GenerateMock<IRxUser>();
            rxUserMock.Stub(x => x.GetUserBasicInfo(new Guid(), ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(row);
            
            //act
            var result = new ChangeRxTask().CheckForDelegateProvider(userId, Guid.Empty, delegateId, Constants.UserCategory.POB_REGULAR,  rxUserMock, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1);

            //assert
            Assert.AreEqual(delegateId, result);
        }

        [TestMethod]
        public void should_return_providerId_from_sm_if_loggedIn_user_is_pob_and_provider_from_sm_is_not_pa_with_sup()
        {
            //arrange
            var userId = new Guid("AD8F7EB5-B1EF-4FD0-B944-937F4E75BBBD");
            var delegateId = new Guid("72F258BC-78DB-47F6-8DAF-08EC4AD2F398");
            var providerIdFromSm = new Guid("A2914187-9BAA-424F-888A-F0B663CD525C");

            var dt = new DataTable();
            dt.Columns.Add("UserType");
            var row = dt.NewRow();
            row["UserType"] = Constants.UserCategory.PROVIDER;

            var rxUserMock = MockRepository.GenerateMock<IRxUser>();
            rxUserMock.Stub(x => x.GetUserBasicInfo(new Guid(), ConnectionStringPointer.SHARED_DB)).IgnoreArguments().Return(row);
            
            //act
            var result = new ChangeRxTask().CheckForDelegateProvider(userId, providerIdFromSm, delegateId, Constants.UserCategory.POB_REGULAR,  rxUserMock, ConnectionStringPointer.AUDIT_ERXDB_SERVER_1);

            //assert
            Assert.AreEqual(providerIdFromSm, result);
        }
    }
}
