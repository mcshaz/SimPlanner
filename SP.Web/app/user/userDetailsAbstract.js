(function (undefined) {
    'use strict';

    var serviceId = 'userDetails.abstract';
    angular.module('app').factory(serviceId,
        ['common', 'datacontext', '$q', 'participantBase.abstract', 'tokenStorageService', '$routeParams', AbstractUserDetails]);

    function AbstractUserDetails(common, datacontext, $q, abstractParticipantBase, tokenStorageService, $routeParams) {

        return {
            constructor: Ctor
        };

        function Ctor(argObj) {
            var vm = this;
            var controllerId = argObj.controllerId;
            abstractParticipantBase.constructor.call(this, {
                controllerId: controllerId,
                watchedEntityNames: 'participant',
                $scope: argObj.$scope
            });

            var id = argObj.$routeParams.id;
            var professionalRoles;

            vm.activate = activate;
            vm.hotDrinks = [];
            vm.institutions = [];
            vm.isLoggedIn = tokenStorageService.isLoggedIn();
            vm.isNew = id === 'new';
            vm.emailChanged = vm.isNew ? emailChanged : angular.noop;

            function activate() {
                var alertMessage;
                var promises = vm.getPromises();
                promises.push(datacontext.hotDrinks.findServerIfCacheEmpty().then(function (data) {
                    vm.hotDrinks = data;
                }));
                if (vm.isNew && $routeParams.departmentId) {
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
                    promises.push(datacontext.institutions.all({ expand: 'culture' }).then(function (data) {
                        vm.institutions = data;
                    }))
                    if (vm.isNew) {
                        vm.participant = datacontext.participants.create({adminApproved:true, emailOnCreate:true});
                        alertMessage = "Create User";
                    } else {
                        alertMessage = id ? "Update User" : "Update My Details";
                        promises.push(datacontext.participants.fetchByKey(id || tokenStorageService.getUserId(), { expand: 'roles' }).then(function (data) {
                            vm.participant = data;
                        }));
                    }
                    common.activateController(promises, controllerId).then(
                        function () {
                            loaded();
                            defer.resolve();
                        },
                        function () {
                            vm.log.error({ msg: 'Failed to load user data', data: arguments[0] });
                            defer.reject();
                        });
                });
                return defer.promise;

                function loaded() {
                    if (vm.participant.department) { vm.institution = vm.participant.department.institution; }
                    vm.filterRoles();
                    vm.notifyViewModelLoaded();
                    vm.log(alertMessage + ' Activated');
                }
            }

            function emailChanged(usrNameFormEl){
                if (usrNameFormEl.$pristine) {
                    vm.participant.userName = vm.participant.email;
                }
            }
        }
    }
})();