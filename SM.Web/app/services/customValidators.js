(function () {
    var Validator = breeze.Validator;
    Validator.registerFactory(createRangeValidator, "numericRange");

    function createTwitterValidator() {
        // create the validator
        var val = Validator.makeRegExpValidator(
            'twitter',
            /^@([a-zA-Z]+)([a-zA-Z0-9_]+)$/,
            "Invalid Twitter user name: '%value%'");

        // register it with the Validator class
        Validator.register(val);

        // make it available as a Validator static fn like the others
        Validator.twitter = function () { return val; }

    }

    function createRangeValidator(context) {
        var valFn = function (v, ctx) {
            if (v == null) return true;
            if (typeof (v) !== "number") return false;
            if (ctx.min != null && v < ctx.min) return false;
            if (ctx.max != null && v > ctx.max) return false;
            return true;
        };
        var template = breeze.core.formatString(
        "'%displayName%' must be a number between the values of %1 and %2",
        context.min, context.max);
        // The last parameter below is the 'context' object that will be passed into the 'ctx' parameter above
        // when this validator executes. Several other properties, such as displayName will get added to this object as well.
        return new Validator("numericRange", valFn, {
            messageTemplate: template,
            min: context.min,
            max: context.max
        });
    };
})();
