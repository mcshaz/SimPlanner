import angular from 'angular';

    var controllerId = 'manikinModels';
export default angular
        .module('app')
        .controller(controllerId, controller).name;

    (controller as any).$inject = ['common', 'datacontext', '$scope', '$modal', 'breeze'];
    //changed $uibModalInstance to $scope to get the events

    function controller(common, datacontext, $scope, $modal, breeze) {
        /* jshint validthis:true */
        var vm = this;
        var log = common.logger.getLogFn(controllerId);

        vm.editManufacturer = editManufacturer;
        vm.editModel = editModel;
        vm.manufacturers = [];

        activate();

        function activate() {
            datacontext.ready().then(function () {
                common.activateController([
                    datacontext.manikinManufacturers.all({ expand: ['manikinModels'] }).then(function (data) {
                        data.sort(common.sortOnPropertyName('name'));
                        vm.manufacturers = data;
                        data.forEach(function (el) {
                            el.manikinModels.sort(common.sortOnPropertyName('description'));
                        });
                    })], controllerId).then(function () {
                        log('Activated Course Participant Dialog');
                    });
            });
        }

        function editManufacturer(man) {
            var modal = getManModalInstance();
            modal.$scope.manufacturer = man === 'new'
                ? datacontext.manikinManufacturers.create()
                : man;
            modal.$promise.then(modal.show);
        }

        function editModel(model) {
            var modal = getModelModalInstance();
            modal.$scope.model = model === 'new'
                ? datacontext.manikinModels.create()
                : model;
            modal.$promise.then(modal.show);
        }


        var _manModalInstance;
        function getManModalInstance() {
            if (!_manModalInstance) {
                var modalScope = $scope.$new();
                _manModalInstance = $modal({
                    templateUrl: 'app/manikinModels/manikinManufacturer.html',
                    controller: 'manikinManufacturer',
                    show: false,
                    id: 'manufacturerModal',
                    scope: modalScope,
                    controllerAs: 'man'
                });
                $scope.$on('$destroy', function () { _manModalInstance.destroy(); });
                modalScope.$on('modal.hide', function () {
                    var man = arguments[1].$scope.manufacturer;
                    if (man.entityAspect.entityState !== breeze.EntityState.Deleted && man.entityAspect.entityState !== breeze.EntityState.Detached && vm.manufacturers.indexOf(man) === -1) {
                        vm.manufacturers.push(man);
                    }
                    arguments[1].$scope.manufacturer = null;
                });
            }
            return _manModalInstance;
        }

        var _modelModalInstance;
        function getModelModalInstance() {
            if (!_modelModalInstance) {
                var modalScope = $scope.$new();
                _modelModalInstance = $modal({
                    templateUrl: 'app/manikinModels/manikinModel.html',
                    controller: 'manikinModel',
                    show: false,
                    id: 'modelModal',
                    scope: modalScope,
                    controllerAs: 'mm'
                });
                $scope.$on('$destroy', function () { _modelModalInstance.destroy(); });
            }
            return _modelModalInstance;
        }

    }

