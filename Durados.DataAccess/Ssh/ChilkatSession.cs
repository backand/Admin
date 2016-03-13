using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Durados.Security.Ssh;

namespace Durados.DataAccess.Ssh
{
    public class ChilkatSession : ISession
    {
        private ITunnel tunnel;
        private int remotePort;
        private int localPort;
        Chilkat.SshTunnel session = null;

        public ChilkatSession(ITunnel tunnel, int remotePort, int localPort)
        {
            this.tunnel = tunnel;
            this.remotePort = remotePort;
            this.localPort = localPort;


        }

        public void Open()
        {
            try
            {
                if (session != null && IsOpen())
                    return;

                bool success;

                session = new Chilkat.SshTunnel();

                success = session.UnlockComponent("ITAYHESSH_d674QVQunRnj");
                if (success != true)
                {
                    throw new DuradosException(session.LastErrorText);

                }

                //  The destination host/port is the database server.
                //  The DestHostname may be the domain name or
                //  IP address (in dotted decimal notation) of the database
                //  server.
                session.DestPort = remotePort;
                session.DestHostname = "localhost";

                //  Provide information about the location of the SSH server,
                //  and the authentication to be used with it. This is the
                //  login information for the SSH server (not the database server).
                session.SshHostname = tunnel.RemoteHost;
                session.SshPort = tunnel.Port;
                session.SshLogin = tunnel.User;
                if (string.IsNullOrEmpty(tunnel.Password))
                {
                    Chilkat.SshKey sshKey = new Chilkat.SshKey();
                    //string sss = sshKey.LoadText(@"G:\Dev\Relly - new\Duradus.Web.Mvc.App\Modubiz2012.pem");
                    string keyText = tunnel.PrivateKey;
                    success = sshKey.FromOpenSshPrivateKey(keyText);
                    //string fff = sshKey.ToOpenSshPrivateKey(false);
                    success = session.SetSshAuthenticationKey(sshKey);
                    if (success)
                    {
                    }
                }
                else
                {
                    session.SshPassword = tunnel.Password;
                }

                //  Start accepting connections in a background thread.
                //  The SSH tunnels are autonomously run in a background
                //  thread.  There is one background thread for accepting
                //  connections, and another for managing the tunnel pool.
                int listenPort;
                listenPort = localPort;
                success = session.BeginAccepting(listenPort);
                if (success != true)
                {
                    throw new DuradosException(session.LastErrorText);

                }

                //  At this point you may connect to the database server through
                //  the SSH tunnel.  Your database connection string would
                //  use "localhost" for the hostname and 3316 for the port.
                //  We're not going to show the database coding here,
                //  because it can vary depending on the API you're using
                //  (ADO, ODBC, OLE DB, etc. )

            }
            catch (Exception exception)
            {
                throw new DuradosException("Could not  open SSH tunnel", exception);
            }
        }

        public int Open(int attempts)
        {
            int i = 0;
            bool success = false;
            for (i = 0; i < attempts; i++)
            {
                success = TryOpen();
                if (success)
                    break;
            }
            System.Threading.Thread.Sleep(200);
            if (!success)
                Open();

            return i;
        }

        private bool TryOpen()
        {
            try
            {
                Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsOpen()
        {
            return session != null && session.IsAccepting;
        }

        public void Close()
        {
            bool success = session.StopAccepting();
            if (success != true)
            {
                throw new DuradosException(session.LastErrorText);
            }

        }

        public void AllStop()
        {
            //  If any background tunnels are still in existence (and managed
            //  by a single SSH tunnel pool background thread), stop them...
            int maxWaitMs;
            maxWaitMs = 1000;

            session.StopAllTunnels(maxWaitMs);

        }
    }
}