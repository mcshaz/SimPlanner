(function(){
'use strict';

var controllerId = 'loginWidget';
    angular.module('app')
        .controller(controllerId, ['$rootScope', 'loginFactory', '$modal', 'AUTH_EVENTS', 'tokenStorageService','common', 'commonConfig',loginWidget]);
    
    function loginWidget($rootScope, loginFactory, $modal,AUTH_EVENTS,tokenStorageService, common,commonConfig) {
        var modalInstance = null;
        var vm = this;
        vm.currentUser = tokenStorageService.getUserName();
        vm.getCredentials = getCredentials;
        vm.logout = logout;

        $rootScope.$on(AUTH_EVENTS.loginRequired, getCredentials);
        $rootScope.$on(AUTH_EVENTS.loginConfirmed, loggedIn);
        //now we have a method to display login if the token has expired, we can notify
        $rootScope.$broadcast(AUTH_EVENTS.loginWidgetReady);
        function getCredentials() {
            common.$broadcast(commonConfig.config.controllerActivateSuccessEvent); //switch the spinner off
            var modal = getModalInstance();
            modal.$promise.then(modal.show);
        };

        var _modalInstance;
        function getModalInstance() {
            return _modalInstance || (_modalInstance=$modal({
                templateUrl: 'app/auth/getCredentials.html',
                controller: 'getCredentials',
                animation: 'am-fade-and-slide-top',
                controllerAs: 'gc',
                show:false
            }));
        }

        function loggedIn(e, data) {
            vm.currentUser = tokenStorageService.getUserName();
            if (_modalInstance) {
                _modalInstance.hide(); //(could use hide)
            }
        }

        function logout() {
            loginFactory.logout();
            vm.currentUser = '';
        }
    };
})();