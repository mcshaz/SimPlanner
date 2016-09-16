; window.emptyGuid = "00000000-0000-0000-0000-000000000000";
(function () {
    var validator = breeze.Validator;
    //factory methods (to be loaded from metadata)
    validator.registerFactory(createRangeValidator, "numericRange");
    validator.registerFactory(createFullnameValidator, "personFullName");

    //validator instances (to be added manually)
    requireReferenceValidator = createRequireReferenceValidator();
    validator.register(requireReferenceValidator);
    validator.requireReferenceValidator = requireReferenceValidator;
    

    function createTwitterValidator() {
        // create the validator
        var val = validator.makeRegExpValidator(
            'twitter',
            /^@([a-zA-Z]+)([a-zA-Z0-9_]+)$/,
            "Invalid Twitter user name: '%value%'");

        // register it with the validator class
        validator.register(val);

        // make it available as a validator static fn like the others
        validator.twitter = function () { return val; }

    }

    function createRangeValidator(context) {
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
            if (v == null) return true;
            if (typeof (v) !== "number") return false;
            if (ctx.min != null && v < ctx.min) return false;
            if (ctx.max != null && v > ctx.max) return false;
            return true;
        };
    };

    function createFullnameValidator(context) {
        // The last parameter below is the 'context' object that will be passed into the 'ctx' parameter above
        // when this validator executes. Several other properties, such as displayName will get added to this object as well.
        var template = breeze.core.formatString(
            "'%displayName%' of %1-%2 words, first & last of ≥%3 letters",
            context.minNames, context.maxNames, context.minNameLength);
        return new validator("personFullName", valFn, {
            messageTemplate: template
        });

        function valFn(v, ctx) {
            if (v == null) return true;
            if (typeof (v) !== "string" || !v.length) return false;
            var names = v.trim().split(' ');
            return (names.length <= context.maxNames && names.length >= context.minNames &&
                names[0].length >= context.minNameLength &&
                names[names.length - 1].length >= context.minNameLength);
        };
    };
    //http://stackoverflow.com/questions/16733251/breezejs-overriding-displayname
    function createRequireReferenceValidator() {
        var name = 'requireReferenceEntity';
        // isRequired = true so zValidate directive displays required indicator
        var ctx = { messageTemplate: 'Missing %displayName%', isRequired: true };
        var val = new validator(name, valFunction, ctx);
        return val;

        // passes if reference has a value and is not the nullo (whose id===0)
        function valFunction(value) {
            return value ? value.id !== window.emptyGuid : false;
        }
    }
})();
