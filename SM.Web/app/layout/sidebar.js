(function () { 
    'use strict';
    
    var controllerId = 'sidebar';
    angular.module('app').controller(controllerId,
        ['$route', 'config', 'routes', sidebar]);

    function sidebar($route, config, routes) {
        var vm = this;

        vm.isCurrent = isCurrent;

        activate();

        function activate() { getNavRoutes(); }
        
        function getNavRoutes() {
            var rx = /:id$/;
            vm.navRoutes = routes.filter(function(r) {
                return r.config.settings && r.config.settings.nav;
            }).sort(function(r1, r2) {
                return r1.config.settings.nav - r2.config.settings.nav;
            });
            vm.navRoutes.forEach(function (el) { el.url = el.url.replace(rx, 'new')});
        }
        
        function isCurrent(route) {
            if (!route.config.title || !$route.current || !$route.current.title) {
                return '';
            }
            var menuName = route.config.title;
            return $route.current.title.substr(0, menuName.length) === menuName ? 'current' : '';
        }
    };
})();
