"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
var app = angular_1.default.module('app');
app.directive('ccImgPerson', ['config', function (config) {
        var basePath = config.imageSettings.imageBasePath;
        var unknownImage = config.imageSettings.unknownPersonImageSource;
        var directive = {
            link: link,
            restrict: 'A'
        };
        return directive;
        function link(_scope, _element, attrs) {
            attrs.$observe('ccImgPerson', function (value) {
                value = basePath + (value || unknownImage);
                attrs.$set('src', value);
            });
        }
    }]);
app.directive("compareTo", function () {
    return {
        require: "ngModel",
        scope: {
            otherModelValue: "=compareTo"
        },
        link: function (scope, _element, _attributes, ngModel) {
            ngModel.$validators.compareTo = function (modelValue) {
                return modelValue === scope.otherModelValue;
            };
            scope.$watch("otherModelValue", function () {
                ngModel.$validate();
            });
        }
    };
});
app.directive("validPassword", function () {
    return {
        require: "ngModel",
        link: function (_scope, _element, _attributes, ngModel) {
            ngModel.$validators.validPassword = function (modelValue) {
                if (!modelValue) {
                    return true;
                }
                var containsDigit = /[0-9]/.test(modelValue);
                var containsLC = /[a-z]/.test(modelValue);
                var containsUC = /[A-Z]/.test(modelValue);
                var containsNonAlnum = /[^a-zA-Z0-9]/.test(modelValue);
                return containsDigit && containsLC && containsUC && containsNonAlnum;
            };
        }
    };
});
app.directive('ccScrollToTop', ['$window',
    function ($window) {
        var directive = {
            link: link,
            template: '<a href="#"><i class="fa fa-chevron-up"></i></a>',
            restrict: 'A'
        };
        return directive;
        function link(_scope, element, _attrs) {
            var $win = $($window);
            element.addClass('totop');
            $win.scroll(toggleIcon);
            element.find('a').click(function (e) {
                e.preventDefault();
                $('html, body').animate({ scrollTop: 0 }, 500);
            });
            function toggleIcon() {
                $win.scrollTop() > 300 ? element.slideDown() : element.slideUp();
            }
        }
    }
]);
app.directive('ccSpinner', ['$window', function ($window) {
        var directive = {
            link: link,
            restrict: 'A'
        };
        return directive;
        function link(scope, element, attrs) {
            scope.spinner = null;
            scope.$watch(attrs.ccSpinner, function (options) {
                if (scope.spinner) {
                    scope.spinner.stop();
                }
                scope.spinner = new $window.Spinner(options);
                scope.spinner.spin(element[0]);
            }, true);
        }
    }]);
//# sourceMappingURL=directives.js.map