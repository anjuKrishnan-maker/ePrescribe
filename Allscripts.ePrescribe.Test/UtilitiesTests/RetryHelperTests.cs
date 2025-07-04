using System;
using eRxWeb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Allscripts.ePrescribe.Test.UtilitiesTests
{
    public class Dummy
    {
        public string Name { get; set; }
        public bool Status { get; set; }
    }

    [TestClass]
    public class RetryHelperTests
    {
        [TestMethod]
        public void CheckRetryStrategyReturnSeries()
        {
            RetryStrategy retryStrategy = new RetryStrategy(3);
            var x = retryStrategy.GetTimeInterval(0);
            Assert.AreEqual(x, TimeSpan.FromSeconds(2));
            var y = retryStrategy.GetTimeInterval(1);
            Assert.AreEqual(y, TimeSpan.FromSeconds(6));
            var z = retryStrategy.GetTimeInterval(2);
            Assert.AreEqual(z, TimeSpan.FromSeconds(12));
        }

        [TestMethod]
        public void should_execute_retry_logic_until_max_attempts()
        {
            RetryStrategy retryStrategy = new RetryStrategy(2);
            var retryHelper = new RetryHelper(retryStrategy);
            int i = 0;
            Func<Dummy> logic = () =>
            {
                i++;
                return new Dummy { Name = "xxx" + i, Status = false };
            };

            Func<Dummy, bool> exitStrategy = (x) =>
             {
                 return false;
             };
            var response = retryHelper.Retry<Dummy>(logic, exitStrategy);
            Assert.IsTrue(Equals("xxx2", response.Name));
        }

        [TestMethod]
        public void should_exit_retry_logic_when_eitcriteriapass_and_not_reached_max_attempts()
        {
            RetryStrategy retryStrategy = new RetryStrategy(2);
            var retryHelper = new RetryHelper(retryStrategy);
            int i = 0;
            Func<Dummy> logic = () =>
            {
                i++;
                return new Dummy { Name = "xxx" + i, Status = false };
            };

            Func<Dummy, bool> exitStrategy = (x) =>
            {
                return true;
            };
            var response = retryHelper.Retry<Dummy>(logic, exitStrategy);
            Assert.IsTrue(Equals("xxx1", response.Name));
        }

        [TestMethod]
        public void should_return_what_we_have_when_any_error_on_retry_or_exit_criterion()
        {
            RetryStrategy retryStrategy = new RetryStrategy(2);
            var retryHelper = new RetryHelper(retryStrategy);
            int i = 0;
            Func<Dummy> logic = () =>
            {
                i++;
                return new Dummy { Name = "xxx" + i, Status = false };
            };

            Func<Dummy, bool> exitStrategy = (x) =>
            {
                throw new Exception("");
            };
            var response = retryHelper.Retry<Dummy>(logic, exitStrategy);
            Assert.IsTrue(Equals("xxx1", response.Name));
        }
    }
}
