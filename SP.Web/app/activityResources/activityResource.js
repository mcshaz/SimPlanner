"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
'use strict';
var controllerId = 'activityResource';
angular_1.default
    .module('app')
    .controller(controllerId, controller);
controller.$inject = ['common', 'datacontext', '$scope', 'controller.abstract'];
function controller(common, datacontext, $scope, abstractController) {
    var vm = this;
    abstractController.constructor.call(this, {
        controllerId: controllerId,
        watchedEntityNames: 'courseActivity.activityChoices',
        $scope: $scope
    });
    vm.courseActivity = $scope.courseActivity;
    vm.createActivityResource = createActivityResource;
    vm.deleteResource = deleteResource;
    var baseSave = vm.save;
    vm.save = saveOverride;
    activate();
    function activate() {
        datacontext.ready().then(function () {
            var ent = vm.courseActivity.entityAspect;
            var promises = ent.isNavigationPropertyLoaded('activityChoices')
                ? []
                : [ent.loadNavigationProperty('activityChoices')];
            common.activateController(promises, controllerId)
                .then(function () {
                vm.notifyViewModelLoaded();
                vm.log('Activated Activity Resources Dialog');
            });
        });
    }
    function createActivityResource() {
        vm.selectedActivityResource = datacontext.activityResources.create({
            courseActivityId: $scope.courseActivity.id
        });
    }
    function saveOverride() {
        baseSave();
        vm.selectedActivityResource = null;
    }
    function deleteResource(activityResource) {
        activityResource.entityAspect.setDeleted();
        if (vm.selectedActivityResource === activityResource) {
            vm.selectedActivityResource = null;
        }
    }
}
//# sourceMappingURL=activityResource.js.map