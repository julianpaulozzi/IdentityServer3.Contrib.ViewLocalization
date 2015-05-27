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
using Thinktecture.IdentityServer.Core.Configuration;

namespace IdentityServer3.Contrib.ViewLocalization.Configuration
{
    // TODO: Replace with DependencyResolver
    internal class OptionsState
    {
        private static readonly Lazy<OptionsState> _current = new Lazy<OptionsState>();

        public static OptionsState Current
        {
            get { return _current.Value; }
        }

        public void RegisterConfiguration(IdentityServerOptions identityServerOptions, IdentityServerViewLocalizationOptions options)
        {
            if (identityServerOptions == null) throw new ArgumentNullException("identityServerOptions");
            if (options == null) throw new ArgumentNullException("options");
            if (IdentityServerOptions != null) throw new InvalidOperationException("Options are already registered");

            IdentityServerOptions = identityServerOptions;
            Options = options;
        }

        public IdentityServerViewLocalizationOptions Options { get; private set; }
        public IdentityServerOptions IdentityServerOptions { get; private set; }
    }
}