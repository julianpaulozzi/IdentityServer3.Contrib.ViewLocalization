/*
 * Copyright 2014, 2015 Dominick Baier, Brock Allen
 *
 * Adapted by Julian Paulozzi
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.ComponentModel;
using System.Net.Http;
using System.Web.Http;
using IdentityServer3.Contrib.ViewLocalization.Configuration;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Logging;

namespace IdentityServer3.Contrib.ViewLocalization.Endpoints
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class WelcomeController : ApiController
    {
        private readonly static ILog Logger = LogProvider.GetCurrentClassLogger();

        private readonly IdentityServerOptions _options;

        public WelcomeController()
        {
            _options = OptionsState.Current.IdentityServerOptions;
        }

        // [Route(Constants.RoutePaths.Welcome, Name = "locale.welcome")]
        public IHttpActionResult Get()
        {
            Logger.Info("Welcome page requested");

            if (!_options.EnableWelcomePage)
            {
                Logger.Error("welcome page disabled, returning 404");
                return NotFound();
            }

            Logger.Info("Rendering welcome page");
            return new WelcomeActionResult(Request.GetOwinContext());
        }
    }
}