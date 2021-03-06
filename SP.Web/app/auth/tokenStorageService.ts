﻿import angular from 'angular';
import * as jQuery from 'jquery';
import * as moment from 'moment';


    var serviceId = 'tokenStorageService';
    var headerAuthVal = null; //to allow logging errors to server without circular reference
export default angular.module('app')
        .service(serviceId, ['authService', 'AUTH_EVENTS', '$http', 'localStorageService' , '$rootScope', 'common', 'tmhDynamicLocale', 'USER_ROLES',tokenStorageService]);
    function tokenStorageService(authService, AUTH_EVENTS, $http, localStorage, $rootScope, common, tmhDynamicLocale,USER_ROLES) {
        var currentUser = localStorage.get('currentUser');
        var token = localStorage.get('token');
        var unAuthUser = {
            name: '',
            roles: [],
            roleIds: [],
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
            if (typeof authorizedRoles === 'string') {
                authorizedRoles = authorizedRoles.split(',');
            }
            return authorizedRoles.some(function (rl) {
                switch (rl) {
                    case USER_ROLES.authenticated:
                        return isLoggedIn();
                    case USER_ROLES.anonymous:
                        return !isLoggedIn();
                    default:
                        return currentUser.roles.indexOf(rl) > -1 || currentUser.roleIds.indexOf(rl) > -1;
                }
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
                currentUser.roleIds = currentUser.roles.map(function (r) { return USER_ROLES[r[0].toLowerCase() + r.substring(1)];});
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
                headerAuthVal = $http.defaults.headers.common.authorization = 'Bearer ' + token;
            } else if ($http.defaults.headers.common.authorization) {
                delete $http.defaults.headers.common.authorization;
                headerAuthVal = null;
            }

        }

        function replaceToken(request) { //to be used on a 401 if the token has expired
            headerAuthVal = request.headers.authorization = 'Bearer ' + token;
            return request;
        }

        //bit of a hack putting this in here
        jQuery.ajaxSetup({
            beforeSend: function (xhr) {
                if (headerAuthVal) {
                    xhr.setRequestHeader('Authorization', headerAuthVal);
                }
            }
        });
    }
