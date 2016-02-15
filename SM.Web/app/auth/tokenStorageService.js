(function () {
    'use strict';
    var serviceId = 'tokenStorageService';
    angular.module('app')
        .service(serviceId, ['$rootScope', 'AUTH_EVENTS', '$http', 'localStorageService', tokenStorageService]);
    function tokenStorageService($rootScope, AUTH_EVENTS, $http, localStorage){
        setTokenHeader();
        $rootScope.$on(AUTH_EVENTS.loginCancelled, onLogout);
        $rootScope.$on(AUTH_EVENTS.loginConfirmed, onLogin);

        this.isLoggedIn = isLoggedIn;
        
        function isLoggedIn() {
            return(!!localStorage.get('token'));
        }

        function onLogin(e, data) {
            localStorage.set('token', data.user_token || data.access_token);
            localStorage.set('currentUser', {
                name: data.fullName,
                roles: data.userRoles.split(',')
            });
            setTokenHeader();
        }

        function onLogout() {
            localStorage.remove('token');
            localStorage.remove('currentUser');
            setTokenHeader();
        }

        function setTokenHeader() {
            var token = localStorage.get('token');
            if (token) {
                $http.defaults.headers.common.authorization = 'Bearer ' + token;
            } else if ($http.defaults.headers.common.authorization) {
                delete $http.defaults.headers.common.authorization;
            }

        };
    }

})();