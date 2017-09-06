(function () { 
    'use strict';
    
    var controllerId = 'shell';
    angular.module('app').controller(controllerId,
        ['$scope','$rootScope', 'common', 'config', 'tokenStorageService', 'AUTH_EVENTS', '$window',shell]);

    function shell($scope, $rootScope, common, config, tokenStorageService, AUTH_EVENTS, $window) {
        var vm = this;
        var log = common.logger.getLogFn(controllerId);
        var events = config.events;
        var lastSize = common.currentBootstrapSize();
        vm.menuOpen = !lastSize.lte('sm');
        //vm.busyMessage = 'Please wait ...';
        vm.isBusy = true;
        vm.spinnerOptions = {
            radius: 40,
            lines: 7,
            length: 0,
            width: 30,
            speed: 1.7,
            corners: 1.0,
            trail: 100,
            color: '#F58A00'
        };

        activate();

        function activate() {
            log.success('Simulation Planner loaded!');
            common.activateController([], controllerId);
        }

        function toggleSpinner(on) { vm.isBusy = on; }

        $rootScope.$on('$routeChangeStart', newRoute);

        var _isGuid = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/;
        function newRoute(event, next, current) {
            if (event.defaultPrevented) { return; }
            if (current.params && current.params.id === "new" && next.params && _isGuid.test(next.params.id)) {
                return;
            }
            toggleSpinner(true);
        }
        
        $rootScope.$on(events.controllerActivateSuccess,
            function (data) { toggleSpinner(false); }
        );

        $rootScope.$on(events.spinnerToggle,
            function (data) { toggleSpinner(data.show); }
        );

        angular.element($window).bind('resize', function () {
            common.debouncedThrottle('shellWindowResize', function () {
                var currentSize = common.currentBootstrapSize();
                if (!lastSize.lte('sm') && currentSize.lte('sm')) {
                    vm.menuOpen = false;
                    $scope.$apply();
                }
                lastSize = currentSize;
            }, 150);
        });
    }
})();