(function(){
    'use strict';
    //usage {65 | timeFilter:'m':'hh:mm'}
    angular.module('app')
    .filter('timeFilter', function () {
        var conversions = {
            's': angular.identity,
            'm': function(value) { return value * 60; },
            'h': function(value) { return value * 3600; }
        };
        var testUiGridAggregateVals = /^([^\d]*)(\d+\.?\d*)$/;

        return function (value, unit, format) {
            var prefix = '';
            if (value === angular.undefined) { return ''; }
            if (typeof value === 'string') {
                value = testUiGridAggregateVals.exec(value);
                prefix = value[1];
                value = value[2];
            }
            value = parseFloat(value);
            var totalSeconds = conversions[(unit || 's')[0]](value);
            format = format || 'hh:mm:ss';

            return prefix + format.replace(/hh?/, function(capture){
                var h = Math.floor(totalSeconds / 3600);
                return capture.length === 1
                    ? h
                    : addPadding(h);
            }).replace(/mm?/, function(capture){
                var m = Math.floor(totalSeconds % 3600 / 60);
                return capture.length === 1
                    ? m
                    : addPadding(m);
            }).replace(/ss?/, function(capture){
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
})();