import angular from 'angular';

// Include in index.html so that app level exceptions are handled.
// Exclude from testRunner.html which should run exactly what it wants to run

    
    var app = angular.module('app');

    // Configure by setting an optional string value for appErrorPrefix.
    // Accessible via config.appErrorPrefix (via config value).

    app.config(['$provide', function ($provide) {
        $provide.decorator('$exceptionHandler',
            ['$delegate', 'config', 'logger', extendExceptionHandler]);
    }]);
    
    // Extend the $exceptionHandler service to also display a toast.
    function extendExceptionHandler($delegate, config, logger) {
        var appErrorPrefix = config.appErrorPrefix;
        var log = logger.getLogFn('app');
        return function (exception, cause) {
            $delegate(exception, cause);
            if (appErrorPrefix && exception.message.indexOf(appErrorPrefix) === 0) { return; }

            var errorData = { exception: exception, cause: cause };
            var msg = appErrorPrefix + exception.message;
            log.error({ msg: msg, data: errorData, showToast: true });
        };
    }
