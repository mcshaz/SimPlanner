import angular from 'angular';

    //taken from ui-select examples
export default angular.module('app').filter('propsFilter', [propsFilter]);
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
                out = items;
            }

            return out;
        };
    }
