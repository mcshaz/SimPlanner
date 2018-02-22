"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
angular_1.default.module('app')
    .filter('fileDate', ['moment', fileDate])
    .filter('sizeKib', ['$filter', sizeKib]);
function fileDate(moment) {
    return function (date) {
        if (!date) {
            return '';
        }
        var dt = moment(date);
        return dt.format('L') + ' ' + dt.format('LT');
    };
}
function sizeKib($filter) {
    return function (bytes) {
        if (!bytes) {
            return '';
        }
        return $filter('number')(bytes / 1024, 1);
    };
}
//# sourceMappingURL=fileFilters.js.map