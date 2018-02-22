"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
var controllerId = 'manikinService';
angular_1.default
    .module('app')
    .controller(controllerId, controller);
controller.$inject = ['common', 'datacontext', '$scope', 'controller.abstract'];
function controller(common, datacontext, $scope, abstractController) {
    var vm = this;
    abstractController.constructor.call(this, {
        controllerId: controllerId,
        watchedEntityNames: 'manikinService',
        $scope: $scope
    });
    vm.manikinService = $scope.manikinService;
    activate();
    function activate() {
        datacontext.ready().then(function () {
            common.activateController([], controllerId)
                .then(function () {
                vm.notifyViewModelLoaded();
            });
        });
    }
}
//# sourceMappingURL=manikinService.js.map