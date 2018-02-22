import angular from 'angular';

    var controllerId = 'rsvp';
export default angular.module('app')
            .controller(controllerId, ['$http', 'common', '$routeParams',controller]).name;

    function controller($http,common,$routeParams) {
        var vm = this;
        var log = common.logger.getLogFn(controllerId);
        vm.serverMessage = '';

        activate();

        function activate() {
            $routeParams.Token = $routeParams.Token.replace(/ /g, '+');
            common.activateController([$http({
                method: 'POST',
                url: 'api/CoursePlanning/Rsvp',
                data: $routeParams
            }).then(function (response) {
                vm.serverMessage = response.data;
                log.success('RSVP registered');
            }, function (response) {
                vm.serverMessage = "Appologies - an error has occured";
                log.error({ msg: vm.serverMessage, data: response.data });
            })], controllerId);
        }

    }
