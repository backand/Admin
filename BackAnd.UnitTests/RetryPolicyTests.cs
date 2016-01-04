using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Durados.SmartRun;

namespace BackAnd.UnitTests
{
    [TestClass]
    public class RetryPolicyTests
    {
        [TestMethod]
        public void TestTryToRunTwiceIfParamIsTwo()
        {
            int i = 0;
            RunWithRetry.Run<NullReferenceException>(
                () =>
                    {
                        i++;
                        if (i == 1)
                        {
                            throw new NullReferenceException();
                        }
                    }, 2, 0);

            Assert.AreEqual(2, i);
        }

        [TestMethod]
        public void TestSmartRunnerThrowExactlyThrowedException()
        {
            int i = 0;

            try
            {
            
            RunWithRetry.Run<NullReferenceException>(
                () =>
                {
                    i++;
                    throw new NullReferenceException(i.ToString());
                }, 2, 0);
            }
            catch(NullReferenceException e)
            {
                Assert.AreEqual(e.Message, "3");
            }

            Assert.AreEqual(3, i);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestSmartRunnerStrategyCatchInheritedException()
        {
            int i = 0;

            // SystemException is parent of NullReferenceException
            RunWithRetry.Run<SystemException>(
                () =>
                {
                    i++;
                    throw new NullReferenceException(i.ToString());
                }, 2, 0);

            Assert.AreEqual(2, i);
        }
    }
}
