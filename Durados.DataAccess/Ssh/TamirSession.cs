using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Durados.Security.Ssh;

namespace Durados.DataAccess.Ssh
{
    public class TamirSession : ISession
    {
        private Tamir.SharpSsh.jsch.Session session = null;
        private ITunnel tunnel;
        private int remotePort;

        public TamirSession(ITunnel tunnel, int remotePort) 
        {
            this.tunnel = tunnel;
            this.remotePort = remotePort;

            
        }

        public void Open()
        {
            try
            {
                if (session != null && session.isConnected())
                    return;

                Tamir.SharpSsh.jsch.JSch jsch = new Tamir.SharpSsh.jsch.JSch();

                session = jsch.getSession(tunnel.User, tunnel.RemoteHost, tunnel.Port);
                session.setHost(tunnel.RemoteHost);
                session.setPassword(tunnel.Password);

                Tamir.SharpSsh.jsch.UserInfo ui = new MyUserInfo();
                session.setUserInfo(ui);

                
                session.connect();

                try
                {
                    //Set port forwarding on the opened session
                    session.setPortForwardingL(3306, "localhost", remotePort);
                }
                catch 
                {
                    if (IsOpen())
                        Close();
                }

                if (!session.isConnected())
                {
                    throw new DuradosException("Could not  open SSH tunnel");
                }
            }
            catch (Exception exception)
            {
                throw new DuradosException("Could not  open SSH tunnel", exception);
            }
        }

        public bool IsOpen()
        {
            return session.isConnected();
        }

        public void Close()
        {
            session.disconnect();
        }

        public void AllStop()
        {
        }
    }

    public class MyUserInfo : Tamir.SharpSsh.jsch.UserInfo
    {
        /// <summary>
        /// Holds the user password
        /// </summary>
        private String passwd;

        /// <summary>
        /// Returns the user password
        /// </summary>
        public String getPassword() { return passwd; }

        /// <summary>
        /// Prompt the user for a Yes/No input
        /// </summary>
        public bool promptYesNo(String str)
        {
            return true;
        }

        /// <summary>
        /// Returns the user passphrase (passwd for the private key file)
        /// </summary>
        public String getPassphrase() { return null; }

        /// <summary>
        /// Prompt the user for a passphrase (passwd for the private key file)
        /// </summary>
        public bool promptPassphrase(String message) { return true; }

        /// <summary>
        /// Prompt the user for a password
        /// </summary>\
        public bool promptPassword(String message) { return true; }

        /// <summary>
        /// Shows a message to the user
        /// </summary>
        public void showMessage(String message) { }
    }
}
