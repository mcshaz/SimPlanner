import angular from 'angular';

    var controllerId = 'manikinService';
export default angular
        .module('app')
        .controller(controllerId, controller).name;

    (controller as any).$inject = ['common', 'datacontext', '$scope','controller.abstract'];
    //changed $uibModalInstance to $scope to get the events

    function controller(common, datacontext, $scope, abstractController) {
        /* jshint validthis:true */
        var vm = this;

        abstractController.constructor.call(this, {
            controllerId: controllerId,
            watchedEntityNames: 'manikinService',
            $scope: $scope
        })

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

