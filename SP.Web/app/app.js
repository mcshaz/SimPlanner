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
        'ui.select',
        'ngLocationUpdate',
        'mwl.calendar',
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
    .constant('emptyGuid', window.emptyGuid)
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

if (!Array.from) {
    Array.from = (function () {
        var toStr = Object.prototype.toString;
        var isCallable = function (fn) {
            return typeof fn === 'function' || toStr.call(fn) === '[object Function]';
        };
        var toInteger = function (value) {
            var number = Number(value);
            if (isNaN(number)) { return 0; }
            if (number === 0 || !isFinite(number)) { return number; }
            return (number > 0 ? 1 : -1) * Math.floor(Math.abs(number));
        };
        var maxSafeInteger = Math.pow(2, 53) - 1;
        var toLength = function (value) {
            var len = toInteger(value);
            return Math.min(Math.max(len, 0), maxSafeInteger);
        };

        // The length property of the from method is 1.
        return function from(arrayLike/*, mapFn, thisArg */) {
            // 1. Let C be the this value.
            var C = this;

            // 2. Let items be ToObject(arrayLike).
            var items = Object(arrayLike);

            // 3. ReturnIfAbrupt(items).
            if (arrayLike === null || arrayLike === undefined) {
                throw new TypeError("Array.from requires an array-like object - not null or undefined");
            }

            // 4. If mapfn is undefined, then let mapping be false.
            var mapFn = arguments.length > 1 ? arguments[1] : void undefined;
            var T;
            if (typeof mapFn !== 'undefined') {
                // 5. else
                // 5. a If IsCallable(mapfn) is false, throw a TypeError exception.
                if (!isCallable(mapFn)) {
                    throw new TypeError('Array.from: when provided, the second argument must be a function');
                }

                // 5. b. If thisArg was supplied, let T be thisArg; else let T be undefined.
                if (arguments.length > 2) {
                    T = arguments[2];
                }
            }

            // 10. Let lenValue be Get(items, "length").
            // 11. Let len be ToLength(lenValue).
            var len = toLength(items.length);

            // 13. If IsConstructor(C) is true, then
            // 13. a. Let A be the result of calling the [[Construct]] internal method of C with an argument list containing the single item len.
            // 14. a. Else, Let A be ArrayCreate(len).
            var A = isCallable(C) ? Object(new C(len)) : new Array(len);

            // 16. Let k be 0.
            var k = 0;
            // 17. Repeat, while k < len… (also steps a - h)
            var kValue;
            while (k < len) {
                kValue = items[k];
                if (mapFn) {
                    A[k] = typeof T === 'undefined' ? mapFn(kValue, k) : mapFn.call(T, kValue, k);
                } else {
                    A[k] = kValue;
                }
                k += 1;
            }
            // 18. Let putStatus be Put(A, "length", len, true).
            A.length = len;
            // 20. Return A.
            return A;
        };
    }());
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