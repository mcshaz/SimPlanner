"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
(function () {
    'use strict';
    angular_1.default.module('breeze.directives', [])
        .directive('zFloat', [zFloat])
        .directive('zValidate', ['zDirectivesConfig', 'zValidateInfo', zValidate])
        .service('zValidateInfo', zValidateInfo)
        .provider('zDirectivesConfig', zDirectivesConfig);
    function zFloat() {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function (_scope, _elm, attr, ngModelCtrl) {
                if (attr.type === 'radio' || attr.type === 'checkbox')
                    return;
                ngModelCtrl.$formatters.push(equivalenceFormatter);
                function equivalenceFormatter(modelValue) {
                    var viewValue = ngModelCtrl.$viewValue;
                    return (+viewValue === +modelValue) ? viewValue : modelValue;
                }
            }
        };
    }
    function zValidate(config, validateInfo) {
        var directive = {
            link: link,
            restrict: 'A',
            scope: true
        };
        return directive;
        function link(scope, element, attrs) {
            var info = validateInfo.create(scope, attrs.ngModel || attrs.kNgModel, attrs.zValidate);
            if (!info.getValErrs) {
                return;
            }
            var domEl = element[0];
            var nodeName = domEl.nodeName;
            var isInput = nodeName == 'INPUT' || nodeName == 'SELECT' || nodeName == 'TEXTAREA';
            isInput ? linkForInput() : linkForNonInput();
            function linkForInput() {
                var valTemplate = config.zValidateTemplate;
                var requiredTemplate = config.zRequiredTemplate || '';
                var decorator = angular_1.default.element('<span class="z-decorator"></span>');
                if (attrs.zAppendTo) {
                    var selectedEls = document.querySelectorAll(attrs.zAppendTo);
                    if (selectedEls.length > 1) {
                        var currentEl = domEl;
                        var selectedElContainsCurrent = Array.prototype.some.bind(selectedEls, function (el) {
                            return el === currentEl;
                        });
                        while (!selectedElContainsCurrent() && currentEl.tagName !== 'BODY') {
                            currentEl = currentEl.parentElement;
                        }
                        selectedEls = currentEl.tagName === 'BODY'
                            ? null
                            : currentEl;
                    }
                    angular_1.default.element(selectedEls).append(decorator);
                }
                else {
                    element.after(decorator);
                }
                decorator = decorator[0];
                scope.$watch(info.getValErrs, valErrsChanged);
                function valErrsChanged(newValue) {
                    if (domEl.setCustomValidity) {
                        domEl.setCustomValidity(newValue || '');
                    }
                    var errorHtml = newValue ? valTemplate.replace(/%error%/, newValue) : "";
                    var isRequired = info.getIsRequired();
                    var requiredHtml = isRequired ? requiredTemplate : '';
                    decorator.innerHTML = (isRequired || !!errorHtml) ? requiredHtml + errorHtml : "";
                }
            }
            function linkForNonInput() {
                scope.$watch(info.getValErrs, valErrsChanged);
                function valErrsChanged(newValue) {
                    var errorMsg = newValue ? newValue : "";
                    scope.z_error = errorMsg;
                    scope.z_invalid = !!errorMsg;
                    scope.z_required = info.getIsRequired();
                }
            }
        }
    }
    function zValidateInfo() {
        function Info(scope, modelPath, validationPath) {
            if (!modelPath && !validationPath) {
                return;
            }
            this.scope = scope;
            setEntityAndPropertyPaths(this, modelPath, validationPath);
            this.getEntityAspect = this.entityPath ?
                getEntityAspectFromEntityPath(this) :
                getEntityAspect(this);
            this.getValErrs = createGetValErrs(this);
            this.isRequired = void 0;
        }
        Info.prototype = {
            constructor: Info,
            getIsRequired: getIsRequired,
            getType: getType
        };
        return {
            create: create,
        };
        function create(scope, modelPath, validationPath) {
            return new Info(scope, modelPath, validationPath);
        }
        function createGetValErrs(info) {
            return function () {
                var aspect = info.getEntityAspect();
                if (aspect) {
                    var errs = aspect.getValidationErrors(info.propertyPath);
                    if (errs.length) {
                        return errs
                            .map(function (e) { return e.errorMessage; })
                            .join('; ');
                    }
                    return '';
                }
                return null;
            };
        }
        function getEntityAspect(info) {
            return function () {
                return info.scope.entityAspect;
            };
        }
        function getEntityAspectFromEntityPath(info) {
            return function () {
                try {
                    var entity = info.scope.$eval(info.entityPath);
                    if (!entity.entityAspect) {
                        return info.scope.$eval(info.entityPath)['complexAspect'].getEntityAspect();
                    }
                    return info.scope.$eval(info.entityPath)['entityAspect'];
                }
                catch (_) {
                    return void 0;
                }
            };
        }
        function getIsRequired() {
            var info = this;
            if (info.isRequired !== void 0) {
                return info.isRequired;
            }
            var entityType = info.getType();
            if (entityType) {
                var requiredProperties = getRequiredPropertiesForEntityType(entityType);
                return info.isRequired = !!requiredProperties[info.propertyPath];
            }
            return void 0;
        }
        function getType() {
            var aspect = this.getEntityAspect();
            return aspect ? aspect.entity.entityType : null;
        }
        function getRequiredPropertiesForEntityType(type) {
            if (type.custom && type.custom.required) {
                return type.custom.required;
            }
            if (!type.custom) {
                type.custom = {};
            }
            var required = {};
            type.custom.required = required;
            var props = type.getProperties();
            var childProps = getComplexChildProperties(props);
            props = props.concat(childProps);
            props.forEach(function (prop) {
                var vals = prop.validators;
                for (var i = vals.length; i--;) {
                    var val = vals[i];
                    if (val.context.isRequired || val.name === 'required') {
                        required[prop.name] = true;
                        break;
                    }
                }
            });
            return required;
        }
        function getComplexChildProperties(props) {
            var children = [];
            props.forEach(function (prop) {
                if (prop.isComplexProperty) {
                    children = children.concat(prop.dataType.getProperties());
                }
            });
            return children;
        }
        function setEntityAndPropertyPaths(info, modelPath, validationPath) {
            if (modelPath) {
                parsePath(modelPath);
            }
            if (validationPath) {
                var paths = validationPath.split(',');
                var pPath = paths.pop();
                var ePath = paths.pop();
                if (ePath) {
                    info.entityPath = ePath.trim();
                }
                if (info.entityPath) {
                    info.propertyPath = pPath;
                }
                else {
                    parsePath(pPath);
                }
            }
            function parsePath(path) {
                if (path[path.length - 1] === ']') {
                    parseIndexedPaths(path);
                }
                else {
                    parseDottedPath(path);
                }
            }
            function parseDottedPath(path) {
                paths = path.split('.');
                info.propertyPath = paths.pop();
                info.entityPath = paths.join('.');
            }
            function parseIndexedPaths(path) {
                var opensb = path.lastIndexOf('[');
                info.entityPath = path.substring(0, opensb);
                var propertyPath = path.substring(opensb + 1, path.length - 1);
                try {
                    var evalPath = info.scope.$eval(propertyPath);
                }
                catch (_) { }
                info.propertyPath = evalPath ? evalPath : propertyPath;
            }
        }
    }
    function zDirectivesConfig() {
        this.zValidateTemplate =
            '<span class="invalid">%error%</span>';
        this.zRequiredTemplate =
            '<span class="icon-asterisk-invalid z-required" title="Required">*</span>';
        return {
            $get: function () {
                return {
                    zValidateTemplate: this.zValidateTemplate,
                    zRequiredTemplate: this.zRequiredTemplate
                };
            }
        };
    }
    ;
})();
//# sourceMappingURL=custom-breeze-directive.js.map