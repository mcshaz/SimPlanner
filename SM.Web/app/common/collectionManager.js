(function () {
    'use strict';

    // Must configure the common service and set its 
    // events via the commonConfigProvider
    var serviceId = 'collectionManager';

    angular.module('common')
        .factory(serviceId, [factoryMethod]);

    function factoryMethod() {
        var service = {
            collectionChange: collectionChange,
            manageCollectionChange: manageCollectionChange
        };

        return service;

        function manageCollectionChange(repo, idPropName, createKey) {
            return collectionChange.bind(null, repo, idPropName, createKey);
        }

        function collectionChange(repo, idPropName, createKey, newVals, oldVals) {
            oldVals.forEach(function (o) {
                if (!newVals.some(function (n) {
                    return n[idPropName] === o[idPropName];
                })) {
                    var key = createKey(o);
                    var member = repo.getByKey(key);
                    if (member) {
                        member.entityAspect.setDeleted();
                    } else {
                        log.debug({
                            msg: 'collection member looks to be deleted in viewmodel, but cannot be found by key',
                            data: {
                                oldVals: oldVals,
                                newVals: newVals,
                            }
                        });
                    }

                }
            });
            newVals.forEach(function (n) {
                if (!oldVals.some(function (o) {
                    return n[idPropName] === o[idPropName];
                })) {
                    var key = createKey(n);
                    var member = repo.getByKey(key, true);
                    if (member) {
                        if (member.entityAspect.entityState.isDeleted()) {
                            member.entityAspect.setUnchanged();
                        } else if (!member.entityAspect.entityState.isUnchanged()) {
                            vm.log.debug({ msg: 'collection member found in cache & to be added but in state other than deleted', data: member });
                        }
                    } else {
                        repo.create(key);
                    }
                }
            });
        }
    }
})();