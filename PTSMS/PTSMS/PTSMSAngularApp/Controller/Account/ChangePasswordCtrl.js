/// <reference path="../../rootModule.js" />

'use strict'
eVISAApp
    .controller('ChangePasswordCtr', ['$scope', '$state', '$http', 'notificationService', 'membershipService', function ($scope, $state, $http, notificationService, membershipService) {

        document.title = "Change Password | Ethiopian e-VISA";

        $scope.PasswordChangeMessage = 'Change Your Password.';

        $scope.PasswordChangeSubmit = function () {

            var data = {
                "OldPassword": $scope.User.OldPassword,
                "NewPassword": $scope.User.NewPassword,
                "ConfirmPassword": $scope.User.ConfirmNewPassword
            };

            $http.post('/api/Account/ChangePassword', JSON.stringify(data), headerConfig).success(function (data) {

                if (data.ProcessStatus === 'OK') {
                    notificationService.displaySuccess(data.Message + ' You will be redicted to login page in 2 seconds.');
                    membershipService.startTimerAndRedirectTo('Login');
                }
                else {
                    //ADD LIST OF ERRORS
                    var matches = [];
                    angular.forEach(data.Errors, function (value) {
                        matches.push(value);
                    });
                    notificationService.displayError(data.Message + matches.join(' '));
                }

            }).error(function (err, status) {
                notificationService.displayError('Sorry, error occured while connecting to the server.');
            });
        };

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
    }]);