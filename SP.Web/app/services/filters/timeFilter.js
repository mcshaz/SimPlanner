"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
angular_1.default.module('app')
    .filter('timeFilter', function () {
    var conversions = {
        's': angular_1.default.identity,
        'm': function (value) { return value * 60; },
        'h': function (value) { return value * 3600; }
    };
    var testUiGridAggregateVals = /^([^\d]*)(\d+\.?\d*)$/;
    return function (value, unit, format) {
        var prefix = '';
        if (value === void 0) {
            return '';
        }
        if (typeof value === 'string') {
            value = testUiGridAggregateVals.exec(value);
            prefix = value[1];
            value = value[2];
        }
        value = parseFloat(value);
        var totalSeconds = conversions[(unit || 's')[0]](value);
        format = format || 'hh:mm:ss';
        return prefix + format.replace(/hh?/, function (capture) {
            var h = Math.floor(totalSeconds / 3600);
            return capture.length === 1
                ? h
                : addPadding(h);
        }).replace(/mm?/, function (capture) {
            var m = Math.floor(totalSeconds % 3600 / 60);
            return capture.length === 1
                ? m
                : addPadding(m);
        }).replace(/ss?/, function (capture) {
            var s = totalSeconds % 60;
            return capture.length === 1
                ? s
                : addPadding(s);
        });
    };
    function addPadding(value) {
        return value < 10
            ? '0' + value
            : value;
    }
});
//# sourceMappingURL=timeFilter.js.map