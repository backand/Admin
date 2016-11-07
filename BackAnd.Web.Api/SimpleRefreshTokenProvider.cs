using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BackAnd.Web.Api
{
    class SimpleRefreshTokenProvider : IAuthenticationTokenProvider 
    {
        private static ConcurrentDictionary<string, AuthenticationTicket> _refreshTokens = new ConcurrentDictionary<string, AuthenticationTicket>();

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            try
            {
                Durados.Web.Mvc.Map map =  Durados.Web.Mvc.Maps.Instance.GetMap();
                if (map.Database.UseRefreshToken || map.Equals(Durados.Web.Mvc.Maps.Instance.DuradosMap))
                {
                    //var guid = Guid.NewGuid().ToString();

                    //// maybe only create a handle the first time, then re-use for same client
                    //var refreshToken = System.Web.Helpers.Crypto.HashPassword(guid);
                    //_refreshTokens.TryAdd(refreshToken, context.Ticket);

                    // consider storing only the hash of the handle
                    string username = map.Database.GetCurrentUsername();

                    string refreshToken = Durados.Web.Mvc.UI.Helpers.RefreshToken.Get(map.Equals(Durados.Web.Mvc.Maps.Instance.DuradosMap) ? Durados.Web.Mvc.Maps.DuradosAppName : map.AppName, username);
                    if (refreshToken != null)
                    {
                        context.SetToken(refreshToken);
                    }
                }
            }
            catch { }
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            AuthenticationTicket ticket;
            if (_refreshTokens.TryRemove(context.Token, out ticket))
            {
                context.SetTicket(ticket);
            }
        }
    }
}
