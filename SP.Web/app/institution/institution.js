"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
var controllerId = 'institution';
angular_1.default
    .module('app')
    .controller(controllerId, courseTypesCtrl);
courseTypesCtrl.$inject = ['common', 'datacontext', '$routeParams', 'controller.abstract', '$scope', '$http', 'tokenStorageService', '$location', 'USER_ROLES', 'selectOptionMaps', 'googleTools'];
function courseTypesCtrl(common, datacontext, $routeParams, abstractController, $scope, $http, tokenStorageService, $location, USER_ROLES, selectOptionMaps, googleTools) {
    var vm = this;
    abstractController.constructor.call(this, {
        controllerId: controllerId,
        watchedEntityNames: 'institution',
        $scope: $scope
    });
    var id = $routeParams.id;
    var isNew = id === 'new';
    var baseSave = vm.save;
    vm.institution = {};
    vm.isLoggedIn = tokenStorageService.isLoggedIn();
    vm.clearFileData = clearFileData;
    vm.cultureFormats = [];
    vm.timeZonesForCulture = [];
    vm.googleMapAddress = "";
    vm.googleMapsLink = "";
    vm.mapAddressChanged = mapAddressChanged;
    vm.onLocaleSelected = onLocaleSelected;
    vm.flagUrl = '';
    vm.save = save;
    activate();
    function activate() {
        var promises = [selectOptionMaps.fetchCultureFormats().then(function (data) {
                data.sort(sortCulture);
                vm.cultureFormats = data;
                vm.cultureFormats.forEach(function (el) {
                    el.searchString = el.culture.toLowerCase() + '#' + el.language.toLowerCase() + '#' + el.localeCode.toLowerCase();
                });
            }, vm.log.error)];
        if (isNew) {
            vm.institution = datacontext.institutions.create({ adminApproved: tokenStorageService.isAuthorized(USER_ROLES.accessAllData) });
            if ($routeParams.localeCode) {
                vm.institution.localeCode = $routeParams.localeCode;
            }
            vm.notifyViewModelLoaded();
        }
        else {
            promises.push(datacontext.ready().then(function () {
                datacontext.institutions.fetchByKey(id).then(function (data) {
                    if (!data) {
                        vm.log.warning('Could not find institution id = ' + id);
                        return;
                    }
                    vm.institution = data;
                    if (vm.institution.standardTimeZone) {
                        vm.timeZonesForCulture = [vm.institution.standardTimeZone];
                    }
                    vm.notifyViewModelLoaded();
                });
            }));
        }
        common.activateController(promises, controllerId)
            .then(function () {
            onLocaleSelected(vm.institution.localeCode);
            getGoogleLink();
            vm.log.info('Activated Institution View');
        });
    }
    function clearFileData() {
        vm.institution.logoImageFileName = vm.institution.fileSize = vm.institution.fileModified = vm.institution.file = null;
    }
    function onLocaleSelected(key) {
        $http({ method: 'GET', url: 'api/utilities/timeZones/' + key }).then(function (response) {
            vm.timeZonesForCulture = response.data;
            if (vm.timeZonesForCulture.length === 1) {
                vm.institution.standardTimeZone = vm.timeZonesForCulture[0];
            }
        }, vm.log.error);
        getGoogleLink();
    }
    function mapAddressChanged() {
        var matches = /\/@(-?\d+\.\d+),(-?\d+\.\d+)/.exec(vm.googleMapAddress);
        var float;
        if (matches && matches.length === 3) {
            float = parseFloat(matches[1]);
            if (isFinite) {
                vm.institution.latitude = float;
            }
            float = parseFloat(matches[2]);
            if (isFinite) {
                vm.institution.longitude = float;
            }
        }
    }
    function save() {
        baseSave().then(function () {
            if (!vm.isLoggedIn) {
                $location.path('/department/new').search({ institutionId: vm.institution.id });
            }
        });
    }
    function getGoogleLink() {
        var localeCode = vm.institution.localeCode;
        localeCode = typeof localeCode === 'string' && localeCode.length >= 2
            ? localeCode.substr(localeCode.length - 2, 2)
            : '';
        vm.googleMapsLink = googleTools.getLocaleHostname(localeCode) + '/maps' +
            (vm.institution.name
                ? '/search/' + vm.institution.name.replace(/ /g, '+')
                : '');
    }
}
function sortCulture(a, b) {
    if (a.culture > b.culture) {
        return 1;
    }
    if (a.culture < b.culture) {
        return -1;
    }
    if (a.language > b.language) {
        return 1;
    }
    if (a.language < b.language) {
        return -1;
    }
    return 0;
}
//# sourceMappingURL=institution.js.map