"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
'use strict';
var controllerId = 'changePassword';
angular_1.default.module('app')
    .controller(controllerId, ['$http', 'common', changePassword]);
function changePassword($http, common) {
    var vm = this;
    var log = common.logger.getLogFn(controllerId);
    vm.user = {
        oldPassword: '',
        newPassword: '',
        confirmPassword: ''
    };
    vm.errors = '';
    vm.passwordRequired = true;
    vm.submit = submit;
    activate();
    function activate() {
        common.activateController([], controllerId)
            .then(function () {
            log('Activated Change Password View');
        });
    }
    function submit(credentials) {
        vm.errors = '';
        $http({
            method: 'POST',
            url: 'api/Account/ChangePassword',
            data: mapObjectToPascalCase(credentials)
        }).then(function (_response) {
            log.success('password changed');
        }, function (response) {
            log.error({ msg: 'change password error', data: response });
            vm.errors = '';
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
//# sourceMappingURL=changePassword.js.map