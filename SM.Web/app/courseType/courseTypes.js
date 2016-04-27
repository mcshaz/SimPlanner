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
        vm.courseTypes = [];

        activate();

        function activate() {
            datacontext.ready().then(function () {
                common.activateController([
                    datacontext.courseTypes.findServerIfCacheEmpty({ expand: ['courseFormats', 'scenarios', 'courseTypeScenarioRoles.facultyScenarioRole', 'courseTypeDepartments'] }).then(function (data) {
                        data.sort(common.sortOnPropertyName('description'));
                        vm.courseTypes = data;
                        data.forEach(function (el) {
                            el.courseTypeScenarioRoles.sort(common.sortOnChildPropertyName('facultyScenarioRole','order'));
                        });
                    })], controllerId)
            });
        }
    }

})();