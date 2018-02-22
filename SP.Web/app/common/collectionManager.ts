import angular from 'angular';


    // Must configure the common service and set its 
    // events via the commonConfigProvider
    var serviceId = 'collectionManager';

export default angular.module('common')
        .factory(serviceId, ['logger',factoryMethod]);

    function factoryMethod(logger) {
        var service = {
            collectionChange: collectionChange,
            manageCollectionChange: manageCollectionChange,
            addItem: addItem,
            removeItem: removeItem
        };
        var log = logger.getLogFn(serviceId);

        return service;

        function manageCollectionChange(repo, idPropName, createKey) {
            return collectionChange.bind(null, repo, idPropName, createKey);
        }

        function collectionChange(repo, idPropName, createKey, newVals, oldVals) {
            oldVals.forEach(function (o) {
                if (!newVals.some(function (n) {
                    return n[idPropName] === o[idPropName];
                })) {
                    removeItem(repo,createKey,o);
                }
            });
            newVals.forEach(function (n) {
                if (!oldVals.some(function (o) {
                    return n[idPropName] === o[idPropName];
                })) {
                    addItem(repo, createKey, n);
                }
            });
        }
        function removeItem(repo,createKey,item) {
            var key = createKey(item);
            var member = repo.getByKey(key);
            if (member) {
                member.entityAspect.setDeleted();
            } else {
                log.debug({
                    msg: 'collection member looks to be deleted in viewmodel, but cannot be found by key',
                    data: item
                });
            }
        }

        function addItem(repo, createKey, item) {
            var key = createKey(item);
            var member = repo.getByKey(key, true);
            if (member) {
                if (member.entityAspect.entityState.isDeleted()) {
                    member.entityAspect.setUnchanged();
                } else if (!member.entityAspect.entityState.isUnchanged()) {
                    log.debug({ msg: 'collection member found in cache & to be added but in state other than deleted', data: member });
                }
            } else {
                repo.create(key);
            }
        }
    }
