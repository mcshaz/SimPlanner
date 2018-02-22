"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
angular_1.default.module('app')
    .factory('utilities', [factory]);
function factory() {
    return {
        getCurrentDate: getCurrentDate,
        getSaveValidationErrorMessage: getSaveValidationErrorMessage,
        getEntityValidationErrorMessage: getEntityValidationErrorMessage
    };
    function getCurrentDate() {
        return new Date();
    }
    function getSaveValidationErrorMessage(saveError) {
        try {
            var firstError = saveError.entityErrors[0];
            return 'Validation Error: ' + firstError.errorMessage;
        }
        catch (e) {
            return "Save validation error";
        }
    }
    function getEntityValidationErrorMessage(entity) {
        try {
            var errs = entity.entityAspect.getValidationErrors();
            var errmsgs = errs.map(function (ve) { return ve.errorMessage; });
            return errmsgs.length ? errmsgs.join("; ") : "no validation errors";
        }
        catch (e) {
            return "not an entity";
        }
    }
}
//# sourceMappingURL=utilities.js.map