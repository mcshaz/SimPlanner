(function (undefined) {
    'use strict';

    var serviceId = 'participantBase.abstract';
    angular.module('app').factory(serviceId,
        ['common', 'datacontext', '$q', 'controller.abstract', AbstractUserDetails]);

    function AbstractUserDetails(common, datacontext, $q, abstractController) {

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
                } else if (vm.participant && vm.participant.professionalRole && vm.professionalRoles.indexOf(vm.participant.professionalRole) === -1) {
                    vm.professionalRoles.push(vm.participant.professionalRole);
                }

                function isCurrentInstitution(pri) {
                    return pri.institutionId === vm.institution.id;
                }
            }
        }
    }
})();