"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
'use strict';
var controllerId = 'updateMyDetails';
angular_1.default
    .module('app')
    .controller(controllerId, updateDetails);
updateDetails.$inject = ['$scope', '$routeParams', 'userDetails.abstract', '$location'];
function updateDetails($scope, $routeParams, abstractUserDetails, $location) {
    var vm = this;
    abstractUserDetails.constructor.call(this, { $scope: $scope, $routeParams: $routeParams, controllerId: controllerId });
    vm.passwordRequired = vm.isLoggedIn;
    vm.submit = submit;
    vm.user = {
        newPassword: '',
        confirmPassword: ''
    };
    vm.activate();
    function submit(form) {
        if (form.$valid) {
            if (vm.user.newPassword) {
                vm.participant.password = vm.user.newPassword;
            }
            vm.save().then(function () {
                if (vm.isNew) {
                    $location.path('/finishedSubmission').search({});
                }
            });
        }
    }
}
//# sourceMappingURL=updateMyDetails.js.map