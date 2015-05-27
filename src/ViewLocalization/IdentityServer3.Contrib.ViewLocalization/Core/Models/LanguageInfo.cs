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

using System.Globalization;

namespace IdentityServer3.Contrib.ViewLocalization
{
    public class LanguageInfo
    {
        public LanguageInfo(string isoLanguageName, string englishName = null, string displayName = null)
        {
            IsoLanguageName = isoLanguageName;
            try
            {
                var cultureInfo = CultureInfo.GetCultureInfo(isoLanguageName);
                if(englishName == null)
                    EnglishName = cultureInfo.EnglishName;
                if(displayName == null)
                    DisplayName = cultureInfo.NativeName;
            }
            catch (CultureNotFoundException) {}

            if (EnglishName == null)
                EnglishName = englishName ?? isoLanguageName;

            if (DisplayName == null)
                DisplayName = displayName ?? EnglishName;
        }

        public string IsoLanguageName { get; set; }
        public string EnglishName { get; set; }
        public string DisplayName { get; set; }
    }
}