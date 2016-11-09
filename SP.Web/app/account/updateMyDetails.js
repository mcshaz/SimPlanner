(function () {
    'use strict';
    var controllerId = 'updateMyDetails';
    angular
        .module('app')
        .controller(controllerId, updateDetails);

    updateDetails.$inject = ['common', 'datacontext', '$scope', 'controller.abstract', 'tokenStorageService', '$routeParams'];
    //changed $uibModalInstance to $scope to get the events

    function updateDetails(common, datacontext, $scope, abstractController, tokenStorageService, $routeParams) {
        /* jshint validthis:true */
        var vm = this;

        abstractController.constructor.call(this, {
            controllerId: controllerId,
            watchedEntityNames: 'participant',
            $scope: $scope
        });
        var id = $routeParams.id;
        var isNew = id === 'new';

        vm.hotDrinks = [];
        vm.institution = {};
        vm.institutions = [];
        vm.participant = {};
        vm.passwordRequired = false;
        vm.professionalRoles = [];
        vm.baseSave = vm.save;
        vm.save = save;

        activate();

        function activate() {
            var promises;
            var alertMessage;
            if (isNew) {
                vm.participant = datacontext.participants.create({ departmentId: $routeParams.departmentId });
                promises = [datacontext.departments.fetchByKey($routeParams.departmentId, { expand: 'institution.culture' })];
                alertMessage = "Register User";
            } else {
                datacontext.ready().then(function () {
                    alertMessage = "Update My Details";
                    promises = [
                        datacontext.participants.fetchByKey(tokenStorageService.getUserId()).then(function (data) {
                            vm.participant = data;
                        }),
                        datacontext.institutions.all({ expand: 'culture' }).then(function (data) {
                            vm.institutions = data;
                        }),
                        datacontext.professionalRoles.all().then(function (data) {
                            vm.professionalRoles = data;
                        }),
                        datacontext.hotDrinks.findServerIfCacheEmpty().then(function (data) {
                            vm.hotDrinks = data;
                        })];
                });
            }
            common.activateController(promises, controllerId).then(function () {
                    vm.institution = vm.participant.department.institution;
                    vm.notifyViewModelLoaded();
                    vm.log('Activated ' + alertMessage + ' dialog');
            });
        }

        function save() {
            baseSave().then(function () {
                if (isNew) {
                    $location.path('/finishedSubmission');
                }
            });
        }
    }
})();
