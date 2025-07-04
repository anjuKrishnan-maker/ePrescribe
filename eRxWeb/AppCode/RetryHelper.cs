using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace eRxWeb
{
    public class RetryStrategy
    {
        private int _maxRetries { get; set; }

        private TimeSpan _interval { get; set; }

        public RetryStrategy(int maxRetries)
        {
            this._maxRetries = maxRetries;
        }

        public int GetMaxRetries()
        {
            return _maxRetries;
        }

        /// <summary>
        /// Series of 2,6,12 i.e n*(n+1) 
        /// IEnumerable<int> ranges = Enumerable.Range(1, 3).Select(x => x * (x+1));
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetTimeInterval(int attempt = 0)
        {
            IEnumerable<int> ranges = Enumerable.Range(1, _maxRetries).Select(x => x * (x + 1));
            var seconds = ranges.ElementAt(attempt);
            _interval = TimeSpan.FromSeconds(seconds);
            return _interval;
        }
    }

    public class RetryHelper
    {
        public RetryStrategy retryStrategy;

        public RetryHelper(RetryStrategy retryStrategy)
        {
            this.retryStrategy = retryStrategy;
        }
        public T Retry<T>(Func<T> retryLogic, Func<T, bool> exitStrategy)
        {
            int retries = 0;
            int maxRetries = retryStrategy.GetMaxRetries();
            TimeSpan interval = retryStrategy.GetTimeInterval(retries);
            var executeOutput = default(T);
            Task.Delay(interval).Wait();
            try
            {
                while (true)
                {
                    retries++;
                    executeOutput = retryLogic();
                    if (exitStrategy(executeOutput))
                        break;

                    if (retries == maxRetries)
                        break;
                    else
                    {
                        interval = retryStrategy.GetTimeInterval(retries);
                        Task.Delay(interval).Wait();
                    }

                }
            }
            catch (Exception ex)//Eat exception and return default
            {
                return executeOutput;
            }
            return executeOutput;
        }
    }
}
