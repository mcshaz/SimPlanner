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
        });
        var id = $routeParams.id;
        var isNew = id === 'new';

        vm.department = {};
        vm.institutions = [];

        activate();

        function activate() {
            datacontext.ready().then(function () {
                var promises = [datacontext.institutions.all().then(function (data) {
                    vm.institutions = data;
                })];
                if (isNew) {
                    vm.department = $routeParams.institutionId
                        ? datacontext.departments.create({institutionId:$routeParams.institutionId})
                        :datacontext.departments.create();
                } else {
                    promises.push(datacontext.departments.fetchByKey(id).then(function(data){
                        vm.department = data;
                        if (!vm.department) {
                            vm.log.warning('Could not find department id = ' + id);
                            return;
                            //gotoCourses();
                        }
                    }));
                }
                common.activateController(promises, controllerId)
                    .then(function () {
                        vm.notifyViewModelLoaded();
                        vm.log('Activated Department View');
                    });
            });
        }
    }

})();
