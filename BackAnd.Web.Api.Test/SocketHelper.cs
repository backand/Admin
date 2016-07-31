using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quobject.SocketIoClientDotNet.Client;

namespace BackAnd.Web.Api.Test
{
    public class SocketHelper
    {
        private Socket socket = IO.Socket("https://localhost:4000");

        public SocketHelper()
        {
        }

        public void On(string eventName, Action fn)
        {
            socket.On(eventName, fn);
        }

        public void On(string eventName, Action<object> fn)
        {
            socket.On(eventName, fn);
        }
    }
}
