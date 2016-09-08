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
        vm.errors = '';
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
                url: 'api/Account/ResetPassword',
                data: {
                    NewPassword: credentials.newPassword,
                    ConfirmPassword: credentials.confirmPassword,
                    Token: decodeURIComponent($routeParams.token),
                    UserId: $routeParams.userId
                }
            }).then(function (data) {
                log.success('password changed');
            }, function (data) {
                log.error({ msg: 'change password error', data: data });
                vm.errors = ''; //todo here
            });
        }

    }
})();