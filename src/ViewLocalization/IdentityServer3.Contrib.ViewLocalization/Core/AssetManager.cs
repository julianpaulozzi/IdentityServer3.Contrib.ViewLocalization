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

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Thinktecture.IdentityServer.Core.Services.Default;

namespace IdentityServer3.Contrib.ViewLocalization
{
    internal class AssetManager
    {
        public const string PageAssetsNamespace = "IdentityServer3.Contrib.ViewLocalization.wwwroot";
        private const string PagesPrefix = PageAssetsNamespace + ".";
        private const string Layout = PagesPrefix + "layout.html";
        const string Welcome = PagesPrefix + "welcome.html";

        private static readonly ResourceCache cache = new ResourceCache();

        private const string PageNameTemplate = PagesPrefix + "{0}" + ".html";

        public static string LoadPage(string pageName)
        {
            pageName = string.Format(PageNameTemplate, pageName);
            return LoadResourceString(pageName);
        }

        public static string ApplyContentToLayout(string layout, string content)
        {
            return Format(layout, new {pageContent = content});
        }

        public static string LoadLayoutWithContent(string content)
        {
            var layout = LoadResourceString(Layout);
            return ApplyContentToLayout(layout, content);
        }

        public static string LoadLayoutWithPage(string pageName)
        {
            var pageContent = LoadPage(pageName);
            return LoadLayoutWithContent(pageContent);
        }

        internal static string LoadWelcomePage(string applicationPath, string version)
        {
            applicationPath = applicationPath.RemoveTrailingSlash();
            return LoadResourceString(Welcome, new
            {
                applicationPath,
                version
            });
        }

        private static string LoadResourceString(string name)
        {
            string value = cache.Read(name);
            if (value == null)
            {
                var assembly = typeof (AssetManager).Assembly;
                using (var sr = new StreamReader(assembly.GetManifestResourceStream(name)))
                {
                    value = sr.ReadToEnd();
                    cache.Write(name, value);
                }
            }
            return value;
        }

        private static string LoadResourceString(string name, object data)
        {
            string value = LoadResourceString(name);
            value = Format(value, data);
            return value;
        }

        private static string Format(string value, IDictionary<string, object> data)
        {
            foreach (var key in data.Keys)
            {
                var val = data[key];
                val = val ?? string.Empty;
                value = value.Replace("@{" + key + "}", val.ToString());
            }
            return value;
        }

        public static string Format(string value, object data)
        {
            return Format(value, Map(data));
        }

        private static IDictionary<string, object> Map(object values)
        {
            var dictionary = values as IDictionary<string, object>;

            if (dictionary == null)
            {
                dictionary = new Dictionary<string, object>();
                foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(values))
                {
                    dictionary.Add(descriptor.Name, descriptor.GetValue(values));
                }
            }

            return dictionary;
        }
    }
}