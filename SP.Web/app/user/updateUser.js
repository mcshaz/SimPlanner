(function () {
    'use strict';
    var controllerId = 'updateUser';
    angular
        .module('app')
        .controller(controllerId, updateUser);

    updateUser.$inject = ['datacontext', '$scope', 'userDetails.abstract', 'tokenStorageService', '$routeParams', 'USER_ROLES'];
    //changed $uibModalInstance to $scope to get the events

    function updateUser(datacontext, $scope, abstractUserDetails, tokenStorageService, $routeParams, USER_ROLES) {
        /* jshint validthis:true */
        var vm = this;
        var originalRoles;
        var principal = { roles:[] };

        abstractUserDetails.constructor.call(this, { $scope:$scope, $routeParams:$routeParams, controllerId:controllerId });

        vm.canAlter = canAlter;
        vm.changed = changed;
        vm.institution = '';
        vm.permissions = {
            access: null,
            siteAdmin: false
        };
        vm.submit = submit;

        vm.activate().then(activate);

        function activate(){
            var adminLevels = [USER_ROLES.accessDepartment, USER_ROLES.accessInstitution, USER_ROLES.accessAllData];
            var roles = vm.participant.roles;
            var adminLevel = roles.find(function (r) { return adminLevels.indexOf(r.roleId) > -1; });
            vm.permissions = {
                adminLevel: adminLevel
                    ? getRoleName(adminLevel.roleId)
                    : '',
                siteAdmin: roles.some(function (r) { return r.roleId === USER_ROLES.siteAdmin; }),
                dptManikinBookings: roles.some(function (r) { return r.roleId === USER_ROLES.dptManikinBookings; }),
                dptRoomBookings: roles.some(function (r) { return r.roleId === USER_ROLES.dptRoomBookings; })
            };
            originalRoles = angular.copy(vm.permissions);
            principal.roles = tokenStorageService.getUserRoles();
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
                case 'DptRoomBookings':
                case 'DptManikinBookings':
                    return principal.roles.length > 1; //?should check if it is the same department?
                default:
                    throw new Error('Unknown role name:' + roleName);
            }
            //return false;
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

        function submit() {
            ["siteAdmin", "dptManikinBookings", "dptRoomBookings"].forEach(alterPermission);

            if (vm.permissions.adminLevel !== originalRoles.adminLevel) {
                if (originalRoles.adminLevel) {
                    var rId =  USER_ROLES[originalRoles.adminLevel];
                    data.roles.find(function (ur) { return ur.roleId === rid; }).entityAspect.setDeleted();
                }
                if (vm.permissions.adminLevel) {
                    datacontext.userRoles.create({
                        userId: vm.participant.id,
                        roleId: USER_ROLES[vm.permissions.adminLevel]
                    });
                }
            }

            vm.save();

            function alterPermission(permissionName) {
                if (vm.permissions[permissionName] !== originalRoles[permissionName]) {
                    if (originalRoles[permissionName]) {
                        vm.participant.roles.find(function (ur) { return ur.roleId === USER_ROLES[permissionName]; }).entityAspect.setDeleted();
                    } else {
                        datacontext.userRoles.create({
                            userId: vm.participant.id,
                            roleId: USER_ROLES[permissionName]
                        });
                    }
                }
            }
        }
    }
})();
