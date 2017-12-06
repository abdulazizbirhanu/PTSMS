

'use strict'
eVISAApp
    .factory('authInterceptorService', ['$q', '$location', 'localStorageService', 'notificationService', '$state', function ($q, $location, localStorageService, notificationService, $state) {

    var authInterceptorServiceFactory = {};

    var _request = function (config) {

        config.headers = config.headers || {};

        var authData = localStorageService.get('authorizationData');

        if (authData) {

            config.headers.Authorization = authData.loggedUser.token_type + ' ' + authData.loggedUser.access_token;
        }
        return config;
    }

    var _responseError = function (rejection) {
        if (rejection.status === 401) {
            notificationService.displayError('You are not authorize to access this functionality.');
            $state.go('Login');
        }
        return $q.reject(rejection);
    }

    authInterceptorServiceFactory.request = _request;
    authInterceptorServiceFactory.responseError = _responseError;

    return authInterceptorServiceFactory;
    }]);


