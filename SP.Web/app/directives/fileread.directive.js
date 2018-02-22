"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
angular_1.default.module('app').directive('appFilereader', ['$q', function ($q) {
        return {
            restrict: 'A',
            require: '?ngModel',
            scope: {
                ngLastModified: '=',
                ngFileSize: '=',
                ngFileName: '='
            },
            link: function (scope, element, _attrs, ngModel) {
                if (!ngModel)
                    return;
                var stripData = /^data:[^;]*;base64,/;
                ngModel.$render = function () { };
                element.bind('change', function (e) {
                    var file = e.target.files[0];
                    scope.ngFileName = file.name;
                    scope.ngLastModified = file.lastModified;
                    scope.ngFileSize = file.size;
                    readFile(file)
                        .then(function (values) {
                        ngModel.$setViewValue(values.length ? values.replace(stripData, '') : null);
                    });
                    function readFile(file) {
                        var deferred = $q.defer();
                        var reader = new FileReader();
                        reader.onload = function (e) {
                            deferred.resolve(e.target.result);
                        };
                        reader.onerror = function (e) {
                            deferred.reject(e);
                        };
                        reader.readAsDataURL(file);
                        return deferred.promise;
                    }
                });
                scope.$on('$destory', function () { element.unbind("change"); });
            }
        };
    }]);
//# sourceMappingURL=fileread.directive.js.map