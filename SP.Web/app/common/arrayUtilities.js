"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
'use strict';
angular_1.default.module('common').factory('arrayUtils', [arrayUtils]);
function arrayUtils() {
    return {
        toLookup: toLookupObject,
        toLookupArray: toLookupArray,
        takeWhile: takeWhile,
        removeFromArray: removeFromArray
    };
}
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
function removeFromArray(arr) {
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
//# sourceMappingURL=arrayUtilities.js.map