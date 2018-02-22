import angular from 'angular';

import 'angular-animate';
import 'angular-route';
import 'angular-sanitize';
import 'angular-cookies';
import 'angular-messages';

import 'moment'
import 'moment/locale/en-au';
import 'moment/locale/en-ca';
import 'moment/locale/en-gb';
import 'moment/locale/en-ie';
import 'moment/locale/en-nz';
import 'angular-moment';
import 'angular-http-auth';
import 'angular-local-storage';

/*utilities*/
import 'angular-dynamic-Locale';
/*multi-select*/
import 'ui-select';
/*end-multi-select*/

import 'angular-location-update';

/*Bootstrap-UI*/
//import 'bootstrap';
import 'angular-bootstrap';
/*typeahead*/
/* whole library for now - otherwise have to work out how to include templates
import 'position';
import 'debounce';
import 'typeahead';
*/
/*sortable*/
import 'angular-ui-sortable';
import 'angular-ui-grid';

/* TODO Touch device - sortable, calendar drag and drop */
/*import 'jquery.ui.touch-punch';
/*calendar*/
import 'angular-bootstrap-calendar';
//import 'angular-bootstrap-calendar-tpls';

/*Breeze*/
import 'breeze-client';
/*my breeze scripts*/
import { emptyGuid } from './services/customValidators';
import './services/customValidators';
/*my breeze scripts*/
//import 'breeze.bridge.angular';
//import 'breeze.savequeuing';
/*
import 'breeze.directives';
*/
//import 'breeze-client-labs';
import 'breeze-client-labs/breeze.savequeuing';
import './directives/custom-breeze-directive';

/* common.angularStrap Modules */
import 'angular-strap';
//import 'angular-strap.tpl';


'use strict';
var serviceBase = 'https://localhost:44300/'; // change to production host
var app = angular.module('app', [
    // Angular modules 
    'ngAnimate',        // animations
    'ngRoute',          // routing
    'ngSanitize',       // sanitizes html bindings (ex: sidebar.js)
    'ngCookies',
    'ngMessages',
    // Custom modules 
    'common',           // common functions, logger, spinner
//       'ui.router', 
    'angular-http-auth', 
    'LocalStorageModule',

    // 3rd Party Modules
    'mgcrea.ngStrap',
    'ui.bootstrap', //put specific ui modules here if using limited size package
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
    //"com.2fdevs.videogular",
    //"com.2fdevs.videogular.plugins.controls",
    //"com.2fdevs.videogular.plugins.overlayplay",
    //"com.2fdevs.videogular.plugins.buffering",
    //"com.2fdevs.videogular.plugins.dash"
]);

/*Constants regarding user login defined here*/
app.constant('AUTH_EVENTS', {
    forbidden: 'event:auth-forbidden',
    loginRequired: 'event:auth-loginRequired',
    loginConfirmed: 'event:auth-loginConfirmed',
    loginCancelled: 'event:auth-loginCancelled',
    loginWidgetReady: 'event:auth-loginWidgetReady'
})
.constant('emptyGuid', emptyGuid)
.config(['localStorageServiceProvider', function (localStorageServiceProvider) {
    localStorageServiceProvider.setPrefix('loginApp')
        .setStorageType('sessionStorage');
}])
.constant('ngAuthSettings', {
    apiServiceBaseUri: serviceBase,
    clientId: 'SimPlanner'
})
.config(['zDirectivesConfigProvider', function (zDirectivesConfigProvider) {
    // Custom template with warning icon before the error message
    zDirectivesConfigProvider.zRequiredTemplate = null;
}])
.config(['tmhDynamicLocaleProvider', function (tmhDynamicLocaleProvider) {
    tmhDynamicLocaleProvider.useStorage('$cookies');
    tmhDynamicLocaleProvider.localeLocationPattern("https://cdnjs.cloudflare.com/ajax/libs/angular-i18n/" + angular.version.full + "/angular-locale_{{locale}}.min.js");
}])
.config(['uiSelectConfig', function (uiSelectConfig) {
    uiSelectConfig.theme = 'bootstrap';
    //uiSelectConfig.resetSearchInput = true;
    uiSelectConfig.appendToBody = true;
}]);
//http://stackoverflow.com/questions/25470475/angular-js-format-minutes-in-template

// Include $route to kick start the router.
app.run(['tokenStorageService', 'entityManagerFactory', 'modelBuilder', '$rootScope', 'AUTH_EVENTS','$route',
    function (tokenStorageService, entityManagerFactory, modelBuilder, $rootScope, AUTH_EVENTS, _$route) {
        entityManagerFactory.modelBuilder = modelBuilder.extendMetadata;

        $rootScope.$on('$routeChangeStart', function(_event, next, current){
            if (next.access && next.access.requiresLogin && !tokenStorageService.isLoggedIn()) {
                if (!current) {//page loading
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

export { app };

