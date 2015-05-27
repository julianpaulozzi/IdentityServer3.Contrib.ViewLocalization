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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Resources;
using IdentityServer3.Contrib.ViewLocalization.Configuration;
using IdentityServer3.Contrib.ViewLocalization.Resources;
using IdentityServer3.Contrib.ViewLocalization.Resources.Permissions;
using Newtonsoft.Json.Linq;
using Thinktecture.IdentityServer.Core;

namespace IdentityServer3.Contrib.ViewLocalization
{
    public class TranslationManager
    {
        public const string Commun_Part = "commun";
        public const string Welcome_Part = "welcome";
        public const string Login_Part = "login";
        public const string Logout_Part = "logout";
        public const string Loggedout_Part = "loggedout";
        public const string Consent_Part = "consent";
        public const string Permissions_Part = "permissions";
        public const string Error_Part = "error";
        public const string Scopes_Part = "scopes";

        public const string Scopes_RosourcePrefix = "Scope_";
        public const string ScopeDisplayNameSuffix = "_DisplayName";
        public const string ScopeDescriptionSuffix = "_Description";

        private static readonly TranslationCache _cache = new TranslationCache();
        public static IEnumerable<LanguageInfo> AvailableLanguages
        {
            get { return VLConstants.Localization.AvailableLanguages; }
        }

        public static bool HasLanguage(string isoLanguageName)
        {
            return VLConstants.Localization.AvailableLanguages.HasLanguage(isoLanguageName);
        }

        public static string NegotiatePreferredLanguage(IEnumerable<string> requestLanguages)
        {
            var result = OptionsState.Current.Options.LocalizationServiceOptions.DefaultLanguage;
            foreach (var lang in requestLanguages)
            {
                var hasLanguage = HasLanguage(lang);
                if (hasLanguage)
                {
                    result = lang;
                    break;
                }
            }
            return result;
        }

        public static JObject GetBundleJson(HttpRequestMessage request, string part)
        {
            var acceptLanguages = request.GetAcceptLanguagesHeader();
            var preferredLanguage = NegotiatePreferredLanguage(acceptLanguages);
            // Set defaults and avoid flash of untranslated content
            var allowUserChangeLanguage = OptionsState.Current.Options.LocalizationServiceOptions.AllowUserChangeLanguage;
            var availableLanguages = JToken.FromObject(AvailableLanguages);
            var resourceJson = GetResourceJson(request, preferredLanguage, part);
            var result = new JObject
            {
                { "part", part },
                { "loaderUrl", "locale" },
                { "allowChangeLanguage", allowUserChangeLanguage },
                { "preferredLanguage", preferredLanguage},
                { "acceptLanguages", JToken.FromObject(acceptLanguages)},
                { "availableLanguages", availableLanguages },
                { "values", JToken.FromObject(resourceJson) },
            };

            return result;
        }

        public static IDictionary<string, string> GetResourceJson(HttpRequestMessage request, string lang, string part)
        {
            var cacheTranslations = OptionsState.Current.Options.LocalizationServiceOptions.CacheTranslations;

            var values = GetCommunValues(lang, cacheTranslations);

            FillPartValues(values, lang, part, cacheTranslations);

            if (part.Equals(Consent_Part))
            {
                var requestedScopes = GetQueryStringScopesNames(request);
                FillScopesValue(values, lang, requestedScopes, cacheTranslations);
            }

            return values;
        }

        public static IEnumerable<string> GetQueryStringScopesNames(HttpRequestMessage request)
        {
            var values = request.Headers.Referrer.ParseQueryString().GetValues(Constants.AuthorizeRequest.Scope);
            if (values != null && values.Any())
            {
                var scopes = values[0].Split(null);
                return scopes;
            }

            return null;
        }
        
        public static void FillScopesValue(IDictionary<string, string> commun, string lang, IEnumerable<string> requestedScopes, bool cacheTranslations)
        {
            // TODO: Fill from config factory
            // Ensure scopes part resources
            // Dont cache this, variable with the query string
            var scopeValues = GetPartResources(lang, Scopes_Part);
            var scopeNames = requestedScopes as string[] ?? requestedScopes.ToArray();
            if (scopeValues != null)
            {
                foreach (var scopeValue in scopeValues)
                {
                    var scopeName = GetScopeNameFromResource(scopeValue.Key);
                    if (!scopeNames.Contains(scopeName))
                        continue;

                    var key = Scopes_RosourcePrefix + scopeValue.Key;
                    commun.Add(key, scopeValue.Value);
                }
            }

            if (OptionsState.Current.Options.LocalizationServiceOptions.ScopeLocalizationRequest != null)
            {
                var resourceRequestInfo = new ResourceRequestInfo
                {
                    Part = Scopes_Part,
                    Lang = lang,
                    CacheTranslations = cacheTranslations,
                    RequestedScopes = scopeNames
                };

                var userScopesLocalizations = OptionsState.Current.Options.LocalizationServiceOptions.ScopeLocalizationRequest(resourceRequestInfo);
                if(userScopesLocalizations == null)
                    return;

                var scopeTranslation = userScopesLocalizations.GetTranslation(lang, scopeNames);
                if (scopeTranslation != null)
                {
                    foreach (var locale in scopeTranslation)
                    {
                        if (!string.IsNullOrWhiteSpace(locale.DisplayName))
                        {
                            var displayNameKey = Scopes_RosourcePrefix + locale.ScopeName + ScopeDisplayNameSuffix;
                            commun[displayNameKey] = locale.DisplayName;
                        }
                            
                        if (!string.IsNullOrWhiteSpace(locale.Description))
                        {
                            var descriptionKey = Scopes_RosourcePrefix + locale.ScopeName + ScopeDescriptionSuffix;
                            commun[descriptionKey] = locale.Description;
                        }
                        
                    }
                }
            }
        }

        private static string GetScopeNameFromResource(string resourceValue)
        {
            var lastIndexOf = resourceValue.LastIndexOf("_", StringComparison.Ordinal);
            return lastIndexOf == -1 ? resourceValue : resourceValue.Substring(0, lastIndexOf);
        }

        public static void FillPartValues(IDictionary<string, string> commun, string lang, string part, bool cacheTranslations)
        {
            var cachedPart = cacheTranslations ? _cache.Read(lang, part) : null;
            var result = cachedPart ?? new Dictionary<string, string>();
            if (cachedPart == null)
            {
                // Ensure part resources
                var partResources = GetPartResources(lang, part);
                if (partResources != null)
                {
                    foreach (var partValue in partResources)
                    {
                        result.Add(partValue.Key, partValue.Value);
                    }
                }

                if (cacheTranslations)
                    _cache.Write(lang, part, partResources);
            }

            foreach (var partValue in result)
            {
                commun.Add(partValue.Key, partValue.Value);
            }
        }

        private static IDictionary<string, string> GetCommunValues(string lang, bool cacheTranslations)
        {
            var cachedPart = cacheTranslations ? _cache.Read(lang, Commun_Part) : null;
            var result = cachedPart ?? new Dictionary<string, string>();

            if (cachedPart == null)
            {
                var commumResources = GetPartResources(lang, Commun_Part);
                // Ensure commun resources
                if (commumResources != null)
                {
                    foreach (var communValue in commumResources)
                    {
                        result.Add(communValue.Key, communValue.Value);
                    }
                }

                if (cacheTranslations)
                    _cache.Write(lang, Commun_Part, commumResources);
            }

            return new Dictionary<string, string>(result);
        }

        private static IDictionary<string, string> GetPartResources(string lang, string part, bool tryParents = true)
        {
            var cultureInfo = new CultureInfo(lang);
            return GetPartResources(cultureInfo, part, tryParents);
        }

        private static IDictionary<string, string> GetPartResources(CultureInfo cultureInfo, string part, bool tryParents = true)
        {
            part = part.ToLower();
            ResourceSet resul = null;
            switch (part)
            {
                case Commun_Part:
                    resul = Commun.ResourceManager.GetResourceSet(cultureInfo, true, tryParents);
                    break;
                case Welcome_Part:
                    resul = Welcome.ResourceManager.GetResourceSet(cultureInfo, true, tryParents);
                    break;
                case Login_Part:
                    resul = Login.ResourceManager.GetResourceSet(cultureInfo, true, tryParents);
                    break;
                case Logout_Part:
                    resul = Logout.ResourceManager.GetResourceSet(cultureInfo, true, tryParents);
                    break;
                case Loggedout_Part:
                    resul = LoggedOut.ResourceManager.GetResourceSet(cultureInfo, true, tryParents);
                    break;
                case Consent_Part:
                    resul = Consent.ResourceManager.GetResourceSet(cultureInfo, true, tryParents);
                    break;
                case Permissions_Part:
                    resul = Permissions.ResourceManager.GetResourceSet(cultureInfo, true, tryParents);
                    break;
                case Error_Part:
                    resul = Error.ResourceManager.GetResourceSet(cultureInfo, true, tryParents);
                    break;
                case Scopes_Part:
                    resul = Scopes.ResourceManager.GetResourceSet(cultureInfo, true, tryParents);
                    break;
            }

            return resul != null ? resul.Cast<DictionaryEntry>().ToDictionary(r => r.Key.ToString(), r => r.Value.ToString()) : null;
        }
    }
}