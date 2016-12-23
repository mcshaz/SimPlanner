(function (undefined) {
    'use strict';

    var serviceId = 'userDetails.abstract';
    angular.module('app').factory(serviceId,
        ['common', 'datacontext', '$q', 'participantBase.abstract', 'tokenStorageService', AbstractUserDetails]);

    function AbstractUserDetails(common, datacontext, $q, abstractController, tokenStorageService) {

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
            vm.institutions = [];
            vm.isNew = id === 'new';

            function activate() {
                var alertMessage;
                var promises = vm.getPromises();
                promises.push(datacontext.hotDrinks.findServerIfCacheEmpty().then(function (data) {
                    vm.hotDrinks = data;
                }));
                if (vm.isNew) {
                    vm.participant = datacontext.participants.create({ departmentId: argObj.$routeParams.departmentId });
                    promises.push(datacontext.departments.fetchByKey(argObj.$routeParams.departmentId, { expand: 'institution.culture' })
                        .then(function (data) {
                            vm.participant.department = data;
                            vm.institutions = [vm.participant.department.institution];
                        }));
                    alertMessage = "Register User";
                    return common.activateController(promises, controllerId).then(loaded);
                } // else {
                var defer = $q.defer();
                datacontext.ready().then(function () {
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
                return defer.promise;

                function loaded() {
                    vm.institution = vm.participant.department.institution;
                    vm.filterRoles();
                    vm.notifyViewModelLoaded();
                    vm.log(alertMessage + ' Activated');
                }
            }
        }
    }
})();