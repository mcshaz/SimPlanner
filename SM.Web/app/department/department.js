(function () {
    'use strict';
    var controllerId = 'department';
    angular
        .module('app')
        .controller(controllerId, courseTypesCtrl);

    courseTypesCtrl.$inject = ['common', 'datacontext', '$routeParams', 'controller.abstract', '$scope', '$http'];
    //changed $uibModalInstance to $scope to get the events

    function courseTypesCtrl(common, datacontext, $routeParams, abstractController, $scope, $http) {
        /* jshint validthis:true */
        var vm = this;
        abstractController.constructor.call(this, {
            controllerId: controllerId,
            watchedEntityNames: 'department',
            $scope: $scope
        })
        var id = $routeParams.id;
        var isNew = id == 'new';

        vm.department = {};
        vm.institutions = [];

        activate();

        function activate() {
            if (isNew) {
                vm.institution = datacontext.institutions.create();
            } 
            common.activateController([datacontext.ready()], controllerId)
                .then(function () {
                    vm.department = datacontext.departments.getByKey(id);
                    if (!vm.department) {
                        vm.log.warning('Could not find department id = ' + id);
                        return;
                        //gotoCourses();
                    }
                    datacontext.institutions.all().then(function (data) {
                        vm.institutions = data;
                    });
                    if (isNew && $routeParams.institutionId) {
                        vm.department.institutionId = $routeParams.institutionId;
                    }
                    vm.notifyViewModelLoaded();
                    vm.log('Activated Department View');
                });
        }
    }

})();
