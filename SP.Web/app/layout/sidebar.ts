import angular from 'angular';
    
    var controllerId = 'sidebar';
export default angular.module('app').controller(controllerId,
        ['$route', 'routes', 'tokenStorageService', '$rootScope','AUTH_EVENTS',sidebar]);

    function sidebar($route, routes, tokenStorageService, $rootScope, AUTH_EVENTS) {
        var vm = this;

        vm.isCurrent = isCurrent;

        $rootScope.$on(AUTH_EVENTS.loginConfirmed, getNavRoutes);
        $rootScope.$on(AUTH_EVENTS.loginCancelled, getNavRoutes);

        activate();

        function activate() { getNavRoutes(); }
        
        function getNavRoutes() {
            var rx = /:id$/;
            vm.navRoutes = routes.filter(function(r) {
                return r.config.settings && r.config.settings.nav && (!r.config.access || tokenStorageService.isAuthorized(r.config.access.allowedRoles));
            }).sort(function(r1, r2) {
                return r1.config.settings.nav - r2.config.settings.nav;
            });
            vm.navRoutes.forEach(function (el) { el.url = el.url.replace(rx, 'new');});
        }
        
        function isCurrent(route) {
            if (!route.config.title || !$route.current || !$route.current.title) {
                return '';
            }
            var menuName = route.config.title;
            return $route.current.title.substr(0, menuName.length) === menuName ? 'current' : '';
        }
    }

