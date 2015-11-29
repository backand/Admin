using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackAnd.Web.Api.Test
{
    public class SimpleUtil
    {
        public int current = 0;
    }


    public static class SimpleExtension
    {
        public static SimpleUtil Add(this SimpleUtil util, int num)
        {
            util.current += num;
            return util;
        }

        public static SimpleUtil Remove(this SimpleUtil util, int num)
        {
            util.current -= num;
            return util;
        }

        public static SimpleUtil Multiply(this SimpleUtil util, int num)
        {
            util.current *= num;
            return util;
        }


    }


    [TestClass]
    public class MyTestClass
    {
        [TestMethod]
        public void MyTestMethod()
        {
            var res = new SimpleUtil()
             .Add(1)
            .Remove(2)
            .Multiply(5);

            Assert.AreEqual(res.current, -5);
            
        }
    }

}
