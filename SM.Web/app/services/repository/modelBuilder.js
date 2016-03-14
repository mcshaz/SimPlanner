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
                return this.entityAspect.entityManager.createEntity('CourseParticipantDto', {
                    participantId: participantId || breeze.core.getUuid(),
                    courseId: this.id
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

            courseCtor.prototype.includesUser = function (userId) {
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

            metadataStore.registerEntityTypeCtor('CourseDto', courseCtor /*, courseInitializer */);

        }

        function ensureEntityType(obj, entityTypeName) {
            if (!obj.entityType || obj.entityType.shortName !== entityTypeName) {
                throw new Error('Object must be an entity of type ' + entityTypeName);
            }
        }
    }

})(window.angular);