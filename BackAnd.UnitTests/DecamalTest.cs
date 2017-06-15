using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Durados;

namespace BackAnd.UnitTests
{
    [TestClass]
    public class DecamalTest
    {
        public class Sample
        {
            public Sample(string actual, string expected)
            {
                Actual = actual;
                Expected = expected;
            }
            public string Actual { get; private set; }
            public string Expected { get; private set; }
        }
        [TestMethod]
        public void Test()
        {
            List<Sample> samples = new List<Sample>() { 
                new Sample("abcDefGh","Abc Def Gh"),
                new Sample("abc_def_gh","Abc Def Gh"),
                new Sample("abc-def-gh","Abc Def Gh")
            };

            foreach (var sample in samples)
            {
                Assert.AreEqual(sample.Expected, Decamal(sample.Actual));
            }
        }

        private string Decamal(string s)
        {
            return s.GetDecamal();
        }
    }
}

