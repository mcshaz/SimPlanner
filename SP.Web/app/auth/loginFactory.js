"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
'use strict';
var serviceId = 'loginFactory';
angular_1.default.module('app')
    .factory(serviceId, ['$http', '$httpParamSerializerJQLike', 'tokenStorageService', 'common', '$q', '$sce', loginFactory]);
function loginFactory($http, $httpParamSerializerJQLike, tokenStorageService, common, $q, $sce) {
    var log = common.logger.getLogFn(serviceId);
    var service = {
        login: login,
        logout: logout,
        downloadFileLink: downloadFileLink,
        registerExternal: tokenStorageService.notifyLogin
    };
    return service;
    function login(credentials) {
        var loginData = {
            grant_type: 'password',
            username: credentials.username,
            password: credentials.password
        };
        return $http({
            method: 'POST',
            url: '/Token',
            data: $httpParamSerializerJQLike(loginData),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }).then(function (response) {
            tokenStorageService.notifyLogin(response.data);
            return response.data;
        });
    }
    function logout() {
        if (tokenStorageService.isLoggedIn()) {
            return $http({
                method: 'POST',
                url: 'api/Account/Logout',
                ignoreAuthModule: true
            }).then(function (response) {
                tokenStorageService.notifyLogout(response);
                log.success({ msg: 'logged out' });
            }, function (response) {
                log.warn({ msg: 'Logout failed', data: response });
            });
        }
        else {
            log.debug({ msg: 'logout called - Not logged in' });
            return $q.when();
        }
    }
    function downloadFileLink(actionName, entitySetId) {
        return $http({
            method: 'POST',
            url: 'api/Account/GenerateDownloadToken'
        }).then(function (response) {
            var params = {
                EntitySetId: entitySetId,
                Token: response.data,
                UserId: tokenStorageService.getUserId()
            };
            var location = common.windowOrigin() + '/api/utilities/' + actionName
                + '?' + $httpParamSerializerJQLike(params);
            return $sce.trustAsResourceUrl(location);
        }, log.error);
    }
}
//# sourceMappingURL=loginFactory.js.map