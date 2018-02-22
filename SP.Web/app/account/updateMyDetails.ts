import angular from 'angular';

    'use strict';
    var controllerId = 'updateMyDetails';
export default angular
        .module('app')
        .controller(controllerId, updateDetails).name;

    (updateDetails as any).$inject = ['$scope', '$routeParams', 'userDetails.abstract', '$location'];
    //changed $uibModalInstance to $scope to get the events

    function updateDetails($scope, $routeParams, abstractUserDetails, $location) {
        /* jshint validthis:true */
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

