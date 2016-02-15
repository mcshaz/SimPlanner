(function () {
    'use strict';

    angular.module('app').run(['$rootScope', 'AUTH_EVENTS', '$http', 'localStorageService', setupAuth]);
    function setupAuth($rootScope, AUTH_EVENTS, $http, localStorage){
        setTokenHeader();
        $rootScope.$on(AUTH_EVENTS.loginCancelled, loggedOut);
        $rootScope.$on(AUTH_EVENTS.loginConfirmed, loggedIn);

        function loggedIn(e, data) {
            localStorage.set('token', data.user_token || data.access_token);
            localStorage.set('currentUser', {
                name: data.fullName,
                roles: data.userRoles.split(',')
            });
        }

        function loggedOut() {
            localStorage.remove('token');
            localStorage.remove('currentUser')
        }

        function setTokenHeader() {
            var token = localStorage.get('token')
            if (token) {
                $http.defaults.headers.common.authorization = 'Bearer ' + token;
            } else if ($http.defaults.headers.common.authorization) {
                delete $http.defaults.headers.common.authorization;
            }

        };
    }

})();