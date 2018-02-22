import angular from 'angular';

    var controllerId = 'room';
export default angular
        .module('app')
        .controller(controllerId, courseTypesCtrl).name;

    (courseTypesCtrl as any).$inject = ['common', 'datacontext', '$routeParams', 'controller.abstract', '$scope'];
    //changed $uibModalInstance to $scope to get the events

    function courseTypesCtrl(common, datacontext, $routeParams, abstractController, $scope) {
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
        vm.clearFileData = clearFileData;

        activate();

        function activate() {
            //too many calls to server here, but given infrequency of use, leave as is for time being
            var promises = [datacontext.ready().then(function () {
                datacontext.departments.all().then(function (data) {
                    vm.departments = data;
                });
                if (isNew) {
                    vm.room = $routeParams.departmentId
                        ? datacontext.rooms.create({ departmentId: $routeParams.departmentId })
                        :datacontext.rooms.create();
                } else {
                    promises.push(datacontext.rooms.fetchByKey(id).then(function (data) {
                        vm.room = data;
                        if (!vm.room) {
                            vm.log.warning('Could not find room id = ' + id);
                            return;
                            //gotoCourses();
                        }
                    }));
                }
            })];
            common.activateController(promises, controllerId)
                .then(function () {
                    vm.notifyViewModelLoaded();
                    vm.log('Activated Room View');
                });
        }

        function clearFileData() {
            vm.room.fileName = vm.room.fileSize = vm.room.fileModified = vm.room.file = null;
        }
    }

