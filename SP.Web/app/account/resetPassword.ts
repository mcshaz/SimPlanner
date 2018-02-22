import angular from 'angular';
    'use strict';
    var controllerId = 'resetPassword';
export default angular.module('app')
            .controller(controllerId, ['$http', 'common', '$routeParams',controller]).name;

    function controller($http,common,$routeParams) {
        var vm = this;
        var log = common.logger.getLogFn(controllerId);
        vm.user = {
            newPassword: '',
            confirmPassword:''
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
                    //bizzare trustwave Scanmail cannot symetrically encode & decode url encoded query strings at the moment
                    //Token: decodeURIComponent($routeParams.token),
                    Token: $routeParams.token.replace(/ /g,'+'),
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
