(function() { 
    'use strict';
    angular.module('ngLocationUpdate', [])
        .run(['$route', '$rootScope', '$location', 'common', 'commonConfig', locationUpdate]);
    function locationUpdate($route, $rootScope, $location,common, commonConfig) {
        // todo: would be proper to change this to decorators of $location and $route
        $location.updatePath = function (path, keep_previous_path_in_history) {
            if ($location.path() === path) { return; }

            var routeToKeep = $route.current;
            var unbind = $rootScope.$on('$locationChangeSuccess', function () {
                    if (routeToKeep) {
                        $route.current = routeToKeep;
                        routeToKeep = null;
                    }
                    common.$broadcast(commonConfig.config.controllerActivateSuccessEvent); //switch the spinner off
                    unbind();
                    unbind = null;
                });

            $location.path(path);
            if (!keep_previous_path_in_history) { $location.replace(); }
        };
    };
})();