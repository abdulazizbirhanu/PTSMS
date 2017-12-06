
'use strict'
PTSMSApp
    .factory('membershipService', ['$rootScope', '$state', '$http', '$q', '$log', 'localStorageService', 'notificationService', '$timeout', function ($rootScope, $state, $http, $q, $log, localStorageService, notificationService, $timeout) {

        //var serviceBase = 'http://localhost:57570/';
        var serviceBase = "http://svhqmwa01:2223/";
        var service = {
            saveCredentials: saveCredentials,
            removeCredentials: removeCredentials,
            _authentication: _authentication,
            _saveRegistration: _saveRegistration,
            _login: _login,
            _logOut: _logOut,
            _fillAuthData: _fillAuthData,
            isUserLoggedIn: isUserLoggedIn,
            getUsename: getUsename,
            startTimerAndRedirectTo: startTimerAndRedirectTo,
            redirectToLogin: redirectToLogin
        };

        var _authentication = {
            isAuth: false,
            userName: ""
        };

        function _saveRegistration(registration, registerSuccess, registerFailed) {

            redirectToLogin();
            $http.post('api/Account/Register', JSON.stringify(registration), headerConfig).success(function (data) {
                registerSuccess(data);
            }).error(function (err, status) {
                registerFailed(err);
            });
        }

        var _login = function (loginData) {

            var deferred = $q.defer();

            $http.post('/api/Account/Login', JSON.stringify(loginData), headerConfig).success(function (data) {

                if (data.ProcessStatus === 'OK') {
                    //REMOVE THE EXISTING CREDANTIAL 
                    removeCredentials();

                    //STORE TOKEN AND OTHER INFORMATION IN COOKIES AS NEW CREDENTIAL
                    saveCredentials(data);

                    //DISPLAY USER sINFORMARION
                    _authentication.isAuth = true;
                    _authentication.userName = loginData.Email;

                    deferred.resolve(response);

                    $scope.request = data;
                }
                else {
                    notificationService.displayError(data.Message);
                }

            }).error(function (err, status) {
                _logOut();
                deferred.reject(err);
            });

            return deferred.promise;
        };

        function _logOut() {
            localStorageService.remove('authorizationData');
            _authentication.isAuth = false;
            _authentication.userName = "";
            $http.defaults.headers.common.Authorization = '';
            $state.go('Login');
            notificationService.displayWarning('You are logged out from the system.');
        }

        function redirectToLogin() {
            localStorageService.remove('authorizationData');
            _authentication.isAuth = false;
            _authentication.userName = "";
            $http.defaults.headers.common.Authorization = '';
            $state.go('Login');
        }

        var _fillAuthData = function () {

            var authData = localStorageService.get('authorizationData');
            if (authData) {
                _authentication.isAuth = true;
                _authentication.userName = authData.loggedUser.userName;
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        function saveCredentials(user) {

            // var membershipData = $base64.encode(user.username + ':' + user.password);
            //console.log('SaveCredentials');
            //Save token and other user information 
            $rootScope.repository = {
                //access_token,token_type,userName,issued,expires
                loggedUser: {
                    access_token: user.access_token,
                    token_type: user.token_type,
                    userName: user.userName,
                    issued: user.issued,
                    expires: user.expires
                }
            };
            // $http.defaults.headers.common['Authorization'] = $rootScope.repository.loggedUser.token_type + ' ' + $rootScope.repository.loggedUser.access_token;
            localStorageService.set('authorizationData', $rootScope.repository);
            // console.log(localStorageService.get('authorizationData'));
            //  console.log($rootScope.repository.loggedUser.token_type + ' ' + $rootScope.repository.loggedUser.access_token);
        }

        function removeCredentials() {
            //console.log('removeCredentials' + localStorageService.get('authorizationData'));
            $rootScope.repository = {};
            localStorageService.remove('authorizationData');
            $http.defaults.headers.common.Authorization = '';
        }

        function isUserLoggedIn() {

            _fillAuthData();
            return _authentication.isAuth;
        }

        function getUsename() {

            var authData = localStorageService.get('authorizationData');
            if (authData) {
                return authData.loggedUser.userName;
            }
            else {
                return "guest";
            }
        }

        function startTimerAndRedirectTo(stateName) {
            var timer = $timeout(function () {
                $timeout.cancel(timer);
                $state.go(stateName);
            }, 4000);
        }

        return service;

    }]);