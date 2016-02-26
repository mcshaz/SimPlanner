(function () {
    'use strict';
    var serviceId = 'tokenStorageService';
    angular.module('app')
        .service(serviceId, ['authService', 'AUTH_EVENTS', '$http', 'localStorageService', tokenStorageService]);
    function tokenStorageService(authService, AUTH_EVENTS, $http, localStorage) {
        var currentUser = localStorage.get('currentUser');
        var token = localStorage.get('token');
        var unAuthUser = {
            name: '',
            roles: [],
            id:null
        };
        var self = this;

        self.getUserName = getUserName;
        self.getUserRoles = getUserRoles;
        self.getUserId = getUserId;
        self.getUserLocales = getUserLocales;
        self.isLoggedIn = isLoggedIn;
        self.isAuthorized = isAuthorized;
        self.notifyLogin = notifyLogin;
        self.notifyLogout = notifyLogout;
        self.notifyModulesLoaded = notifyModulesLoaded;

        if (!isLoggedIn()) {
            currentUser = unAuthUser;
        }

        setTokenHeader();

        function notifyModulesLoaded() {
            if (isLoggedIn()) {
                authService.loginConfirmed();
            }
        }
        
        function isLoggedIn() {
            return(!!token);
        }

        function getUserName() {
            return currentUser.name;
        }

        function getUserId() {
            return currentUser.id;
        }

        function getUserRoles(){
            return currentUser.roles.slice(0);
        }

        function getUserLocales() {
            return currentUser.locales;
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

        function notifyLogin(data) {
            token = data.user_token || data.access_token;
            currentUser = {
                name: data.fullName,
                roles: data.userRoles.split(','),
                id: data.userId,
                locales: data.userLocales.split(',')
            }
            localStorage.set('token',token);
            localStorage.set('currentUser', currentUser);
            setTokenHeader();
            //now broadcast
            authService.loginConfirmed();
        }

        function notifyLogout() {
            token = null;
            currentUser = unAuthUser;
            localStorage.remove('token');
            localStorage.remove('currentUser');
            setTokenHeader();
            //now broadcast
            authService.loginCancelled();
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