(function(angular) {
    'use strict';
    var serviceId = 'repository';
    angular.module('app').factory(serviceId, ['breeze', 'common', '$q', factory]);

    function factory(breeze, common, $q) {
        var Repository = (function () {

            var repository = function (manager, entityTypeName, resourceName, fetchStrategy) {
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
                    return manager.hasChanges(entityTypeName);
                }

                self.fetchByKey = function (key, argObj) {
                    return executeKeyQuery(keyPropsToArray(key), argObj);
                };

                self.getByKey = function (key) {
                    return manager.getEntityByKey(entityTypeName, keyPropsToArray(key));
                }

                self.find = function () {
                    var query = createQuery.apply(null, arguments);

                    return executeQuery(query);
                };

                self.findServerIfCacheEmpty = function(/*arguments*/) {
                    var query = createQuery.apply(null,arguments);
                    var entities = executeCacheQuery(query);
                    entities = filterWithParameters(entities, query.parameters);
                    if (entities.length) {
                        //to do - as per find missing expands, should check each member of array
                        var missingExpands = findMissingExpands(entities[0], query.expandClause);
                        if (!missingExpands.length) {
                            return $q.when(entities);
                        }
                        query = query.expand(missingExpands.map(joinExpandProps));
                    }
                    return executeQuery(query, true);
                }

                self.findInCache = function () {
                    var query = createQuery.apply(null,arguments);

                    return executeCacheQuery(query);
                };

                self.all = function () {
                    var query = breeze.EntityQuery
                        .from(resourceName);

                    return executeQuery(query);
                };

                self.create = function (/*initialVals and/or entityState*/) {
                    var initialVals;
                    var entityState;
                    var newIds = {};
                    switch (arguments.length) {
                        case 0:
                            break;
                        case 1:
                            var arg = arguments[0];
                            if (arg && arg.parentEnum && arg.parentEnum.name=="EntityState") {
                                entityState = arg;
                            } else {
                                initialVals = arg;
                            }
                            break;
                        case 2:
                            initialVals = arguments[0];
                            entityState = arguments[1];
                            break;
                        default:
                            throw new SyntaxError('create requires 0-2 arguments');
                    }

                    var keyProps = entityType.keyProperties;

                    if (!initialVals) {
                        if (keyProps.length !== 1) {
                            throw new Error(entityTypeName + ' has a complex key - ids must be specified when creating');
                        }
                        initialVals = { id: breeze.core.getUuid() };
                    } else if (keyProps.length === 1 && !initialVals.id) {
                        initialVals.id = breeze.core.getUuid();
                    }

                    return manager.createEntity(entityType, initialVals, entityState);
                }

                function keyPropsToArray(key) {
                    if (Array.isArray(key)) { return key; }
                    switch (typeof (key)) {
                        case 'string':
                        case 'number':
                            return [key];
                        case 'object':
                            return entityType.keyProperties.map(function (el) {
                                var returnVar = key[el.name];
                                if (typeof (returnVar) === 'undefined') {
                                    throw new TypeError('entity key object is missing required key ' + el.name);
                                }
                                return returnVar;
                            });
                        default:
                            throw new TypeError('entity key of type' + typeof(key));
                    }

                }

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
                    //['where', 'withParameters', 'select', 'orderBy', 'skip', 'take', 'expand', 'inlineCount']
                    for(var propName in argObj) {
                        query = query[propName](argObj[propName]);
                    };
                    return query;
                }

                function executeKeyQuery(key, argObj) {
                    var query = getKeyQuery(key, argObj);
                    var ent = executeCacheQuery(query)[0];
                    
                    if (ent) {
                        var missingExpands = findMissingExpands(ent, query.expandClause);
                        if (!missingExpands.length) {
                            return $q.when(ent);
                        }
                        if (missingExpands.length === 1 && missingExpands[0].props.length === 1) {
                            var defer = $q.defer(); //need defer here as the promise below will return the related entities
                            ent.entityAspect.loadNavigationProperty(missingExpands[0].props[0],
                                function (data) { defer.resolve(ent); },
                                function (err) { defer.reject(err); });
                            return defer.promise;
                        } else {
                            query = query.expand(missingExpands.map(joinExpandProps));
                        }
                    }
                    return executeQuery(query,true).then(function (data) {
                        return data?data[0]:undefined;
                    });
                }

                function joinExpandProps(el) { return el.props.join('.'); };

                function filterWithParameters(entities, withParameters) {
                    if (!withParameters || common.isEmptyObject(withParameters)) { return entities; }
                    var propName;
                    for (propName in withParameters) {
                        if (entityType.dataProperties.every(function (el) { return el.name !== propName})) {
                            throw new Error('undefined property ' + propName + ' on entity type ' + entityType.shortName);
                        }
                    }
                    return entities.filter(function(ent){
                        for (propName in withParameters) {
                            var compare = withParameters[propName];
                            var entVal = ent[propName];
                            if (Array.isArray(compare)) {
                                if (compare.every(function (el) { return el !== entVal; })) {
                                    return false;
                                }
                            } else if (compare !== entVal) {
                                return false;
                            }
                        }
                        return true;
                    });
                }

                function findMissingExpands(entity, expandClause) {
                    var returnVar = [];
                    if (!expandClause) { return returnVar; }
                    expandClause.propertyPaths.forEach(function (el) {
                        var hasArray = false;
                        var props = el.split('.');
                        var currentProp = entity;
                        var i;
                        if (props.some(function(p,indx){
                            if (!currentProp.entityAspect.isNavigationPropertyLoaded(p)) {
                                i = indx;
                                return true;
                            }
                            var np = currentProp.entityType.navigationProperties.find(function (np) { return np.name === p; });
                            currentProp = currentProp[p];
                            if (!np.isScalar) {
                                if (currentProp.length) {
                                    currentProp = currentProp[0]; //todo recursive function or at least every to ensure every member of the collection has children properties loaded
                                } else {
                                    return false;
                                }
                            }
                            return false;
                        })){
                            returnVar.push({ props: props, missingIndex: i || (props.length - 1), hasArray: hasArray });
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
                        //['select', 'expand'].forEach(function (el) {
                        for (var propName in argObj) {
                            query = query[propName](argObj[propName]);
                        };
                    }
                    return query;
                }

                function executeQuery(query, force) {
                    var fetch = force
                        ? breeze.FetchStrategy.FromServer
                        : fetchStrategy || breeze.FetchStrategy.FromServer;
                    return manager
                        .executeQuery(query.using(fetch))
                        .then(function (data) {
                            if (data.inlineCount) { data.results.inlineCount = data.inlineCount; }
                            return data.results;
                        }, log.error );
                }

                function executeCacheQuery(query) {
                    return manager.executeQueryLocally(query);
                }

                function getMetastore() {
                    return manager.metadataStore;
                }

            };

            return repository;
        })();

        return {
            create: create
        };

        function create(manager, entityTypeName, resourceName, fetchStrategy) {
            return new Repository(manager, entityTypeName, resourceName, fetchStrategy);
        }
    }

})(window.angular);