(function () {
    'use strict';

    // Must configure the common service and set its 
    // events via the commonConfigProvider
    var serviceId = 'selectOptionMaps';

    angular.module('app')
        .factory(serviceId, ['tokenStorageService', '$http', factoryMethod]);

    function factoryMethod(tokenStorageService, $http) {
        var _roleIcons;
        var service = {
            fetchCultureFormats: fetchCultureFormats,
            filterLocalDepartments: filterLocalDepartments,
            getEnumValues: window.medsimMetadata.getEnums,
            getRoleIcon: getRoleIcon,
            getFlagClassFromLocaleCode: getFlagClassFromLocaleCode,
            mapDepartment: mapDepartment,
            roleSymbols: {
                Medical: 'stethoscope',
                Tech: 'wrench',
                Perfusionist: 'cog',
                Other: 'question',
                Paramedic: 'ambulance',
                Nursing: 'heartbeat',
                Educator: 'graduation-cap'
            },
            sortAndMapDepartment: sortAndMapDepartment,
            sortDepartment: sortDepartment
        };
        //var log = logger.getLogFn(serviceId);

        return service;

        function fetchCultureFormats() {
            return $http({ method: 'GET', url: 'api/utilities/cultureFormats' }).then(function (response) {
                var parseLanguageCulture = /([\w ]+)\(([\w ,.]+)\)/;
                return response.data.map(function (el) {
                    var parsed = parseLanguageCulture.exec(el.DisplayName);
                    if (!parsed || parsed.length < 3) {
                        console.log(el);
                        console.log(parsed);
                    }
                    return {
                        localeCode: el.LocaleCode,
                        flagClass: getFlagClassFromLocaleCode(el.LocaleCode),
                        culture: parsed[2],
                        language: parsed[1].trimRight()
                    };
                });
            });
        }

        function filterLocalDepartments(ds) {
            var userCulture = tokenStorageService.getUserLocale();
            return function (d) { return d.institution.culture.localeCode === userCulture; };
        }

        function getFlagClassFromLocaleCode(localeCode) {
            return 'flag ' + localeCode.substring(localeCode.length - 2).toLowerCase();
        }

        function getRoleIcon(roleName) {
            if (!_roleIcons) {
                _roleIcons = {};
                angular.forEach(service.roleSymbols, function (val, key) {
                    _roleIcons[key] = "fa fa-" + val;
                });
            }
            return _roleIcons[roleName];
        }

        function mapDepartment(d) {
            var localeCode = d.institution.culture.localeCode;
            var countryCode = localeCode.substring(localeCode.length - 2);
            return {
                id: d.id,
                name: d.name,
                abbreviation: d.institution.abbreviation + ' ' + d.abbreviation,
                searchString: countryCode + "#" + d.institution.culture.name + "#" + d.institution.name.toLowerCase() + "#" + d.institution.abbreviation.toLowerCase() + "#" + d.name.toLowerCase() + "#" + d.abbreviation.toLowerCase(),
                institutionName: countryCode + '-' + d.institution.abbreviation,
                flagClass: getFlagClassFromLocaleCode(localeCode)
            };
        }

        function sortAndMapDepartment(ds) {
            ds.sort(sortDepartment); //maybe a hack - the deparmtents will be stored in the entity manager, and sorting this array may make subsequent sorts quicker
            return ds.map(mapDepartment);
        }

        function sortDepartment(a,b) {
            if (a.institution.name > b.institution.name) { return 1; }
            if (a.institution.name < b.institution.name) { return -1; }
            if (a.abbreviation > b.abbreviation) { return 1; }
            if (a.abbreviation < b.abbreviation) { return -1; }
            return 0;
        }
    }
})();