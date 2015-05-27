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
using System.Linq;

namespace IdentityServer3.Contrib.ViewLocalization
{
    public class ScopeLocalizationCollection : IEnumerable<ScopeLocalization>
    {
        private readonly List<ScopeLocalization> _list = new List<ScopeLocalization>();

        private readonly Dictionary<string, Tuple<string, string>> _dictionary = 
            new Dictionary<string, Tuple<string, string>>();

        public void Add(string isoLanguageName, ScopeTranslations translations)
        {
            if (string.IsNullOrWhiteSpace(isoLanguageName)) throw new ArgumentNullException("isoLanguageName");
            if(HasLanguage(isoLanguageName)) throw new ArgumentException("An element with the same language already exists.");
            
            _list.Add(new ScopeLocalization(isoLanguageName, translations));
        }

        public bool HasLanguage(string isoLanguageName)
        {
            return !string.IsNullOrEmpty(isoLanguageName) && _list.Any(p => p.IsoLanguageName.Equals(isoLanguageName, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<ScopeTranslation> GetTranslation(string isoLanguageName, IEnumerable<string> scopeNames)
        {
            var locale = _list.FirstOrDefault(p => p.IsoLanguageName.Equals(isoLanguageName, StringComparison.OrdinalIgnoreCase));
            return locale != null ? locale.GetScopes(scopeNames) : null;
        }

        public ScopeTranslation GetTranslation(string isoLanguageName, string scopeName)
        {
            var locale = _list.FirstOrDefault(p=> p.IsoLanguageName.Equals(isoLanguageName, StringComparison.OrdinalIgnoreCase));
            return locale != null ? locale.GetScope(scopeName) : null;
        }

        public IEnumerator<ScopeLocalization> GetEnumerator()
        {
            return _list.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }
    }
}