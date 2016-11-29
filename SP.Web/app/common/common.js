(function () {
    'use strict';

    // Define the common module 
    // Contains services:
    //  - common
    //  - logger
    //  - spinner
    var commonModule = angular.module('common', []);

    // Must configure the common service and set its 
    // events via the commonConfigProvider
    commonModule.provider('commonConfig', function () {
        this.config = {
            // These are the properties we need to set
            //controllerActivateSuccessEvent: '',
            //spinnerToggleEvent: ''
        };

        this.$get = function () {
            return {
                config: this.config
            };
        };
    });

    commonModule.factory('common',
        ['$q', '$rootScope', '$timeout', 'commonConfig', 'logger', '$http', 'collectionManager', common]);

    function common($q, $rootScope, $timeout, commonConfig, logger, $http, collectionManager) {
        var throttles = {};

        var service = {
            // common angular dependencies
            $broadcast: $broadcast,
            $timeout: $timeout,
            // generic
            activateController: activateController,
            addCollectionItem: collectionManager.addItem,
            createSearchThrottle: createSearchThrottle,
            debouncedThrottle: debouncedThrottle,
            isEmptyObject: isEmptyObject,
            isNumber: isNumber,
            logger: logger, // for accessibility
            manageCollectionChange: collectionManager.manageCollectionChange,  // for accessibility
            mapToCamelCase: mapToCamelCase,
            removeCollectionItem: collectionManager.removeItem,
            collectionChange: collectionManager.collectionChange,
            sortOnPropertyName: sortOnPropertyName,
            sortOnChildPropertyName: sortOnChildPropertyName,
            textContains: textContains,
            toSeparateWords: toSeparateWords,
            alphaNumericEqual: alphaNumericEqual
        };

        return service;

        function activateController(promises, controllerId) {
            return $q.all(promises).then(function (eventArgs) {
                var data = { controllerId: controllerId };
                $broadcast(commonConfig.config.controllerActivateSuccessEvent, data);
            });
        }

        function $broadcast() {
            return $rootScope.$broadcast.apply($rootScope, arguments);
        }

        function alphaNumericEqual(str1, str2){
            var comp = /[^A-Za-z0-9]/g;
            return str1.toLowerCase().replace(comp,"") === str2.toLowerCase().replace(comp,"");
        }

        function createSearchThrottle(viewmodel, list, filteredList, filter, delay) {
            // After a delay, search a viewmodel's list using 
            // a filter function, and return a filteredList.

            // custom delay or use default
            delay = +delay || 300;
            // if only vm and list parameters were passed, set others by naming convention 
            if (!filteredList) {
                // assuming list is named sessions, filteredList is filteredSessions
                filteredList = 'filtered' + list[0].toUpperCase() + list.substr(1).toLowerCase(); // string
                // filter function is named sessionFilter
                filter = list + 'Filter'; // function in string form
            }

            // create the filtering function we will call from here
            var filterFn = function () {
                // translates to ...
                // vm.filteredSessions 
                //      = vm.sessions.filter(function(item( { returns vm.sessionFilter (item) } );
                viewmodel[filteredList] = viewmodel[list].filter(function(item) {
                    return viewmodel[filter](item);
                });
            };

            return (function () {
                // Wrapped in outer IFFE so we can use closure 
                // over filterInputTimeout which references the timeout
                var filterInputTimeout;

                // return what becomes the 'applyFilter' function in the controller
                return function(searchNow) {
                    if (filterInputTimeout) {
                        $timeout.cancel(filterInputTimeout);
                        filterInputTimeout = null;
                    }
                    if (searchNow || !delay) {
                        filterFn();
                    } else {
                        filterInputTimeout = $timeout(filterFn, delay);
                    }
                };
            })();
        }

        function debouncedThrottle(key, callback, delay, immediate) {
            // Perform some action (callback) after a delay. 
            // Track the callback by key, so if the same callback 
            // is issued again, restart the delay.

            var defaultDelay = 1000;
            delay = delay || defaultDelay;
            if (throttles[key]) {
                $timeout.cancel(throttles[key]);
                throttles[key] = undefined;
            }
            if (immediate) {
                callback();
            } else {
                throttles[key] = $timeout(callback, delay);
            }
        }

        function isNumber(val) {
            // negative or positive
            return /^[-]?\d+$/.test(val);
        }

        function isEmptyObject(val) {
            for (var p in val) {
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
                // a must be equal to b
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
                // a must be equal to b
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
                            ?val.map(mapToCamelCase)
                            :mapToCamelCase(val)
                        :val;
                }
            }
            return returnVar;
        }
    }
})();