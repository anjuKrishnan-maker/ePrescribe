using System;
using System.Threading.Tasks;
using Allscripts.ePrescribe.Shared.Logging;
using eRxWeb.AppCode;
using eRxWeb.Controller;
using eRxWeb.ServerModel.Request;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests.ContentLoadAsyncUtilTests
{
    [TestClass]
    public class CreateTask
    {
        [TestMethod]
        public void action_with_istatecontainer_test()
        {
            //arrange
            var payload = "";
            var expected = "SetCorrectly";
            var logger = MockRepository.GenerateMock<ILoggerEx>();

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty("SomeKey")).Return(expected);

            var asyncContext = new AsyncContext(sessionMock);

            //act
            Task task = new LoadAsyncUtil(asyncContext, logger).CreateTask(session => session.GetStringOrEmpty("SomeKey"), p => payload = p);

            Task.WaitAll(task);

            //assert
            Assert.AreEqual(expected, payload);
        }

        [TestMethod]
        public void action_with_asyncContext_test()
        {
            //arrange
            var payload = "";
            var expected = "SetCorrectly";
            var logger = MockRepository.GenerateMock<ILoggerEx>();

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();
            sessionMock.Stub(_ => _.GetStringOrEmpty("SomeKey")).Return(expected);

            var asyncContext = new AsyncContext(sessionMock);

            //act
            Task task = new LoadAsyncUtil(asyncContext, logger).CreateTask(ac => ac.Session.GetStringOrEmpty("SomeKey"), p => payload = p);

            Task.WaitAll(task);

            //assert
            Assert.AreEqual(expected, payload);
        }

        [TestMethod]
        public void action_with_no_params_test()
        {
            //arrange
            var payload = "";
            var expected = "SetCorrectly";
            var logger = MockRepository.GenerateMock<ILoggerEx>();

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();

            var asyncContext = new AsyncContext(sessionMock);

            //act
            Task task = new LoadAsyncUtil(asyncContext, logger).CreateTask(() => expected, p => payload = p);

            Task.WaitAll(task);

            //assert
            Assert.AreEqual(expected, payload);
        }

        [TestMethod]
        public void action_with_pageName_istatecontainer_test()
        {
            //arrange
            var payload = "";
            var expected = "SetCorrectly";
            var pageName = "SpecificPage";
            var logger = MockRepository.GenerateMock<ILoggerEx>();

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();

            var asyncContext = new AsyncContext(sessionMock)
            {
                PageName = pageName
            };

            Func<string, IStateContainer, string> action = (page, session) =>
            {
                if (page == "SpecificPage")
                {
                    return expected;
                }
                else
                {
                    Assert.Fail();
                    return "fail";
                }
            };

            //act
            Task task = new LoadAsyncUtil(asyncContext, logger).CreateTask(action, p => payload = p);

            Task.WaitAll(task);

            //assert
            Assert.AreEqual(expected, payload);
        }

        [TestMethod]
        public void action_with_pagename_test()
        {
            //arrange
            var payload = "";
            var expected = "SetCorrectly";
            var logger = MockRepository.GenerateMock<ILoggerEx>();

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();

            var asyncContext = new AsyncContext(sessionMock) { PageName = expected };

            //act
            Task task = new LoadAsyncUtil(asyncContext, logger).CreateTask(page => page, p => payload = p);

            Task.WaitAll(task);

            //assert
            Assert.AreEqual(expected, payload);
        }

        [TestMethod]
        public void action_medSelectedRequest_istatecontainer_test()
        {
            //arrange
            var payload = "";
            var expected = "SetCorrectly";
            var logger = MockRepository.GenerateMock<ILoggerEx>();

            var sessionMock = MockRepository.GenerateMock<IStateContainer>();

            var asyncContext = new AsyncContext(sessionMock);

            var medSelectedRequest = new MedicationSelectedRequest { DDI = expected };

            //act
            Task task = new LoadAsyncUtil(asyncContext, logger).CreateTask((medReq, session) => medReq.DDI, medSelectedRequest, p => payload = p);

            Task.WaitAll(task);

            //assert
            Assert.AreEqual(expected, payload);
        }
    }
}
