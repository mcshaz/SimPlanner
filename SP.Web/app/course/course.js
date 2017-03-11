(function () {
    'use strict';
    var controllerId = 'course';
    angular
        .module('app')
        .controller(controllerId, controller);

    controller.$inject = ['controller.abstract', '$routeParams', 'common', 'datacontext', '$aside', 'breeze', '$scope', '$location', '$http', '$q', 'commonConfig', 'moment', 'loginFactory', 'tokenStorageService'];

    function controller(abstractController, $routeParams, common, datacontext,  $aside, breeze, $scope, $location, $http, $q, commonConfig, moment, loginFactory, tokenStorageService) {
        /* jshint validthis:true */
        var vm = this;
        abstractController.constructor.call(this, {
            controllerId: controllerId,
            watchedEntityNames: ['course', 'course.courseParticipants', 'course.courseDays'],
            $scope: $scope
        });
        var id = $routeParams.id;
        var isNew = id === 'new';
        var saveBase = vm.save;

        vm.course = {};
        vm.courseDays = [];
        vm.courseFormats = [];
        vm.cycleConfirmed = cycleConfirmed;
        vm.dateChanged = dateChanged;
        vm.dateFormat = '';
        vm.deleteCourseParticipant = deleteCourseParticipant;
        vm.downloadCertificates = downloadCertificates;
        vm.downloadTimetable = downloadTimetable;
        vm.dpPopup = { isOpen: false };
        vm.departments = [];
        vm.formatChanged = formatChanged;
        vm.goAssignRoles = goAssignRoles;
        vm.hasChanges = false;
        vm.maxDate = new Date();
        vm.maxDate.setFullYear(vm.maxDate.getFullYear() + 1);
        vm.minDate = new Date(2007, 0);
        vm.openDp = openDp;
        vm.openCourseParticipant = openCourseParticipant;
        vm.rooms = [];
        vm.save = save;
        vm.sendEmails = sendEmails;
        vm.title = 'course';
        vm.timeUpdated = timeUpdated;

        activate();

        function activate() {
            var promises = [ datacontext.ready().then(function(){
                datacontext.courseFormats.find({
                    where: breeze.Predicate.create('obsolete', '==', false)
                }).then(function (data) {
                    vm.courseFormats = data.sort(common.sortOnPropertyName('description'));
                }), datacontext.departments.all().then(function (data) {
                    vm.departments = data.sort(sortDepartments);
                }), datacontext.rooms.all().then(function (data) {
                    vm.rooms = data.sort(common.sortOnPropertyName('shortDescription'));
                });
            })];
            if (isNew) {
                var now = new Date();
                vm.course = datacontext.courses.create({ created: now, lastModified: now, departmentId: tokenStorageService.getUserDepartmentId()});
                vm.course.entityAspect.markNavigationPropertyAsLoaded('courseParticipants');
                vm.course.entityAspect.markNavigationPropertyAsLoaded('courseDays');
                vm.courseDays = [vm.course];
            }else{
                promises.push(datacontext.courses.fetchByKey(id, {expand:'courseParticipants.participant,courseDays'}).then(function (data) {
                    if (!data) {
                        vm.log.warning('Could not find course id = ' + id);
                        return;
                        //gotoCourses();
                    }
                    vm.log.debug({ msg:'got course',data:data });
                    vm.course = data;
                    vm.courseDays = concatCourseDays();
                }));
            }
            common.activateController(promises, controllerId)
                .then(function () {
                    vm.notifyViewModelLoaded();
                    vm.log('Activated Course View');
                });
        }


        function cycleConfirmed(cp) {
            switch (cp.isConfirmed) {
                case true:
                    cp.isConfirmed = false;
                    break;
                case false:
                    cp.isConfirmed = null;
                    break;
                default: // case null
                    cp.isConfirmed = true;
            }
        }

        function openDp() {
            this.dpPopup.isOpen = true;
        }

        function openCourseParticipant(participantId) {
            var modal = getModalInstance();
            var scope = modal.$scope;
            var isNew = participantId.startsWith('new');
            scope.courseParticipant = isNew
                ? null
                : getCourseParticipant(participantId);
            scope.isFaculty = isNew
                ? participantId.endsWith('Faculty')
                : scope.courseParticipant.isFaculty;
            modal.$promise.then(modal.show);
        }

        var _modalInstance;
        function getModalInstance() {
            if (!_modalInstance) {
                var modalScope = $scope.$new();
                _modalInstance = $aside({
                    templateUrl: 'app/courseParticipant/courseParticipant.html',
                    controller: 'courseParticipant',
                    show: false,
                    id: 'cpModal',
                    placement: 'left',
                    animation: 'am-slide-left',
                    scope: modalScope,
                    controllerAs: 'cp'
                });
                modalScope.course = vm.course;
                $scope.$on('$destroy', function () { _modalInstance.destroy(); });
            }
            return _modalInstance;
        }

        function dateChanged(dateInst,propName,courseDay) {
            if (!dateInst) { return; }
            if (vm.course.courseFormat && dateInst.getHours() === 0 && dateInst.getMinutes() === 0) { //bad luck if you want your course to start at midnight local time, but this would be an extreme edge case!
                var msOffset = vm.course.courseFormat.defaultStartMsOffset();
                dateInst.setHours(Math.floor(msOffset / 3600000), msOffset % 3600000 / 60000);
            }
            if (propName === 'startFacultyUtc') {
                if (courseDay === vm.courseDays[0] && vm.courseDays.every(function(cd,indx){return indx===1 || !cd.startFacultyUtc;})){
                    var date = vm.courseDays[0].startFacultyUtc;
                    for (var i=1; i<vm.courseDays.length; i++){
                        vm.courseDays[i].startFacultyUtc = new Date(date).setDate(date.getDate() + i);
                    }
                }
            } else if (propName === 'facultyMeeting') {
                if (!vm.course.facultyMeetingDuration) {
                    vm.course.facultyMeetingDuration = 30;
                }
            }
        }
        // hack - the directive is obviously using setHours, and so breeze sees the entity as unchaged 
        function timeUpdated(owner) {
            if (owner.entityAspect.entityState.isUnchanged()) {
                owner.entityAspect.setModified();
            }
        }


        function concatCourseDays() {
            vm.course.courseDays.sort(common.sortOnPropertyName('day'));
            return [vm.course].concat(vm.course.courseDays);
        }

        function deleteCourseParticipant(participantId) {
            var cp = getCourseParticipant(participantId);
            var name = cp.participant.fullName;
            cp.entityAspect.setDeleted();
            datacontext.save(cp).then(function (data) { vm.log('removed ' + name + ' from course'); },
                function (error) { vm.log.error({ msg: 'failed to remove ' + name + ' from course' }); });
        }

        function formatChanged() {
            if (!vm.course.courseFormat) {
                //_courseLength = null;
                vm.course.durationFacultyMins = null;
                vm.course.courseDays.forEach(function (cd) {
                    cd.entityAspect.setDeleted();
                });
                return;
            }
            adjustCourseDays();
            if (vm.course.startFacultyUtc) {
                dateChanged(vm.course.startFacultyUtc, "startFacultyUtc", vm.course);
                //not updating in the UI at present
                vm.course.startFacultyUtc = new Date(vm.course.startFacultyUtc);
            }
        }

        function adjustCourseDays() {
            getCourseLengthPromise().then(function (courseLength) {
                var maxDays = vm.course.courseFormat
                    ? vm.course.courseFormat.daysDuration
                    : 1;
                var courseDay;
                var key;
                vm.courseDays = concatCourseDays();
                for (var i = 2; i <= maxDays; i++) {
                    courseDay = vm.courseDays.find(function (cd) { return cd.day === i; });
                    if (!courseDay) {
                        key = { courseId: vm.course.id, day: i };
                        courseDay = datacontext.courseDays.getByKey(key, true);
                        if (courseDay) {
                            if (courseDay.entityAspect.entityState.isDeleted()) {
                                courseDay.entityAspect.setUnchanged();
                            } else {
                                vm.log.debug({ msg: 'courseDay found in cache & to be added but in state other than deleted', data: courseDay });
                            }
                        } else {
                            courseDay = dataContext.courseDays.create(key);

                        }
                    }
                }
                for (; i <= vm.course.courseDays.length; i++) {
                    courseDay = vm.courseDays.find(function (cd) { return cd.day === i; });
                    if (courseDay) {
                        courseDay.entityAspect.setDeleted();
                    }
                }
                vm.courseDays = concatCourseDays();
                vm.courseDays.forEach(function (cd) {
                    cd.durationFacultyMins = courseLength[cd.day];
                });

            });

            var _courseLength = null;
            var _lastFormat;
            function getCourseLengthPromise() {
                if (!vm.course.courseFormat) {
                    return $q.resolve(null);
                } else if (!_courseLength || _lastFormat !== vm.course.courseFormat) {
                    _lastFormat = vm.course.courseFormat;
                    if (!vm.course.courseFormat.entityAspect.isNavigationPropertyLoaded('courseSlots')) {
                        return vm.course.courseFormat.entityAspect.loadNavigationProperty('courseSlots').then(getSlotDuration);
                    } else {
                        return $q.resolve(getSlotDuration());
                    }
                } else {
                    return $q.resolve(_courseLength);
                }

                function getSlotDuration() {
                    _courseLength = [];
                    vm.course.courseFormat.courseSlots.forEach(function (cs) {
                        _courseLength[cs.day] = (_courseLength[cs.day] || 0) + cs.minutesDuration;
                    });
                    return _courseLength;
                }
            }
        }

        function getCourseParticipant(participantId) {
            return vm.course.courseParticipants.find(function (cp) {
                return cp.participantId === participantId;
            });
        }

        function goAssignRoles() {
            $location.path('/courseRoles/' + vm.course.id);
        }

        function save() {
            saveBase().then(function () {
                if (isNew) {
                    $location.update_path('course/' + vm.course.id, false);
                    isNew = false;
                }
            });
        }

        function sendEmails() {
            $http({
                method: 'POST',
                url: 'api/CoursePlanning/EmailAll/',
                data: { CourseId: vm.course.id }
            }).then(function (response) {
                if (response.status !== 200) {
                    vm.log.error(response);
                }
                var result = response.data;
                if (typeof result === 'string') {
                    vm.log(result);
                } else {
                    if (result.FailRecipients.length) {
                        var failPart = result.FailRecipients.map(mapToParticipantName);

                        vm.log.warning({ msg: 'Emails unable to be sent to ' + failPart.join(', '), data: response});
                    } else {
                        vm.log.success('participants & faculty emailed');
                    }
                    result.SuccessRecipients.map(mapToParticipant).forEach(function (cp) {
                        cp.isEmailed = true;
                    });
                }
            }, vm.log.error);
            function mapToParticipant(guid) {
                return vm.course.courseParticipants.find(function (cp) { return cp.participantId === guid; });
            }

            function mapToParticipantName(guid){
                return mapToParticipant(guid).fullName;
            }
        }
        function downloadCertificates() {
            loginFactory.downloadFileLink('GetCertificates', vm.course.id)
                .then(function (url) {
                    vm.downloadFileUrl = url;
                });
        }
        function downloadTimetable() {
            loginFactory.downloadFileLink('GetTimetable', vm.course.id)
                .then(function (url) {
                    vm.downloadFileUrl = url;
                });
        }
    }
    function sortDepartments(dpt1, dpt2) {

        if (dpt1.institution.name > dpt2.institution.name)
            return 1;
        if (dpt1.institution.name < dpt2.institution.name)
            return -1;

        if (dpt1.name > dpt2.name)
            return 1;
        if (dpt1.name < dpt2.name)
            return -1;

        return 0;
    }
})();
