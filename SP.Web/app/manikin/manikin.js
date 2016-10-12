(function () {
    'use strict';
    var controllerId = 'manikin';
    angular
        .module('app')
        .controller(controllerId, courseTypesCtrl);

    courseTypesCtrl.$inject = ['common', 'datacontext', '$routeParams', 'controller.abstract', '$scope', '$http', '$locale', '$modal'];
    //changed $uibModalInstance to $scope to get the events

    function courseTypesCtrl(common, datacontext, $routeParams, abstractController, $scope, $http, $locale,$modal) {
        /* jshint validthis:true */
        var vm = this;
        abstractController.constructor.call(this, {
            controllerId: controllerId,
            watchedEntityNames: 'manikin',
            $scope: $scope
        });
        var id = $routeParams.id;
        var isNew = id === 'new';

        vm.departments = [];
        vm.editService = editService;
        vm.createService = createService;
        vm.manikin = {};
        vm.minDate = new Date(1998, 0);
        vm.maxDate = new Date();
        vm.models = [];
        vm.currencySymbol = $locale.NUMBER_FORMATS.CURRENCY_SYM;
        vm.isoCurrency = '';
        activate();

        function activate() {
            //too many calls to server here, but given infrequency of use, leave as is for time being
            var promises = [datacontext.ready().then(function (data) {
                datacontext.departments.all().then(function (data) {
                    vm.departments = data;
                });
                if (isNew) {
                    vm.manikin = $routeParams.departmentId
                        ? datacontext.manikins.create({ departmentId: $routeParams.departmentId })
                        : datacontext.manikins.create();
                }
            }), datacontext.manikinModels.all().then(function(data){
                vm.models = data;
            })];

            if (!isNew) {
                promises.push(datacontext.manikins.fetchByKey(id, { expand: 'manikinServices' }).then(function (data) {
                    vm.manikin = data;
                    if (!vm.manikin) {
                        vm.log.warning('Could not find manikin id = ' + id);
                        return;
                        //gotoCourses();
                    }
                    getIsoCurrency();
                }));
            }
            common.activateController(promises, controllerId)
                .then(function () {
                    vm.notifyViewModelLoaded();
                    vm.log('Activated Department View');
                });
        }

        function getIsoCurrency() {
            var cultureCode = vm.manikin && vm.manikin.department && vm.manikin.department.institution.localeCode;
            if (cultureCode) {
                $http({ method: 'GET', url: 'api/utilities/currencyInfo/' + cultureCode }).then(function (response) {
                    vm.isoCurrency = response.data;
                }, vm.log.error);
            }
        }

        function editService(service) {
            var modal = getModalInstance();
            modal.$scope.manikinsService = service;
            modal.$promise.then(modal.show);
        }

        function createService() {
            editService(datacontext.manikinServices.create({manikinId: vm.manikin.id}));
        }

        var _modalInstance;
        function getModalInstance() {
            if (!_modalInstance) {
                var scope = $scope.$new();
                _modalInstance = $modal({
                    templateUrl: 'app/manikinService/manikinService.html',
                    controller: 'manikinService',
                    show: false,
                    scope: scope,
                    controllerAs: 'ms'
                });
            }
            return _modalInstance;
        }
    }
})();
