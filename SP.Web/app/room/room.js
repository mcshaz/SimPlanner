(function () {
    'use strict';
    var controllerId = 'room';
    angular
        .module('app')
        .controller(controllerId, courseTypesCtrl);

    courseTypesCtrl.$inject = ['common', 'datacontext', '$routeParams', 'controller.abstract', '$scope', '$http', '$locale'];
    //changed $uibModalInstance to $scope to get the events

    function courseTypesCtrl(common, datacontext, $routeParams, abstractController, $scope, $http, $locale) {
        /* jshint validthis:true */
        var vm = this;
        abstractController.constructor.call(this, {
            controllerId: controllerId,
            watchedEntityNames: 'room',
            $scope: $scope
        });
        var id = $routeParams.id;
        var isNew = id === 'new';

        vm.departments = [];
        vm.room = {};

        activate();

        function activate() {
            //too many calls to server here, but given infrequency of use, leave as is for time being
            var promises = [datacontext.ready().then(function (data) {
                datacontext.departments.all().then(function (data) {
                    vm.departments = data;
                });
                if (isNew) {
                    vm.room = $routeParams.departmentId
                        ? datacontext.rooms.create({ departmentId: $routeParams.departmentId })
                        :datacontext.rooms.create();
                }
            })];
            if (!isNew) {
                promises.push(datacontext.rooms.fetchByKey(id).then(function (data) {
                    vm.room = data;
                    if (!vm.room) {
                        vm.log.warning('Could not find room id = ' + id);
                        return;
                        //gotoCourses();
                    }
                }));
            }
            common.activateController(promises, controllerId)
                .then(function () {
                    vm.notifyViewModelLoaded();
                    vm.log('Activated Room View');
                });
        }
    }
})();
