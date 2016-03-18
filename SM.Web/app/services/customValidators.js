(function () {
    var validator = breeze.Validator;
    validator.registerFactory(createRangeValidator, "numericRange");
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
        var template = breeze.core.formatString(
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

    function createRequireReferenceValidator() {
        var name = 'requireReferenceEntity';
        // isRequired = true so zValidate directive displays required indicator
        var ctx = { messageTemplate: 'Missing %displayName%', isRequired: true };
        var val = new validator(name, valFunction, ctx);
        return val;

        // passes if reference has a value and is not the nullo (whose id===0)
        function valFunction(value) {
            return value ? value.id !== "00000000-0000-0000-0000-000000000000" : false;
        }
    }
})();
