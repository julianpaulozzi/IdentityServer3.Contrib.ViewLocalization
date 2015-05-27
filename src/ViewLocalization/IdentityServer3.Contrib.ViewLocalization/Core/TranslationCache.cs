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

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;

namespace IdentityServer3.Contrib.ViewLocalization
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class TranslationCache
    {
        private readonly ConcurrentDictionary<string, IDictionary<string, string>> _cache = 
            new ConcurrentDictionary<string, IDictionary<string, string>>();

        public IDictionary<string, string> Read(string lang, string part)
        {
            var key = GetKey(lang, part);
            IDictionary<string, string> dictionary;
            return _cache.TryGetValue(key, out dictionary) ? dictionary : null;
        }

        public void Write(string lang, string part, IDictionary<string, string> value)
        {
            var key = GetKey(lang, part);
            _cache[key] = value;
        }

        private static string GetKey(string lang, string part)
        {
            if (string.IsNullOrEmpty(part))
                return lang;
            return string.Format("{0}|{1}", lang, part);
        }
    }
}