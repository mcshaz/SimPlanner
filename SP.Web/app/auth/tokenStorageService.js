(function () {
    'use strict';
    var serviceId = 'tokenStorageService';
    angular.module('app')
        .service(serviceId, ['authService', 'AUTH_EVENTS', '$http', 'localStorageService' , '$rootScope', 'common', 'tmhDynamicLocale', tokenStorageService]);
    function tokenStorageService(authService, AUTH_EVENTS, $http, localStorage, $rootScope, common, tmhDynamicLocale) {
        var currentUser = localStorage.get('currentUser');
        var token = localStorage.get('token');
        var unAuthUser = {
            name: '',
            roles: [],
            id: null,
            departmentId:null
        };
        var self = this;
        var log = common.logger.getLogFn(serviceId);

        self.getUserName = getUserName;
        self.getUserRoles = getUserRoles;
        self.getUserId = getUserId;
        self.getUserDepartmentId = getUserDepartmentId;
        self.getUserLocale = getUserLocale;
        self.isLoggedIn = isLoggedIn;
        self.isAuthorized = isAuthorized;
        self.notifyLogin = notifyLogin;
        self.notifyLogout = notifyLogout;

        if (!isLoggedIn()) {
            currentUser = unAuthUser;
        }

        setTokenHeader();

        //$rootScope.$on(AUTH_EVENTS.loginRequired, nullifyToken);

        var unwatchWidget = $rootScope.$on(AUTH_EVENTS.loginWidgetReady, function () {
            if (isLoggedIn()) {
                setLocaleThenConfirmLogin();
            }
            unwatchWidget();
            unwatchWidget = null;
        });
        
        function isLoggedIn() {
            return !!token;
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

        function getUserLocale() {
            return currentUser.locale;
        }

        function getUserDepartmentId() {
            return currentUser.departmentId;
        }

        //check if the user is authorized to access the next route
        //this function can be also used on element level
        //e.g. <p ng-if="isAuthorized(authorizedRoles)">show this only to admins</p>
        function isAuthorized(authorizedRoles) {
            if (!isLoggedIn()) {return false;}
            if (typeof authorizedRoles === 'string') {
                authorizedRoles = authorizedRoles.split(',');
            }
            if (authorizedRoles.length === 1 && authorizedRoles[0] === '*') {
                return true;
            }
            return authorizedRoles.some(function (el) {
                currentUser.roles.indexOf(el) !== -1;
            });
        }

        function notifyLogin(data) {
            //2 scenarios - logged in or recredentialled
            var recredentialled; 
            if (currentUser.name) {
                if (currentUser.name !== data.fullName) {
                    recredentialled = false;
                    notifyLogout();
                } else {
                    recredentialled = true;
                }
            } else {
                recredentialled = false;
            }
            token = data.user_token || data.access_token;
            localStorage.set('token', token);
            setTokenHeader();
            if (recredentialled) {
                authService.loginConfirmed({ recredentialled: true }, replaceToken);
            } else {
                currentUser = {
                    name: data.fullName,
                    roles: data.userRoles.split(','),
                    id: data.userId,
                    locale: data.locale,
                    departmentId: data.departmentId
                };
                localStorage.set('currentUser', currentUser);
                //now broadcast
                setLocaleThenConfirmLogin();
            }
        }

        function setLocaleThenConfirmLogin(){
            //set locale once logged in 
            //not sure if having this function in the tokenStorageService is enough separation of concerns
            //however it does ensure user local is set before notifying login
            //which is important if any subscribers need to present locale specific information
            //although could have a userLocaleReady function
            moment.locale([currentUser.locale, "en-gb"]); //fallback on british format
            return tmhDynamicLocale.set(currentUser.locale.toLowerCase())
                .then(null, function (data) {
                    log.error({ msg:'unable to set user locale',data:data });
                }).finally(function () {
                    //only broadcast after the appropriate locale for the user is loaded
                    authService.loginConfirmed({ recredentialled: false }, replaceToken);
                }); //moment().localeData().longDateFormat('L').replace(/D/g, "d").replace(/Y/g, "y");
        }

        function notifyLogout() {
            currentUser = unAuthUser;
            localStorage.remove('currentUser');
            nullifyToken();
            //now broadcast
            authService.loginCancelled();
        }

        function nullifyToken() {
            localStorage.remove('token');
            token = null;
            setTokenHeader();
        }

        function setTokenHeader() {
            if (token) {
                $http.defaults.headers.common.authorization = 'Bearer ' + token;
            } else if ($http.defaults.headers.common.authorization) {
                delete $http.defaults.headers.common.authorization;
            }

        }

        function replaceToken(request) { //to be used on a 401 if the token has expired
            request.headers.authorization = 'Bearer ' + token;
            return request;
        }
    }

})();