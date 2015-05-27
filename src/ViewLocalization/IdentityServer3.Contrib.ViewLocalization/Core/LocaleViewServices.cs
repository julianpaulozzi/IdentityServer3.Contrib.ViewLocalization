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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer3.Contrib.ViewLocalization.Configuration;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;
using Thinktecture.IdentityServer.Core.ViewModels;

namespace IdentityServer3.Contrib.ViewLocalization
{
    public class LocaleViewServices : IViewService
    {
        static readonly Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings()
        {
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
        };

        private readonly ViewServiceOptions _config;
        public LocaleViewServices()
        {
            // TODO: Replace with DependencyResolver
            _config = OptionsState.Current.Options.ViewServiceOptions;
        }

        public virtual async Task<Stream> Login(LoginViewModel model, SignInMessage message)
        {
            return await Render(model, "login");
        }

        public virtual Task<Stream> Logout(LogoutViewModel model)
        {
            return Render(model, "logout");
        }

        public virtual Task<Stream> LoggedOut(LoggedOutViewModel model)
        {
            return Render(model, "loggedOut");
        }

        public virtual Task<Stream> Consent(ConsentViewModel model)
        {
            return Render(model, "consent");
        }

        public Task<Stream> ClientPermissions(ClientPermissionsViewModel model)
        {
            return Render(model, "permissions");
        }

        public virtual Task<Stream> Error(ErrorViewModel model)
        {
            return Render(model, "error");
        }

        protected virtual Task<Stream> Render(CommonViewModel model, string page, string clientName = null)
        {
            var html = AssetManager.LoadLayoutWithPage(page);

            var data = BuildModel(model, page, _config.Stylesheets, _config.Scripts);
            html = AssetManager.Format(html, data);

            return Task.FromResult(ToStream(html));
        }

        object BuildModel(CommonViewModel model, string page, ICollection<string> stylesheets, ICollection<string> scripts)
        {
            if (model == null) throw new ArgumentNullException("model");
            if (stylesheets == null) throw new ArgumentNullException("stylesheets");
            if (scripts == null) throw new ArgumentNullException("scripts");

            var applicationPath = new Uri(model.SiteUrl).AbsolutePath;
            if (applicationPath.EndsWith("/")) applicationPath = applicationPath.Substring(0, applicationPath.Length - 1);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(model, Newtonsoft.Json.Formatting.None, settings);

            var additionalStylesheets = BuildTags("<link href='{0}' rel='stylesheet'>", applicationPath, stylesheets);
            var additionalScripts = BuildTags("<script src='{0}'></script>", applicationPath, scripts);

            return new
            {
                siteName = Microsoft.Security.Application.Encoder.HtmlEncode(model.SiteName),
                applicationPath,
                model = Microsoft.Security.Application.Encoder.HtmlEncode(json),
                page,
                stylesheets = additionalStylesheets,
                scripts = additionalScripts
            };
        }

        string BuildTags(string tagFormat, string basePath, IEnumerable<string> values)
        {
            if (values == null || !values.Any()) return string.Empty;

            var sb = new StringBuilder();
            foreach (var value in values)
            {
                var path = value;
                if (path.StartsWith("~/"))
                {
                    path = basePath + path.Substring(1);
                }
                sb.AppendFormat(tagFormat, path);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public static Stream ToStream(string s)
        {
            if (s == null) throw new ArgumentNullException("s");

            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            sw.Write(s);
            sw.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
    }
}