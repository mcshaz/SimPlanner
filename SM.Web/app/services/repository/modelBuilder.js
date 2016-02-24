(function(angular) {
    'use strict';

    angular.module('app')
        .factory('modelBuilder', [factory]);

    function factory() {
        var self = {
            extendMetadata: extendMetadata
        };

        return self;

        function extendMetadata(metadataStore) {
            extendCourse(metadataStore);
        }

        function extendCourse(metadataStore) {
            var courseCtor = function () {
                this.id = breeze.core.getUuid();
            };

            courseCtor.prototype.addParticipant = function (participantId) {
                return this.entityAspect.entityManager.createEntity('CourseParticipant', {
                    participantId: participantId,
                    id: this.id
                });
            };

            courseCtor.prototype.removeParticipant = function (courseParticipant) {
                ensureEntityType(courseParticipant, 'CourseParticipant')
                this.throwIfNotOwnerOf(courseParticipant);
                return this.entityAspect.setDeleted(courseParticipant);
            };

            courseCtor.prototype.throwIfNotOwnerOf = function (obj) {
                if (!obj.courseId || obj.courseId !== this.id) {
                    throw new Error('Object is not associated with current course');
                }
            };
            /*
            var staffingResourceInitializer = function (staffingResource) {
                Object.defineProperty(staffingResourceCtor.prototype, 'fullName', {
                    enumerable: true,
                    configurable: true,
                    get: function() {
                        if (this.middleName) {
                            return this.firstName + ' ' + this.middleName + ' ' + this.lastName;
                        }
                        return this.firstName + ' ' + this.lastName;
                    }
                });
            };
            */
            metadataStore.setEntityTypeForResourceName('Instructor', 'CourseParticipant');
            metadataStore.setEntityTypeForResourceName('Student', 'CourseParticipant');

            metadataStore.registerEntityTypeCtor('Course', courseCtor);

        }

        function ensureEntityType(obj, entityTypeName) {
            if (!obj.entityType || obj.entityType.shortName !== entityTypeName) {
                throw new Error('Object must be an entity of type ' + entityTypeName);
            }
        }
    }

})(window.angular);