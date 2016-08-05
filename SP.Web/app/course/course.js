(function () {
    'use strict';
    var controllerId = 'course';
    angular
        .module('app')
        .controller(controllerId, controller);

    controller.$inject = ['controller.abstract', '$routeParams', 'common', 'datacontext', '$aside', 'breeze', '$scope', '$location', '$http', '$q', 'commonConfig', 'moment'];

    function controller(abstractController, $routeParams, common, datacontext,  $aside, breeze, $scope, $location, $http, $q, commonConfig, moment) {
        /* jshint validthis:true */
        var vm = this;
        abstractController.constructor.call(this, {
            controllerId: controllerId,
            watchedEntityNames: ['course', 'course.courseParticipants'],
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
        vm.dpPopup = { isOpen: false };
        vm.departments = [];
        vm.formatChanged = formatChanged;
        
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

        activate();

        function activate() {
            datacontext.ready().then(function () {
                var promises = [ datacontext.courseFormats.all().then(function (data) {
                    vm.courseFormats = data;
                }),datacontext.departments.all().then(function (data) {
                    vm.departments = data;
                }), datacontext.rooms.all().then(function (data) {
                    vm.rooms = data;
                }) ];
                if (isNew) {
                    vm.course = datacontext.courses.create();
                    vm.course.entityAspect.markNavigationPropertyAsLoaded('courseParticipants');
                    vm.course.entityAspect.markNavigationPropertyAsLoaded('courseDays');
                    vm.notifyViewModelLoaded();
                }else{
                    promises.push(datacontext.courses.fetchByKey(id, {expand:'courseParticipants.participant,courseDays'}).then(function (data) {
                        if (!data) {
                            vm.log.warning('Could not find course id = ' + id);
                            return;
                            //gotoCourses();
                        }
                        vm.course = data;
                        vm.notifyViewModelLoaded();
                        vm.courseDays = concatCourseDays();
                    }));
                }
                common.activateController(promises, controllerId)
                    .then(function () {
                        vm.log('Activated Course View');
                    });
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

        function dateChanged(propName,courseDay) {
            var dateInst = vm.course[propName];
            if (!dateInst instanceof Date) { return; }
            if (dateInst.getHours() === 0 && dateInst.getMinutes() === 0) { //bad luck if you want your course to start at midnight, but this would be an extreme edge case!
                dateInst.setHours(8);
            }
            if (propName === 'start') {
                if (courseDay === vm.courseDays[0] && vm.courseDays.every(function(cd,indx){return indx===1 || !cd.start;})){
                    var date = vm.courseDays[0].start;
                    for (var i=1; i<vm.courseDays.length; i++){
                        vm.courseDays[i].start = new Date(date).setDate(date.getDate() + i);
                    }
                }
            } else if (propName === 'facultyMeeting') {
                if (!vm.course.facultyMeetingDuration) {
                    vm.course.facultyMeetingDuration = 30;
                }
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
                function (error) { log.error({ msg: 'failed to remove ' + name + ' from course' }); });
        }

        function formatChanged() {
            if (!vm.course.courseFormat) {
                //_courseLength = null;
                vm.course.duration = null;
                vm.course.courseDays.forEach(function (cd) {
                    cd.entityAspect.setDeleted();
                });
                return;
            }
            adjustCourseDays();
        }

        function adjustCourseDays() {
            getCourseLengthPromise().then(function (courseLength) {
                var maxDays = vm.course.courseFormat
                    ? vm.course.courseFormat.daysDuration
                    : 1;
                var courseDay, key;
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
                    cd.duration = moment.duration(courseLength[cd.day], 'm').toJSON();
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
                alert(response.data || 'emails sent');
            });
        }
    }
})();
