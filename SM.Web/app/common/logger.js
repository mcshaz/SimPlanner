(function () {
    'use strict';
    
    angular.module('common').factory('logger', ['$log', logger]);

    function logger($log) {
        var service = getLogFunction('');
        service.autoToast = ['log', 'warning', 'success', 'error', 'info'];
        service.getLogFn = getLogFunction;

        return service;

        function getLogFunction(source) {
            var returnVar = function (argOpts) {
                var logType = (typeof arguments[1] === 'string') ? arguments[1] : 'log';
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
            var showToast = argOpts.showToast === true || (argOpts.showToast === undefined && service.autoToast.indexOf(logType) > -1);
            var toastType = logType;
            var msg = argOpts.message || argOpts.msg;
            var src = argOpts.source || argOpts.src;
            var data = argOpts.data;
            if (data) data = JSON.prune(data, {
                arrayMaxLength: 5,
                depthDecr:3,
                //inheritedProperties:true,
                replacer:function(value, defaultValue, circular){
                    if (value === undefined) return '"-undefined-"';
                    if (Array.isArray(value)) return '"-array('+value.length+')-"';
                    return defaultValue;
                }
            });

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

            $log[logType](src, msg, data);

            if (showToast) {
                toastr[toastType](msg);
            }
        }



    }
})();