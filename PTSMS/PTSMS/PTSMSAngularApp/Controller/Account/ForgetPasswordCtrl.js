
'use strict'
eVISAApp
    .controller('ForgetPasswordCtrl', ['$scope', '$state', '$http', 'notificationService', 'membershipService', function ($scope, $state, $http, notificationService, membershipService) {

        document.title = "Forget Password | Ethiopian e-VISA";

        $scope.ForgetPasswordTitle = 'Forget Your Password.';
        $scope.ForgetPasswordSubTitle = 'Enter your email.';
        $scope.ForgetPasswordSubmit = function () {
       
            var data = {
                "Email": $scope.User.Email
            };
           
            $http.post('/api/Account/ForgotPassword', JSON.stringify(data), headerConfig).success(function (data) {

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
                notificationService.displayError('Sorry, error occured while connecting to the server.');
            });

    };

}]);