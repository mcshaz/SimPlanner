"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
angular_1.default.module('app').filter('propsFilter', [propsFilter]);
function propsFilter() {
    return function (items, props) {
        var out = [];
        if (angular_1.default.isArray(items)) {
            var keys = Object.keys(props);
            items.forEach(function (item) {
                if (keys.some(isMatchingString)) {
                    out.push(item);
                }
                function isMatchingString(prop) {
                    var text = props[prop].toLowerCase().split(' ');
                    return text.every(function (t) {
                        return item[prop].indexOf(t) !== -1;
                    });
                }
            });
        }
        else {
            out = items;
        }
        return out;
    };
}
//# sourceMappingURL=propsFilter.js.map