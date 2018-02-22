"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
var toastr = require("toastr");
var app = angular_1.default.module('app');
toastr.options.timeOut = 4000;
toastr.options.positionClass = 'toast-bottom-right';
var remoteServiceName = 'breeze/Breeze';
var events = {
    controllerActivateSuccess: 'controller.activateSuccess',
    spinnerToggle: 'spinner.toggle'
};
var config = {
    appErrorPrefix: '[SP Error] ',
    docTitle: 'SimPlanner: ',
    events: events,
    remoteServiceName: remoteServiceName,
    version: '0.0.1'
};
app.value('config', config);
app.config(['$logProvider', function ($logProvider) {
        if ($logProvider.debugEnabled) {
            $logProvider.debugEnabled(true);
        }
    }]);
app.config(['commonConfigProvider', function (cfg) {
        cfg.config.controllerActivateSuccessEvent = config.events.controllerActivateSuccess;
        cfg.config.spinnerToggleEvent = config.events.spinnerToggle;
    }]);
//# sourceMappingURL=config.js.map