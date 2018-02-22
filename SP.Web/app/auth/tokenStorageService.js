"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
var jQuery = require("jquery");
var moment = require("moment");
var serviceId = 'tokenStorageService';
var headerAuthVal = null;
angular_1.default.module('app')
    .service(serviceId, ['authService', 'AUTH_EVENTS', '$http', 'localStorageService', '$rootScope', 'common', 'tmhDynamicLocale', 'USER_ROLES', tokenStorageService]);
function tokenStorageService(authService, AUTH_EVENTS, $http, localStorage, $rootScope, common, tmhDynamicLocale, USER_ROLES) {
    var currentUser = localStorage.get('currentUser');
    var token = localStorage.get('token');
    var unAuthUser = {
        name: '',
        roles: [],
        roleIds: [],
        id: null,
        departmentId: null
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
    function getUserRoles() {
        return currentUser.roles.slice(0);
    }
    function getUserLocale() {
        return currentUser.locale;
    }
    function getUserDepartmentId() {
        return currentUser.departmentId;
    }
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
        var recredentialled;
        if (currentUser.name) {
            if (currentUser.name !== data.fullName) {
                recredentialled = false;
                notifyLogout();
            }
            else {
                recredentialled = true;
            }
        }
        else {
            recredentialled = false;
        }
        token = data.user_token || data.access_token;
        localStorage.set('token', token);
        setTokenHeader();
        if (recredentialled) {
            authService.loginConfirmed({ recredentialled: true }, replaceToken);
        }
        else {
            currentUser = {
                name: data.fullName,
                roles: data.userRoles.split(','),
                id: data.userId,
                locale: data.locale,
                departmentId: data.departmentId
            };
            currentUser.roleIds = currentUser.roles.map(function (r) { return USER_ROLES[r[0].toLowerCase() + r.substring(1)]; });
            localStorage.set('currentUser', currentUser);
            setLocaleThenConfirmLogin();
        }
    }
    function setLocaleThenConfirmLogin() {
        moment.locale([currentUser.locale, "en-gb"]);
        return tmhDynamicLocale.set(currentUser.locale.toLowerCase())
            .then(null, function (data) {
            log.error({ msg: 'unable to set user locale', data: data });
        }).finally(function () {
            authService.loginConfirmed({ recredentialled: false }, replaceToken);
        });
    }
    function notifyLogout() {
        currentUser = unAuthUser;
        localStorage.remove('currentUser');
        nullifyToken();
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
        }
        else if ($http.defaults.headers.common.authorization) {
            delete $http.defaults.headers.common.authorization;
            headerAuthVal = null;
        }
    }
    function replaceToken(request) {
        headerAuthVal = request.headers.authorization = 'Bearer ' + token;
        return request;
    }
    jQuery.ajaxSetup({
        beforeSend: function (xhr) {
            if (headerAuthVal) {
                xhr.setRequestHeader('Authorization', headerAuthVal);
            }
        }
    });
}
//# sourceMappingURL=tokenStorageService.js.map