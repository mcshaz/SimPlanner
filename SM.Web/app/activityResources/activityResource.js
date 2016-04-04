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
        vm.selectedActivityResource = {};

        activate();

        function activate() {
            datacontext.ready().then(function () {
                var ent = vm.courseActivity.entityAspect;
                var promises = ent.isNavigationPropertyLoaded('activityChoices')
                    ? []
                    : ent.loadNavigationProperty('activityChoices');
                common.activateController(promises, controllerId)
                    .then(function () {
                        cp.log('Activated Course Participant Dialog');
                    });
            });
        }

        function selectActivityResource(activityResource) {
            vm.selectedActivityResource = activityResource;
        }

        function createActivityResource() {
            vm.currentActivityResource = datacontext.activityResourcess.create({
                courseActivityId: $scope.courseActivity.id
            });
        }
    }
})();
