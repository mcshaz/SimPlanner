"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
var controllerId = 'deleteCourseTypeCtrl';
angular_1.default
    .module('app')
    .controller(controllerId, controller);
controller.$inject = ['$scope', 'options'];
function controller($scope, options) {
    activate();
    function activate() {
        var v;
        for (var p in options) {
            v = options[p];
            if (angular_1.default.isFunction(v)) {
                $scope[p] = function () {
                    v();
                    $scope.$parent.$hide();
                };
            }
            else {
                $scope[p] = v;
            }
        }
    }
}
//# sourceMappingURL=deleteCourseType.js.map