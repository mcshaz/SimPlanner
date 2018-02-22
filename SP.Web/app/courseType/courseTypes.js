"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
var controllerId = 'courseTypes';
angular_1.default
    .module('app')
    .controller(controllerId, courseTypesCtrl);
courseTypesCtrl.$inject = ['common', 'datacontext'];
function courseTypesCtrl(common, datacontext) {
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
                })
            ], controllerId);
        });
    }
}
//# sourceMappingURL=courseTypes.js.map