(function(angular) {
    'use strict';
    var serviceId = 'repository';
    angular.module('app').factory(serviceId, ['breeze', 'common', '$q', factory]);

    function factory(breeze, common, $q) {
        var Repository = (function () {

            var repository = function (entityManagerFactory, entityTypeName, resourceName, fetchStrategy) {
                var log = common.logger.getLogFn(serviceId);
                // Ensure resourceName is registered
                var self = this;
                var entityType;
                if (entityTypeName) {
                    entityType = getMetastore().getEntityType(entityTypeName);
                    entityType.setProperties({ defaultResourceName: resourceName });

                    getMetastore().setEntityTypeForResourceName(resourceName, entityTypeName);
                }

                self.hasChanges = function () {
                    return manager().hasChanges(entityTypeName);
                }

                self.fetchByKey = function (key, argObj) {
                    return executeKeyQuery(key, argObj);
                };

                self.getByKey = function (key) {
                    if (!entityTypeName)
                        throw new Error("Repository must be created with an entity type specified");
                    return manager().getEntityByKey(entityTypeName, key);
                }

                self.find = function () {
                    var query = createQuery.apply(null, arguments);

                    return executeQuery(query);
                };

                self.findInCache = function () {
                    var query = createQuery.apply(null,arguments);

                    return executeCacheQuery(query);
                };

                self.all = function () {
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
                    ['where', 'select', 'orderBy', 'skip', 'take', 'expand'].forEach(function (el) {
                        if (argObj[el]) {
                            query = query[el](argObj[el]);
                        }
                    });
                    return query;
                }

                function executeKeyQuery(key, argObj) {
                    var query = getKeyQuery(key, argObj);
                    var ent = executeCacheQuery(query)[0];
                    
                    if (ent) {
                        var missingExpands = findMissingExpands(ent, (argObj || {}).expand);
                        if (!missingExpands.length) {
                            return $q.when(ent);
                        }
                        query = getKeyQuery(key,{
                            expand: missingExpands.map(function (el) { return el.props.join('.'); }),
                            select: argObj.select
                        });
                    }
                    return executeQuery(query).then(function (data) {
                        return data?data[0]:undefined;
                    });
                }

                function findMissingExpands(entity, expand) {
                    var returnVar = [];
                    if (!expand) { return returnVar; }
                    var i = 0;
                    if (!Array.isArray(expand)) {
                        expand = [expand];
                    }
                    expand.forEach(function (el) {
                        var hasArray = false;
                        var props = el.split('.');
                        var currentProp = entity;
                        var i = 0;
                        for (; i < props.length; i++) {
                            //to do deal with collections - null vs empty
                            if (Array.isArray(currentProp)) {
                                if (!currentProp.length) {
                                    return; 
                                }
                                currentProp = currentProp[0][props[i]];
                                hasArray = true;
                            } else {
                                currentProp = currentProp[props[i]];
                            }
                            if (!currentProp) {
                                returnVar.push({props:props, missingIndex:i, hasArray:hasArray});
                            }
                        }
                    });
                    return returnVar;
                };

                function executeKeyQueryLocally(key, argObj) {
                    return executeCacheQuery(getKeyQuery(key, argObj))[0];
                }

                function getKeyQuery(key, argObj) {
                    var entityKey = new breeze.EntityKey(entityType, key);
                    var query = breeze.EntityQuery.fromEntityKey(entityKey);
                    if (argObj) {
                        ['select', 'expand'].forEach(function (el) {
                            //TODO - check this!
                            if (argObj[el]) {
                                query = query[el](argObj[el]);
                            }
                        });
                    }
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