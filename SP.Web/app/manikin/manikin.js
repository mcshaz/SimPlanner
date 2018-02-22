"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
var controllerId = 'manikin';
angular_1.default
    .module('app')
    .controller(controllerId, manikinCtrl);
manikinCtrl.$inject = ['common', 'datacontext', '$routeParams', 'controller.abstract', '$scope', '$http', '$locale', '$modal'];
function manikinCtrl(common, datacontext, $routeParams, abstractController, $scope, $http, $locale, $modal) {
    var vm = this;
    abstractController.constructor.call(this, {
        controllerId: controllerId,
        watchedEntityNames: 'manikin',
        $scope: $scope
    });
    var id = $routeParams.id;
    var isNew = id === 'new';
    vm.addServiceErrMsg = addServiceErrMsg;
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
        var promises = [datacontext.ready().then(function () {
                datacontext.departments.all().then(function (data) {
                    vm.departments = data;
                });
                if (isNew) {
                    vm.manikin = $routeParams.departmentId
                        ? datacontext.manikins.create({ departmentId: $routeParams.departmentId })
                        : datacontext.manikins.create();
                }
            }), datacontext.manikinModels.all().then(function (data) {
                vm.models = data;
            })];
        if (!isNew) {
            promises.push(datacontext.manikins.fetchByKey(id, { expand: 'manikinServices' }).then(function (data) {
                vm.manikin = data;
                if (!vm.manikin) {
                    vm.log.warning('Could not find manikin id = ' + id);
                    return;
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
    function addServiceErrMsg() {
        if (vm.manikin.entityAspect) {
            if (vm.manikin.entityAspect.entityState.isAdded()) {
                return 'Please save manikin first';
            }
            if (vm.manikin.manikinServices.some(function (ms) { return !ms.Returned; })) {
                return 'Please enter date returned from prior service first';
            }
        }
        return null;
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
        modal.$scope.manikinService = service;
        modal.$promise.then(modal.show);
    }
    function createService() {
        editService(datacontext.manikinServices.create({ manikinId: vm.manikin.id }));
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
//# sourceMappingURL=manikin.js.map