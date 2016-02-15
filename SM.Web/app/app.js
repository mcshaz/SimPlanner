(function () {
    'use strict';
    var serviceBase = 'https://localhost:44300/'; // change to production host
    var app = angular.module('app', [
        // Angular modules 
        'ngAnimate',        // animations
        'ngRoute',          // routing
        'ngSanitize',       // sanitizes html bindings (ex: sidebar.js)

        // Custom modules 
        'common',           // common functions, logger, spinner
 //       'ui.router', 
        'http-auth-interceptor', 
        'LocalStorageModule',

        // 3rd Party Modules
        'ui.bootstrap'      // ui-bootstrap (ex: carousel, pagination, dialog)
    ]);

    /*Constants regarding user login defined here*/
    app.constant('USER_ROLES', {
            all: '*',
            siteAdmin: 'siteAdmin',
            institutionAdmin: 'institutionAdmin',
            faculty: 'faculty',
            participant: 'participant'
    })
        .constant('AUTH_EVENTS', {
            forbidden: 'event:auth-forbidden',
            loginRequired: 'event:auth-loginRequired',
            loginConfirmed: 'event:auth-loginConfirmed',
            loginCancelled: 'event:auth-loginCancelled'
        })
        .config(['localStorageServiceProvider', function (localStorageServiceProvider) {
            localStorageServiceProvider
                .setPrefix('loginApp')
                .setStorageType('sessionStorage');
        }])
        .constant('ngAuthSettings', {
            apiServiceBaseUri: serviceBase,
            clientId: 'simmanager'
        });
    // Handle routing errors and success events
    app.run(['$route', function ($route) {
        // Include $route to kick start the router.
    }]);
})();