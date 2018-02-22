import breezeValidators from './../../breezeValidators';
import angular from 'angular';

export default angular.module('app')
        .factory('modelBuilder', ['breeze', factory]);

    function factory(breeze) {
        var self = {
            extendMetadata: extendMetadata
        };

        return self;

        function extendMetadata(metadataStore) {
            extendCourse(metadataStore);
            extendCourseDay(metadataStore);
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

            var courseInitializer = function () {
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
                        //.filter(function (cd) { return cd.day <= this.courseFormat.daysDuration; }) - unnecesary as server setting course day to 0 in such cases
                        return this.courseDays.reduce(function (a, b) { return a + b.durationFacultyMins; }, this.durationFacultyMins);
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

            var instInitializer = function () {
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
            var courseFormatInitializer = function () {
                Object.defineProperty(CourseFormatCtor.prototype, 'typeFormatDescriptor', {
                    enumerable: true,
                    configurable: true,
                    get: function () {
                        return this.courseType.abbreviation + ' - ' + this.description;
                    }
                });

                CourseFormatCtor.prototype.defaultStartMsOffset = function () {
                    var duration = parseSimpleDuration.exec(this.defaultStartTime);
                    return (parseInt(duration[1]) || 0) * 3600000 + (parseInt(duration[2])|| 0) * 60000;
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
            var courseDayInitializer = function () {
                Object.defineProperty(CourseDayCtor.prototype, 'finish', {
                    enumerable: true,
                    configurable: true,
                    get: getFinish
                });
            };
            metadataStore.registerEntityTypeCtor('CourseDayDto', CourseDayCtor, courseDayInitializer);
        }

        function extendCourseSlot(metadataStore) {

            var CourseSlotCtor = function () { };

            var courseSlotInitializer = function () {
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

            var dptInitializer = function () {
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
            var validator = breeze.Validator;
            for (var typeName in breezeValidators) {
                if (breezeValidators.hasOwnProperty(typeName)) {
                    var repo = metadataStore.getEntityType(typeName);
                    breezeValidators[typeName].forEach(function (propName) {
                        repo.getProperty(propName).validators.push(validator.requireReferenceValidator);
                    });
                }
            }

            //comes after
            var courseEntity = metadataStore.getEntityType('CourseDto');
            var comesBeforeStart = validator.comesBeforeValidatorFactory(courseEntity.getProperty('startFacultyUtc'));
            courseEntity.getProperty('facultyMeeting').validators.push(comesBeforeStart);

            //no repeat activity name
            var courseActivityEntity = metadataStore.getEntityType('CourseActivityDto');
            courseActivityEntity.getProperty('name').validators.push(validator.noRepeatActivityNameValidatorFactory());
            
        }

        function getFinish() {
            if (!this.startFacultyUtc) {
                return this.startFacultyUtc;
            }
            return new Date(this.startFacultyUtc.getTime() + this.durationFacultyMins * 60000);
        }

    }
