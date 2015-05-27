/*
 * Copyright 2015 Julian Paulozzi - Paulozzi&Co.
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

using System;
using Owin;
using Thinktecture.IdentityServer.Core.Configuration;
using Thinktecture.IdentityServer.Core.Services;

namespace IdentityServer3.Contrib.ViewLocalization.Configuration
{
    public static class UseIdentityServerViewLocalizationExtension
    {
        public static IAppBuilder UseIdentityServerViewLocalization(this IAppBuilder app, 
            IdentityServerOptions identityServerOptions, 
            IdentityServerViewLocalizationOptions options)
        {
            if (identityServerOptions == null) throw new ArgumentNullException("identityServerOptions");
            if (options == null) throw new ArgumentNullException("options");

            var factory = identityServerOptions.Factory;
            if (factory == null) throw new ArgumentNullException("factory");
            
            app.ValidateOptions(identityServerOptions, options);

            OptionsState.Current.RegisterConfiguration(identityServerOptions, options);

            factory.ViewService = new Registration<IViewService>(typeof(LocaleViewServices));

            app.ConfigureIdentityServerBaseUrl(identityServerOptions.PublicOrigin);

            app.UseEmbeddedResourceFile(
                        "wwwroot/",
                        "/fonts",
                        "/content",
                        "/scripts"
                        );

            var httpConfig = WebApiConfig.Configure(identityServerOptions);
            app.UseWebApi(httpConfig);

            return app;
        }
    }
}