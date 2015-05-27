/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * 
 * Adapted by Julian Paulozzi
 * 
 * see license
 */

(function () {
    'use strict';

    var app = angular.module('app');

    app.directive("antiForgeryToken", function () {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                token: "="
            },
            template: "<input type='hidden' name='{{token.name}}' value='{{token.value}}'>"
        };
    });

    app.filter('HasTranslatedScope', ['$translate', function ($translate) {
        return function (idsrvScope, valueName) {
            var isDescription = angular.isDefined(valueName) && valueName === 'description';
            var keySufix = isDescription ? '_Description' : '_DisplayName';
            var transKey = 'Scope_' + idsrvScope.name + keySufix;

            var translation = $translate.instant(transKey);
            var defaulVal = isDescription ? idsrvScope.description : idsrvScope.displayName;

            return (defaulVal && defaulVal.length > 0) || (translation && translation !== transKey);
        };
    }]);

    app.directive('idsrvTranslateScope', ['$compile', '$translate', '$rootScope', function ($compile, $translate, $rootScope) {
        return {
            restrict: 'A',
            scope: {
                idsrvTranslateScope: '='
            },
            priority: 0,
            compile: function (element, attrs) {
                var isDescription = angular.isDefined(attrs.description);
                var keySufix = isDescription ? '_Description' : '_DisplayName';
                return {
                    pre: function (scope, element) {
                        
                        var idsrvScope = scope.idsrvTranslateScope;
                        var transKey = 'Scope_' + idsrvScope.name + keySufix;

                        var translationChangeOccurred = function () {
                            var result = $translate.instant(transKey);
                            if (result && result !== transKey) {
                                element.html(result);
                            } else {
                                element.html(isDescription ? idsrvScope.description : idsrvScope.displayName);
                            }

                            $compile(element.contents())(scope);
                        };

                        //translation changes by default while linking!
                        translationChangeOccurred();
                        var unsubscribe = $rootScope.$on('$translateChangeEnd', translationChangeOccurred);
                        element.bind('$destroy', function () {
                            if (angular.isFunction(unsubscribe)) {
                                unsubscribe();
                            }
                        });

                    },
                    post: function () {
                    }
                };
            }
        };
    }]);

})();