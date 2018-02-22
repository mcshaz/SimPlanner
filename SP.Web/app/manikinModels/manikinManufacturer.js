"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
var controllerId = 'manikinManufacturer';
angular_1.default
    .module('app')
    .controller(controllerId, controller);
controller.$inject = ['common', 'datacontext', '$scope', 'controller.abstract'];
function controller(common, datacontext, $scope, abstractController) {
    var vm = this;
    abstractController.constructor.call(this, {
        controllerId: controllerId,
        watchedEntityNames: 'manufacturer',
        $scope: $scope
    });
    vm.cancel = cancel;
    vm.manufacturer = $scope.manufacturer;
    vm.save = save;
    activate();
    function activate() {
        common.activateController([], controllerId)
            .then(function () {
            vm.notifyViewModelLoaded();
            vm.log('Activated Manikin Manufacturer Dialog');
        });
    }
    function cancel() {
        vm.manufacturer.entityAspect.rejectChanges();
        vm.close();
    }
    function save() {
        datacontext.save([vm.manufacturer]).then(vm.close);
    }
}
//# sourceMappingURL=manikinManufacturer.js.map