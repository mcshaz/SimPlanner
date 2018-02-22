"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
var serviceId = 'participantBase.abstract';
angular_1.default.module('app').factory(serviceId, ['datacontext', 'controller.abstract', AbstractUserDetails]);
function AbstractUserDetails(datacontext, abstractController) {
    return {
        constructor: Ctor
    };
    function Ctor(argObj) {
        var vm = this;
        var controllerId = argObj.controllerId;
        abstractController.constructor.call(this, {
            controllerId: controllerId,
            watchedEntityNames: 'participant',
            $scope: argObj.$scope
        });
        var professionalRoles;
        vm.getPromises = getPromises;
        vm.filterRoles = filterRoles;
        vm.institution = {};
        vm.participant = {};
        vm.professionalRoles = professionalRoles = [];
        function getPromises() {
            return [datacontext.professionalRoles.findServerIfCacheEmpty({ expand: 'professionalRoleInstitutions' }).then(function (data) {
                    professionalRoles = data;
                })];
        }
        function filterRoles() {
            vm.professionalRoles = professionalRoles.filter(function (pr) {
                return pr.professionalRoleInstitutions.some(isCurrentInstitution);
            });
            if (vm.professionalRoles.length === 0) {
                vm.professionalRoles = professionalRoles;
            }
            else if (vm.participant && vm.participant.professionalRole && vm.professionalRoles.indexOf(vm.participant.professionalRole) === -1) {
                vm.professionalRoles.push(vm.participant.professionalRole);
            }
            function isCurrentInstitution(pri) {
                return pri.institutionId === vm.institution.id;
            }
        }
    }
}
//# sourceMappingURL=participantBaseAbstract.js.map