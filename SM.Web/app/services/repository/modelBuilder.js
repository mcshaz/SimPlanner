(function(angular) {
    'use strict';

    angular.module('app')
        .factory('modelBuilder', ['breeze', factory]);

    function factory(breeze) {
        var self = {
            extendMetadata: extendMetadata
        };

        return self;

        function extendMetadata(metadataStore) {
            extendCourse(metadataStore);
            extendValidators(metadataStore);
        }

        function extendCourse(metadataStore) {
            var CourseCtor = function () {
                //
            };

            CourseCtor.prototype.addParticipant = function (participant) {
                if (!participant.entityAspect) { throw new TypeError("addParticipant requires a participant entity as an argument");}
                var keys = {
                    participantId: participant.id ,
                    courseId: this.id,
                    departmentId: participant.defaultDepartmentId,
                    professionalRoleId: participant.defaultProfessionalRoleId
                };
                if (arguments.length > 1) {
                    angular.extend(keys, arguments[1]);
                }
                return this.entityAspect.entityManager.createEntity('CourseParticipantDto', keys);
            };

            CourseCtor.prototype.removeParticipant = function (courseParticipant) {
                ensureEntityType(courseParticipant, 'CourseParticipant')
                this.throwIfNotOwnerOf(courseParticipant);
                return this.entityAspect.setDeleted(courseParticipant);
            };

            CourseCtor.prototype.throwIfNotOwnerOf = function (obj) {
                if (!obj.courseId || obj.courseId !== this.id) {
                    throw new Error('Object is not associated with current course');
                }
            };

            CourseCtor.prototype.includesUser = function (userId) {
                return this.courseParticipants.some(function (cp) {
                    return cp.participantId == userId;
                });
            }

            /*
            var courseInitializer = function (course) {
                Object.defineProperty(courseCtor.prototype, 'includesUser', {
                    enumerable: true,
                    configurable: true,
                    get: function(userId) {
                        return this.courseParticipants.some(function (p) {
                            return p.id == userId;
                        });
                    }
                });
            };
            */

            metadataStore.registerEntityTypeCtor('CourseDto', CourseCtor /*, courseInitializer */);

        }

        function ensureEntityType(obj, entityTypeName) {
            if (!obj.entityType || obj.entityType.shortName !== entityTypeName) {
                throw new Error('Object must be an entity of type ' + entityTypeName);
            }
        }

        function extendValidators(metadataStore)
        {
            var required = window.medsimMetadata.getBreezeValidators();
            var validator = breeze.Validator;
            for (var typeName in required) {
                if (required.hasOwnProperty(typeName)) {
                    var repo = metadataStore.getEntityType(typeName);
                    required[typeName].forEach(function (propName) {
                        repo.getProperty(propName).validators.push(validator.requireReferenceValidator);
                    });
                }
            }
        }

    }

})(window.angular);