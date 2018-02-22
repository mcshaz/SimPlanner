"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
'use strict';
var serviceId = 'selectOptionMaps';
angular_1.default.module('app')
    .factory(serviceId, ['tokenStorageService', '$http', factoryMethod]);
function factoryMethod(tokenStorageService, $http) {
    var _roleIcons;
    var service = {
        fetchCultureFormats: fetchCultureFormats,
        filterLocalDepartments: filterLocalDepartments,
        getRoleIcon: getRoleIcon,
        getFlagClassFromLocaleCode: getFlagClassFromLocaleCode,
        mapDepartment: mapDepartment,
        roleSymbols: {
            Medical: 'stethoscope',
            Tech: 'wrench',
            Allied: 'sign-language',
            Other: 'question',
            Paramedic: 'ambulance',
            Nursing: 'heartbeat',
            Educator: 'graduation-cap'
        },
        sortAndMapDepartment: sortAndMapDepartment,
        sortDepartment: sortDepartment
    };
    return service;
    function fetchCultureFormats() {
        return $http({ method: 'GET', url: 'api/utilities/cultureFormats' }).then(function (response) {
            var parseLanguageCulture = /([\w ]+)\((.+)\)/;
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
    function filterLocalDepartments() {
        var userCulture = tokenStorageService.getUserLocale();
        return function (d) { return d.institution.culture.localeCode === userCulture; };
    }
    function getFlagClassFromLocaleCode(localeCode) {
        return 'flag ' + localeCode.substring(localeCode.length - 2).toLowerCase();
    }
    function getRoleIcon(roleName) {
        if (!_roleIcons) {
            _roleIcons = {};
            angular_1.default.forEach(service.roleSymbols, function (val, key) {
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
        ds.sort(sortDepartment);
        return ds.map(mapDepartment);
    }
    function sortDepartment(a, b) {
        if (a.institution.name > b.institution.name) {
            return 1;
        }
        if (a.institution.name < b.institution.name) {
            return -1;
        }
        if (a.abbreviation > b.abbreviation) {
            return 1;
        }
        if (a.abbreviation < b.abbreviation) {
            return -1;
        }
        return 0;
    }
}
//# sourceMappingURL=selectOptionMaps.js.map