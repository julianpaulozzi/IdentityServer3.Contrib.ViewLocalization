/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * 
 * Adapted by Julian Paulozzi
 * 
 * see license
 */

(function () {
    "use strict";

    (function () {
        var app = angular.module("app", [
            'ngSanitize',
            'ngCookies',
            'pascalprecht.translate'    // See: https://github.com/angular-translate/angular-translate
        ]);

        app.config(['$translateProvider', 'localeConfig', 'Model', function ($translateProvider, localeConfig, Model) {
            $translateProvider.useSanitizeValueStrategy('sanitizeParameters');
            $translateProvider.translations(localeConfig.preferredLanguage, localeConfig.values);
            $translateProvider.useUrlLoader('/' + localeConfig.loaderUrl + '/' + localeConfig.part);
            $translateProvider.preferredLanguage(localeConfig.preferredLanguage);
            $translateProvider.useCookieStorage();
        }]);

        app.run(['$rootScope', '$translate', 'localeConfig', '$sce', function ($rootScope, $translate, localeConfig, $sce) {

            init();

            function init() {

                /*for (var key in localeConfig.values) {
                    if (localeConfig.values.hasOwnProperty(key)) {
                        var html = localeConfig.values[key];
                        $sce.trustAsHtml(html);
                    }
                }*/

                $rootScope.flags = {
                    loadingTranslation: false
                };
                $rootScope.locale = {
                    lang: 'en',
                    allowChangeLanguage: localeConfig.allowChangeLanguage,
                    availableLanguages: localeConfig.availableLanguages,
                    languageHeader: $translate.instant('Language')
                };
                $rootScope.changeLanguage = changeLanguage;
                $rootScope.$on('$translateChangeEnd', onTranslateChangeEnd);
                $rootScope.$on('$translateLoadingStart', onTranslateLoadingStart);
                $rootScope.$on('$translateLoadingEnd', onTranslateLoadingEnd);

                onLanguageChange();
            }

            function onTranslateChangeEnd() {
                onLanguageChange();
            }

            function onTranslateLoadingStart() {
                $rootScope.flags.loadingTranslation = true;
            }

            function onTranslateLoadingEnd() {
                $rootScope.flags.loadingTranslation = false;
            }

            function changeLanguage(language) {
                if (!language)
                    return;

                var langKey = language.IsoLanguageName;
                $translate.use(langKey);
            }

            function onLanguageChange() {
                var langKey = $translate.use();
                $rootScope.locale.lang = langKey;
                var availableLanguages = localeConfig.availableLanguages;
                var done = false;
                for (var i = 0; i < availableLanguages.length; i++) {
                    var lang = availableLanguages[i];
                    lang.active = false;
                    if (done) continue;
                    if (lang.IsoLanguageName === langKey) {
                        done = true;
                        lang.active = true;
                        $rootScope.locale.languageHeader = lang.DisplayName;
                    }
                }

                if (!done)
                    $rootScope.locale.languageHeader = $translate.instant('Language');
            }

        }]);

    })();

    (function () {
        var modelJson = document.getElementById("modelJson");
        if (!modelJson) {
            angular.module("app").constant("Model", {});
            return;
        }

        var encodedJson = modelJson.textContent;
        var json = Encoder.htmlDecode(encodedJson);
        var model = JSON.parse(json);
        angular.module("app").constant("Model", model);
        if (model.autoRedirect && model.redirectUrl) {
            if (model.autoRedirectDelay < 0) {
                model.autoRedirectDelay = 0;
            }
            window.setTimeout(function () {
                window.location = model.redirectUrl;
            }, model.autoRedirectDelay * 1000);
        }
    })();

})();
