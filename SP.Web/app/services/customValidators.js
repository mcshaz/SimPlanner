window.emptyGuid = "00000000-0000-0000-0000-000000000000";
(function () {
    var validator = breeze.Validator;
    //factory methods (to be loaded from metadata)
    validator.registerFactory(rangeValidatorFactory, "numericRange");
    validator.registerFactory(createFullnameValidatorFactory, "personFullName");

    //validator instances (to be added manually)
    validator.requireReferenceValidator = requireReferenceValidatorFactory();
    validator.register(validator.requireReferenceValidator);

    validator.comesBeforeValidatorFactory = comesBeforeValidatorFactory;

    validator.noRepeatActivityNameValidatorFactory = noRepeatActivityNameValidatorFactory;

    function createTwitterValidator() {
        // create the validator
        var val = validator.makeRegExpValidator(
            'twitter',
            /^@([a-zA-Z]+)([a-zA-Z0-9_]+)$/,
            "Invalid Twitter user name: '%value%'");

        // register it with the validator class
        validator.register(val);

        // make it available as a validator static fn like the others
        validator.twitter = function () { return val; };

    }

    function rangeValidatorFactory(context) {
        var template = context.messageTemplate || breeze.core.formatString(
            "'%displayName%' must be a number between the values of %1 and %2",
            context.min, context.max);
        // The last parameter below is the 'context' object that will be passed into the 'ctx' parameter above
        // when this validator executes. Several other properties, such as displayName will get added to this object as well.
        return new validator("numericRange", valFn, {
            messageTemplate: template,
            min: context.min,
            max: context.max
        });

        function valFn(v, ctx) {
            if (v === null) return true;
            if (typeof v !== "number") return false;
            if (ctx.min !== null && v < ctx.min) return false;
            if (ctx.max !== null && v > ctx.max) return false;
            return true;
        }
    }

    function createFullnameValidatorFactory(context) {
        // The last parameter below is the 'context' object that will be passed into the 'ctx' parameter above
        // when this validator executes. Several other properties, such as displayName will get added to this object as well.
        var template = breeze.core.formatString(
            "'%displayName%' of %1-%2 words, first & last of ≥%3 letters",
            context.minNames, context.maxNames, context.minNameLength);
        return new validator("personFullName", valFn, {
            messageTemplate: template
        });

        function valFn(v, ctx) {
            if (v === null) return true;
            if (typeof v !== "string" || !v.length) return false;
            var names = v.trim().split(' ');
            return names.length <= context.maxNames && names.length >= context.minNames &&
                names[0].length >= context.minNameLength &&
                names[names.length - 1].length >= context.minNameLength;
        }
    }
    //http://stackoverflow.com/questions/16733251/breezejs-overriding-displayname
    function requireReferenceValidatorFactory() {
        return new validator('requireReferenceEntity', valFunction, { messageTemplate: 'Missing %displayName%', isRequired: true });

        // passes if reference has a value and is not the nullo (whose id===0)
        function valFunction(value) {
            return value ? value.id !== window.emptyGuid : false;
        }
    }

    function comesBeforeValidatorFactory(ctx) {
        // isRequired = true so zValidate directive displays required indicator
        return new validator('comesBefore' + ctx.nameOnServer, valFunction, { messageTemplate: '%displayName% must be before %later%', later: ctx.displayName });

        function valFunction(earlierVal, c) {
            var laterVal = c.entity.getProperty(ctx.name);

            return !(laterVal && earlierVal) || earlierVal<laterVal;
        }
    }

    function noRepeatActivityNameValidatorFactory() {
        // isRequired = true so zValidate directive displays required indicator
        return new validator('noRepeatActivityName', valFunction, { messageTemplate: 'Cannot have repeat names. Delete and creating a new %value% rather than rename %originalValue%' });

        function valFunction(name, c) {
            var otherActivities = c.entity.getProperty('courseType').courseActivities;
            var id = c.entity.getProperty('id');
            name = name.toLowerCase();
            return !otherActivities.some(function (a) { return a.id !== id && a.name.toLowerCase() === name; }); //quick and dirty - should somehow hook into breeze engine which uses LocalQueryComparisonOptions
        }
    }

})();
