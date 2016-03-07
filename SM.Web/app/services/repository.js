(function(angular) {
    'use strict';
    var serviceId = 'repository';
    angular.module('app').factory(serviceId, ['breeze', 'common', factory]);

    function factory(breeze, common) {
        var Repository = (function () {

            var repository = function (entityManagerFactory, entityTypeName, resourceName, fetchStrategy) {
                var log = common.logger.getLogFn(serviceId);
                // Ensure resourceName is registered
                var entityType;
                if (entityTypeName) {
                    entityType = getMetastore().getEntityType(entityTypeName);
                    entityType.setProperties({ defaultResourceName: resourceName });

                    getMetastore().setEntityTypeForResourceName(resourceName, entityTypeName);
                }

                this.hasChanges = function () {
                    return manager().hasChanges(entityTypeName);
                }

                this.fetchByKey = function (key) {
                    return manager().fetchEntityByKey(entityTypeName, key, true) //true refers to check local cache 1st
                        .then(function (data) {
                            return data.entity;
                        });
                };

                this.getByKey = function (key) {
                    if (!entityTypeName)
                        throw new Error("Repository must be created with an entity type specified");
                    return manager().getEntityByKey(entityTypeName, key);
                }

                this.find = function () {
                    var query = createQuery.apply(null, arguments);

                    return executeQuery(query);
                };

                this.findInCache = function () {
                    var query = createQuery.apply(null,arguments);

                    return executeCacheQuery(query);
                };

                this.all = function () {
                    var query = breeze.EntityQuery
                        .from(resourceName);

                    return executeQuery(query);
                };

                function createQuery(argObj) {
                    var query = breeze.EntityQuery.from(resourceName);
                    switch (typeof argObj){
                        case 'undefined':
                            return query
                        case 'string':
                            return query.where.apply(query, arguments);
                    }
                    //default
                    if (argObj instanceof breeze.Predicate) {
                        return query.where(argObj);
                    }
                    ['where', 'predicate', 'select', 'orderBy', 'skip', 'take', 'expand'].forEach(function (el) {
                        //TODO - check this!
                        if (argObj[el]) {
                            query = query[el](argObj[el]);
                        }
                    });
                    return query;
                }

                function executeQuery(query) {
                    return manager()
                        .executeQuery(query.using(fetchStrategy || breeze.FetchStrategy.FromServer))
                        .then(function (data) {
                            return data.results;
                        }, log.error );
                }

                function executeCacheQuery(query) {
                    return manager().executeQueryLocally(query);
                }

                function getMetastore() {
                    return manager().metadataStore;
                }

                function manager() {
                    return entityManagerFactory;
                }
            };

            return repository;
        })();

        return {
            create: create
        };

        function create(entityManagerFactory, entityTypeName, resourceName, fetchStrategy) {
            return new Repository(entityManagerFactory, entityTypeName, resourceName, fetchStrategy);
        }
    }

})(window.angular);