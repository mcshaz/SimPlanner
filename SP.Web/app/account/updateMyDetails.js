(function () {
    'use strict';
    var controllerId = 'updateMyDetails';
    angular
        .module('app')
        .controller(controllerId, updateDetails);

    updateDetails.$inject = ['common', 'datacontext', '$scope', 'controller.abstract', 'tokenStorageService'];
    //changed $uibModalInstance to $scope to get the events

    function updateDetails(common, datacontext, $scope, abstractController, tokenStorageService) {
        /* jshint validthis:true */
        var vm = this;

        abstractController.constructor.call(this, {
            controllerId: controllerId,
            watchedEntityNames: 'participant',
            $scope: $scope
        });

        vm.hotDrinks = [];
        vm.institution = {};
        vm.institutions = [];
        vm.participant = {};
        vm.professionalRoles = [];

        activate();

        function activate() {
            datacontext.ready().then(function () {
                var promises = [
                    datacontext.participants.fetchByKey(tokenStorageService.getUserId()).then(function (data) {
                        vm.participant = data;
                    }),
                    datacontext.institutions.all({ expand: 'culture' }).then(function (data) {
                        vm.institutions = data;
                    }),
                    datacontext.professionalRoles.all().then(function (data) {
                        vm.professionalRoles = data;
                    }),
                    datacontext.hotDrinks.findServerIfCacheEmpty().then(function (data) {
                        vm.hotDrinks = data;
                    })];
                common.activateController(promises, controllerId)
                    .then(function () {
                        vm.institution = vm.participant.department.institution;
                        vm.notifyViewModelLoaded();
                        vm.log('Activated Course Participant Dialog');
                    });
            });
        }
    }
})();
