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
            var CourseCtor = function () {
                //
            };

            CourseCtor.prototype.addParticipant = function (participantId) {
                return this.entityAspect.entityManager.createEntity('CourseParticipantDto', {
                    participantId: participantId || breeze.core.getUuid(),
                    courseId: this.id
                });
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
    }

})(window.angular);