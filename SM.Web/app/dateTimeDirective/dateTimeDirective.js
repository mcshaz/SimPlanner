(function() {
    'use strict';
    var app = angular.module('app');
    var controllerId = 'smDatetime';
    app.directive(controllerId, [dateTimeDirective]);

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

        vm.maxDate;
        vm.minDate;
        vm.datetime;
        vm.openDp = openDp;
        vm.dpPopup = { isOpen: false };
        vm.dateFormat = moment().localeData().longDateFormat('L').replace(/D/g, "d").replace(/Y/g, "y");


        function openDp() {
            vm.dpPopup.isOpen = true;
        }


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