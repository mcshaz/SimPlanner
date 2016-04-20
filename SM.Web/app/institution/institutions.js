(function () {
    'use strict';
    var controllerId = 'cultures';
    angular
        .module('app')
        .controller(controllerId, controller);

    controller.$inject = ['common', 'datacontext'];
    //changed $uibModalInstance to $scope to get the events

    function controller(common, datacontext) {
        /* jshint validthis:true */
        var vm = this;
        var log = common.logger.getLogFn(controllerId);
        vm.cultures = [];

        activate();

        function activate() {
            common.activateController([
                datacontext.cultures.findServerIfCacheEmpty({ expand: ['institutions.departments.manequins', 'institutions.departments.scenarios'] }).then(function (data) {
                    data.sort(common.sortOnPropertyName('name'));
                    vm.cultures = data;
                })], controllerId).then(function () {
                    vm.cultures.forEach(function (c) {
                        c.flagUrl = common.getFlagUrlFromLocaleCode(c.localeCode);
                    });

                    log.info('Activated Admin overview');
                });
        }
    }

})();