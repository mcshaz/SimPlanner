"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
var useManualMetadata = false;
var serviceId = 'model';
angular_1.default.module('app').factory(serviceId, ['model.metadata', 'model.validation', 'breeze', 'moment', model]);
function model(manualMetadata, modelValidation, breeze, moment) {
    var nulloDate = new Date(1900, 0, 1);
    var nullosExist = false;
    var entityNames = {
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
    function configureMetadataStore(metadataStore) {
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
        if (nullosExist)
            return;
        nullosExist = true;
        var unchanged = breeze.EntityState.Unchanged;
        createNullo(entityNames.timeslot, { start: nulloDate, isSessionSlot: true });
        createNullo(entityNames.room);
        createNullo(entityNames.speaker, { firstName: ' [Select a person]' });
        createNullo(entityNames.track);
        function createNullo(entityName, values) {
            var initialValues = values || { name: ' [Select a ' + entityName.toLowerCase() + ']' };
            return manager.createEntity(entityName, initialValues, unchanged);
        }
    }
    function registerResourceNames(metadataStore) {
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
            this.isPartial = false;
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
        metadataStore.registerEntityTypeCtor('TimeSlot', TimeSlot);
        function TimeSlot() { }
        Object.defineProperty(TimeSlot.prototype, 'name', {
            get: function () {
                var unknown = '[Unknown]';
                var start = this.start;
                if (!start) {
                    return unknown;
                }
                if (start.getTime() === nulloDate.getTime()) {
                    return ' [Select a timeslot]';
                }
                var utc = moment.utc(start);
                if (!utc.isValid()) {
                    return unknown;
                }
                return utc.format('ddd hh:mm a');
            }
        });
    }
}
//# sourceMappingURL=model.js.map