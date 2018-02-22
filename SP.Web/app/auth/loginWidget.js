"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
'use strict';
var controllerId = 'loginWidget';
angular_1.default.module('app')
    .controller(controllerId, ['$rootScope', 'loginFactory', '$modal', 'AUTH_EVENTS', 'tokenStorageService', 'common', 'commonConfig', '$location', '$route', loginWidget]);
function loginWidget($rootScope, loginFactory, $modal, AUTH_EVENTS, tokenStorageService, common, commonConfig, $location, $route) {
    var vm = this;
    vm.currentUser = tokenStorageService.getUserName();
    vm.getCredentials = getCredentials;
    vm.logout = logout;
    $rootScope.$on(AUTH_EVENTS.loginRequired, getCredentials);
    $rootScope.$on(AUTH_EVENTS.loginConfirmed, loggedIn);
    $rootScope.$broadcast(AUTH_EVENTS.loginWidgetReady);
    function getCredentials() {
        common.$broadcast(commonConfig.config.controllerActivateSuccessEvent);
        var modal = getModalInstance();
        modal.$promise.then(modal.show);
    }
    var _modalInstance;
    function getModalInstance() {
        return _modalInstance || (_modalInstance = $modal({
            templateUrl: 'app/auth/getCredentials.html',
            controller: 'getCredentials',
            animation: 'am-fade-and-slide-top',
            controllerAs: 'gc',
            show: false
        }));
    }
    function loggedIn(_e, _data) {
        vm.currentUser = tokenStorageService.getUserName();
        if (_modalInstance) {
            _modalInstance.hide();
        }
    }
    function logout() {
        loginFactory.logout().then(function () {
            vm.currentUser = '';
            if ($location.path() === '/') {
                $route.reload();
            }
            else {
                $location.path('/');
            }
        });
    }
}
//# sourceMappingURL=loginWidget.js.map