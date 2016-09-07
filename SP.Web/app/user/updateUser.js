(function () {
    'use strict';
    var controllerId = 'updateUser';
    angular
        .module('app')
        .controller(controllerId, updateDetails);

    updateDetails.$inject = ['common', 'datacontext', '$scope', 'controller.abstract', 'tokenStorageService', '$routeParams', 'USER_ROLES'];
    //changed $uibModalInstance to $scope to get the events

    function updateDetails(common, datacontext, $scope, abstractController, tokenStorageService, $routeParams, USER_ROLES) {
        /* jshint validthis:true */
        var vm = this;
        var principal = { roles:[] };
        var isSiteAdmin;
        var currentRole;

        abstractController.constructor.call(this, {
            controllerId: controllerId,
            watchedEntityNames: 'participant',
            $scope: $scope
        });

        var baseSave = this.save;
        this.save = save;

        vm.canAlter = canAlter;
        vm.changed = changed;
        vm.institution = {};
        vm.institutions = [];
        vm.participant = {};
        vm.permissions = {
            access: null,
            siteAdmin: false
        };
        vm.professionalRoles = [];

        activate();

        function activate() {
            datacontext.ready().then(function () {
                var promises = [
                    datacontext.participants.fetchByKey($routeParams.id, {expand:'roles'}).then(function (data) {
                        vm.participant = data;
                        var ur = data.roles.find(function (ur) { return ur.roleId !== USER_ROLES.siteAdmin; });
                        isSiteAdmin = data.roles.some(function (ur) { return ur.roleId === USER_ROLES.siteAdmin; });
                        currentRole = ur
                                ? getRoleName(ur.roleId)
                                : '';
                        vm.permissions = {
                            access: currentRole,
                            siteAdmin: isSiteAdmin
                        };
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
            return false;
        }

        function changed() {
            this.isEntityStateChanged = true;
        }

        function getRoleName(id) {
            for (var key in USER_ROLES) {
                if (USER_ROLES[key]===id) {
                    return key;
                }
            }
        }

        function save() {
            if (vm.permissions.siteAdmin !== isSiteAdmin) {
                if (isSiteAdmin) {
                    data.roles.find(function (ur) { return ur.roleId === USER_ROLES.siteAdmin; }).entityAspect.setDeleted();
                } else {
                    datacontext.userRoles.create({ 
                        userId: vm.participant.id,
                        roleId: USER_ROLES.siteAdmin
                    });
                }
            }
            if (vm.permissions.access !== currentRole) {
                if (vm.permissions.access === '') {
                    data.roles.find(function (ur) { return ur.roleId !== USER_ROLES.siteAdmin; }).entityAspect.setDeleted();
                } else {
                    datacontext.userRoles.create({
                        userId: vm.participant.id,
                        roleId: USER_ROLES[vm.permissions.access]
                    });
                }
            }
            baseSave();
        }
    }
})();
