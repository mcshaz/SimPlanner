(function () {
    'use strict';
    var controllerId = 'updateUser';
    angular
        .module('app')
        .controller(controllerId, updateDetails);

    updateDetails.$inject = ['common', 'datacontext', '$scope', 'controller.abstract', 'tokenStorageService', '$routeParams'];
    //changed $uibModalInstance to $scope to get the events

    function updateDetails(common, datacontext, $scope, abstractController, tokenStorageService, $routeParams) {
        /* jshint validthis:true */
        var vm = this;
        var principal = {};

        abstractController.constructor.call(this, {
            controllerId: controllerId,
            watchedEntityNames: 'participant',
            $scope: $scope
        });

        vm.canAlter = canAlter;
        vm.institution = {};
        vm.institutions = [];
        vm.participant = {};
        vm.professionalRoles = [];

        activate();

        function activate() {
            datacontext.ready().then(function () {
                var promises = [
                    datacontext.participants.fetchByKey($routeParams.id).then(function (data) {
                        vm.participant = data;
                    }),
                    datacontext.institutions.all({ expand: 'culture' }).then(function (data) {
                        vm.institutions = data;
                    }),
                    datacontext.professionalRoles.all().then(function (data) {
                        vm.professionalRoles = data;
                    })];
                common.activateController(promises, controllerId)
                    .then(function () {
                        vm.institution = vm.participant.department.institution;
                        vm.notifyViewModelLoaded();
                        vm.log('Activated Course Participant Dialog');
                    });
                principal.roles = tokenStorageService.getUserRoles();
            });
        }

        function canAlter(roleName) {
            if (principal.roles.indexOf(roleName) > -1) { return true; }
            switch (roleName) {
                case 'SiteAdmin':
                case 'AccessAllData':
                    return false;
                case 'AccessInstitution':
                    return principal.roles.indexOf('AccessAllData') > -1;
                case 'AccessDepartment':
                    return principal.roles.indexOf('AccessInstitution') > -1 || principal.roles.indexOf('AccessAllData') > -1;
                default:
                    throw new Error('Unknown role name:' + roleName);
            }
        }
    }
})();
