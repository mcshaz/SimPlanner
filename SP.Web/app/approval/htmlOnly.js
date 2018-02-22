"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
'use strict';
var controllerId = 'htmlOnly';
angular_1.default.module('app')
    .controller(controllerId, ['common', '$location', controller]);
function controller(common, $location) {
    var vm = this;
    vm.linkTo = linkTo;
    vm.title = 'registration';
    activate();
    function activate() {
        common.activateController([], vm.title);
    }
    function linkTo(path, params) {
        if (params) {
            return $location.path(path).search(params);
        }
        return $location.path(path);
    }
}
//# sourceMappingURL=htmlOnly.js.map