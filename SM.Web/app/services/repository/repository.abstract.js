(function() {
    'use strict';

    var serviceId = 'repository.abstract';
    angular.module('app').factory(serviceId,
        ['common', 'config', 'zStorage', 'zStorageWip', AbstractRepository]);

    function AbstractRepository(common, config, zStorage, zStorageWip) {
        var predicates = {
            isNotNullo: breeze.Predicate.create('id', '!=', 0),
            isNullo: breeze.Predicate.create('id', '==', 0)
        };

        function Ctor(entityManager, entityName, repoServiceId) {
            // instance members that are stateful
            this.entityName = entityName;
            this.getById = getById.bind(this);
            this.getEntityByIdOrFromWip = getEntityByIdOrFromWip.bind(this);
            this.log = common.logger.getLogFn(repoServiceId);
            this.manager = entityManager;
            this.queryFailed = queryFailed.bind(this); // Bind to self so we establish 'this' as the context
        }

        /* stateless methods that can be shared across all repos */
        Ctor.prototype = {
            constructor: Ctor,
            getAllLocal: getAllLocal,
            getLocalEntityCount: getLocalEntityCount,
            predicates: predicates,
            setIsPartialTrue: setIsPartialTrue,
        };

        return Ctor;

        /* Implementation */

        function getAllLocal(resource, ordering, predicate) {
            return breeze.EntityQuery.from(resource)
                .where(predicate)
                .orderBy(ordering)
                .using(this.manager)
                .executeLocally();
        }

        function getById(id, forceRemote) {
            var self = this;
            var entityName = self.entityName;
            var manager = self.manager;
            if (!forceRemote) {
                // Check cache first (synchronous)
                var entity = manager.getEntityByKey(entityName, id);
                if (entity && !entity.isPartial) {
                    self.log({msg:'Retrieved [' + entityName + '] id:' + entity.id + ' from cache.', data: entity });
                    if (entity.entityAspect.entityState.isDeleted()) {
                        entity = null; // hide session marked-for-delete
                    }
                    // Should not need to call $apply because it is synchronous
                    return $q.when(entity);
                }
            }

            // It was not found in cache, so let's query for it.
            // fetchEntityByKey will go remote because 
            // 3rd parm is false/undefined. 
            return manager.fetchEntityByKey(entityName, id)
                .then(querySucceeded).catch(self.queryFailed);

            function querySucceeded(data) {
                entity = data.entity;
                if (!entity) {
                    self.log('Could not find [' + entityName + '] id:' + id);
                    return null;
                }
                entity.isPartial = false;
                self.log({msg:'Retrieved [' + entityName + '] id ' + entity.id + ' from remote data source', data:entity});
                return entity;
            }
        }

        function getLocalEntityCount(resource) {
            var entities = breeze.EntityQuery.from(resource)
                .where(predicates.isNotNullo)
                .using(this.manager)
                .executeLocally();
            return entities.length;
        }        

        function queryFailed(error) {
            var msg = config.appErrorPrefix + 'Error retrieving data. ' + (error.message||'');
            this.log.error({msg:msg, data:error});
            return $q.reject(new Error(msg));
        }

        function setIsPartialTrue(entities) {
            // call for all "partial queries"
            for (var i = entities.length; i--;) { entities[i].isPartial = true; }
            return entities;
        }
    }
})();