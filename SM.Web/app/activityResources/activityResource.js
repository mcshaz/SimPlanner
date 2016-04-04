(function () {
    'use strict';
    var controllerId = 'activityResource';
    angular
        .module('app')
        .controller(controllerId, controller);

    controller.$inject = ['common', 'datacontext', 'breeze', '$scope','controller.abstract'];
    //changed $uibModalInstance to $scope to get the events

    function controller(common, datacontext, breeze, $scope, abstractController) {
        /* jshint validthis:true */
        var vm = this;

        abstractController.constructor.call(this, {
            controllerId: controllerId,
            watchedEntityName: 'courseActivity',
            $scope: $scope
        })

        vm.courseActivity = $scope.courseActivity;
        vm.createActivityResource = createActivityResource;
        vm.selectedActivityResource = null;

        vm.selectActivityResource = selectActivityResource;
        vm.save = save;

        activate();

        function activate() {
            datacontext.ready().then(function () {
                var ent = vm.courseActivity.entityAspect;
                var promises = ent.isNavigationPropertyLoaded('activityChoices')
                    ? []
                    : [ent.loadNavigationProperty('activityChoices')];
                common.activateController(promises, controllerId)
                    .then(function () {
                        vm.log('Activated Activity Resources Dialog');
                    });
            });
        }

        function selectActivityResource(activityResource) {
            vm.selectedActivityResource = activityResource;
        }

        function createActivityResource() {
            vm.selectedActivityResource = datacontext.activityResources.create({
                courseActivityId: $scope.courseActivity.id
            });
        }

        function save() {
            datacontext.save(vm.courseActivity.activityChoices);
            vm.selectedActivityResource = null;
        }
    }
})();
