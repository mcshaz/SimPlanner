(function () {
    'use strict';
    var controllerId = 'updateMyDetails';
    angular
        .module('app')
        .controller(controllerId, updateDetails);

    updateDetails.$inject = ['$scope', '$routeParams', 'userDetails.abstract', '$location'];
    //changed $uibModalInstance to $scope to get the events

    function updateDetails($scope, $routeParams, abstractUserDetails, $location) {
        /* jshint validthis:true */
        var vm = this;

        abstractUserDetails.constructor.call(this, { $scope: $scope, $routeParams: $routeParams, controllerId: controllerId });

        vm.passwordRequired = false;
        vm.submit = submit;

        vm.activate();

        function submit(form) {
            if (form.$valid) {
                if (vm.user.password) {
                    vm.participant.password = user.password;
                }
                vm.save().then(function () {
                    if (vm.isNew) {
                        $location.path('/finishedSubmission').search({});
                    }
                });
            }
        }
    }
})();
