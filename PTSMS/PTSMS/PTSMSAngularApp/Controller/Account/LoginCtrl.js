
'use strict'
eVISAApp
    .controller('LoginCtrl', ['$scope', '$state', '$http', '$rootScope', 'notificationService', 'membershipService', function ($scope, $state, $http, $rootScope, notificationService, membershipService) {

        document.title = "Login | Ethiopian e-VISA";

        $scope.LoginMessage = 'Provide Login Detail.';

        $scope.LoginSubmit = function () {

            var data = {
                "Email": $scope.User.Email,
                "Password": $scope.User.Password
            };

            $http.post('/api/Account/Login', JSON.stringify(data), headerConfig).success(function (data) {

                    if (data.ProcessStatus === 'OK') {

                        ////REMOVE THE EXISTING CREDANTIAL 
                        membershipService.removeCredentials();
                       
                        ////STORE TOKEN AND OTHER INFORMATION IN COOKIES AS NEW CREDENTIAL
                        membershipService.saveCredentials(data);

                        //DISPLAY USER sINFORMARION                       
                        $state.go('HomePage');
                        notificationService.displaySuccess('WELCOME ' + $scope.User.Email);
                    }
                    else {
                        
                        if (data.Errors == null)
                            notificationService.displayError(data.Message);
                        else
                            notificationService.displayError(data.Message + ' ' + data.Errors.join(' '));
                    }

                }).error(function (data) {
                    notificationService.displayError('Failed to connect to the server.');
                });
        };
    }]);