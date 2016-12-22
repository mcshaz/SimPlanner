(function () {
    'use strict';
    var controllerId = 'reviewRegistrationRequest';
    angular
        .module('app')
        .controller(controllerId, reviewRegistrationRequest);

    reviewRegistrationRequest.$inject = ['common', 'datacontext', '$routeParams'];
    //changed $uibModalInstance to $scope to get the events

    function reviewRegistrationRequest(common, datacontext, $routeParams) {
        var vm = this;
        var log = common.logger.getLogFn(controllerId);

        vm.approve = approve;
        vm.personRequesting = {};
        vm.reject = reject;

        activate();

        function activate() {
            //datacontext.ready().then(function () {
            //possibly a bit of a hack which could be improved by specifically checking if the user is authorised on any new route
            //the usually will usually not be logged in and so receive a not authorised 401 which will show the login window
            var promises = [datacontext.institutions.fetchByKey($routeParams.id, { expand: 'departments.participants' }).then(function (data) {
                vm.personRequesting = data.departments.find(function (d) { return d.participants.length > 0; }).participants[0];
            })];
            common.activateController(promises, controllerId).then(function () {
                log.info('Activated Review Registration Request');
            });
        }

        function approve() {
            vm.personRequesting.adminApproved = vm.personRequesting.department.adminApproved = vm.personRequesting.department.institution.adminApproved = true;
            datacontext.save([vm.personRequesting, vm.personRequesting.department, vm.personRequesting.department.institution]);
        }

        function reject() {
            if (confirm("DELETE this user, department and institution?")) {
                var inst = datacontext.institutions.getByKey($routeParams.id);
                var dpts = inst.departments.slice();
                var participants = dpts.reduce(function (a, dpt) {
                    return a.concat(dpt.participants);
                }, []);
                participants.forEach(function (p) {
                    var dpt = p.department;
                    p.entityAspect.setDeleted();
                    p.department = dpt;
                });
                dpts.forEach(function (d) {
                    d.entityAspect.setDeleted();
                    d.institution = inst;
                });
                inst.entityAspect.setDeleted();
                
                datacontext.save();
            }

            function deleteEntity(e) {
                e.entityAspect.setDeleted();
            }
        }

    }
})();