(function () {
    'use strict';
    var controllerId = 'approval';
    angular
        .module('app')
        .controller(controllerId, approvalCtrl);

    approvalCtrl.$inject = ['datacontext', '$routeParams', 'USER_ROLES'];

    function approvalCtrl(datacontext, $routeParams, USER_ROLES) {
        var id = $routeParams.id;
        var vm = this;

        vm.approve = approve;
        vm.deleteAll = deleteAll;
        vm.department = {};
        vm.displayProp = displayProp;
        vm.institution = {};
        vm.user = {};

        activate();

        function activate() {
            var promises = [datacontext.ready().then(function () {
                datacontext.users.fetchByKey(id, {expand:'department.institution'}).then(function (data) {
                    vm.user = data;
                    vm.department = data.department;
                    vm.institution = data.department.institution;
                });
            })];
            common.activateController(promises, controllerId);
        }

        function approve() {
            vm.user.adminApproved = vm.department.adminApproved = vm.institution.adminApproved = true;
            datacontext.userRoles.create({
                userId: vm.user.id,
                roleId: USER_ROLES.accessInstitution
            });
            datacontext.save();

        }

        function deleteAll() {
            if (confirm("DELETE this user, department and institution?")) {
                vm.user.entityAspect.setDeleted();
                vm.department.entityAspect.setDeleted();
                vm.institution.entityAspect.setDeleted();
                datacontext.save();
            }
        }

        var _displayTypes = ["string", "boolean", "number"];
        function displayProp(prop) {
            return _displayTypes.indexOf(typeof prop) > -1;
        }
        
    }
})();