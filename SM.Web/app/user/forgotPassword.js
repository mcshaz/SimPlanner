(function () {
    'use strict';
    var controllerId = 'forgotPassword';
    angular.module('app')
            .controller(controllerId, ['$http', 'common',changePassword]);

    function changePassword($http,common) {
        var vm = this;
        var log = common.logger.getLogFn(controllerId);
        vm.user = {
            email: ''
        };
        vm.errors = '';
        vm.submit = submit;

        activate();

        function activate() {
            common.activateController([], controllerId)
                .then(function () {
                    log('Activated Forgot Password View');
                });
        }

        function submit(credentials) {
            vm.errors = '';
            $http({
                method: 'POST',
                url: 'api/Account/ForgotPassword',
                data: mapObjectToPascalCase(credentials)
            }).then(function (data) {
                log.success('email sent (IF it is on our system)');
            }, function (data) {
                log.error({ msg: 'change password error', data: data });
                vm.errors = ''; //todo here
            });
        };

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
})();