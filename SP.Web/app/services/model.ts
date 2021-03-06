﻿import angular from 'angular';


    var useManualMetadata = false; // true: use model.metadata; false: use generated metadata

    var serviceId = 'model';
export default angular.module('app').factory(serviceId, ['model.metadata', 'model.validation', 'breeze', 'moment',model]);

    function model(manualMetadata, modelValidation, breeze, moment) {
        var nulloDate = new Date(1900, 0, 1);
        var nullosExist = false;

        var entityNames:any = {
            participant: 'Participant',
            country: 'Country',
            department: 'Department',
            sessionRoleType: 'SessionRoleType',
            scenarioRoleType: 'ScenarioRoleType',
            hospital: 'Hospital',
            instructorCourse: 'InstructorCourse',
            instructorCourseParticipant: 'InstructorCourseParticipant',
            manequin: 'Manequin',
            professionalRole: 'ProfessionalRole',
            scenario: 'Scenario',
            scenarioResource: 'ScenarioResource',
            session: 'Session',
            sessionParticipant: 'SessionParticipant',
            sessionResource: 'SessionResource',
            sessionType: 'SessionType',
        };

        var service = {
            configureMetadataStore: configureMetadataStore,
            createNullos: createNullos,
            entityNames: entityNames,
            extendMetadata: extendMetadata,
            useManualMetadata: useManualMetadata
        };

        return service;

        //#region internal methods
        function configureMetadataStore(metadataStore) {
            // Pass the Type, Ctor (breeze tracks properties created here), and initializer 
            // Assume a Session or Person is partial by default
            registerSession(metadataStore);
            registerPerson(metadataStore);
            registerTimeSlot(metadataStore);

            modelValidation.createAndRegister(entityNames);

            if (useManualMetadata) {
                manualMetadata.fillMetadataStore(metadataStore);
                extendMetadata(metadataStore);
            }
        }
        
        function extendMetadata(metadataStore) {
            modelValidation.applyValidators(metadataStore);
            registerResourceNames(metadataStore);
        }
        
        function createNullos(manager) {
            if (nullosExist) return;
            nullosExist = true;
            var unchanged = breeze.EntityState.Unchanged;

            createNullo(entityNames.timeslot, { start: nulloDate, isSessionSlot: true });
            createNullo(entityNames.room);
            createNullo(entityNames.speaker, { firstName: ' [Select a person]' });
            createNullo(entityNames.track);

            function createNullo(entityName:string, values?:any) {
                var initialValues = values || { name: ' [Select a ' + entityName.toLowerCase() + ']' };
                return manager.createEntity(entityName, initialValues, unchanged);
            }
        }

        // Wait to call until entityTypes are loaded in metadata
        function registerResourceNames(metadataStore) {
            // every entityName is its own resource name
            var types = metadataStore.getEntityTypes();
            types.forEach(function (type) {
                if (type instanceof breeze.EntityType) {
                    set(type.shortName, type);
                }
            });

            var personEntityName = entityNames.person;
            ['Speakers', 'Speaker', 'Attendees', 'Attendee'].forEach(function (r) {
                set(r, personEntityName);
            });

            function set(resourceName, entityName) {
                metadataStore.setEntityTypeForResourceName(resourceName, entityName);
            }
        }

        function registerSession(metadataStore) {
            metadataStore.registerEntityTypeCtor('Session', Session);

            function Session() {
                this.isPartial = false; // presume full session objects until informed otherwise
            }

            Object.defineProperty(Session.prototype, 'tagsFormatted', {
                get: function () {
                    return this.tags ? this.tags.replace(/\|/g, ', ') : this.tags;
                },
                set: function (value) {
                    this.tags = value.replace(/\, /g, '|');
                }
            });
        }
        
        function registerPerson(metadataStore) {
            metadataStore.registerEntityTypeCtor('Person', Person);

            function Person() {
                this.isPartial = false;
                this.isSpeaker = false;
            }

            Object.defineProperty(Person.prototype, 'fullName', {
                get: function () {
                    var fn = this.firstName;
                    var ln = this.lastName;
                    return ln ? fn + ' ' + ln : fn;
                }
            });

        }
        
        function registerTimeSlot(metadataStore) {
            metadataStore.registerEntityTypeCtor(
                'TimeSlot', TimeSlot);

            function TimeSlot() { }

            Object.defineProperty(TimeSlot.prototype, 'name', {
                get() {
                    const unknown = '[Unknown]';
                    let start: Date = this.start;
                    if (!start) {
                        return unknown;
                    }
                    if (start.getTime() === nulloDate.getTime()) {
                        return ' [Select a timeslot]';
                    }
                    let utc = moment.utc(start);
                    if (!utc.isValid()) {
                        return unknown;
                    }
                    return utc.format('ddd hh:mm a')
                }
            });
        }

        //#endregion
    }
