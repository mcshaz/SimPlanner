(function () {
    'use strict';
    var controllerId = 'institution';
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
            watchedEntityNames: 'institution',
            $scope: $scope
        });
        var id = $routeParams.id;
        var isNew = id === 'new';

        vm.institution = {};
        vm.clearFileData = clearFileData;
        vm.cultureFormats=[];
        vm.timeZonesForCulture = [];
        vm.googleMapAddress="";
        vm.mapAddressChanged = mapAddressChanged;
        vm.onLocaleSelected = onLocaleSelected;
        vm.flagUrl = '';

        activate();

        function activate() {
            var promises = [common.fetchCultureFormats().then(function (data) {
                data.sort(sortCulture);
                vm.cultureFormats = data;
                vm.cultureFormats.forEach(function (el) {
                    el.searchString = el.culture.toLowerCase() + '#' + el.language.toLowerCase() + '#' + el.localeCode.toLowerCase();
                });
            }, vm.log.error)];
            if (isNew) {
                vm.institution = datacontext.institutions.create();
                if ($routeParams.localeCode) {
                    vm.institution.localeCode = $routeParams.localeCode;
                }
                vm.notifyViewModelLoaded();
            } else {
                promises.push(datacontext.ready().then(function() {
                    datacontext.institutions.fetchByKey(id).then(function (data) {
                        if (!data) {
                            vm.log.warning('Could not find institution id = ' + id);
                            return;
                            //gotoCourses();
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
                    vm.log('Activated Institution View');
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
    }
    function sortCulture(a, b) {
        if (a.culture > b.culture) {
            return 1;
        }
        if (a.culture < b.culture) {
            return -1;
        }
        // a must be equal to b
        if (a.language > b.language) {
            return 1;
        }
        if (a.language < b.language) {
            return -1;
        }
        return 0;
    };
})();
