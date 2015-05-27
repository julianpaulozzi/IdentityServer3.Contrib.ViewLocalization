/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * 
 * Adapted by Julian Paulozzi
 * 
 * see license
 */

(function () {
    'use strict';

    angular.module('app')
        .controller("LayoutCtrl", ['$scope', 'Model',
            function ($scope, Model) {
                $scope.model = Model;
            }]);

})();