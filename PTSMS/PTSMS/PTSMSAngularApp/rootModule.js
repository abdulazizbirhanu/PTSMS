

'use strict'
var urlAddress = "http://localhost:57570/";
//var urlAddress = "http://svhqmwa01:2223/";
// Declare app level module which depends on filters, and services   

angular.module('PTSMS.controllers', [function () {
    
}]);
angular.module('PTSMS.Config', ['PTSMS.controllers', 'ui.router', 'LocalStorageModule', 'base64', 'ngFileUpload', 'angularValidator', 'angucomplete-alt', 'ui.bootstrap', 'angular-loading-bar', function () {
    alert('PTSMS module');
}]).run(
  ['$rootScope', '$state', '$stateParams',
    function ($rootScope, $state, $stateParams) {
        //alert('dfd');
        $rootScope.$state = $state;
        $rootScope.$stateParams = $stateParams;

        //$rootScope.$on('$stateChangeStart', function (event, toState, toParams) {
            //alert('run Method');
            //var requireLogin = toState.data.requireLogin;
            //var currentUser = localStorageService.get('authorizationData');

            //if (!(currentUser === 'undefined' || currentUser === null)) {
            //    $rootScope.CurrentUserEmail = currentUser.loggedUser.userName;
            //}
            //if (requireLogin && (typeof currentUser === 'undefined' || currentUser === null)) {
            //    $state.go('Login');
            //    event.preventDefault();
            //    notificationService.displayWarning('Access Denied. Please login to access the page.');
            //}
            //$window.scrollTo(0, 0);
        //});
        
        /*1.Scroll to top
        $window.scrollTo(0, 0); 
        2.Focus on element
        $window.scrollTo(0, angular.element('put here your element').offsetTop); */
    }
  ]
)
    //Configure the routes
    .config(['$stateProvider', '$urlRouterProvider', function ($stateProvider, $urlRouterProvider) {
        // For any unmatched url, redirect to homePage (Landing Page)
        $urlRouterProvider.otherwise("/HomePage");       
        $stateProvider
            .state('HomePage', {
                url: '/HomePage',
                templateUrl: '/PTSMSAngularApp/View/Account/HomePage.html',
                controller: 'HomePageCtrl'
            });
    }]);



var PTSMSApp = angular.module('PTSMS', ['PTSMS.controllers', 'PTSMS.Config', 'ui.router', 'LocalStorageModule', 'base64', 'ngFileUpload', 'angularValidator', 'angucomplete-alt', 'ui.bootstrap', 'angular-loading-bar',
                                            function () {

                                            }]);

PTSMSApp.controller('HomePageCtrl', ['$scope', '$state', '$http', function ($scope, $state, $http) {
        alert("HomePageCtrl");
        document.title = "Welcome to ET Pilot Training";        
        $scope.WelComeMessage = 'Welcome to ET Pilot Training.';        
    }]);

//var serviceBase = urlAddress;

//PTSMSApp.constant('ngAuthSettings', {
//    apiServiceBaseUri: serviceBase,
//    clientId: 'ngAuthApp'
//}).
//PTSMSApp.config(function ($httpProvider) {
//    $httpProvider.interceptors.push('authInterceptorService');
//});
//.
//run(['authService', function (authService) {
//    authService.fillAuthData();
//}]);


//var headerConfig = {
//    headers: {
//        'Content-Type': 'application/json'
//    }
//};

// Global Variables for Day, Month and Year drop downs      
//Date
//var resultDate = [];
//for (var i = 1; i <= 31; i++) {
//    resultDate.push(i);
//}

////Year
//var thisYear = new Date().getFullYear();
//var resultYear = [];
//var comingYear = [];
//for (var i = thisYear; i >= 1900; i--) {
//    resultYear.push(i);
//}
//for (var i = thisYear; i <= thisYear + 6; i++) {
//    comingYear.push(i);
//}

////Month
//var resultMonth = [{
//    comboLable: 'January',
//    comboValue: '1'
//}, {
//    comboLable: 'February',
//    comboValue: '2'
//}, {
//    comboLable: 'March',
//    comboValue: '3'
//}, {
//    comboLable: 'April',
//    comboValue: '4'
//}, {
//    comboLable: 'May',
//    comboValue: '5'
//}, {
//    comboLable: 'June',
//    comboValue: '6'
//}, {
//    comboLable: 'July',
//    comboValue: '7'
//}, {
//    comboLable: 'August',
//    comboValue: '8'
//}, {
//    comboLable: 'September',
//    comboValue: '9'
//}, {
//    comboLable: 'October',
//    comboValue: '10'
//}, {
//    comboLable: 'November',
//    comboValue: '11'
//}, {
//    comboLable: 'December',
//    comboValue: '12'
//}];