(function() {
    'use strict';

    var serviceId = 'controller.abstract';
    angular.module('app').factory(serviceId,
        ['common', 'entityManagerFactory', '$window', 'commonConfig', 'breeze',AbstractRepository]);

    function AbstractRepository(common, entityManagerFactory, $window, commonConfig, breeze) {

        function Ctor(argObj /* controllerId, $scope, watchedEntityName*/) {
            var provider = entityManagerFactory.manager;
            var vm = this;
            var hasAddedEntityPropertyChanged = false;
            var unwatch = argObj.$scope.$on((argObj.$scope.$close && argObj.$scope.$dismiss) ? 'modal.closing' : '$routeChangeStart', beforeLeave) //todo can we check if this is a modal //$destroy //for UI Router this would be $stateChangeStart

            vm.log = common.logger.getLogFn(argObj.controllerId);
            vm.disableSave = disableSave;

            $window.onbeforeunload = beforeLeave;

            function beforeLeave(evtArgs) {
                var watched = vm[argObj.watchedEntityName];
                if (watched && watched.entityAspect) {
                    var entityState = watched.entityAspect.entityState;
                    if (entityState && ([breeze.EntityState.Modified, breeze.EntityState.Deleted].indexOf(entityState) > -1
                        || (entityState === breeze.EntityState.Added && !common.isEmptyObject(watched.entityAspect.originalValues)))
                            && !confirm('Are you sure you want to discard changes?')) {
                        if (evtArgs && evtArgs.preventDefault) { evtArgs.preventDefault(); }
                        common.$broadcast(commonConfig.config.controllerActivateSuccessEvent); //switch the spinner off
                        return false; //the false if it is a beforeunload event
                    }
                    vm[argObj.watchedEntityName].entityAspect.rejectChanges();
                }

                $window.onbeforeunload = null;
                unwatch();
                return true;
            }

            function disableSave() {
                var watched = vm[argObj.watchedEntityName];
                if (watched && watched.entityAspect) {
                    return !watched.entityAspect.entityState.isAddedModifiedOrDeleted()
                        || watched.entityAspect.hasValidationErrors;
                }
                return true;
            }
        }

        //no point instantiating above (as true factory method) as will only extend other methods
        Ctor /* .prototype */ = { 
            constructor: Ctor,
        };

        return Ctor;

        /* Implementation */
        /*
        function setIsPartialTrue(entities) {
            // call for all "partial queries"
            for (var i = entities.length; i--;) { entities[i].isPartial = true; }
            return entities;
        }
        */
    }
})();