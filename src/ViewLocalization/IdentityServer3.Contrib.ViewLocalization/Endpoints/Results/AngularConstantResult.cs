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

using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace IdentityServer3.Contrib.ViewLocalization.Endpoints
{
    public class AngularConstantResult : IHttpActionResult
    {
        readonly HttpRequestMessage _request;
        readonly AngularConstantInfo _angularConstantInfo;
        public AngularConstantResult(HttpRequestMessage request, AngularConstantInfo angularConstantInfo)
        {
            _request = request;
            _angularConstantInfo = angularConstantInfo;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(GetResponseMessage(_request, _angularConstantInfo));
        }

        public static HttpResponseMessage GetResponseMessage(HttpRequestMessage request, AngularConstantInfo angularConstantInfo)
        {
            var jsonData = CamelCaseJson(angularConstantInfo.Data);
            var script = GetScriptBoby(angularConstantInfo.ModuleName, angularConstantInfo.ConstantName, jsonData);
            return new HttpResponseMessage
            {
                Content = new StringContent(script, Encoding.UTF8, "text/javascript")
            };
        }

        private static readonly JsonSerializerSettings _jsonSerializerSettings =
            new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

        private static string CamelCaseJson(object value)
        {
            Formatting format = Formatting.None;
#if DEBUG
            format = Formatting.Indented;
#endif
            return JsonConvert.SerializeObject(value, format, _jsonSerializerSettings);
        }

        private static string GetScriptBoby(string moduleName, string constantName, string data)
        {
            const string angularConstant = "var app = angular.module('{0}');\n app.constant('{1}', angular.fromJson({2}));";

            var angularConstantValue = string.Format(angularConstant, moduleName, constantName, data);
            return "(function() {\n'use strict';\n" + angularConstantValue + "\n})();";
        }
    }

    public class AngularConstantInfo
    {
        public string ModuleName { get; set; }
        public string ConstantName { get; set; }
        public object Data { get; set; }
    }
}