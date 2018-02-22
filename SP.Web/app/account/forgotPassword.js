"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
'use strict';
var controllerId = 'forgotPassword';
angular_1.default.module('app')
    .controller(controllerId, ['$http', 'common', changePassword]);
function changePassword($http, common) {
    var vm = this;
    var log = common.logger.getLogFn(controllerId);
    vm.user = {
        email: ''
    };
    vm.errors = [];
    vm.submit = submit;
    vm.successMsg = '';
    vm.serverRespondedTime = null;
    activate();
    function activate() {
        common.activateController([], controllerId)
            .then(function () {
            log('Activated Forgot Password View');
        });
    }
    function submit(credentials) {
        vm.errors = [];
        $http({
            method: 'POST',
            url: 'api/Account/ForgotPassword',
            data: mapObjectToPascalCase(credentials)
        }).then(function (_response) {
            vm.successMsg = 'email sent (IF it is on our system)';
            log.success(vm.successMsg);
        }, function (response) {
            log.error({ msg: 'forgot password error', data: response.data });
            vm.errors = response.data.ModelState[""] || ["unknown error"];
        }).finally(function () {
            vm.serverRespondedTime = new Date();
        });
    }
    function mapObjectToPascalCase(obj) {
        var returnVar = {};
        for (var propName in obj) {
            if (propName.length) {
                returnVar[propName[0].toUpperCase() + propName.substr(1)] = obj[propName];
            }
        }
        return returnVar;
    }
}
//# sourceMappingURL=forgotPassword.js.map