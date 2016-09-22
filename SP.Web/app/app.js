(function () {
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
        'http-auth-interceptor', 
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
        'isteven-multi-select',
        'ngLocationUpdate',
        'mwl.calendar'
    ]);

    /*Constants regarding user login defined here*/
    app.constant('AUTH_EVENTS', {
            forbidden: 'event:auth-forbidden',
            loginRequired: 'event:auth-loginRequired',
            loginConfirmed: 'event:auth-loginConfirmed',
            loginCancelled: 'event:auth-loginCancelled',
            loginWidgetReady: 'event:auth-loginWidgetReady'
    })
    .constant('emptyGuid', window.emptyGuid)
    .config(['localStorageServiceProvider', function (localStorageServiceProvider) {
        localStorageServiceProvider
            .setPrefix('loginApp')
            .setStorageType('sessionStorage');
    }])
    .constant('ngAuthSettings', {
        apiServiceBaseUri: serviceBase,
        clientId: 'SimPlanner'
    })
    .config(['zDirectivesConfigProvider', configDirective])
    .config(['tmhDynamicLocaleProvider',
        function (tmhDynamicLocaleProvider) {
        tmhDynamicLocaleProvider.useStorage('$cookies');
        tmhDynamicLocaleProvider.localeLocationPattern("https://cdnjs.cloudflare.com/ajax/libs/angular-i18n/1.5.2/angular-locale_{{locale}}.min.js");
        }])
    //http://stackoverflow.com/questions/25470475/angular-js-format-minutes-in-template
    //usage {65 | timeFilter:'m':'hh:mm'}
    .filter('timeFilter', function () {
        var conversions = {
            's': angular.identity,
            'm': function(value) { return value * 60; },
            'h': function(value) { return value * 3600; }
        };
        var testUiGridAggregateVals = /^[\w\s]+:\s*/;

        return function (value, unit, format) {
            if (typeof value === 'string') {
                value = value.replace(testUiGridAggregateVals, '');
            }
            value = parseFloat(value);
            var totalSeconds = conversions[(unit || 's')[0]](value);
            format = format || 'hh:mm:ss';

            return format.replace(/hh?/, function(capture){
                var h = Math.floor(totalSeconds / 3600);
                return capture.length===1
                    ?h
                    :addPadding(h)
            }).replace(/mm?/, function(capture){
                var m = Math.floor((totalSeconds % 3600) / 60);
                return capture.length===1
                    ?m
                    :addPadding(m)
            }).replace(/ss?/, function(capture){
                var s = totalSeconds % 60;
                return capture.length===1
                    ?s
                    :addPadding(s)
            });
        };

        function addPadding(value) {
            return value < 10
                ? '0' + value
                : value;
        };
    });

    // Include $route to kick start the router.
    app.run(['tokenStorageService', 'entityManagerFactory', 'modelBuilder', '$rootScope', 'AUTH_EVENTS','$route',
    function (tokenStorageService, entityManagerFactory, modelBuilder, $rootScope, AUTH_EVENTS,$route) {
        entityManagerFactory.modelBuilder = modelBuilder.extendMetadata;

        $rootScope.$on('$routeChangeStart', function(event, next, current){
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
    }]);

    //Configure the Breeze Validation Directive for bootstrap 2
    function configDirective(cfg) {
        // Custom template with warning icon before the error message
        cfg.zRequiredTemplate = null;
    }
})();
//polyfill https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Array/find
if (!Array.prototype.find) {
    Array.prototype.find = function (predicate) {
        if (this === null) {
            throw new TypeError('Array.prototype.find called on null or undefined');
        }
        if (typeof predicate !== 'function') {
            throw new TypeError('predicate must be a function');
        }
        var list = Object(this);
        var length = list.length >>> 0;
        var thisArg = arguments[1];
        var value;

        for (var i = 0; i < length; i++) {
            value = list[i];
            if (predicate.call(thisArg, value, i, list)) {
                return value;
            }
        }
        return undefined;
    };
}

if (!Array.isArray) {
    Array.isArray = function (arg) {
        return Object.prototype.toString.call(arg) === '[object Array]';
    };
}

if (!String.prototype.startsWith) {
    String.prototype.startsWith = function (searchString, position) {
        position = position || 0;
        return this.substr(position, searchString.length) === searchString;
    };
}

if (!String.prototype.endsWith) {
    String.prototype.endsWith = function (searchString, position) {
        var subjectString = this.toString();
        if (typeof position !== 'number' || !isFinite(position) || Math.floor(position) !== position || position > subjectString.length) {
            position = subjectString.length;
        }
        position -= searchString.length;
        var lastIndex = subjectString.indexOf(searchString, position);
        return lastIndex !== -1 && lastIndex === position;
    };
}
/*
// Production steps of ECMA-262, Edition 5, 15.4.4.21
// Reference: http://es5.github.io/#x15.4.4.21
if (!Array.prototype.reduce) {
    Array.prototype.reduce = function (callback /*, initialValue*//*) {
        'use strict';
        if (this == null) {
            throw new TypeError('Array.prototype.reduce called on null or undefined');
        }
        if (typeof callback !== 'function') {
            throw new TypeError(callback + ' is not a function');
        }
        var t = Object(this), len = t.length >>> 0, k = 0, value;
        if (arguments.length == 2) {
            value = arguments[1];
        } else {
            while (k < len && !(k in t)) {
                k++;
            }
            if (k >= len) {
                throw new TypeError('Reduce of empty Array with no initial value');
            }
            value = t[k++];
        }
        for (; k < len; k++) {
            if (k in t) {
                value = callback(value, t[k], k, t);
            }
        }
        return value;
    };
}

*/