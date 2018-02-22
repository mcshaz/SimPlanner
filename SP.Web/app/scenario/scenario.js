"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
var breezeEnums_1 = require("./../breezeEnums");
var controllerId = 'scenario';
angular_1.default
    .module('app')
    .controller(controllerId, courseTypesCtrl);
courseTypesCtrl.$inject = ['common', 'datacontext', '$routeParams', 'controller.abstract', '$scope', 'loginFactory', 'breeze'];
function courseTypesCtrl(common, datacontext, $routeParams, abstractController, $scope, loginFactory, breeze) {
    var vm = this;
    abstractController.constructor.call(this, {
        controllerId: controllerId,
        watchedEntityNames: ['scenario', 'scenario.scenarioResources'],
        $scope: $scope
    });
    var id = $routeParams.id;
    var isNew = id === 'new';
    var added = breeze.EntityState.Added;
    vm.scenario = { scenarioResources: [] };
    vm.addResource = addResource;
    vm.downloadResources = downloadResources;
    vm.isResourceFilesOnServer = isResourceFilesOnServer;
    vm.departments = [];
    vm.courseTypes = [];
    vm.complexities = breezeEnums_1.default.difficulty;
    vm.emersionCategories = breezeEnums_1.default.emersion;
    vm.sharingLevels = breezeEnums_1.default.sharingLevel.map(function (el) { return { value: el, display: common.toSeparateWords(el) }; });
    activate();
    function activate() {
        var promises = [datacontext.ready().then(function () {
                datacontext.departments.all().then(function (data) {
                    vm.departments = data;
                });
                datacontext.courseTypes.all().then(function (data) {
                    vm.courseTypes = data;
                });
                if (isNew) {
                    vm.scenario = $routeParams.departmentId
                        ? datacontext.scenarios.create({ departmentId: $routeParams.departmentId })
                        : datacontext.scenarios.create();
                }
            })];
        if (!isNew) {
            promises.push(datacontext.scenarios.fetchByKey(id, { expand: 'scenarioResources' }).then(function (data) {
                vm.scenario = data;
            }));
        }
        common.activateController(promises, controllerId)
            .then(function () {
            vm.notifyViewModelLoaded();
            vm.log('Activated Department View');
        });
    }
    function addResource() {
        datacontext.scenarioResources.create({ scenarioId: vm.scenario.id });
    }
    function downloadResources() {
        loginFactory.downloadFileLink('ScenarioResources', vm.scenario.id)
            .then(function (url) {
            vm.downloadFileUrl = url;
        });
    }
    function isResourceFilesOnServer() {
        return vm.scenario.scenarioResources.some(function (sr) {
            return sr.entityAspect.entityState !== added;
        });
    }
}
//# sourceMappingURL=scenario.js.map