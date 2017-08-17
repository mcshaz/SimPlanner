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

    if (new Set([0]).size === 0) {
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

// Wrapped in IIFE to prevent leaking to global scope.
//https://github.com/Financial-Times/polyfill-service/blob/master/polyfills/Array/from/polyfill.js
    if (!Array.from) {
        function parseIterable(arrayLike) {
            var done = false;
            var iterableResponse;
            var tempArray = [];

            // if the iterable doesn't have next;
            // it is an iterable if 'next' is a function but it has not been defined on
            // the object itself.
            if (typeof arrayLike.next === 'function') {
                while (!done) {
                    iterableResponse = arrayLike.next();
                    if (
                        iterableResponse.hasOwnProperty('value') &&
                        iterableResponse.hasOwnProperty('done')
                    ) {
                        if (iterableResponse.done === true) {
                            done = true;
                            break;

                            // handle the case where the done value is not Boolean
                        } else if (iterableResponse.done !== false) {
                            break;
                        }

                        tempArray.push(iterableResponse.value);
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

        function iterateForEach(arrayLike, asKeyValArrays) {
            if (typeof arrayLike.forEach !== 'function') {
                return false;
            }
            var tempArray = [];
            var addEl = asKeyValArrays
                ? function (val, key) { tempArray.push([key, val]); }
                : function (val) { tempArray.push(val); };
            arrayLike.forEach(addEl);
            return tempArray;
        }

        Object.defineProperty(Array, 'from', {
            configurable: true,
            value: function from(source) {
                // handle non-objects
                if (source === undefined || source === null) {
                    throw new TypeError(source + ' is not an object');
                }

                // handle maps that are not functions
                if (1 in arguments && !(arguments[1] instanceof Function)) {
                    throw new TypeError(arguments[1] + ' is not a function');
                }

                var arrayLike = typeof source === 'string' ? source.split('') : Object(source);
                var map = arguments[1];
                var scope = arguments[2];
                var array = [];
                var index = -1;
                var length = Math.min(Math.max(Number(arrayLike.length) || 0, 0), 9007199254740991);
                var value;

                // variables for rebuilding array from iterator
                var arrayFromIterable;

                // if it is an iterable treat like one
                arrayFromIterable = parseIterable(arrayLike);

                if (!arrayFromIterable) {
                    if (arrayLike instanceof Map) {
                        arrayFromIterable = iterateForEach(arrayLike,  true);
                    } else if (arrayLike.forEach) {
                        arrayFromIterable = iterateForEach(arrayLike);
                    }
                }

                if (arrayFromIterable) {
                    arrayLike = arrayFromIterable;
                    length = arrayFromIterable.length;
                }

                while (++index < length) {
                    value = arrayLike[index];

                    array[index] = map ? map.call(scope, value, index) : value;
                }

                array.length = length;

                return array;
            },
            writable: true
        });
    }
}());