"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
var app = angular_1.default.module('app');
app.config(['$provide', function ($provide) {
        $provide.decorator('$exceptionHandler', ['$delegate', 'config', 'logger', extendExceptionHandler]);
    }]);
function extendExceptionHandler($delegate, config, logger) {
    var appErrorPrefix = config.appErrorPrefix;
    var log = logger.getLogFn('app');
    return function (exception, cause) {
        $delegate(exception, cause);
        if (appErrorPrefix && exception.message.indexOf(appErrorPrefix) === 0) {
            return;
        }
        var errorData = { exception: exception, cause: cause };
        var msg = appErrorPrefix + exception.message;
        log.error({ msg: msg, data: errorData, showToast: true });
    };
}
//# sourceMappingURL=config.exceptionHandler.js.map