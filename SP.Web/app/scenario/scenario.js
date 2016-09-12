(function () {
    'use strict';
    var controllerId = 'scenario';
    angular
        .module('app')
        .controller(controllerId, courseTypesCtrl);

    courseTypesCtrl.$inject = ['common', 'datacontext', '$routeParams', 'controller.abstract', '$scope', '$http'];
    //changed $uibModalInstance to $scope to get the events

    function courseTypesCtrl(common, datacontext, $routeParams, abstractController, $scope, $http) {
        /* jshint validthis:true */
        var vm = this;
        abstractController.constructor.call(this, {
            controllerId: controllerId,
            watchedEntityNames: 'department',
            $scope: $scope
        });
        var id = $routeParams.id;
        var isNew = id === 'new';
        var enums = common.getEnumValues();

        vm.scenario = {};
        vm.departments = [];
        vm.courseTypes = [];
        vm.complexities = enums.difficulty;
        vm.emersionCategories = enums.emersion;
        vm.sharingLevels = enums.sharingLevel.map(function (el) { return { value:el, display: common.toSeparateWords(el) }; });

        activate();

        function activate() {
            var promises = [datacontext.ready().then(function(){
                datacontext.departments.all().then(function(data){
                    vm.departments = data;
                });
                datacontext.courseTypes.all().then(function (data) {
                    vm.courseTypes = data;
                });
            })];
            if (isNew) {
                vm.scenario = datacontext.scenarios.create();
            } else {
                promises.push(datacontext.scenarios.fetchByKey(id).then(function(data){
                    vm.scenario = data;
                }));
            }
            common.activateController(promises, controllerId)
                .then(function () {
                    vm.notifyViewModelLoaded();
                    vm.log('Activated Department View');
                });
        }
    }

})();
