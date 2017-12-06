/// <reference path="../../rootModule.js" />

'use strict'
PTSMSApp
    .controller('HomePageCtrl', ['$scope', '$state', '$http', function ($scope, $state, $http) {
        alert("HomePageCtrl");
        document.title = "Welcome to ET Pilot Training";        
        $scope.WelComeMessage = 'Welcome to ET Pilot Training.';        
    }]);








