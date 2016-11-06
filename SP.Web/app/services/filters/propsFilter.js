(function () {
    'use strict';
    //taken from ui-select examples
    angular.module('app').filter('propsFilter', [propsFilter]);
    function propsFilter() {
        return function (items, props) {
            var out = [];

            if (angular.isArray(items)) {
                var keys = Object.keys(props);

                items.forEach(function (item) {
                    if (keys.some(isMatchingString)) {
                        out.push(item);
                    }

                    function isMatchingString(prop) {
                        var text = props[prop].toLowerCase().split(' ');
                        return text.every(function (t) {
                            return item[prop]/*.toString().toLowerCase()*/.indexOf(t) !== -1;
                        });
                    }
                });
            } else {
                // Let the output be the input untouched
                out = item;
            }

            return out;
        };
    }
})();