(function(angular) {
    'use strict';

    angular.module('app')
        .factory('modelBuilder', ['breeze', 'moment', factory]);

    function factory(breeze, moment) {
        var self = {
            extendMetadata: extendMetadata
        };

        return self;

        function extendMetadata(metadataStore) {
            extendCourse(metadataStore);
            extendCourseFormat(metadataStore);
            extendDepartment(metadataStore);
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
                ensureEntityType(courseParticipant, 'CourseParticipant');
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
                    return cp.participantId === userId;
                });
            };

            CourseCtor.prototype.lastDay = function () {
                var days = course.courseFormat.daysDuration;
                var course = this;
                return days > 1
                    ? course.courseDays.find(function (cd) { return cd.day === days; })
                    : course;
            };

            CourseCtor.prototype.courseFinish = function () {
                var lastDay = this.lastDay();
                return new Date(lastDay.start + lastDay.duration);
            };

            CourseCtor.prototype.finish = function () {
                return new Date(lastDay.start + lastDay.duration);
            };

            var courseInitializer = function (courseFormat) {
                Object.defineProperty(CourseCtor.prototype, 'finish', {
                    enumerable: true,
                    configurable: true,
                    get: getFinish
                });

                Object.defineProperty(CourseCtor.prototype, 'day', { //to implement ICourseDay interface
                    value: 1,
                    writable: false,
                    enumerable: true,
                    configurable: true
                });
            };

            metadataStore.registerEntityTypeCtor('CourseDto', CourseCtor, courseInitializer);
        }

        function extendCourseFormat(metadataStore) {

            var CourseFormatCtor = function () {};

            var courseFormatInitializer = function (courseFormat) {
                Object.defineProperty(CourseFormatCtor.prototype, 'typeFormatDescriptor', {
                    enumerable: true,
                    configurable: true,
                    get: function () {
                        return this.courseType.abbreviation + ' - ' + this.description;
                    }
                });
            };

            metadataStore.registerEntityTypeCtor('CourseFormatDto', CourseFormatCtor, courseFormatInitializer);
        }

        function extendCourseDay(metadataStore) {

            var CourseDayCtor = function () { };

            var courseDayInitializer = function (courseFormat) {
                Object.defineProperty(CourseDayCtor.prototype, 'finish', {
                    enumerable: true,
                    configurable: true,
                    get: getFinish
                });
            };

            metadataStore.registerEntityTypeCtor('CourseFormatDto', CourseFormatCtor, courseFormatInitializer);
        }

        function extendDepartment(metadataStore) {
            var dptCtor = function(){};

            var dptInitializer = function (department) {
                Object.defineProperty(dptCtor.prototype, 'primaryColourHtml', {
                    enumerable: true,
                    configurable: true,
                    get: function () {
                        return this.primaryColour
                            ? '#' + this.primaryColour
                            : null;
                    },
                    set: function (value) {
                        this.primaryColour = value.substr(1);
                    }
                });
                Object.defineProperty(dptCtor.prototype, 'secondaryColourHtml', {
                    enumerable: true,
                    configurable: true,
                    get: function () {
                        return this.secondaryColour
                            ? '#' + this.secondaryColour
                            : null;
                    },
                    set: function (value) {
                        this.secondaryColour = value.substr(1);
                    }
                });
            };

            metadataStore.registerEntityTypeCtor('DepartmentDto', dptCtor, dptInitializer);
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

        function getFinish() {
            var self = this;
            var isoDuration = moment.duration(self.duration);
            return moment(self.start).add(isoDuration).toDate();
        }

    }

})(window.angular);