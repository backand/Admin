using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Security.Ssh
{
    public interface ISession
    {
        void Open();

        int Open(int attempts);

        bool IsOpen();

        void Close();

        void AllStop();
    }
}
