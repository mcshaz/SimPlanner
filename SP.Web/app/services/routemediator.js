"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
var app = angular_1.default.module('app');
var serviceId = 'routemediator';
app.factory(serviceId, ['$location', '$rootScope', 'config', 'logger', routemediator]);
function routemediator($location, $rootScope, config, logger) {
    var handlingRouteChangeError = false;
    var service = {
        setRoutingEventHandlers: setRoutingEventHandlers
    };
    function setRoutingEventHandlers() {
        handleRoutingErrors();
        updateDocTitle();
    }
    function handleRoutingErrors() {
        $rootScope.$on('$routeChangeError', function (_event, current, _previous, rejection) {
            if (handlingRouteChangeError) {
                return;
            }
            handlingRouteChangeError = true;
            var destination = (current && (current.title || current.name || current.loadedTemplateUrl))
                || "unknown target";
            var msg = 'Error routing to ' + destination + '. ' + (rejection.msg || '');
            logger.logWarning(msg, current, serviceId, true);
            $location.path('/');
        });
    }
    function updateDocTitle() {
        $rootScope.$on('$routeChangeSuccess', function (_event, current, _previous) {
            handlingRouteChangeError = false;
            var title = config.docTitle + ' ' + (current.title || '');
            $rootScope.title = title;
        });
    }
    return service;
}
//# sourceMappingURL=routemediator.js.map