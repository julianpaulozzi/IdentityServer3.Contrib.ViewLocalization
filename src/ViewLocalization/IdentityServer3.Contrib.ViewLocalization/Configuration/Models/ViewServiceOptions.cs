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

using System.Collections.Generic;

namespace IdentityServer3.Contrib.ViewLocalization.Configuration
{
    /// <summary>
    /// Configures the assets for the localized view service.
    /// 
    /// </summary>
    public class ViewServiceOptions
    {
        /// <summary>
        /// Stylesheets to be rendered into the layout.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The stylesheets.
        /// 
        /// </value>
        public IList<string> Stylesheets { get; set; }

        /// <summary>
        /// Scripts to be rendered into the layout.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The scripts.
        /// 
        /// </value>
        public IList<string> Scripts { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether HTML will be cached by the default view cache.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// <c>true</c> if [cache views]; otherwise, <c>false</c>.
        /// 
        /// </value>
        public bool CacheViews { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewServiceOptions"/> class.
        /// 
        /// </summary>
        public ViewServiceOptions()
        {
            Stylesheets = new List<string>();
            Scripts = new List<string>();
            CacheViews = true;
        }
    }
}