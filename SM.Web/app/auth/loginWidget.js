(function(){
'use strict';

var controllerId = 'loginWidget';
    angular.module('app')
        .controller(controllerId, ['$rootScope', 'loginFactory', '$uibModal', 'AUTH_EVENTS', 'tokenStorageService', loginWidget]);
    
    function loginWidget($rootScope, loginFactory, $uibModal,AUTH_EVENTS,tokenStorageService) {
        var modalInstance = null;
        var vm = this;
        vm.currentUser = tokenStorageService.getUserName();
        vm.getCredentials = getCredentials;
        vm.logout = logout;

        $rootScope.$on(AUTH_EVENTS.loginRequired, getCredentials);
        $rootScope.$on(AUTH_EVENTS.loginConfirmed, loggedIn);

        function getCredentials() {
            modalInstance =  $uibModal.open({
                templateUrl: 'app/auth/getCredentials.html',
                controller: 'getCredentials',
            });
        };

        function loggedIn(e, data) {
            vm.currentUser = tokenStorageService.getUserName();
            if (modalInstance) {
                modalInstance.close();
                modalInstance = null;
            }
        }

        function logout() {
            loginFactory.logout();
            vm.currentUser = '';
        }
    };
})();