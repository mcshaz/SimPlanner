(function () {
    'use strict';
    var serviceId = 'googleTools';
    angular.module('app')
        .factory(serviceId, [factory]);

    function factory() {

        var service = {
            getLocaleHostname: getLocaleHostname
        };
        return service;
    }

    function getLocaleHostname(countryCode) {
        var baseUrl = "https://www.google";
        if (!countryCode) {
            return baseUrl + '.com';
        }
        countryCode = countryCode.toLowerCase();
        if (countryCode === 'us') {
            return baseUrl + '.com';
        }
        return baseUrl
            + getCom(countryCode)
            + "." + countryCode;
    }

    function getCom(countryCode) {
        if (['ao', 'bw', 'ck', 'cr', 'id', 'il', 'in', 'jp', 'ke', 'kr', 'ls', 'ma', 'mz', 'nz', 'th', 'tz', 'ug', 'uk', 'uz', 've', 'vi', 'za', 'zm', 'zw']
                .indexOf(countryCode) > -1) {
            return ".co";
        }
        if (['af','ag','ai','ar','au','bd','bh','bn','bo','br','bz','co','cu','cy','do','ec','eg','et','fj','gh','gi','gt','hk','jm','kh','kw','lb','lc','ly','mm','mt','mx','my','na','nf','ng','ni','np','om','pa','pe','pg','ph','pk','pr','py','qa','sa','sb','sg','sl','sv','tj','tr','tw','ua','us','uy','vc','vn']
                .indexOf(countryCode) > -1) {
            return ".com";
        }
        return "";
    }
})();