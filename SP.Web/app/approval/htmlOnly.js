(function () {
    'use strict';
    var controllerId = 'htmlOnly';
    angular.module('app')
            .controller(controllerId, ['common', '$location', controller]);

    function controller(common, $location) {
        var vm = this;
        vm.linkTo = linkTo;
        vm.title = 'registration';

        activate();

        function activate() {
            common.activateController([], vm.title);
        }

        function linkTo(path) {
            $location.path(path);
        }
    }

})();