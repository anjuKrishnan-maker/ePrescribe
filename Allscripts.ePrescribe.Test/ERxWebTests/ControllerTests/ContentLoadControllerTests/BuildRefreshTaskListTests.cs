using System;
using eRxWeb.AppCode;
using eRxWeb.AppCode.Interfaces;
using eRxWeb.Controller;
using eRxWeb.ServerModel;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.ControllerTests.ContentLoadControllerTests
{
    [TestClass]
    public class BuildRefreshTaskListTests
    {
        [TestMethod]
        public void should_create_task_list()
        {
            var asyncUtil = MockRepository.GenerateMock<ILoadAsyncUtil>();

            var result = ContentLoadController.BuildRefreshTaskList(asyncUtil);

            asyncUtil.AssertWasCalled(_ => _.CreateTask(Arg<Func<AsyncContext, RightPanelPayload>>
                                                            .Matches(f => f.Method.Name == "RetrievePayload"),
                                                        Arg<Action<RightPanelPayload>>.Is.Anything));

            asyncUtil.AssertWasCalled(_ => _.CreateTask(Arg<Func<string, string>>
                                                            .Matches(f => f.Method.Name == "GetErxILearnPageUrl"),
                                                        Arg<Action<string>>.Is.Anything));

            //asyncUtil.AssertWasCalled(_ => _.CreateTask(Arg<Func<IStateContainer, string>>
            //                                                .Matches(f => f.Method.Name == "GetILearnVideoNotification"),
            //                                            Arg<Action<string>>.Is.Anything));

            Assert.AreEqual(2, result.Item1.Count);
        }
    }
}
