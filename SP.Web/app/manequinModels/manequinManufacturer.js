(function () {
    'use strict';
    var controllerId = 'manikinManufacturer';
    angular
        .module('app')
        .controller(controllerId, controller);

    controller.$inject = ['common', 'datacontext', '$scope','controller.abstract'];
    //changed $uibModalInstance to $scope to get the events

    function controller(common, datacontext, $scope, abstractController) {
        /* jshint validthis:true */
        var vm = this;
        
        abstractController.constructor.call(this, {
            controllerId: controllerId,
            watchedEntityNames: 'manufacturer',
            $scope: $scope
        })
        
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
        };

        function save() {
            datacontext.save([vm.manufacturer]).then(vm.close);
        };

    }
})();
