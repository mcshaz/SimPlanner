"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
'use strict';
var serviceId = 'controller.abstract';
angular_1.default.module('app').factory(serviceId, ['common', '$window', 'commonConfig', 'breeze', 'datacontext', '$q', AbstractRepository]);
function AbstractRepository(common, $window, commonConfig, breeze, datacontext, $q) {
    var confirmDiscardMsg = 'Are you sure you want to discard changes without saving?';
    return {
        constructor: Ctor
    };
    function Ctor(argObj) {
        var vm = this;
        var $on = argObj.$scope.$on.bind(argObj.$scope);
        var manager = datacontext.provider;
        var unwatchers = [$on('$destroy', removeListeners)];
        var breezeWatcher;
        var errorEntities = new Set();
        var watchedEntityNames;
        vm.isSaving = false;
        if (argObj.watchedEntityNames) {
            watchedEntityNames = Array.isArray(argObj.watchedEntityNames)
                ? argObj.watchedEntityNames
                : argObj.watchedEntityNames.split(',');
            for (var i = 0; i < watchedEntityNames.length; i++) {
                watchedEntityNames[i] = watchedEntityNames[i].split('.');
            }
        }
        unwatchers.push($on('$routeChangeStart', beforeRouteChange));
        if (argObj.$scope.$hide) {
            vm.close = modalClose;
        }
        if (argObj.$watchers) {
            unwatchers = unwatchers.concat(argObj.$watchers);
        }
        $window.addEventListener("beforeunload", beforeUnload);
        vm.log = common.logger.getLogFn(argObj.controllerId);
        vm.disableSave = disableSave;
        vm.notifyViewModelLoaded = notifyViewModelLoaded;
        vm.save = save;
        var _watched;
        function getWatched(force) {
            if (force === void 0) { force = false; }
            if (!watchedEntityNames) {
                return manager.getEntities();
            }
            if (!_watched || force) {
                var watched = (_watched || []).filter(filterDeletedEnts);
                watchedEntityNames.forEach(function (wen) {
                    var ent = vm[wen[0]];
                    var currentLevel;
                    if (!ent) {
                        return;
                    }
                    if (!Array.isArray(ent)) {
                        ent = [ent];
                    }
                    for (var i = 1; i < wen.length; i++) {
                        currentLevel = [];
                        ent.forEach(function (el) {
                            var child = el[wen[i]];
                            if (child === void 0) {
                                var errorMsg = 'watched entity child is void 0 - ' + wen[i] + ' (' + wen.join('.') + ') entity ';
                                if (el.entityType) {
                                    errorMsg += el.entityType.typeName + ' [available options =(' + el.entityType.navigationProperties.map(function (dp) { return dp.name; }).join(',') + ')]';
                                }
                                else {
                                    errorMsg += ' el.entityType is also void 0';
                                }
                                vm.log.debug({ msg: errorMsg, data: el });
                            }
                            if (Array.isArray(child)) {
                                currentLevel = currentLevel.concat(child);
                            }
                            else {
                                currentLevel.push(child);
                            }
                        });
                        ent = currentLevel;
                    }
                    watched = watched.concat(ent.filter(filterEnts));
                });
                _watched = Array.from(new Set(watched));
            }
            return _watched;
        }
        function filterEnts(ent) {
            return !!(ent && ent.entityAspect);
        }
        function filterDeletedEnts(ent) {
            return ent && ent.entityAspect && ent.entityAspect.entityState === breeze.EntityState.Deleted;
        }
        function filterHasValidationErrors(el) { return el.entityAspect.hasValidationErrors && el.entityAspect.entityState !== breeze.EntityState.Deleted; }
        function notifyViewModelLoaded() {
            var watched = getWatched(true);
            vm.isEntityStateChanged = watched.some(isUserChanged);
            errorEntities = new Set(watched.filter(filterHasValidationErrors));
            if (!breezeWatcher) {
                breezeWatcher = manager.entityChanged.subscribe(entityChanged);
            }
        }
        function entityChanged(changeArgs) {
            var ent = changeArgs.entity;
            switch (changeArgs.entityAction) {
                case breeze.EntityAction.EntityStateChange:
                    if (getWatched().indexOf(ent) !== -1) {
                        if (!vm.isEntityStateChanged) {
                            vm.isEntityStateChanged = isUserChanged(ent);
                        }
                        if (ent.entityAspect.entityState === breeze.EntityState.Deleted) {
                            errorEntities.delete(ent);
                        }
                    }
                    break;
                case breeze.EntityAction.PropertyChange:
                    if (changeArgs.args.property.isNavigationProperty) {
                        notifyViewModelLoaded();
                    }
                    if (getWatched().indexOf(ent) === -1) {
                        return;
                    }
                    if (ent.entityAspect.entityState !== breeze.EntityState.Deleted
                        && ent.entityAspect.hasValidationErrors
                        && ent.entityAspect.getValidationErrors().some(function (e) {
                            return !(e.isServerError && e.propertyName === changeArgs.args.propertyName);
                        })) {
                        errorEntities.add(ent);
                    }
                    else {
                        errorEntities.delete(ent);
                    }
                    if (!vm.isEntityStateChanged) {
                        vm.isEntityStateChanged = isUserChanged(ent);
                    }
                    break;
                case breeze.EntityAction.MergeOnSave:
                    vm.isEntityStateChanged = getWatched().some(isUserChanged);
                    break;
                case breeze.EntityAction.Attach:
                case breeze.EntityAction.Detach:
                    notifyViewModelLoaded();
                    break;
            }
        }
        function isUserChanged(ent) {
            switch (ent.entityAspect.entityState) {
                case breeze.EntityState.Modified:
                case breeze.EntityState.Deleted:
                    return true;
                case breeze.EntityState.Added:
                    var instantiationVal = ent.entityAspect.instantiationValues || {};
                    var maxAutoGenKeyCount = 1;
                    return !ent.entityType.dataProperties.every(function (dp) {
                        if (dp.isPartOfKey) {
                            return maxAutoGenKeyCount-- > 0;
                        }
                        var propVal = ent[dp.name];
                        return propVal === dp.defaultValue || propVal === instantiationVal[dp.name];
                    });
                default:
                    return false;
            }
        }
        function beforeUnload(e) {
            if (vm.isEntityStateChanged) {
                e.returnValue = confirmDiscardMsg;
                return confirmDiscardMsg;
            }
        }
        function beforeRouteChange(e) {
            vm.log.debug("changed entity count: " + manager.getChanges().length);
            if (!e.defaultPrevented) {
                if (vm.isEntityStateChanged && !confirm(confirmDiscardMsg)) {
                    e.preventDefault();
                    common.$broadcast(commonConfig.config.controllerActivateSuccessEvent);
                }
                else {
                    removeListeners({});
                    var breezeProps = ["entityAspect", "_backingStore", "$$hashKey"];
                    getWatched().forEach(function (ent) {
                        if (ent.entityAspect.entityState.isAddedModifiedOrDeleted() && !ent.entityAspect.isBeingSaved) {
                            ent.entityAspect.rejectChanges();
                        }
                        for (var p in ent) {
                            if (ent.hasOwnProperty(p) && breezeProps.indexOf(p) === -1) {
                                delete ent[p];
                            }
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
            return vm.isSaving || errorEntities.size || !vm.isEntityStateChanged;
        }
        function save() {
            vm.isSaving = true;
            var toSave;
            if (arguments.length) {
                toSave = [];
                for (var i = 0; i < arguments.length; i++) {
                    if (Array.isArray(arguments[i])) {
                        toSave = toSave.concat(arguments[i]);
                    }
                    else {
                        toSave.push(arguments[i]);
                    }
                }
            }
            else {
                toSave = watchedEntityNames
                    ? getWatched()
                    : [];
            }
            toSave = toSave.filter(function (el) { return !!(el && el.entityAspect && el.entityAspect.entityState.isAddedModifiedOrDeleted()); });
            return datacontext.save.apply(null, toSave).then(function () {
                vm.isEntityStateChanged = false;
            }, function (response) {
                if (response.innerError && response.innerError.entityErrors && response.innerError.entityErrors.length) {
                    response.innerError.entityErrors.forEach(function (e) {
                        errorEntities.add(e.entity);
                    });
                    vm.isEntityStateChanged = false;
                }
                return $q.reject(response);
            }).finally(function () {
                vm.isSaving = false;
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
                argObj.$scope.$hide();
                removeListeners(evtArg);
            }
        }
    }
}
//# sourceMappingURL=controller.abstract.js.map