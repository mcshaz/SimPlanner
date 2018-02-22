"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var breeze = require("breeze-client");
exports.emptyGuid = "00000000-0000-0000-0000-000000000000";
var validator = breeze.Validator;
validator.registerFactory(rangeValidatorFactory, "numericRange");
validator.registerFactory(createFullnameValidatorFactory, "personFullName");
validator.requireReferenceValidator = requireReferenceValidatorFactory();
validator.register(validator.requireReferenceValidator);
validator.comesBeforeValidatorFactory = comesBeforeValidatorFactory;
validator.noRepeatActivityNameValidatorFactory = noRepeatActivityNameValidatorFactory;
function rangeValidatorFactory(context) {
    var template = context.messageTemplate || breeze.core.formatString("'%displayName%' must be a number between the values of %1 and %2", context.min, context.max);
    return new validator("numericRange", valFn, {
        messageTemplate: template,
        min: context.min,
        max: context.max
    });
    function valFn(v, ctx) {
        if (v === null)
            return true;
        if (typeof v !== "number")
            return false;
        if (ctx.min !== null && v < ctx.min)
            return false;
        if (ctx.max !== null && v > ctx.max)
            return false;
        return true;
    }
}
function createFullnameValidatorFactory(context) {
    var template = breeze.core.formatString("'%displayName%' of %1-%2 words, first & last of ≥%3 letters", context.minNames, context.maxNames, context.minNameLength);
    return new validator("personFullName", valFn, {
        messageTemplate: template
    });
    function valFn(v, _ctx) {
        if (v === null)
            return true;
        if (typeof v !== "string" || !v.length)
            return false;
        var names = v.trim().split(' ');
        return names.length <= context.maxNames && names.length >= context.minNames &&
            names[0].length >= context.minNameLength &&
            names[names.length - 1].length >= context.minNameLength;
    }
}
function requireReferenceValidatorFactory() {
    return new validator('requireReferenceEntity', valFunction, { messageTemplate: 'Missing %displayName%', isRequired: true });
    function valFunction(value) {
        return value ? value.id !== exports.emptyGuid : false;
    }
}
function comesBeforeValidatorFactory(ctx) {
    return new validator('comesBefore' + ctx.nameOnServer, valFunction, { messageTemplate: '%displayName% must be before %later%', later: ctx.displayName });
    function valFunction(earlierVal, c) {
        var laterVal = c.entity.getProperty(ctx.name);
        return !(laterVal && earlierVal) || earlierVal < laterVal;
    }
}
function noRepeatActivityNameValidatorFactory() {
    return new validator('noRepeatActivityName', valFunction, { messageTemplate: 'Cannot have repeat names. Delete and creating a new %value% rather than rename %originalValue%' });
    function valFunction(name, c) {
        var otherActivities = c.entity.getProperty('courseType').courseActivities;
        var id = c.entity.getProperty('id');
        name = name.toLowerCase();
        return !otherActivities.some(function (a) { return a.id !== id && a.name.toLowerCase() === name; });
    }
}
//# sourceMappingURL=customValidators.js.map