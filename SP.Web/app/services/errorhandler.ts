import angular from 'angular';


export default angular.module('app').factory('errorhandler', ['logger', 'utilities', factory]);

    function factory(logger, util /*$componentLoader, $router*/) {
        var ErrorHandler = (function () {

            var ctor = function (targetObject) {
                this.log = logger.getLogFn(getModuleId(targetObject));
                this.handleError = function (error) {
                    if (error.entityErrors) {
                        error.message = util.getSaveValidationErrorMessage(error);
                    }

                    this.log.eror(error.message);
                    throw error;
                };
            };

            return ctor;
        })();

        
        return {
            includeIn: includeIn
        };

        function includeIn(targetObject) {
            /*
            var componentLoader = $componentLoader;
            var routerService = $router;
            var locationService = $location;
            */
            

            return $.extend(targetObject, new ErrorHandler(targetObject));
        }

        function getModuleId(obj) {
            return obj.moduleId;
        }
    }
