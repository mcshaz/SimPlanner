import angular from 'angular';
    'use strict';

export default angular.module('common').factory('arrayUtils', [arrayUtils]);

    function arrayUtils() {
        return {
            toLookup: toLookupObject,
            toLookupArray: toLookupArray,
            takeWhile: takeWhile,
            removeFromArray: removeFromArray
        };
    }
    //use set now!
    function toLookupObject(array, lookupFunction, outputFunction) {
        var returnVar = {};
        outputFunction = outputFunction || doNothing;
        array.forEach(function (el) {
            var key = lookupFunction(el);
            if (!returnVar[key]) {
                returnVar[key] = [];
            }
            returnVar[key].push(outputFunction(el));
        });

        return returnVar;
    }

    function toLookupArray(array, lookupFunction, outputFunction) {
        var returnVar = [];
        outputFunction = outputFunction || doNothing;
        array.forEach(function (el) {
            var key = lookupFunction(el);
            if (!returnVar[key]) {
                returnVar[key] = [];
            }
            returnVar[key].push(outputFunction(el));
        });

        return returnVar;
    }

    function takeWhile(source, predicate) {
        var stopIndex = source.length;

        source.some(function (n, index) {
            return predicate(n, index) ? false : ((stopIndex = index), true);
        });

        return source.slice(0, stopIndex);
    }

    function removeFromArray(arr /*,elements to remove*/) {
        var indx;
        var i = 1;
        for (; i < arguments.length; i++) {
            indx = arr.indexOf(arguments[i]);
            if (indx > -1) {
                arr.splice(indx, 1);
            }
        }
        return arr;
    }

    function doNothing(obj) { return obj; }
