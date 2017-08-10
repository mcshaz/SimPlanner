(function () {
    'use strict';
    var controllerId = 'resetPassword';
    angular.module('app')
            .controller(controllerId, ['$http', 'common', '$routeParams',controller]);

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
                    Token: decodeURIComponent($routeParams.token),
                    UserId: $routeParams.userId
                }
            }).then(function (response) {
                vm.successMsg = 'Password Changed Successfully. Please login with your new password [upper right]';
                log.success(vm.successMsg);
            }, function (response) {
                log.error({ msg: 'change password error', data: response });
                vm.errors = response.data.ModelState[""] || ["unknown error"];
            }).finally(function () {
                vm.serverRespondedTime = new Date();
            });
        }
    }
})();