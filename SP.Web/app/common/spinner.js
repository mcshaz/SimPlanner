"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
require("spin.js");
var angular_1 = require("angular");
'use strict';
angular_1.default.module('common')
    .factory('spinner', ['common', 'commonConfig', spinner]);
function spinner(common, commonConfig) {
    var service = {
        spinnerHide: spinnerHide,
        spinnerShow: spinnerShow
    };
    return service;
    function spinnerHide() { spinnerToggle(false); }
    function spinnerShow() { spinnerToggle(true); }
    function spinnerToggle(show) {
        common.$broadcast(commonConfig.config.spinnerToggleEvent, { show: show });
    }
}
//# sourceMappingURL=spinner.js.map