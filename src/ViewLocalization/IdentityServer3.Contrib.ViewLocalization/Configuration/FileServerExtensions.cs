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

using System.Reflection;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;

namespace IdentityServer3.Contrib.ViewLocalization.Configuration
{
    public static class FileServerExtensions
    {
        private static readonly Assembly _appAssembly = typeof(FileServerExtensions).Assembly;

        private static readonly string _appAssemblyName = AppAssembly.GetName().Name;

        public static Assembly AppAssembly
        {
            get { return _appAssembly; }
        }

        public static string AppAssemblyName
        {
            get { return _appAssemblyName; }
        }

        public static void UseEmbeddedResourceFile(this IAppBuilder app, string rootPath, params string[] paths)
        {
            var hasRootPath = false;
            if (!string.IsNullOrEmpty(rootPath))
            {
                hasRootPath = true;
                if (rootPath.EndsWith("/"))
                    rootPath = rootPath.Remove(rootPath.Length - 1);
            }
            
            foreach (var path in paths)
            {
                var pathString = path;
                if (!pathString.StartsWith("/"))
                {
                    pathString = "/" + pathString;
                }

                if (hasRootPath && !pathString.StartsWith(rootPath))
                {
                    pathString = rootPath + pathString;
                }

                var baseNamespace = GetAppResourceNamespaceByPath(pathString);
                var requestPath = pathString.Replace(rootPath ?? "", string.Empty);

                app.UseFileServer(new FileServerOptions
                {
                    RequestPath = new PathString(requestPath),
                    FileSystem = new EmbeddedResourceFileSystem(AppAssembly, baseNamespace)
                });

                app.UseStageMarker(PipelineStage.MapHandler);
            }
        }

        public static string GetAppResourceNamespaceByPath(string sufixPath)
        {
            return GetAppResourceNamespace(sufixPath.Replace("/", "."));
        }

        public static string GetAppResourceNamespace(string sufix)
        {
            var separator = string.IsNullOrEmpty(sufix) || sufix.StartsWith(".") ? "" : ".";
            return string.Format("{0}{1}{2}", AppAssemblyName, separator, sufix);
        }
    }
}