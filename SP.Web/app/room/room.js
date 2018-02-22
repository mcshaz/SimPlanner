"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
var controllerId = 'room';
angular_1.default
    .module('app')
    .controller(controllerId, courseTypesCtrl);
courseTypesCtrl.$inject = ['common', 'datacontext', '$routeParams', 'controller.abstract', '$scope'];
function courseTypesCtrl(common, datacontext, $routeParams, abstractController, $scope) {
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
        var promises = [datacontext.ready().then(function () {
                datacontext.departments.all().then(function (data) {
                    vm.departments = data;
                });
                if (isNew) {
                    vm.room = $routeParams.departmentId
                        ? datacontext.rooms.create({ departmentId: $routeParams.departmentId })
                        : datacontext.rooms.create();
                }
                else {
                    promises.push(datacontext.rooms.fetchByKey(id).then(function (data) {
                        vm.room = data;
                        if (!vm.room) {
                            vm.log.warning('Could not find room id = ' + id);
                            return;
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
//# sourceMappingURL=room.js.map