

'use strict'
eVISAApp
    .controller('PasswordResetCtrl', ['$scope', '$state', '$http', 'notificationService', 'membershipService', '$stateParams', function ($scope, $state, $http, notificationService, membershipService, $stateParams) {

        document.title = "Reset Password | Ethiopian e-VISA";

        var passwordRecoveryToken = $stateParams.code;
       
        $scope.PasswordResetTitle = 'Reset Password.';
        $scope.PasswordResetSubTitle = 'Reset your password.';
        $scope.ResetPasswordSubmit = function () {
           
            var data = {
                "Email": $scope.User.Email,
                "Password": $scope.User.NewPassword,
                "ConfirmPassword": $scope.User.ConfirmNewPassword,
                "Code": passwordRecoveryToken
            };

            $http.post('/api/Account/ResetPassword', JSON.stringify(data), headerConfig).success(function (data) {

                if (data.ProcessStatus === 'OK') {
                    notificationService.displaySuccess(data.Message + ' You will be redicted to login page in 4 seconds.');
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
                notificationService.displayError('Sorry, error occured while connecting to the server.' + status);
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