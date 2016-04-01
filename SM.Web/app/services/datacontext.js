(function() {
    'use strict';
    var serviceId = 'datacontext';
    angular.module('app')
        .service(serviceId, ['$rootScope', 'entityManagerFactory', 'repository', 'common', 'config',service]);

    function service($rootScope, entityManagerFactory, repository, common, config) {
        var provider = entityManagerFactory.manager;
        var self = this;
        var log = common.logger.getLogFn(serviceId);

        self.addEntity = provider.addEntity.bind(provider);
        self.cloneItem = cloneItem;
        self.ready = entityManagerFactory.ready;

        self.rejectChanges = provider.rejectChanges;

        self.save = function (/*entitiesToSave*/) {
            var saveOptions = new breeze.SaveOptions({ resourceName: 'savechanges' });
            var entititiesToSave;
            switch (arguments.length) {
                case 0:
                    entititiesToSave = null; // = save all;
                    break;
                case 1:
                    entititiesToSave = Array.isArray(arguments[0])
                        ? arguments[0]
                        : [arguments[0]];
                    break;
                default:
                    entititiesToSave = Array.prototype.slice.call(arguments);
            }
            return provider.saveChanges(entititiesToSave, saveOptions)
                .then(function (saveResult) {
                    $rootScope.$broadcast('saved', saveResult.entities);
                    log.success({ msg: 'Saved data', data: saveResult, showToast: true });
                }).catch(saveFailed);
        };

        self.courses = repository.create(provider, 'CourseDto', 'Courses');
        self.countries = repository.create(provider, 'CountryDto', 'Countries', breeze.FetchStrategy.FromLocalCache);
        self.courseActivities = repository.create(provider, 'CourseActivityDto', 'CourseActivities');
        self.courseFormats = repository.create(provider, 'CourseFormatDto', 'CourseFormats');
        self.courseParticipants = repository.create(provider, 'CourseParticipantDto', 'CourseParticipants'/* 'Courses' */);
        self.courseTypes = repository.create(provider, 'CourseTypeDto', 'CourseTypes', breeze.FetchStrategy.FromLocalCache);
        self.departments = repository.create(provider, 'DepartmentDto', 'Departments', breeze.FetchStrategy.FromLocalCache);
        self.institutions = repository.create(provider, 'InstitutionDto', 'Institutions', breeze.FetchStrategy.FromLocalCache);
        self.participants = repository.create(provider, 'ParticipantDto', 'Participants');
        self.professionalRoles = repository.create(provider, 'ProfessionalRoleDto', 'ProfessionalRoles', breeze.FetchStrategy.FromLocalCache);
        self.rooms = repository.create(provider, 'RoomDto', 'Rooms', breeze.FetchStrategy.FromLocalCache);

        function saveFailed(error) {
            var msg = config.appErrorPrefix + 'Save failed: ' +
                breeze.saveErrorMessageService.getErrorMessage(error);
            error.message = msg;
            log.error({ msg: msg, data: error });
            throw error;
        }

        //Ward is a legend - http://stackoverflow.com/questions/20566093/breeze-create-entity-from-existing-one
        function cloneItem(item, collectionNames) {
            var manager = item.entityAspect.entityManager;
            // export w/o metadata and then parse the exported string.
            var exported = JSON.parse(manager.exportEntities([item], false));
            // extract the entity from the export
            var entityType = item.entityType;
            var copy = exported.entityGroupMap[entityType.name].entities[0];
            // remove the entityAspect (todo: remove complexAspect from nested complex types)
            delete copy.entityAspect;
            // remove the key properties
            if (entityType.keyProperties.length > 1) {
                throw new Error("cloneItem not yet implemented for items with complex keys - BM");
            }
            copy[entityType.keyProperties[0].name] = breeze.core.getUuid();

            // the "copy" provides the initial values for the create
            var newItem = manager.createEntity(entityType, copy);

            if (collectionNames && collectionNames.length) {
                // can only handle parent w/ single PK values
                var parentKeyValue = newItem.entityAspect.getKey().values[0];
                collectionNames.forEach(copyChildren);
            }
            return newItem;

            function copyChildren(navPropName) {
                // todo: add much more error handling
                var navProp = entityType.getNavigationProperty(navPropName);
                if (navProp.isScalar) return; // only copies collection navigations. Todo: should it throw?

                // This method only copies children (dependent entities), not a related parent
                // Child (dependent) navigations have inverse FK names, not FK names
                var fk = navProp.invForeignKeyNames[0]; // can only handle child w/ single FK value
                if (!fk) return;

                // Breeze `getProperty` gets values for all model libraries, e.g. both KO and Angular
                var children = item.getProperty(navPropName);
                if (children.length === 0) return;

                // Copy all children
                var childType = navProp.entityType;
                children = JSON.parse(manager.exportEntities(children, false));
                var copies = children.entityGroupMap[childType.name].entities;

                copies.forEach(function (c) {
                    delete c.entityAspect;
                    // remove key properties (assumes keys are store generated)
                    childType.keyProperties.forEach(function (p) {
                        //delete c[p.name];
                        c[p.name] = breeze.core.getUuid();
                    });
                    // set the FK parent of the copy to the new item's PK               
                    c[fk] = parentKeyValue;
                    // merely creating them will cause Breeze to add them to the parent
                    manager.createEntity(childType, c);
                });
            }
        }
    }
})();