

'use strict'
eVISAApp
    .controller('RegisterCtrl', ['$scope', '$state', '$http', 'membershipService', 'notificationService', '$location', function ($scope, $state, $http, membershipService, notificationService, $location) {

        document.title = "Register | Ethiopian e-VISA";

        $scope.RegisterMessage = 'Create e-VISA Account.';

        $scope.passwordValidator = function (password) {

            if (!password) { return; }

            if (password.length < 6) {
                return "Password must be at least " + 6 + " characters long";
            }

            if (!password.match(/[A-Z]/)) {
                return "Password must have at least one capital letter";
            }

            if (!password.match(/[0-9]/)) {
                return "Password must have at least one number";
            }
            return true;
        };
        $scope.phoneNumberPattern = (function () {
            var regexp = /^\(?(\d{3})\)?[ .-]?(\d{3})[ .-]?(\d{4})$/;
            return {
                test: function (value) {
                    if ($scope.requireTel === false) {
                        return true;
                    }
                    return regexp.test(value);
                }
            };
        })();

        $scope.Register = function () {
            
            var data = {
                "Email": $scope.User.Email,
                "Password": $scope.User.Password,
                "ConfirmPassword": $scope.User.ConfirmPassword
            };
            membershipService._saveRegistration(data, registerSuccess, registerFailed);
        };

        function registerSuccess(response) {

            if (response.ProcessStatus === 'OK') {
                notificationService.displaySuccess(response.Message + ' You will be redicted to login page in 4 seconds.');
                notificationService.startTimerAndRedirectTo();
            }
            else {            
                notificationService.displayError(response.Message + ' ' + response.Errors.join(' '));
            }
        }

        function registerFailed(response) {
            notificationService.displayError('Sorry, error has occured. Please try again later.')
        }
        //var startTimer = function () {
        //    var timer = $timeout(function () {
        //        $timeout.cancel(timer);
        //         $state.go('Login');
        //    }, 2000);
        //}


    }]);