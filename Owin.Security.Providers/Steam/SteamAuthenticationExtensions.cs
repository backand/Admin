﻿using Microsoft.Owin;
using System;

namespace Owin.Security.Providers.Steam
{
    /// <summary>
    /// Extension methods for using <see cref="SteamAuthenticationMiddleware"/>
    /// </summary>
    public static class SteamAuthenticationExtensions
    {
        /// <summary>
        /// Authenticate users using Steam
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/> passed to the configuration method</param>
        /// <param name="options">Middleware configuration options</param>
        /// <returns>The updated <see cref="IAppBuilder"/></returns>
        public static IAppBuilder UseSteamAuthentication(this IAppBuilder app, SteamAuthenticationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException("app");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            app.Use(typeof(SteamAuthenticationMiddleware), app, options);
            return app;
        }

        /// <summary>
        /// Authenticate users using Steam
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/> passed to the configuration method</param>
        /// <param name="applicationKey">The steam application key</param>
        /// <returns>The updated <see cref="IAppBuilder"/></returns>
        public static IAppBuilder UseSteamAuthentication(this IAppBuilder app, string applicationKey)
        {
            return UseSteamAuthentication(app, new SteamAuthenticationOptions
            {
                ProviderDiscoveryUri = "http://steamcommunity.com/openid/",
                Caption = "Steam",
                AuthenticationType = "Steam",
                CallbackPath = new PathString("/signin-openidsteam"),
                ApplicationKey = applicationKey
            });
        }
    }
}
