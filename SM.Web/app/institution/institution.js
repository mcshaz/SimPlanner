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
        })
        var id = $routeParams.id;
        var isNew = id == 'new';

        vm.institution = {};
        vm.cultureFormats=[];
        vm.timeZonesForCulture = [];
        vm.getCultureFormats = getCultureFormats;
        vm.googleMapAddress="";
        vm.mapAddressChanged = mapAddressChanged;
        vm.onLocaleSelected = onLocaleSelected;
        vm.flagUrl = '';

        activate();

        function activate() {
            var promises = [common.fetchCultureFormats().then(function (data) {
                vm.cultureFormats = data;
                vm.cultureFormats.forEach(function (el) {
                    el.searchString = el.Key.toLowerCase() + ' ' + el.Value.toLowerCase();
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

        function onLocaleSelected(key) {
            vm.flagUrl = vm.cultureFormats.find(function (el) { return el.Key === key });
            if (vm.flagUrl) {
                vm.flagUrl = vm.flagUrl.flagUrl;
                $http({ method: 'GET', url: 'api/utilities/timeZones/' + key }).then(function (response) {
                    vm.timeZonesForCulture = response.data;
                    if (vm.timeZonesForCulture.length === 1) {
                        vm.institution.standardTimeZone = vm.timeZonesForCulture[0];
                    }
                }, vm.log.error)
            }

        }

        function getCultureFormats(val) {
            val = val.toLowerCase();
            return vm.cultureFormats.filter(function (el) { return el.searchString.indexOf(val) !== -1; });
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

})();
