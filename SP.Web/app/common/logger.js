"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
var toastr = require("toastr");
require("toastr/build/toastr.min.css");
var prune = require("JSON-prune");
var jQuery = require("jquery");
angular_1.default.module('common').factory('logger', ['$log', logger]);
function logger($log) {
    var service = getLogFunction('');
    var logStack = createLogAndStack();
    service.autoToast = ['log', 'warning', 'success', 'error', 'info'];
    service.autoSend = ['error'];
    service.getLogFn = getLogFunction;
    return service;
    function getLogFunction(source) {
        var returnVar = function (argOpts) {
            var logType = typeof arguments[1] === 'string' ? arguments[1] : 'log';
            log(argOpts, logType);
        };
        returnVar.warn = returnVar.warning = function (argOpts) {
            log(argOpts, 'warning');
        };
        returnVar.success = function (argOpts) {
            log(argOpts, 'success');
        };
        returnVar.error = returnVar.err = function (argOpts) {
            if (argOpts instanceof Error) {
                log({ message: argOpts.message, data: argOpts }, 'error');
            }
            else if (argOpts.data && argOpts.data.ExceptionMessage) {
                argOpts.sendToServer = false;
                log({ message: 'Server Error: ' + argOpts.data.ExceptionMessage, data: argOpts.data }, 'error');
            }
            else {
                log(argOpts, 'error');
            }
        };
        returnVar.debug = function (argOpts) {
            log(argOpts, 'debug');
        };
        returnVar.info = function (argOpts) {
            log(argOpts, 'info');
        };
        return returnVar;
        function log(argOpts, logType) {
            if (typeof argOpts === 'string') {
                argOpts = {
                    msg: argOpts
                };
            }
            if (!(argOpts.source || argOpts.src)) {
                argOpts.source = source;
            }
            logIt(argOpts, logType);
        }
    }
    function logIt(argOpts, logType) {
        var showToast = argOpts.showToast === true || argOpts.showToast === void 0 && service.autoToast.indexOf(logType) > -1;
        var sendToServer = argOpts.sendToServer === true || argOpts.sendToServer === void 0 && service.autoSend.indexOf(logType) > -1;
        var toastType = logType;
        var msg = argOpts.message || argOpts.msg;
        var src = argOpts.source || argOpts.src;
        var data = prune(argOpts.data, {
            arrayMaxLength: 5,
            depthDecr: 3,
            inheritedProperties: true
        });
        switch (logType) {
            case 'success':
                logType = 'log';
                break;
            case 'warning':
                logType = 'warn';
                break;
            case 'debug':
            case 'log':
                toastType = 'info';
                break;
        }
        logStack.log(logType, src, msg, data);
        if (showToast) {
            toastr[toastType](msg);
        }
        if (sendToServer) {
            logStack.sendStack();
        }
    }
    function createLogAndStack(maxCount) {
        if (maxCount === void 0) { maxCount = 10; }
        var _count = 0;
        var _log = [];
        return {
            log: function (logType, src, msg, data) {
                $log[logType](src, msg, data);
                _log.push({ Source: src, Message: msg, JsonData: data, LogLevel: logType });
                if (_count >= maxCount) {
                    _log.shift();
                }
                else {
                    _count++;
                }
            },
            sendStack: function () {
                try {
                    jQuery.ajax({
                        type: 'POST',
                        url: "api/Utilities/LogClientError",
                        data: JSON.stringify(_log.reverse()),
                        contentType: 'application/json'
                    });
                }
                catch (e) {
                    if (console.log) {
                        console.log(e);
                    }
                }
            }
        };
    }
}
//# sourceMappingURL=logger.js.map