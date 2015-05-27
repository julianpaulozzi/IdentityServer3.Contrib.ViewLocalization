﻿/*
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
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using Thinktecture.IdentityServer.Core;

namespace IdentityServer3.Contrib.ViewLocalization
{
    internal static class StringExtensions
    {
        public static string EnsureTrailingSlash(this string url)
        {
            if (!url.EndsWith("/"))
            {
                return url + "/";
            }

            return url;
        }

        public static string RemoveTrailingSlash(this string url)
        {
            if (url != null && url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);
            }

            return url;
        }

        [DebuggerStepThrough]
        public static bool IsMissing(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        [DebuggerStepThrough]
        public static bool IsPresent(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
    }

    internal static class OwinExtensions
    {
        internal static void SetIdentityServerHost(this IDictionary<string, object> env, string value)
        {
            env[Constants.OwinEnvironment.IdentityServerHost] = value;
        }

        internal static void SetIdentityServerBasePath(this IDictionary<string, object> env, string value)
        {
            env[Constants.OwinEnvironment.IdentityServerBasePath] = value;
        }
    }

    public static class ContextExtensions
    {
        public static string GetFirstAcceptLanguageHeader(this HttpRequestMessage request)
        {
            string language = null;
            if (request.Headers.AcceptLanguage != null)
                language = request.Headers.AcceptLanguage
                    .OrderBy(p=> p.Quality).Select(p => p.Value).FirstOrDefault();

            return language;
        }

        public static IEnumerable<string> GetAcceptLanguagesHeader(this HttpRequestMessage request)
        {
            List<string> language = null;
            if (request.Headers.AcceptLanguage != null)
                language = request.Headers.AcceptLanguage
                    .OrderBy(p => p.Quality).Select(p => p.Value).ToList();

            return language ?? new List<string>();
        }
    }
}