"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
'use strict';
var controllerId = 'reviewRegistrationRequest';
angular_1.default
    .module('app')
    .controller(controllerId, reviewRegistrationRequest);
reviewRegistrationRequest.$inject = ['common', 'datacontext', '$routeParams'];
function reviewRegistrationRequest(common, datacontext, $routeParams) {
    var vm = this;
    var log = common.logger.getLogFn(controllerId);
    vm.approve = approve;
    vm.personRequesting = {};
    vm.reject = reject;
    activate();
    function activate() {
        var promises = [datacontext.institutions.fetchByKey($routeParams.id, { expand: 'departments.participants' }).then(function (data) {
                vm.personRequesting = data.departments.find(function (d) { return d.participants.length > 0; }).participants[0];
            })];
        common.activateController(promises, controllerId).then(function () {
            log.info('Activated Review Registration Request');
        });
    }
    function approve() {
        vm.personRequesting.adminApproved = vm.personRequesting.department.adminApproved = vm.personRequesting.department.institution.adminApproved = true;
        datacontext.save([vm.personRequesting, vm.personRequesting.department, vm.personRequesting.department.institution]);
    }
    function reject() {
        if (confirm("DELETE this user, department and institution?")) {
            var inst = datacontext.institutions.getByKey($routeParams.id, { expand: 'departments.participants' });
            var i = 0;
            var j = 0;
            var d, p;
            for (i = inst.departments.length - 1; i >= 0; i--) {
                d = inst.departments[i];
                for (j = d.participants.length - 1; j >= 0; j--) {
                    p = d.participants[j];
                    p.entityAspect.setDeleted();
                    p.department = d;
                }
                d.entityAspect.setDeleted();
                d.institution = inst;
            }
            inst.entityAspect.setDeleted();
            datacontext.save();
        }
    }
}
//# sourceMappingURL=reviewRegistrationRequest.js.map