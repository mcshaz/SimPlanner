(function(angular) {
    'use strict';

    angular.module('app')
        .factory('modelBuilder', ['breeze', 'moment', 'common', factory]);

    function factory(breeze, moment,common) {
        var self = {
            extendMetadata: extendMetadata
        };

        return self;

        function extendMetadata(metadataStore) {
            extendCourse(metadataStore);
            extendCourseFormat(metadataStore);
            extendCourseParticipant(metadataStore);
            extendDepartment(metadataStore);
            extendCourseSlot(metadataStore);
            extendValidators(metadataStore);
            extendInstitution(metadataStore);
            extendParticipant(metadataStore);
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

            var courseInitializer = function (course) {
                Object.defineProperty(CourseCtor.prototype, 'finish', {
                    enumerable: true,
                    configurable: true,
                    get: getFinish
                });

                Object.defineProperty(CourseCtor.prototype, 'facultyCount', {
                    enumerable: true,
                    configurable: true,
                    get: function () {
                        return this.courseParticipants.filter(function (el) { return el.isFaculty; }).length;
                    }
                });

                Object.defineProperty(CourseCtor.prototype, 'participantCount', {
                    enumerable: true,
                    configurable: true,
                    get: function () {
                        return this.courseParticipants.filter(function (el) { return !el.isFaculty; }).length;
                    }
                });

                Object.defineProperty(CourseCtor.prototype, 'totalDurationMins', {
                    enumerable: true,
                    configurable: true,
                    get: function () {
                        //.filter(function (cd) { return cd.day <= this.courseFormat.daysDuration; }) - unnecesasry as server setting course day to 0 in such cases
                        return this.courseDays.reduce(function (a, b) { return a + b.durationMins; }, this.durationMins);
                    }
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

        function extendParticipant(metadataStore) {
            var ParticipantCtor = function () {
                this.password = null;
            };

            ParticipantCtor.prototype.mailto = function () {
                var self = this;
                var returnVar = "mailto:" + self.email;
                if (self.alternateEmail) {
                    returnVar += "?cc=" + self.alternateEmail;
                }
                return returnVar;
            };
            metadataStore.registerEntityTypeCtor('ParticipantDto', ParticipantCtor);
        }

        function extendInstitution(metadataStore) {
            var InstCtor = function(){};

            var instInitializer = function (inst) {
                Object.defineProperty(InstCtor.prototype, 'logoImageSrc', {
                    enumerable: false,
                    configurable: true,
                    get: function () {
                        if (!this.logoImageFileName) { return ''; }
                        return "/Content/images/institutions/" + this.id + this.logoImageFileName.substring(this.logoImageFileName.lastIndexOf('.'));
                    }
                });
            };

            metadataStore.registerEntityTypeCtor('InstitutionDto', InstCtor, instInitializer);
        }

        function extendCourseParticipant(metadataStore) {
            var CourseParticipantCtor = function () {
                this.isEmailed = false;
            };
            metadataStore.registerEntityTypeCtor('CourseParticipantDto', CourseParticipantCtor);
        }

        function extendCourseFormat(metadataStore) {

            var CourseFormatCtor = function () { };
            var utcOffset = new Date(0).getTimezoneOffset() * 60000;
            var parseSimpleDuration = /PT(\d*H)?(\d*M)?/;
            var courseFormatInitializer = function (courseFormat) {
                Object.defineProperty(CourseFormatCtor.prototype, 'typeFormatDescriptor', {
                    enumerable: true,
                    configurable: true,
                    get: function () {
                        return this.courseType.abbreviation + ' - ' + this.description;
                    }
                });

                CourseFormatCtor.prototype.defaultStartMsOffset = function () {
                    var duration = parseSimpleDuration.exec(this.defaultStartTime);
                    return parseInt(duration[1] || 0) * 3600000 + parseInt(duration[2] || 0) * 60000;
                };

                Object.defineProperty(CourseFormatCtor.prototype, 'defaultStartAsDate', {
                    enumerable: true,
                    configurable: true,
                    get: function () {
                        return this.defaultStartMsOffset() + utcOffset;
                    },
                    set: function (value) {
                        value = value - utcOffset;
                        var hrs = Math.floor(value/ 3600000);
                        var mins = value % 3600000 / 60000;
                        var newVal = "PT" + hrs + "H";
                        if (mins !== 0) {
                            newVal += mins + "M";
                        }
                        this.defaultStartTime = newVal;//not just setting the minutes as this would change the string
                    }
                });
            };

            metadataStore.registerEntityTypeCtor('CourseFormatDto', CourseFormatCtor, courseFormatInitializer);
        }

        function extendCourseDay(metadataStore) {

            var CourseDayCtor = function () { };

            var courseDayInitializer = function (courseDay) {
                Object.defineProperty(CourseDayCtor.prototype, 'finish', {
                    enumerable: true,
                    configurable: true,
                    get: getFinish
                });
            };

            metadataStore.registerEntityTypeCtor('CourseFormatDto', CourseFormatCtor, courseFormatInitializer);
        }

        function extendCourseSlot(metadataStore) {

            var CourseSlotCtor = function () { };

            var courseSlotInitializer = function (courseSlot) {
                Object.defineProperty(CourseSlotCtor.prototype, 'isScenario', {
                    enumerable: true,
                    configurable: true,
                    get: function () {
                        return !this.activity;
                    }
                });
            };

            metadataStore.registerEntityTypeCtor('CourseSlotDto', CourseSlotCtor, courseSlotInitializer);
        }

        function extendDepartment(metadataStore) {
            var DptCtor = function(){};

            var dptInitializer = function (department) {
                Object.defineProperty(DptCtor.prototype, 'primaryColourHtml', {
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
                Object.defineProperty(DptCtor.prototype, 'secondaryColourHtml', {
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
                Object.defineProperty(DptCtor.prototype, 'institutionDptDescriptor', {
                    enumerable: true,
                    configurable: true,
                    get: function () {
                        return this.institution.abbreviation + ' ' + this.abbreviation;
                    }
                });
            };

            metadataStore.registerEntityTypeCtor('DepartmentDto', DptCtor, dptInitializer);
        }

        function ensureEntityType(obj, entityTypeName) {
            if (!obj.entityType || obj.entityType.shortName !== entityTypeName) {
                throw new Error('Object must be an entity of type ' + entityTypeName);
            }
        }

        function extendValidators(metadataStore)
        {
            //requiredReference
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

            //comes after
            var courseEntity = metadataStore.getEntityType('CourseDto');
            var comesBeforeStart = validator.comesBeforeValidatorFactory(courseEntity.getProperty('startUtc'));
            courseEntity.getProperty('facultyMeeting').validators.push(comesBeforeStart);

            //no repeat activity name
            var courseActivityEntity = metadataStore.getEntityType('CourseActivityDto');
            courseActivityEntity.getProperty('name').validators.push(validator.noRepeatActivityNameValidatorFactory());
            
        }

        function getFinish() {
            if (!this.startUtc) {
                return this.startUtc;
            }
            return new Date(this.startUtc.getTime() + this.durationMins * 60000);
        }

    }

})(window.angular);