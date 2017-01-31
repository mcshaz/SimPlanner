(function () {
    'use strict';
    var controllerId = 'deleteCourseTypeCtrl';
    angular
        .module('app')
        .controller(controllerId, controller);

    controller.$inject = ['$scope', 'options'];

    function controller($scope, options) {
        /* jshint validthis:true */
        //var vm = this;
        activate();

        function activate() {
            var v;
            for (var p in options) {
                v = options[p];
                if (angular.isFunction(v)) {
                    $scope[p] = function () {
                        v();
                        $scope.$parent.$hide();
                    };
                } else {
                    $scope[p] = v;
                }
            }
        }
    }
})();
