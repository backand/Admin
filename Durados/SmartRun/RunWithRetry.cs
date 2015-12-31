using Microsoft.Practices.TransientFaultHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.SmartRun
{
    public static class RunWithRetry
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="count"></param>
        /// <param name="waitMilliSeconds"></param>
        public static void Run<T>(Action action, int count, int waitMilliSeconds) where T : Exception
        {
            var retryStrategy = new Incremental(count, TimeSpan.FromMilliseconds(waitMilliSeconds), TimeSpan.FromSeconds(0));

            // Define your retry policy using the retry strategy and the Azure storage
            // transient fault detection strategy.

            var retryPolicy =new RetryPolicy<SingleErrorRetryDetectionStrategy<T>>(retryStrategy);
            
            // Do some work that may result in a transient fault.
            retryPolicy.ExecuteAction(action);
        }


        public class SingleErrorRetryDetectionStrategy<T> : ITransientErrorDetectionStrategy  where T : Exception
        {
            public bool IsTransient(Exception ex)
            {
                return ex is T;
            }
        }

    }
}
