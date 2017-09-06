(function () {
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

    var testSet = new Set([0]);
    if (testSet.size === 0) {
        //constructor doesnt take an iterable as an argument - thanks IE
        const BuiltinSet = Set;
        Set = function Set(iterable) {
            const set = new BuiltinSet();
            if (iterable) {
                iterable.forEach(set.add, set);
            }
            return set;
        };
        Set.prototype = BuiltinSet.prototype;
        Set.prototype.constructor = Set;
    }

    if (typeof testSet.values !== 'function') {
        Set.prototype.values = Set.prototype.keys = function () {
            return arrayToIterable(iterateForEach(this));
        };
    }
    if (typeof testSet.entries !== 'function') {
        Set.prototype.entries = function () {
            return arrayToIterable(iterateForEach(this).map(function (el) { return [el, el]; }));
        };
    }

    var testMap = new Map();
    if (typeof testSet.keys !== 'function') {
        Set.prototype.keys = function () {
            return arrayToIterable(iterateForEach(this));
        };
    }

    if (typeof testSet.values !== 'function') {
        Set.prototype.values = function () {
            return arrayToIterable(iterateForEach(this, true).map(function (el) { return el[1]; }));
        };
    }

    if (typeof testSet.entries !== 'function') {
        Set.prototype.entries = function () {
            return arrayToIterable(iterateForEach(this, true));
        };
    }

    function arrayToIterable(array) {
        if (!Array.isArray(array)) { throw new TypeError(); }
        var i = 0;
        return {
            next: function () {
                return {
                    value: iterable[i],
                    done: ++i < iterable.length
                };
            }
        };
    }

    function iterateForEach(arraylike, asKeyValArrays) {
        if (typeof arraylike.forEach !== 'function') {
            return false;
        }
        var tempArray = [];
        var addEl = asKeyValArrays
            ? function (val, key) { tempArray.push([key, val]); }
            : function (val) { tempArray.push(val); };
        arraylike.forEach(addEl);
        return tempArray;
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


    //same as instanceOf, but works through iframes
    function appWideInstanceOf(obj, typeName) {
        return Object.prototype.toString.call(obj) === '[object ' + typeName + ']';
    }
    function parseIterable(arraylike) {
        //worth considering the performance bypass in the line below
        //if (appWideInstanceOf(arraylike,'Array')){ return arraylike.slice(); }
        var done = false;
        var iterableResponse;
        var tempArray = [];

        // if the iterable doesn't have next;
        // it is an iterable if 'next' is a function but it has not been defined on
        // the object itself.
        if (typeof arraylike.next === 'function') {
            while (!done) {
                iterableResponse = arraylike.next();
                if (typeof iterableResponse.done === 'boolean') {
                    if (iterableResponse.done === true) {
                        done = true;
                        break;
                    }
                    //was using hasownProperty but changed as 'value' property might be inherited through prototype chain and could still be a valid iterable response
                    if ('value' in iterableResponse) {
                        tempArray.push(iterableResponse.value);
                    } else {
                        break;
                    }
                } else {
                    // it does not conform to the iterable pattern
                    break;
                }
            }
        }

        if (done) {
            return tempArray;
        } else {
            // something went wrong return false;
            return false;
        }
    }

    Object.defineProperty(Array, 'from', {
        configurable: true,
        value: function from(source) {
            // handle non-objects
            if (source === undefined || source === null) {
                throw new TypeError(source + ' is not an object');
            }

            // handle maps that are not functions
            if (1 in arguments && typeof arguments[1] !== 'function') {
                throw new TypeError(arguments[1] + ' is not a function');
            }

            var arraylike = typeof source === 'string' ? source.split('') : Object(source);
            var map = arguments[1];
            var scope = arguments[2];
            var array = [];
            var index = -1;
            var length = Math.min(Math.max(Number(arraylike.length) || 0, 0), 9007199254740991);
            var value;
            // variables for rebuilding array from iterator
            var arrayFromIterable = parseIterable(arraylike);

            //if it is a Map or a Set then handle them appropriately
            if (!arrayFromIterable) {
                if (appWideInstanceOf(arraylike, 'Map')) {
                    arrayFromIterable = iterateForEach(arraylike, true);
                } else if (appWideInstanceOf(arraylike, 'Set')) {
                    arrayFromIterable = iterateForEach(arraylike);
                }
            }

            if (arrayFromIterable) {
                arraylike = arrayFromIterable;
                length = arraylike.length;
            }

            while (++index < length) {
                value = arraylike[index];

                array[index] = map ? map.call(scope, value, index) : value;
            }

            array.length = length;

            return array;
        },
        writable: true
    });
}());