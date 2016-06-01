(function () {
    'use strict';
    var controllerId = 'rsvp';
    angular.module('app')
            .controller(controllerId, ['$http', 'common', '$routeParams',controller]);

    function controller($http,common,$routeParams) {
        var vm = this;
        var log = common.logger.getLogFn(controllerId);
        vm.serverMessage = '';

        activate();

        function activate() {
            common.activateController([$http({
                method: 'POST',
                url: 'api/CoursePlanning/Rsvp',
                data: $routeParams
            }).then(function (response) {
                vm.serverMessage = response.data;
                log.success('RSVP registered');
            }, function (response) {
                vm.serverMessage = "Appologies - an error has occured";
                log.error(err);
            })], controllerId);;
        }

    }
})();