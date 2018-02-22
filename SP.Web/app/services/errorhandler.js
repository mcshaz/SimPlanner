"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
angular_1.default.module('app').factory('errorhandler', ['logger', 'utilities', factory]);
function factory(logger, util) {
    var ErrorHandler = (function () {
        var ctor = function (targetObject) {
            this.log = logger.getLogFn(getModuleId(targetObject));
            this.handleError = function (error) {
                if (error.entityErrors) {
                    error.message = util.getSaveValidationErrorMessage(error);
                }
                this.log.eror(error.message);
                throw error;
            };
        };
        return ctor;
    })();
    return {
        includeIn: includeIn
    };
    function includeIn(targetObject) {
        return $.extend(targetObject, new ErrorHandler(targetObject));
    }
    function getModuleId(obj) {
        return obj.moduleId;
    }
}
//# sourceMappingURL=errorhandler.js.map