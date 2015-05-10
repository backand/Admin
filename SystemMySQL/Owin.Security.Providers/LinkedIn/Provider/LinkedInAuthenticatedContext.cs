// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Security.Claims;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;
using Newtonsoft.Json.Linq;

namespace Owin.Security.Providers.LinkedIn
{
    /// <summary>
    /// Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/>.
    /// </summary>
    public class LinkedInAuthenticatedContext : BaseContext
    {
        /// <summary>
        /// Initializes a <see cref="LinkedInAuthenticatedContext"/>
        /// </summary>
        /// <param name="context">The OWIN environment</param>
        /// <param name="user">The JSON-serialized user</param>
        /// <param name="accessToken">LinkedIn Access token</param>
        /// <param name="expires">Seconds until expiration</param>
        public LinkedInAuthenticatedContext(IOwinContext context, JObject user, string accessToken, string expires)
            : base(context)
        {
            User = user;
            AccessToken = accessToken;

            int expiresValue;
            if (Int32.TryParse(expires, NumberStyles.Integer, CultureInfo.InvariantCulture, out expiresValue))
            {
                ExpiresIn = TimeSpan.FromSeconds(expiresValue);
            }

            Id = TryGetValue(user, "id");
            Name = TryGetValue(user, "formattedName");
            Link = TryGetValue(user, "publicProfileUrl");
            UserName = TryGetValue(user, "formattedName").Replace(" ", "");
            Email = TryGetValue(user, "emailAddress");
        }

        /// <summary>
        /// Gets the JSON-serialized user
        /// </summary>
        /// <remarks>
        /// Contains the LinkedIn user obtained from the endpoint https://api.linkedin.com/v1/people/~
        /// </remarks>
        public JObject User { get; private set; }

        /// <summary>
        /// Gets the LinkedIn access token
        /// </summary>
        public string AccessToken { get; private set; }

        /// <summary>
        /// Gets the LinkedIn access token expiration time
        /// </summary>
        public TimeSpan? ExpiresIn { get; set; }

        /// <summary>
        /// Gets the LinkedIn user ID
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the user's name
        /// </summary>
        public string Name { get; private set; }

        public string Link { get; private set; }

        /// <summary>
        /// Gets the LinkedIn username
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Gets the LinkedIn email
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// Gets the <see cref="ClaimsIdentity"/> representing the user
        /// </summary>
        public ClaimsIdentity Identity { get; set; }

        /// <summary>
        /// Gets or sets a property bag for common authentication properties
        /// </summary>
        public AuthenticationProperties Properties { get; set; }

        private static string TryGetValue(JObject user, string propertyName)
        {
            JToken value;
            return user.TryGetValue(propertyName, out value) ? value.ToString() : null;
        }
    }
}
