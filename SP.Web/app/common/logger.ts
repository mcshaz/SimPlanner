import angular from 'angular';
import * as toastr from 'toastr';
import 'toastr/build/toastr.min.css';
import * as prune from 'JSON-prune';
import * as jQuery from 'jquery';

    
export default angular.module('common').factory('logger', ['$log', logger]);

    function logger($log) {
        var service:any = getLogFunction('');
        var logStack = createLogAndStack();
        service.autoToast = ['log', 'warning', 'success', 'error', 'info'];
        service.autoSend = [ 'error'];
        service.getLogFn = getLogFunction;

        return service;

        function getLogFunction(source) {
            var returnVar:any = function (argOpts) {
                var logType = typeof arguments[1] === 'string' ? arguments[1] : 'log';
                log(argOpts,logType);
            };
            returnVar.warn = returnVar.warning = function (argOpts) {
                log(argOpts,'warning');
            };
            returnVar.success = function (argOpts) {
                log(argOpts, 'success');
            };
            returnVar.error = returnVar.err = function (argOpts) {
                if (argOpts instanceof Error) {
                    log({ message: argOpts.message, data: argOpts }, 'error');
                } else if (argOpts.data && argOpts.data.ExceptionMessage) {
                    argOpts.sendToServer = false;
                    log({ message: 'Server Error: ' + argOpts.data.ExceptionMessage, data: argOpts.data }, 'error');
                } else {
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
            /*
            replacer: function (value, defaultValue, circular) {
                if (value === void 0) return '"-undefined-"';
                if (Array.isArray(value)) return '"-array(' + value.length + ')-"';
                return defaultValue;
            }
            */

            switch(logType){
                case 'success':
                    logType='log';
                    break;
                case 'warning':
                    logType='warn';
                    break;
                case 'debug':
                case 'log':
                    toastType='info';
                    break;
            }

            logStack.log(logType,src, msg, data);

            if (showToast) {
                toastr[toastType](msg);
            }

            if (sendToServer) {
                logStack.sendStack();
            }
        }
        //cheap and nasty function - not designed for reusability
        function createLogAndStack(maxCount:number = 10) {
            var _count = 0;
            var _log = [];
            return {
                log: function (logType, src, msg, data) {
                    $log[logType](src, msg, data);
                    _log.push({ Source: src, Message: msg, JsonData: data, LogLevel: logType });
                    if (_count >= maxCount) {
                        _log.shift();
                    } else {
                        _count++;
                    }
                },
                sendStack: function () {
                    try {
                        //$http.post("api/Utilities/LogClientError", { Source: src, Message: msg, JsonData: data }, angular.noop);
                        jQuery.ajax({
                            type: 'POST',
                            url: "api/Utilities/LogClientError", 
                            data: JSON.stringify(_log.reverse()),
                            contentType: 'application/json'
                        });
                    }
                    catch (e) {
                        if (console.log) { console.log(e); }
                    }
                }
            };
        }
    }
    /*
    function myPrune(data) {
        var type = typeof data;
        if (type === 'object' && data !== null) {
            var returnVar = {};
            for (var p in data) {
                if (p) {
                    var v = data[p];
                    if (Array.isArray(v)) {
                        returnVar[p] = 'Array[length:' + v.length + ']';
                    } else {
                        type = typeof v;
                        if (type === 'boolean' || type === 'number' || type === 'string' || v instanceof RegExp) {
                            returnVar[p] = v;
                        } 
                    }
                }
            }
            return returnVar;
        }
        return data;
    }
    */
