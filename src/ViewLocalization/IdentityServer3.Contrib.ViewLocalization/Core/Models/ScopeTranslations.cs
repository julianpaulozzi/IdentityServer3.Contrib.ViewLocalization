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
    public class ScopeTranslations : IEnumerable<ScopeTranslation>
    {
        private readonly List<ScopeTranslation> _list = new List<ScopeTranslation>();

        public void Add(string scopeName, string displayName, string description = "")
        {
            if (string.IsNullOrWhiteSpace(scopeName)) throw new ArgumentNullException("scopeName");
            
            _list.Add(new ScopeTranslation(scopeName, displayName, description));
        }

        public bool Remove(string scopeName)
        {
            var item = _list.FirstOrDefault(p => p.ScopeName.Equals(scopeName, StringComparison.OrdinalIgnoreCase));
            return item != null && _list.Remove(item);
        }
        public IEnumerable<ScopeTranslation> GetScopes(IEnumerable<string> scopeNames)
        {
            return _list.Where(p => scopeNames.Contains(p.ScopeName));
        }
        public ScopeTranslation GetScope(string scopeName)
        {
            return _list.FirstOrDefault(p => p.ScopeName.Equals(scopeName, StringComparison.OrdinalIgnoreCase));
        }
        public bool HasScope(string scopeName)
        {
            return !string.IsNullOrEmpty(scopeName) && _list.Any(p => p.ScopeName.Equals(scopeName, StringComparison.OrdinalIgnoreCase));
        }
        public IEnumerator<ScopeTranslation> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }
    }
}