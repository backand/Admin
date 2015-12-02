//using System;
//using System.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace BackAnd.Web.Api.Test
//{
//    [TestClass]
//    public class SocketEmitTest
//    {
//        [TestMethod]
//        public void TestSendToAllFunctionSendAllAtModeParam()
//        {
//            var mySocket = new Backand.socket();
//            Backand.socket.SentMessagesMock = new System.Collections.Generic.List<object>();

//            mySocket.emitAll("event", "aaa");

//            var sentMessage = Backand.socket.SentMessagesMock.First();
            
//        }
//    }
//}
