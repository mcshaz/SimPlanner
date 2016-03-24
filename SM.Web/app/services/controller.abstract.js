(function() {
    'use strict';

    var serviceId = 'controller.abstract';
    angular.module('app').factory(serviceId,
        ['common', '$window', 'commonConfig', 'breeze',AbstractRepository]);

    function AbstractRepository(common, $window, commonConfig, breeze) {
        var confirmDiscardMsg = 'Are you sure you want to discard changes without saving?';
        var _savableStates = [breeze.EntityState.Added, breeze.EntityState.Modified];
        //var provider = entityManagerFactory.manager;

        function Ctor(argObj /* controllerId, $scope, watchedEntityName*/) {
            var vm = this;
            var hasAddedEntityPropertyChanged = false;

            var $on = argObj.$scope.$on.bind(argObj.$scope);
            var unwatchers = [$on('$destroy', destroy)]; 

            if (argObj.$scope.asideInstance) {
                unwatchers.push(argObj.$scope.$parent.$on('aside.hide.before', beforeRouteChange));
            } else {
                unwatchers.push($on('$routeChangeStart', beforeRouteChange));
            }

            if (argObj.$watches) {unwatchers = unwatchers.concat(argObj.$watches);}
            $window.addEventListener("beforeunload", beforeUnload);

            vm.log = common.logger.getLogFn(argObj.controllerId);
            vm.disableSave = disableSave;

            function hasDataChanged(){
                var ent = vm[argObj.watchedEntityName];
                if (ent) {
                    var entityState = ent.entityAspect.entityState;
                    return (entityState === breeze.EntityState.Deleted
                        || (_savableStates.indexOf(entityState) > -1 && !common.isEmptyObject(ent.entityAspect.originalValues))) 
                }
                return false;
            }

            function beforeUnload(e){
                if (hasDataChanged()){
                    e.returnValue = confirmDiscardMsg; // Gecko, Trident, Chrome 34+
                    return confirmDiscardMsg;          // Gecko, WebKit, Chrome <34
                }
            }

            function beforeRouteChange(e) {
                if (!e.defaultPrevented) {
                    if (hasDataChanged() && !confirm(confirmDiscardMsg)) {
                        e.preventDefault();
                        common.$broadcast(commonConfig.config.controllerActivateSuccessEvent); //switch the spinner off
                    } else {
                        var watched = vm[argObj.watchedEntityName];
                        if (watched && watched.entityAspect && watched.entityAspect.entityState.isAddedModifiedOrDeleted()) {
                            watched.entityAspect.rejectChanges();
                        }
                        destroy({}); //note this will remove listeners on the hide event, but as the controller has a new controller injected ever time
                        //, this will do for now
                    }
                }
            }

            function destroy(e) {
                if (unwatchers && !e.defaultPrevented) {
                    $window.removeEventListener("beforeunload", beforeUnload);
                    unwatchers.forEach(function (unwatch) {
                        unwatch();
                    });
                    unwatchers = null;
                }
            }

            function disableSave() {
                var watched = vm[argObj.watchedEntityName];
                if (watched && watched.entityAspect) {
                    return _savableStates.indexOf(watched.entityAspect.entityState) === -1
                        || watched.entityAspect.hasValidationErrors;
                }
                return true;
            }

            /*
            function modalClose() {
                var evtArg = {
                    defaultPrevented: false,
                    preventDefault: function () {
                        this.defaultPrevented = true;
                    }
                };
                beforeLeave(evtArg);
                if (!evtArg.defaultPrevented) {
                    argObj.modal.destroy();
                }
            }
            */
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