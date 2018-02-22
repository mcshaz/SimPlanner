import angular from 'angular';
    'use strict';
    var controllerId = 'htmlOnly';
export default angular.module('app')
            .controller(controllerId, ['common', '$location', controller]).name;

    function controller(common, $location) {
        var vm = this;
        vm.linkTo = linkTo;
        vm.title = 'registration';

        activate();

        function activate() {
            common.activateController([], vm.title);
        }

        function linkTo(path, params) {
            if (params) {
                return $location.path(path).search(params);
            }
            return $location.path(path);
        }
    }

