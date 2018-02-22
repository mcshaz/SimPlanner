"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
var controllerId = 'topnav';
angular_1.default.module('app').controller(controllerId, [topnav]);
function topnav() {
    var vm = this;
    vm.drawerOut = true;
    vm.toggleDrawer = toggleDrawer;
    function toggleDrawer() {
        vm.drawerOut = !vm.drawerOut;
    }
}
//# sourceMappingURL=topnav.js.map