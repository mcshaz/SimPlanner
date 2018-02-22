"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
var controllerId = 'manikinModel';
angular_1.default
    .module('app')
    .controller(controllerId, controller);
controller.$inject = ['common', 'datacontext', '$scope', 'controller.abstract'];
function controller(common, datacontext, $scope, abstractController) {
    var vm = this;
    abstractController.constructor.call(this, {
        controllerId: controllerId,
        watchedEntityNames: 'model',
        $scope: $scope
    });
    vm.cancel = cancel;
    vm.manufacturers = [];
    vm.model = $scope.model;
    vm.save = save;
    activate();
    function activate() {
        datacontext.ready().then(function () {
            var promises = [datacontext.manikinManufacturers.all().then(function (data) {
                    vm.manufacturers = data;
                })];
            common.activateController(promises, controllerId)
                .then(function () {
                vm.notifyViewModelLoaded();
                vm.log('Activated Manikin Model Dialog');
            });
        });
    }
    function cancel() {
        vm.model.entityAspect.rejectChanges();
        vm.close();
    }
    function save() {
        datacontext.save([vm.model]).then(vm.close);
    }
}
//# sourceMappingURL=manikinModel.js.map