using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Durados.Web.Mvc.UI.Helpers;
using Durados;

namespace BackAnd.UnitTests
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class RestHelperKeysTests : BackandBaseTest
    {

        [TestInitialize]
        public void Init()
        {
            this.InitInner();
        }

        [TestMethod]
        [ExpectedException(typeof(DuradosException))]
        public void TestUndefinedAppThrowException()
        {
            var res = RestHelper.GetKeys("undefined");
        }

        [TestMethod]
        [ExpectedException(typeof(DuradosException))]
        public void TestNullAppReturnThrowException()
        {
            var res = RestHelper.GetKeys(null);
        }
        
    }
    
}
