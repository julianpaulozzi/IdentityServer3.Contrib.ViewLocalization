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
    internal class LanguagesCollection : IEnumerable<LanguageInfo>
    {
        private readonly List<LanguageInfo> _list = new List<LanguageInfo>();

        public void Add(string isoLanguageName, string englishName = null, string displayName = null)
        {
            _list.Add(new LanguageInfo(isoLanguageName, englishName, displayName));
        }
        public bool Remove(string isoLanguageName)
        {
            var item = _list.FirstOrDefault(p=> p.IsoLanguageName.Equals(isoLanguageName, StringComparison.OrdinalIgnoreCase));
            return item != null && _list.Remove(item);
        }
        public bool HasLanguage(string isoLanguageName)
        {
            return !string.IsNullOrEmpty(isoLanguageName) && _list.Any(p => p.IsoLanguageName.Equals(isoLanguageName, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerator<LanguageInfo> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }
    }
}