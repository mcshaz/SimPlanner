(function () {
    'use strict';

    var controllerId = 'topnav';
    angular.module('app').controller(controllerId,['common',topnav]);
    
    function topnav(common) {
        var vm = this;
        vm.drawerOut = true;
        vm.toggleDrawer = toggleDrawer;
        //$('.mainbar').css("margin-left", "0px");

        function toggleDrawer() {
            vm.drawerOut = !vm.drawerOut;
            common.$broadcast(events.sidebarToggle, vm.drawerOut);
            /*
            if ($('.sidebar').hasClass('hide-drawer')) {
                $('.sidebar').removeClass('hide-drawer');
                $('.mainbar').css("margin-left", "230px");
            } else {
                $('.sidebar').addClass('hide-drawer');
                $('.mainbar').css("margin-left", "0px");
            }
            */
        }
    };
})();
