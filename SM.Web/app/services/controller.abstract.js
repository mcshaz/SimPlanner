(function() {
    'use strict';

    var serviceId = 'controller.abstract';
    angular.module('app').factory(serviceId,
        ['common', '$window', 'commonConfig', 'breeze', 'datacontext',AbstractRepository]);

    function AbstractRepository(common, $window, commonConfig, breeze, datacontext) {
        var confirmDiscardMsg = 'Are you sure you want to discard changes without saving?';

        //var provider = entityManagerFactory.manager;

        function Ctor(argObj /* controllerId, $scope, watchedEntityNames*/) {
            var vm = this;
            var hasAddedEntityPropertyChanged = false;

            var $on = argObj.$scope.$on.bind(argObj.$scope);
            var manager = datacontext.provider;
            var unwatchers = [$on('$destroy', removeListeners)];
            var breezeWatcher;
            var isEntityStateChanged;
            var hasValidationErrors;
            var isSaving = false;

            var watchedEntityNames;
            if (argObj.watchedEntityNames) {
                watchedEntityNames = Array.isArray(argObj.watchedEntityNames)
                    ? argObj.watchedEntityNames
                    : argObj.watchedEntityNames.split(',');//assuming string if not array

                for (var i = 0; i < watchedEntityNames.length; i++) {
                    watchedEntityNames[i] = watchedEntityNames[i].split('.');
                }
            }

            if (argObj.$scope.asideInstance) {
                vm.close = modalClose;
            } else {
                unwatchers.push($on('$routeChangeStart', beforeRouteChange));
            }

            if (argObj.$watches) {unwatchers = unwatchers.concat(argObj.$watches);}
            $window.addEventListener("beforeunload", beforeUnload);

            vm.log = common.logger.getLogFn(argObj.controllerId);
            vm.disableSave = disableSave;
            vm.notifyViewModelPropChanged = notifyViewModelPropChanged;
            vm.save = save;

            function getWatched() {
                if (!watchedEntityNames){
                    return manager.getEntities(undefined, [breeze.EntityState.Modified, breeze.EntityState.Added, breeze.EntityState.Deleted]);
                }
                var returnVar = [];
                watchedEntityNames.forEach(function (el) {
                    var ent = vm[el[0]]; //todo if required allow for array of array
                    for (var i = 1; i < el.length; i++) {
                        if (!ent) { break; }
                        ent = ent[el[i]];
                    }
                    if (ent) {
                        if (ent.entityAspect) {
                            returnVar.push(ent);
                        } else if (ent.length && ent[0].entityAspect) {
                            returnVar = returnVar.concat(ent);
                        }
                    }
                });
                return returnVar;
            }

            function notifyViewModelPropChanged() {
                var watched = getWatched();
                isEntityStateChanged = watched.some(isUserChanged);
                hasValidationErrors = isEntityStateChanged && watched.some(function (el) { return el.entityAspect.hasValidationErrors; });
                //binding the watcher here as no point binding earlier - propertychanged will fire for every property as the entity is being hydrated
                if (!breezeWatcher) {
                    breezeWatcher = manager.entityChanged.subscribe(entityChanged);
                }
            }

            function entityChanged(changeArgs) {
                var ent = changeArgs.entity;
                //note, when creating entities, this will be called before the entity has been assigned to the viewmodel property.
                //this can probaby be used to advantage
                if (watchedEntityNames && getWatched().indexOf(ent)===-1){
                    return;
                }
                switch (changeArgs.entityAction) {
                    case breeze.EntityAction.EntityStateChange:
                        isEntityStateChanged |= isUserChanged(ent);
                        break;
                    case breeze.EntityAction.PropertyChange:
                        hasValidationErrors |= (ent.entityAspect.entityState !== breeze.EntityState.Deleted && ent.entityAspect.hasValidationErrors);
                        break;
                    case breeze.EntityAction.MergeOnSave:
                    case breeze.EntityAction.Detach:
                        notifyViewModelPropChanged();
                        break;
                }
            }

            function isUserChanged(ent) {
                switch (ent.entityAspect.entityState) {
                    case breeze.EntityState.Modified:
                    case breeze.EntityState.Deleted:
                        return true;
                    case breeze.EntityState.Added:
                        //single keys autogenerated
                        var isSingleKey = ent.entityType.keyProperties.length === 1;
                        return !ent.entityType.dataProperties.every(function (dp) {
                            return (isSingleKey && dp.isPartOfKey) || ent[dp.name] === dp.defaultValue;
                        });
                    default:
                        return false;
                }
            }

            function beforeUnload(e){
                if (isEntityStateChanged) {
                    e.returnValue = confirmDiscardMsg; // Gecko, Trident, Chrome 34+
                    return confirmDiscardMsg;          // Gecko, WebKit, Chrome <34
                }
            }

            function beforeRouteChange(e) {
                if (!e.defaultPrevented) {
                    if (isEntityStateChanged && !confirm(confirmDiscardMsg)) {
                        e.preventDefault();
                        common.$broadcast(commonConfig.config.controllerActivateSuccessEvent); //switch the spinner off
                    } else {
                        removeListeners({});//remove before performing next step

                        getWatched().forEach(function (ent) {
                            if (ent.entityAspect.entityState.isAddedModifiedOrDeleted()) {
                                ent.entityAspect.rejectChanges();
                            }
                        });
                    }
                }
            }

            function removeListeners(e) {
                if (unwatchers && !e.defaultPrevented) {
                    $window.removeEventListener("beforeunload", beforeUnload);
                    unwatchers.forEach(function (unwatch) { unwatch(); });
                    if (breezeWatcher) {
                        manager.entityChanged.unsubscribe(breezeWatcher);
                        breezeWatcher = null;
                    }
                    unwatchers = null;
                }
            }
            
            function disableSave() {
                return isSaving || hasValidationErrors || !isEntityStateChanged;
            }

            function save() {
                isSaving = true;

                return datacontext.save().finally(function() {
                    isSaving = false;
                });
            }

            

            function modalClose() {
                var evtArg = {
                    defaultPrevented: false,
                    preventDefault: function () {
                        this.defaultPrevented = true;
                    }
                };
                beforeRouteChange(evtArg);
                if (!evtArg.defaultPrevented) {
                    argObj.$scope.asideInstance.hide();
                    removeListeners(evtArg);
                }
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