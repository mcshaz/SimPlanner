"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
'use strict';
var serviceId = 'repository';
angular_1.default.module('app').factory(serviceId, ['breeze', 'common', '$q', factory]);
function factory(breeze, common, $q) {
    var Repository = (function () {
        var repository = function (manager, entityTypeName, resourceName, fetchStrategy) {
            var log = common.logger.getLogFn(serviceId);
            var self = this;
            var entityType;
            if (entityTypeName) {
                entityType = getMetastore().getEntityType(entityTypeName);
                entityType.setProperties({ defaultResourceName: resourceName });
                getMetastore().setEntityTypeForResourceName(resourceName, entityTypeName);
            }
            self.hasChanges = function () {
                return manager.hasChanges(entityTypeName);
            };
            self.fetchByKey = function (keyObj, argObj) {
                return executeKeyQuery(keyPropsToArray(keyObj), argObj);
            };
            self.getByKey = function (keyObj, includeDeleted) {
                var key = new breeze.EntityKey(entityType, keyPropsToArray(keyObj));
                if (includeDeleted) {
                    return manager.getEntities(entityType).find(function (el) {
                        return el.entityAspect._entityKey._keyInGroup === key._keyInGroup;
                    });
                }
                else {
                    return manager.getEntityByKey(key);
                }
            };
            self.find = function () {
                var query = createQuery.apply(null, arguments);
                return executeQuery(query);
            };
            self.findInCache = function () {
                var query = createQuery.apply(null, arguments);
                return executeCacheQuery(query);
            };
            self.all = function () {
                var query = breeze.EntityQuery
                    .from(resourceName);
                return executeQuery(query);
            };
            self.create = function () {
                var initialVals;
                var entityState;
                var returnVar;
                switch (arguments.length) {
                    case 0:
                        break;
                    case 1:
                        var arg = arguments[0];
                        if (arg && arg.parentEnum && arg.parentEnum.name === "EntityState") {
                            entityState = arg;
                        }
                        else {
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
                }
                else if (keyProps.length === 1 && !initialVals.id) {
                    initialVals.id = breeze.core.getUuid();
                }
                returnVar = manager.createEntity(entityType, initialVals, entityState);
                returnVar.entityAspect.instantiationValues = initialVals;
                return returnVar;
            };
            function keyPropsToArray(key) {
                if (Array.isArray(key)) {
                    return key;
                }
                switch (typeof key) {
                    case 'string':
                    case 'number':
                        return [key];
                    case 'object':
                        return entityType.keyProperties.map(function (el) {
                            var returnVar = key[el.name];
                            if (returnVar === void 0) {
                                throw new TypeError('entity key object is missing required key ' + el.name);
                            }
                            return returnVar;
                        });
                    default:
                        throw new TypeError('entity key of type - ' + typeof key);
                }
            }
            function createQuery(argObj) {
                var query = breeze.EntityQuery.from(resourceName);
                switch (typeof argObj) {
                    case 'undefined':
                        return query;
                    case 'string':
                        return query.where.apply(query, arguments);
                }
                if (argObj instanceof breeze.Predicate) {
                    return query.where(argObj);
                }
                for (var propName in argObj) {
                    query = query[propName](argObj[propName]);
                }
                return query;
            }
            function executeKeyQuery(key, argObj) {
                var query = getKeyQuery(key, argObj);
                var entities = executeCacheQuery(query);
                return loadNavs(entities, query).then(function (data) {
                    return data[0];
                });
            }
            function loadNavs(ent, query) {
                if (ent.length) {
                    var missingExpands = findMissingExpands(ent, query.expandClause);
                    if (!missingExpands.length) {
                        return $q.when(ent);
                    }
                    if (ent.length === 1 && missingExpands.length === 1 && missingExpands[0].props.length === 1) {
                        var defer = $q.defer();
                        ent[0].entityAspect.loadNavigationProperty(missingExpands[0].props[0], function () { defer.resolve(ent); }, function (err) { defer.reject(err); });
                        return defer.promise;
                    }
                    query = query.expand(missingExpands.map(function (el) { return el.props.join('.'); }));
                }
                return executeQuery(query, true);
            }
            self.findServerIfCacheEmpty = function () {
                var query = createQuery.apply(null, arguments);
                var entities = executeCacheQuery(query);
                entities = filterWithParameters(entities, query.parameters);
                return loadNavs(entities, query);
            };
            function filterWithParameters(entities, withParameters) {
                if (!withParameters || common.isEmptyObject(withParameters)) {
                    return entities;
                }
                var propName;
                for (propName in withParameters) {
                    if (entityType.dataProperties.every(function (el) { return el.name !== propName; })) {
                        throw new Error('void 0 property ' + propName + ' on entity type ' + entityType.shortName);
                    }
                }
                return entities.filter(function (ent) {
                    for (propName in withParameters) {
                        var compare = withParameters[propName];
                        var entVal = ent[propName];
                        if (Array.isArray(compare)) {
                            if (compare.every(function (el) { return el !== entVal; })) {
                                return false;
                            }
                        }
                        else if (compare !== entVal) {
                            return false;
                        }
                    }
                    return true;
                });
            }
            function findMissingExpands(entities, expandClause) {
                var returnVar = [];
                if (!expandClause) {
                    return returnVar;
                }
                if (Array.isArray(entities)) {
                    if (!entities.length) {
                        return returnVar;
                    }
                }
                else {
                    if (!entities) {
                        return returnVar;
                    }
                    entities = [entities];
                }
                expandClause.propertyPaths.forEach(function (el) {
                    var props = el.split('.');
                    var currentProp = entities;
                    var missingIndex = -1;
                    var i = 0;
                    var lastIndex = props.length - 1;
                    var p, np;
                    for (; i < props.length; i++) {
                        p = props[i];
                        if (!currentProp.every(function (el) { return el.entityAspect.isNavigationPropertyLoaded(p); })) {
                            missingIndex = i;
                            break;
                        }
                        if (i < lastIndex) {
                            np = currentProp[0].entityType.navigationProperties.find(function (n) { return n.name === p; });
                            currentProp = np.isScalar
                                ? currentProp.map(function (el) { return el[p]; }).filter(Boolean)
                                : currentProp.reduce(function (a, b) { return a.concat(b[p]); }, []);
                            if (!currentProp.length) {
                                break;
                            }
                        }
                    }
                    if (missingIndex !== -1) {
                        returnVar.push({ props: props, missingIndex: missingIndex });
                    }
                });
                return returnVar;
            }
            function getKeyQuery(key, argObj) {
                var entityKey = new breeze.EntityKey(entityType, key);
                var query = breeze.EntityQuery.fromEntityKey(entityKey);
                if (argObj) {
                    for (var propName in argObj) {
                        query = query[propName](argObj[propName]);
                    }
                }
                return query;
            }
            function executeQuery(query, force) {
                if (force === void 0) { force = false; }
                var fetch = force
                    ? breeze.FetchStrategy.FromServer
                    : fetchStrategy || breeze.FetchStrategy.FromServer;
                return manager
                    .executeQuery(query.using(fetch))
                    .then(function (data) {
                    if (data.inlineCount !== void 0) {
                        data.results.inlineCount = data.inlineCount;
                    }
                    return data.results;
                }, log.error);
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
//# sourceMappingURL=repository.js.map