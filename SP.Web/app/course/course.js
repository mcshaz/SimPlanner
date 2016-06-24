(function () {
    'use strict';
    var controllerId = 'course';
    angular
        .module('app')
        .controller(controllerId, controller);

    controller.$inject = ['controller.abstract', '$routeParams', 'common', 'datacontext', '$aside', 'breeze', '$scope', '$location', '$http'];

    function controller(abstractController, $routeParams, common, datacontext,  $aside, breeze, $scope, $location, $http) {
        /* jshint validthis:true */
        var vm = this;
        abstractController.constructor.call(this, {
            controllerId: controllerId,
            watchedEntityNames: ['course','course.courseParticipants'],
            $scope: $scope
        })
        var id = $routeParams.id;
        var isNew = id == 'new';
        var saveBase = vm.save;

        vm.course = {};
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
        vm.setFinish = setFinish;
        vm.title = 'course';

        activate();

        function activate() {
            datacontext.ready().then(function () {
                var promises =[ datacontext.courseFormats.all().then(function (data) {
                    vm.courseFormats = data;
                }),datacontext.departments.all().then(function (data) {
                    vm.departments = data;
                }), datacontext.rooms.all().then(function (data) {
                    vm.rooms = data;
                })];
                if (isNew) {
                    vm.course = datacontext.courses.create();
                    vm.course.entityAspect.markNavigationPropertyAsLoaded('courseParticipants');
                    vm.notifyViewModelLoaded();
                }else{
                    promises.push(datacontext.courses.fetchByKey(id, {expand:'courseParticipants.participant'}).then(function (data) {
                        if (!data) {
                            vm.log.warning('Could not find course id = ' +id);
                            return;
                            //gotoCourses();
                        }
                        vm.course = data;
                        vm.notifyViewModelLoaded();
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
                $scope.$on('$destroy', function () { _modalInstance.destroy(); })
            }
            return _modalInstance;
        }

        function dateChanged(propName) {
            var dateInst = vm.course[propName];
            if (!dateInst instanceof Date) { return; }
            if (dateInst.getHours() === 0 && dateInst.getMinutes() === 0) { //bad luck if you want your course to start at midnight, but this would be an extreme edge case!
                dateInst.setHours(8);
            }
            if (propName === 'startTime') {
                setFinish();
            } else if (propName === 'facultyMeetingTime') {
                if (!vm.course.facultyMeetingDuration) {
                    vm.course.facultyMeetingDuration = 30;
                }
            }
        }

        function setFinish() {
            getCourseLengthPromise.then(function (courseLength) {
                vm.course.finishTime = (courseLength === null || !vm.course.startTime)
                    ? null
                    : new Date(vm.course.startTime.getTime() + courseLength);
            });
        }

        function deleteCourseParticipant(participantId) {
            var cp = getCourseParticipant(participantId);
            var name = cp.participant.fullName;
            cp.entityAspect.setDeleted();
            datacontext.save(cp).then(function (data) { vm.log('removed ' + name + ' from course');},
                function (error) { log.error({ msg: 'failed to remove ' + name + ' from course' }) })
        }

        var _courseLength = null;
        function getCourseLengthPromise() {
            if (!vm.course.courseFormat) {
                return $q.resolve(null);
            } else if (_courseLength === null) {
                if (!vm.course.courseFormat.entityAspect.isNavigationPropertyLoaded('courseSlots')) {
                    vm.course.courseFormat.entityAspect.loadNavigationProperty('courseSlots').then(getSlotDuration);
                } else {
                    return $q.resolve(getSlotDuration());
                }
            } else {
                return $q.resolve(_courseLength);
            }

            function getSlotDuration() {
                _courseLength = 0;
                vm.course.courseFormat.courseSlots.forEach(function (el) { _courseLength += el.minutesDuration; });
                _courseLength *= 60000;
                return _courseLength;
            }
        }

        function formatChanged() {
            if (!vm.course.courseFormat) {
                //_courseLength = null;
                vm.course.finishTime = null;
                return;
            }

            setFinish();
        }

        function getCourseParticipant(participantId) {
            return vm.course.courseParticipants.find(function (cp) {
                return cp.participantId === participantId;
            })
        }

        function save() {
            saveBase().then(function () {
                if (isNew) {
                    $location.updatePath('course/' + vm.course.id,false);
                    isNew = false;
                }
            });
        }

        function sendEmails() {
            $http({
                method: 'POST',
                url: 'api/CoursePlanning/EmailAll/',
                data: { CourseId: vm.course.id },
            }).then(function (response) {
                alert(response.data || 'emails sent');
            });
        }
    }
})();
