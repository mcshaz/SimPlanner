(function () {
    'use strict';
    var controllerId = 'courseTypes';
    angular
        .module('app')
        .controller(controllerId, courseTypesCtrl);

    courseTypesCtrl.$inject = ['common', 'datacontext'];
    //changed $uibModalInstance to $scope to get the events

    function courseTypesCtrl(common, datacontext) {
        /* jshint validthis:true */
        var vm = this;
        vm.courseFormats = [];

        activate();

        function activate() {
            datacontext.ready().then(function () {
                common.activateController([
                    datacontext.courseTypes.all().then(function (data) {
                        data.sort(common.sortOnPropertyName('description'));
                        vm.courseFormats = data.reduce(function (prev, cur) {
                            return prev.concat(cur.courseFormats);
                        }, []);
                    })], controllerId)
            });
        }
    }

})();