using System;
using System.Collections.Generic;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.Controller;
using eRxWeb.ServerModel;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ControllerTests.ContentLoadControllerTests
{
    [TestClass]
    public class BuildInitialContentTaskListTests
    {
        [TestMethod]
        public void should_create_task_list()
        {
            var asyncUtil = MockRepository.GenerateMock<ILoadAsyncUtil>();
            var session = MockRepository.GenerateMock<IStateContainer>();
            asyncUtil.Stub(_ => _.AsyncContext).Return(new eRxWeb.AppCode.AsyncContext(session) { PageName = "selectpatient" });

            var result = ContentLoadController.BuildInitialContentTaskList(asyncUtil);

            asyncUtil.AssertWasCalled(_ => _.CreateTask(Arg<Func<IStateContainer, SelectPatientStartupParameters>>
                                                            .Matches(f => f.Method.Name == "GetStartupParameters"),
                                                        Arg<Action<SelectPatientStartupParameters>>.Is.Anything));

            asyncUtil.AssertWasCalled(_ => _.CreateTask(Arg<Func<string, IStateContainer, ContentRefreshPayload>>
                                                            .Matches(f => f.Method.Name == "RetrieveRefreshPayload"),
                                                        Arg<Action<ContentRefreshPayload>>.Is.Anything));

            asyncUtil.AssertWasCalled(_ => _.CreateTask(Arg<Func<IStateContainer, UserPreferenceModel>>
                                                            .Matches(f => f.Method.Name == "GetUserPreference"),
                                                        Arg<Action<UserPreferenceModel>>.Is.Anything));

            asyncUtil.AssertWasCalled(_ => _.CreateTask(Arg<Func<IStateContainer, SiteInfo>>
                                                            .Matches(f => f.Method.Name == "GetSiteInfo"),
                                                        Arg<Action<SiteInfo>>.Is.Anything));

            asyncUtil.AssertWasCalled(_ => _.CreateTask(Arg<Func<CommonUiIInitialPayload>>
                                                            .Matches(f => f.Method.Name == "RetrieveUrls"),
                                                        Arg<Action<CommonUiIInitialPayload>>.Is.Anything));

            asyncUtil.AssertWasCalled(_ => _.CreateTask(Arg<Func<IStateContainer, string>>
                                                            .Matches(f => f.Method.Name == "SessionAppVersion"),
                                                        Arg<Action<string>>.Is.Anything));

            asyncUtil.AssertWasCalled(_ => _.CreateTask(Arg<Func<IStateContainer, List<MenuItem>>>
                                                            .Matches(f => f.Method.Name == "PopulateMenuItmes"),
                                                        Arg<Action<List<MenuItem>>>.Is.Anything));

            asyncUtil.AssertWasCalled(_ => _.CreateTask(Arg<Func<IStateContainer, int>>
                                                            .Matches(f => f.Method.Name == "TimeRemaining"),
                                                        Arg<Action<int>>.Is.Anything));

            asyncUtil.AssertWasCalled(_ => _.CreateTask(Arg<Func<IStateContainer, MedicationSearchHelp>>
                                                            .Matches(f => f.Method.Name == "getSerachUrl"),
                                                        Arg<Action<MedicationSearchHelp>>.Is.Anything));

            Assert.AreEqual(9, result.Item1.Count);
        }

        [TestMethod]
        public void should_not_call_createTask_for_selectPatient()
        {
            var asyncUtil = MockRepository.GenerateMock<ILoadAsyncUtil>();
            var session = MockRepository.GenerateMock<IStateContainer>();
            asyncUtil.Stub(_ => _.AsyncContext).Return(new eRxWeb.AppCode.AsyncContext(session) { PageName = "reviewhistory" });


            var result = ContentLoadController.BuildInitialContentTaskList(asyncUtil);

            asyncUtil.AssertWasNotCalled(_ => _.CreateTask(Arg<Func<IStateContainer, SelectPatientStartupParameters>>
                                                            .Matches(f => f.Method.Name == "GetStartupParameters"),
                                                        Arg<Action<SelectPatientStartupParameters>>.Is.Anything));


            Assert.AreEqual(8, result.Item1.Count);
        }
    }
}