

'use strict'
eVISAApp
    .controller('LogoutCtrl', ['$scope', '$state', '$http', '$rootScope', 'notificationService', 'membershipService', function ($scope, $state, $http, $rootScope, notificationService, membershipService) {
        
        membershipService._logOut();

   }]);