(function () {
    'use strict';
    angular.module('app')
    .filter('JSONtop', [JSONtop]);

    function JSONtop() {
        return function (object, option) {
            if (typeof object === 'object') {
                if (Array.isArray(object)) {
                    object = object.map(function (el) { topOfObject(el, option.objectProperties); });
                } else {
                    object = topOfObject(object, option.objectProperties);
                }
            }
            if (option.pretty){
                return JSON.stringify(object, null, '\t').replace(/\t/g, '&nbsp;').replace(/\n/g, '<br>');
            }
            return JSON.stringify(object);
        };
    }
    function topOfObject(object,objectProperties) {
        var returnVar = {};
        for (var p in object) {
            if (p && p.length) {
                var v = object[p];
                if (typeof v === 'object') {
                    if (Array.isArray(object)) {
                        returnVar[p] = "[...(" + object.length + ")]";
                    } else if (objectProperties) {
                        returnVar[p] = "{...}";
                    }
                } else {
                    returnVar[p] = v;
                }
            }
        }
        return returnVar;
    }
})();