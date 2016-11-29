(function () {
    'use strict';

    angular.module('common').factory('arrayUtils', [arrayUtils]);

    function arrayUtils() {
        return {
            toLookup: toLookup
        };
    }

    function toLookup(array, lookupFunction, outputFunction) {
        var returnVar = {};
        if (!outputFunction) {
            ouputFunction = function (obj) { return obj; };
        }
        array.forEach(function (el) {
            var key = lookupFunction(el);
            if (!returnVar[key]) {
                returnVar[key] = [];
            }
            returnVar[key].push(outputFunction(el));
        });

        return returnVar;
    }
})();