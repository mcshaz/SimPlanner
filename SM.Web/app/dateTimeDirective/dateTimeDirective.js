(function() {
    'use strict';
    var app = angular.module('app');
    var controllerId = 'smDatetime';
    app.directive(controllerId, [dateTimeDirective]);
    app.directive('smTimespan', [timeDirective]);

    function dateTimeDirective() {
        return {
            restrict: 'E',
            templateUrl: 'app/dateTimeDirective/dateTimeCtrl.html',
            require: 'ngModel',
            scope: {},
            bindToController: {
                datetime: "=ngModel",
                minDate: "=",
                maxDate: "="
                //required: "=ngRequired"
            },
            controller: dateTimeDirectiveController,
            controllerAs: 'dtvm'
        }
    }

    dateTimeDirectiveController.$inject = ['$scope'];

    function dateTimeDirectiveController($scope) {
        var vm = this;
        var timespan = timespanMinutes();
        vm.dpPopup = { isOpen: false };
        vm.maxDate;
        vm.minDate;
        vm.date;
        vm.time;
        vm.datetime;
        vm.openDp = openDp;
        vm.dateChange = dateChange;
        vm.timeChange = timeChange;
        vm.dateFormat = moment().localeData().longDateFormat('L').replace(/D/g, "d").replace(/Y/g, "y");
        var deregister = $scope.$watch(function () { return vm.datetime }, datetimeChange);

        function datetimeChange() {
            if (vm.datetime) {
                timespan.setMinutes(vm.datetime);
                vm.time = timespan.toString();
                vm.date = vm.datetime;
                deregister();
            }
        }

        function openDp() {
            vm.dpPopup.isOpen = true;
        }
        function timeChange() {
            timespan.parse(vm.time);
            dateChange();
        }

        function dateChange() {
            if (timespan.isValid && vm.date instanceof Date) {
                vm.datetime = timespan.setTime(vm.date)
            } else {
                vm.datetime = null;
            }
        }

    }

    function timeDirective() {
        return {
            restrict: 'A', //matches only attribute name
            require: 'ngModel',
            link: function (scope, element, attr, ngModelCtrl) {
                ngModelCtrl.$parsers.push(function (timeStr) {
                    return timespanMinutes(timeStr);
                });
                ngModelCtrl.$formatters.push(function (timespan) {
                    if (timespan && timespan.isValid) {
                        return timespan.parsed || timespan.toString();
                    }
                });
            }
        };
    }

    var dateParseFormats;
    function getDateParseFormats() {
        if (dateParseFormats) { return dateParseFormats; }
        var dateParseFormats = ["D MMM YYYY", "Do MMM YYYY", "Do MMMM YYYY", "D MMMM YYYY", "YYYY-MM-DD"]; //universal date formats

        //hackalert
        var splitChars = ['.', '/', '-']; //may well be ommisions
        var shortFormat = moment().localeData.longDateFormat('L');
        var splitter = new RegExp('[\\' + splitChars.join('\\') + ']');
        shortFormat = shortFormat.replace(/\bMM\b/, "M").replace(/\bDD\b/, "D");
        var dateComponents = shortFormat.split(splitter);
        if (dateComponents.length != 3) { throw new Error('unable to split locale date into date month year format'); }
        parseFormats.push(splitChars.map(function (el) {
            return dateComponents.join(el);
        }));
    }


})();