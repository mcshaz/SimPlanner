(function () {
    'use strict';
    var serviceId = 'tokenStorageService';
    angular.module('app')
        .service(serviceId, ['$rootScope', 'AUTH_EVENTS', '$http', 'localStorageService', tokenStorageService]);
    function tokenStorageService($rootScope, AUTH_EVENTS, $http, localStorage) {
        var currentUser = localStorage.get('currentUser');
        var token = localStorage.get('token');
        var unAuthUser = {
            name: '',
            roles: []
        };

        this.getUserName = getUserName;
        this.getUserRoles = getUserRoles;
        this.isLoggedIn = isLoggedIn;
        this.isAuthorized = isAuthorized;

        if (!currentUser) {
            currentUser = unAuthUser;
        }

        setTokenHeader();
        $rootScope.$on(AUTH_EVENTS.loginCancelled, onLogout);
        $rootScope.$on(AUTH_EVENTS.loginConfirmed, onLogin);
        
        function isLoggedIn() {
            return(!!token);
        }

        function getUserName() {
            return currentUser.name;
        }

        function getUserRoles(){
            return currentUser.roles.slice();
        }

        //check if the user is authorized to access the next route
        //this function can be also used on element level
        //e.g. <p ng-if="isAuthorized(authorizedRoles)">show this only to admins</p>
        function isAuthorized(authorizedRoles) {
            if (!isLoggedIn()) {return false;}
            if (!angular.isArray(authorizedRoles)) {
                authorizedRoles = [authorizedRoles];
            }
            if (authorizedRoles.length === 1 && authorizedRoles[0] === '*') {
                return true;
            }
            return authorizedRoles.some(function (el) {
                currentUser.roles.indexOf(el) !== -1
            });
        };

        function onLogin(e, data) {
            token = data.user_token || data.access_token;
            currentUser = {
                name: data.fullName,
                roles: data.userRoles.split(',')
            }
            localStorage.set('token',token);
            localStorage.set('currentUser', currentUser);
            setTokenHeader();
        }

        function onLogout() {
            token = null;
            currentUser = unAuthUser;
            localStorage.remove('token');
            localStorage.remove('currentUser');
            setTokenHeader();
        }

        function setTokenHeader() {
            if (token) {
                $http.defaults.headers.common.authorization = 'Bearer ' + token;
            } else if ($http.defaults.headers.common.authorization) {
                delete $http.defaults.headers.common.authorization;
            }

        };
    }

})();