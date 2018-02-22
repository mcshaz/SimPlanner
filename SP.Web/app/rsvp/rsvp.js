"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
var controllerId = 'rsvp';
angular_1.default.module('app')
    .controller(controllerId, ['$http', 'common', '$routeParams', controller]);
function controller($http, common, $routeParams) {
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
//# sourceMappingURL=rsvp.js.map