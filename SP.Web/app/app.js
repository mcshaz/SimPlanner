"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
require("angular-animate");
require("angular-route");
require("angular-sanitize");
require("angular-cookies");
require("angular-messages");
require("moment");
require("moment/locale/en-au");
require("moment/locale/en-ca");
require("moment/locale/en-gb");
require("moment/locale/en-ie");
require("moment/locale/en-nz");
require("angular-moment");
require("angular-http-auth");
require("angular-local-storage");
require("angular-dynamic-Locale");
require("ui-select");
require("angular-location-update");
require("angular-bootstrap");
require("angular-ui-sortable");
require("angular-ui-grid");
require("angular-bootstrap-calendar");
require("breeze-client");
var customValidators_1 = require("./services/customValidators");
require("./services/customValidators");
require("breeze-client-labs/breeze.savequeuing");
require("./directives/custom-breeze-directive");
require("angular-strap");
'use strict';
var serviceBase = 'https://localhost:44300/';
var app = angular_1.default.module('app', [
    'ngAnimate',
    'ngRoute',
    'ngSanitize',
    'ngCookies',
    'ngMessages',
    'common',
    'angular-http-auth',
    'LocalStorageModule',
    'mgcrea.ngStrap',
    'ui.bootstrap',
    'angularMoment',
    'breeze.angular',
    'breeze.directives',
    'tmh.dynamicLocale',
    'ui.sortable',
    'ui.grid',
    'ui.grid.pagination',
    'ui.grid.grouping',
    'ui.select',
    'ngLocationUpdate',
    'mwl.calendar'
]);
exports.app = app;
app.constant('AUTH_EVENTS', {
    forbidden: 'event:auth-forbidden',
    loginRequired: 'event:auth-loginRequired',
    loginConfirmed: 'event:auth-loginConfirmed',
    loginCancelled: 'event:auth-loginCancelled',
    loginWidgetReady: 'event:auth-loginWidgetReady'
})
    .constant('emptyGuid', customValidators_1.emptyGuid)
    .config(['localStorageServiceProvider', function (localStorageServiceProvider) {
        localStorageServiceProvider.setPrefix('loginApp')
            .setStorageType('sessionStorage');
    }])
    .constant('ngAuthSettings', {
    apiServiceBaseUri: serviceBase,
    clientId: 'SimPlanner'
})
    .config(['zDirectivesConfigProvider', function (zDirectivesConfigProvider) {
        zDirectivesConfigProvider.zRequiredTemplate = null;
    }])
    .config(['tmhDynamicLocaleProvider', function (tmhDynamicLocaleProvider) {
        tmhDynamicLocaleProvider.useStorage('$cookies');
        tmhDynamicLocaleProvider.localeLocationPattern("https://cdnjs.cloudflare.com/ajax/libs/angular-i18n/" + angular_1.default.version.full + "/angular-locale_{{locale}}.min.js");
    }])
    .config(['uiSelectConfig', function (uiSelectConfig) {
        uiSelectConfig.theme = 'bootstrap';
        uiSelectConfig.appendToBody = true;
    }]);
app.run(['tokenStorageService', 'entityManagerFactory', 'modelBuilder', '$rootScope', 'AUTH_EVENTS', '$route',
    function (tokenStorageService, entityManagerFactory, modelBuilder, $rootScope, AUTH_EVENTS, _$route) {
        entityManagerFactory.modelBuilder = modelBuilder.extendMetadata;
        $rootScope.$on('$routeChangeStart', function (_event, next, current) {
            if (next.access && next.access.requiresLogin && !tokenStorageService.isLoggedIn()) {
                if (!current) {
                    var unwatchWidget = $rootScope.$on(AUTH_EVENTS.loginWidgetReady, function () {
                        $rootScope.$broadcast(AUTH_EVENTS.loginRequired);
                        unwatchWidget();
                        unwatchWidget = null;
                    });
                }
                $rootScope.$broadcast(AUTH_EVENTS.loginRequired);
            }
        });
    }
]);
//# sourceMappingURL=app.js.map