"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
(function () {
    'use strict';
    var commonModule = angular_1.default.module('common', []);
    commonModule.provider('commonConfig', function () {
        this.config = {};
        return {
            $get: function () {
                return {
                    config: this.config
                };
            }
        };
    });
    commonModule.factory('common', ['$q', '$rootScope', '$timeout', 'commonConfig', 'logger', 'collectionManager', 'arrayUtils', '$window', common]);
    function common($q, $rootScope, $timeout, commonConfig, logger, collectionManager, arrayUtils, $window) {
        var throttles = {};
        var service = {
            $broadcast: $broadcast,
            $timeout: $timeout,
            activateController: activateController,
            addCollectionItem: collectionManager.addItem,
            arrayUtils: arrayUtils,
            createSearchThrottle: createSearchThrottle,
            debouncedThrottle: debouncedThrottle,
            isEmptyObject: isEmptyObject,
            isNumber: isNumber,
            logger: logger,
            manageCollectionChange: collectionManager.manageCollectionChange,
            mapToCamelCase: mapToCamelCase,
            removeCollectionItem: collectionManager.removeItem,
            collectionChange: collectionManager.collectionChange,
            sortOnPropertyName: sortOnPropertyName,
            sortOnChildPropertyName: sortOnChildPropertyName,
            textContains: textContains,
            toSeparateWords: toSeparateWords,
            alphaNumericEqual: alphaNumericEqual,
            bootstrapSizes: getWindowSizes(),
            currentBootstrapSize: currentBootstrapSize,
            windowOrigin: windowOrigin
        };
        return service;
        function activateController(promises, controllerId) {
            return $q.all(promises).then(function () {
                var data = { controllerId: controllerId };
                $broadcast(commonConfig.config.controllerActivateSuccessEvent, data);
            });
        }
        function $broadcast() {
            var _args = [];
            for (var _i = 0; _i < arguments.length; _i++) {
                _args[_i] = arguments[_i];
            }
            return $rootScope.$broadcast.apply($rootScope, arguments);
        }
        function alphaNumericEqual(str1, str2) {
            var comp = /[^A-Za-z0-9]/g;
            return str1.toLowerCase().replace(comp, "") === str2.toLowerCase().replace(comp, "");
        }
        function createSearchThrottle(viewmodel, list, filteredList, filter, delay) {
            delay = +delay || 300;
            if (!filteredList) {
                filteredList = 'filtered' + list[0].toUpperCase() + list.substr(1).toLowerCase();
                filter = list + 'Filter';
            }
            var filterFn = function () {
                viewmodel[filteredList] = viewmodel[list].filter(function (item) {
                    return viewmodel[filter](item);
                });
            };
            return (function () {
                var filterInputTimeout;
                return function (searchNow) {
                    if (filterInputTimeout) {
                        $timeout.cancel(filterInputTimeout);
                        filterInputTimeout = null;
                    }
                    if (searchNow || !delay) {
                        filterFn();
                    }
                    else {
                        filterInputTimeout = $timeout(filterFn, delay);
                    }
                };
            })();
        }
        function debouncedThrottle(key, callback, delay, immediate) {
            var defaultDelay = 1000;
            delay = delay || defaultDelay;
            if (throttles[key]) {
                $timeout.cancel(throttles[key]);
                throttles[key] = void 0;
            }
            if (immediate) {
                callback();
            }
            else {
                throttles[key] = $timeout(callback, delay);
            }
        }
        function isNumber(val) {
            return /^[-]?\d+$/.test(val);
        }
        function isEmptyObject(val) {
            for (var _p in val) {
                return false;
            }
            return true;
        }
        function textContains(text, searchText) {
            return text && -1 !== text.toLowerCase().indexOf(searchText.toLowerCase());
        }
        function toSeparateWords(text) {
            return text.replace(/[a-z][A-Z]/g, function (match) { return match[0] + ' ' + match[1]; });
        }
        function sortOnPropertyName(propName) {
            return function (a, b) {
                if (a[propName] > b[propName]) {
                    return 1;
                }
                if (a[propName] < b[propName]) {
                    return -1;
                }
                return 0;
            };
        }
        function sortOnChildPropertyName(propName, childPropName) {
            return function (a, b) {
                if (a[propName][childPropName] > b[propName][childPropName]) {
                    return 1;
                }
                if (a[propName][childPropName] < b[propName][childPropName]) {
                    return -1;
                }
                return 0;
            };
        }
        function mapToCamelCase(obj) {
            var returnVar = {};
            var val;
            for (var p in obj) {
                if (p.length) {
                    val = obj[p];
                    returnVar[p[0].toLowerCase() + p.substr(1)] = typeof val === 'object'
                        ? Array.isArray(val)
                            ? val.map(mapToCamelCase)
                            : mapToCamelCase(val)
                        : val;
                }
            }
            return returnVar;
        }
        function windowOrigin() {
            var location = $window.location;
            return location.origin || location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '');
        }
        function getWindowSizes() {
            var min = 0;
            var sizeMap = new Map([['xs', 767], ['sm', 991], ['md', 1199], ['lg', Number.MAX_SAFE_INTEGER]]
                .map(function (kv) {
                var sizeRange = { size: kv[0], min: min, max: kv[1] };
                sizeRange.lte = lte.bind(sizeRange);
                var returnVar = [sizeRange.size, sizeRange];
                min = kv[1] + 1;
                return returnVar;
            }));
            return sizeMap;
            function lte(sz) {
                return this.min <= sizeMap.get(sz).min;
            }
        }
        function currentBootstrapSize() {
            var values = service.bootstrapSizes.values();
            var next = values.next();
            var i = 1;
            while (!$window.matchMedia('(max-width: ' + next.value.max + 'px)').matches) {
                next = values.next();
                if (i++ >= service.bootstrapSizes.size) {
                    break;
                }
            }
            return next.value;
        }
    }
})();
//# sourceMappingURL=common.js.map