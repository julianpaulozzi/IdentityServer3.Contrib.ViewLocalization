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

using System.Web.Http;
using IdentityServer3.Contrib.ViewLocalization.Configuration;

namespace IdentityServer3.Contrib.ViewLocalization.Endpoints
{
    public class LocalizationController : ApiController
    {
        [HttpGet]
        [AllowAnonymous]
        [Route(VLConstants.RoutePaths.Locale)]
        [Route(VLConstants.RoutePaths.LocaleWithPart)]
        public IHttpActionResult Get(string lang, string part = "")
        {
            if (string.IsNullOrEmpty(part))
                part = VLConstants.LocalizationParts.Welcome_Part;

            var resourceJson = TranslationManager.GetResourceJson(Request, lang, part);
            return Ok(resourceJson);
        }
    }
}