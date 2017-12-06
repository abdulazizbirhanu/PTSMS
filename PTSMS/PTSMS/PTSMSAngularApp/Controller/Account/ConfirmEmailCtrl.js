

'use strict'
eVISAApp
    .controller('ConfirmEmailCtrl', ['$scope', '$state', '$http', 'notificationService', 'membershipService', '$stateParams', function ($scope, $state, $http, notificationService, membershipService, $stateParams) {
        
        document.title = "Confirm Email | Ethiopian e-VISA";

        var userId = $stateParams.userId;
        var confirmEmailToken = $stateParams.code;        
       
        $scope.ConfirmEmailTitle = 'Account confirmation.';
        $scope.ConfirmEmailSubTitle = 'Your account is confimed successfully.';
               
            var data = {
                "UserId": userId,
                "Code": confirmEmailToken
            };

            $http.post('/api/Account/ConfirmEmail', JSON.stringify(data), headerConfig).success(function (data) {

                if (data.ProcessStatus === 'OK') {
                    $scope.ResponseMessage = data.Message;
                    notificationService.displaySuccess(data.Message + ' You will be redicted to login page in 4 seconds.');
                    membershipService.startTimerAndRedirectTo('Login');
                }
                else {
                    //ADD LIST OF ERRORS
                    
                    var matches = [];
                    angular.forEach(data.Errors, function (value) {
                        matches.push(value);
                    });
                    $scope.ResponseMessage = data.Message +  matches.join(' ');
                    notificationService.displayError(data.Message + matches.join(' '));
                }

            }).error(function (err, status) {
                notificationService.displayError('Sorry, error occured while connecting to the server.' + status);
            });
    }]);