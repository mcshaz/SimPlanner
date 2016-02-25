(function() {
    'use strict';

    angular.module('app')
        .directive('timeinput', ['common',directive]);

    function directive(common) {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function (scope, element, attr, ngModelCtrl) {
                var beginDate; //this will be used in conjunction with a date input - we need to adjust the time component, but keep the date the same.
                ngModelCtrl.$parsers.push(function (valueStr) {
                    return common.dateUtilities.parseTime(valueStr, beginDate)
                });
                ngModelCtrl.$formatters.push(function (valueDate) {
                    if (valueDate instanceof Date) {
                        beginDate = valueDate;
                        return common.dateUtilities.timeFormatter(valueDate);
                    }
                });
            }
        };

    }
})();