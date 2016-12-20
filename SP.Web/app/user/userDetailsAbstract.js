(function (undefined) {
    'use strict';

    var serviceId = 'userDetails.abstract';
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

            var id = argObj.$routeParams.id;
            var professionalRoles;

            vm.activate = activate;
            vm.hotDrinks = [];
            vm.filterRoles = filterRoles;
            vm.institution = {};
            vm.institutions = [];
            vm.isNew = id === 'new';
            vm.participant = {};
            vm.professionalRoles = [];

            function activate() {
                var alertMessage;
                var promises = [datacontext.professionalRoles.findServerIfCacheEmpty({ expand: 'professionalRoleInstitutions' }).then(function (data) {
                    professionalRoles = data;
                }), datacontext.hotDrinks.findServerIfCacheEmpty().then(function (data) {
                    vm.hotDrinks = data;
                })];
                if (vm.isNew) {
                    vm.participant = datacontext.participants.create({ departmentId: argObj.$routeParams.departmentId });
                    promises.push(datacontext.departments.fetchByKey(argObj.$routeParams.departmentId, { expand: 'institution.culture' })
                        .then(function (data) {
                            vm.participant.department = data;
                            vm.institutions = [vm.participant.department.institution];
                        }));
                    alertMessage = "Register User";
                    return common.activateController(promises, controllerId).then(loaded);
                } // else
                datacontext.ready().then(function () {
                    var defer = $q.defer();
                    alertMessage = id ? "Update User" : "Update My Details";
                    promises.push(datacontext.participants.fetchByKey(id || tokenStorageService.getUserId(), { expand: 'roles' }).then(function (data) {
                        vm.participant = data;
                    }),
                        datacontext.institutions.all({ expand: 'culture' }).then(function (data) {
                            vm.institutions = data;
                        }));
                    common.activateController(promises, controllerId).then(
                        function () {
                            loaded();
                            $q.resolve.apply(this, arguments);
                        },
                        function () {
                            vm.log.error({ msg: 'Failed to load user data', data: arguments[0] });
                            $q.reject.apply(this, arguments);
                        });
                });

                function loaded() {
                    vm.institution = vm.participant.department.institution;
                    filterRoles();
                    vm.notifyViewModelLoaded();
                    vm.log(alertMessage + ' Activated');
                }
            }

            function filterRoles() {
                vm.professionalRoles = professionalRoles.filter(function (pr) {
                    return pr.professionalRoleInstitutions.some(isCurrentInstitution);
                });

                if (vm.professionalRoles.length === 0) {
                    vm.professionalRoles = professionalRoles;
                }

                function isCurrentInstitution(pri) {
                    return pri.institutionId === vm.institution.id;
                }
            }
        }
    }
})();