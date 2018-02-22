"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
'use strict';
var controllerId = 'resetPassword';
angular_1.default.module('app')
    .controller(controllerId, ['$http', 'common', '$routeParams', controller]);
function controller($http, common, $routeParams) {
    var vm = this;
    var log = common.logger.getLogFn(controllerId);
    vm.user = {
        newPassword: '',
        confirmPassword: ''
    };
    vm.errors = [];
    vm.serverRespondedTime = null;
    vm.submit = submit;
    activate();
    function activate() {
        common.activateController([], controllerId)
            .then(function () {
            log('Activated Change Password View');
        });
    }
    function submit(credentials) {
        vm.errors = [];
        $http({
            method: 'POST',
            url: 'api/Account/ResetPassword',
            data: {
                NewPassword: credentials.newPassword,
                ConfirmPassword: credentials.confirmPassword,
                Token: $routeParams.token.replace(/ /g, '+'),
                UserId: $routeParams.userId
            }
        }).then(function (_response) {
            vm.successMsg = 'Password changed successfully.';
            log.success(vm.successMsg);
            vm.successMsg += ' Please login with your new password [upper right]';
        }, function (response) {
            log.error({ msg: 'change password error', data: response });
            vm.errors = response.data.ModelState[""] || ["unknown error"];
        }).finally(function () {
            vm.serverRespondedTime = new Date();
        });
    }
}
//# sourceMappingURL=resetPassword.js.map