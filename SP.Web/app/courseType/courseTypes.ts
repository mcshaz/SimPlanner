import angular from 'angular';

    var controllerId = 'courseTypes';
export default angular
        .module('app')
        .controller(controllerId, courseTypesCtrl).name;

    (courseTypesCtrl as any).$inject = ['common', 'datacontext'];
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
                            el.courseTypeScenarioRoles.sort(common.sortOnChildPropertyName('facultyScenarioRole', 'order'));
                        });
                    })], controllerId);
            });
        }
    }

