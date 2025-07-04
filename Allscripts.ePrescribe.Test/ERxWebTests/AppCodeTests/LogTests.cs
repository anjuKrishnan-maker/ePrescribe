using System;
using Allscripts.ePrescribe.DatabaseSelector;
using Allscripts.ePrescribe.Shared.Logging;
using Allscripts.Impact.Interfaces;
using eRxWeb.AppCode;
using eRxWeb.State;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using Veradigm.AdAgent.Business.Interfaces;

namespace Allscripts.ePrescribe.Test.ERxWebTests.AppCodeTests
{
    [TestClass]
    public class LogTests
    {
        [TestMethod]
        public void should_call_debug()
        {
            //arrange
            var logger = MockRepository.GenerateMock<ILoggerEx>();
            var session = MockRepository.GenerateMock<IStateContainer>();
            var audit = MockRepository.GenerateMock<IAudit>();

            //act
            new AdAgentLogger(session, audit, logger).Log(LogLevel.Debug, "message");

            //assert
            logger.AssertWasCalled(_ => _.Debug("message"));
        }

        [TestMethod]
        public void should_call_error_and_log_exception()
        {
            //arrange
            var logger = MockRepository.GenerateMock<ILoggerEx>();
            var audit = MockRepository.GenerateMock<IAudit>();
            var session = MockRepository.GenerateMock<IStateContainer>();

            //act
            new AdAgentLogger(session, audit, logger).Log(LogLevel.Error, "message");

            //assert
            logger.AssertWasCalled(_ => _.Error("message"));
            audit.AssertWasCalled(_ => _.AddException(Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Equal("message"),
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<ConnectionStringPointer>.Is.Anything));
        }

        [TestMethod]
        public void should_call_Info()
        {
            //arrange
            var logger = MockRepository.GenerateMock<ILoggerEx>();
            var session = MockRepository.GenerateMock<IStateContainer>();
            var audit = MockRepository.GenerateMock<IAudit>();

            //act
            new AdAgentLogger(session, audit, logger).Log(LogLevel.Info, "message");

            //assert
            logger.AssertWasCalled(_ => _.Info("message"));
        }

        [TestMethod]
        public void should_call_Trace()
        {
            //arrange
            var logger = MockRepository.GenerateMock<ILoggerEx>();
            var session = MockRepository.GenerateMock<IStateContainer>();
            var audit = MockRepository.GenerateMock<IAudit>();

            //act
            new AdAgentLogger(session, audit, logger).Log(LogLevel.Trace, "message");

            //assert
            logger.AssertWasCalled(_ => _.Trace("message"));
        }

        [TestMethod]
        public void should_call_Warn()
        {
            //arrange
            var logger = MockRepository.GenerateMock<ILoggerEx>();
            var session = MockRepository.GenerateMock<IStateContainer>();
            var audit = MockRepository.GenerateMock<IAudit>();

            //act
            new AdAgentLogger(session, audit, logger).Log(LogLevel.Warning, "message");

            //assert
            logger.AssertWasCalled(_ => _.Warn("message"));
        }
    }
}
