(function () {
    'use strict';
    var controllerId = 'dashboard';
    angular.module('app').controller(controllerId, ['common', 'errorhandler', 'unitofwork','breeze', 'tokenStorageService','$rootScope','AUTH_EVENTS','$q','$location',dashboard]);

    function dashboard(common, errorhandler, unitofwork, breeze, tokenStorageService, $rootScope, AUTH_EVENTS,$q,$location) {
        var log = common.logger.getLogFn(controllerId);
        var ref = unitofwork.get('courses');
        var uow = ref.value();
        var Predicate = breeze.Predicate;
        var FilterQueryOp = breeze.FilterQueryOp;
        var vm = this;
        var currentUserId;
        vm.gotoCourse = gotoCourse;
        vm.userCourses = [];
        vm.upcomingCourses = [];
        vm.userSummaryData = {};
        vm.title = 'Dashboard';

        activate();

        function activate() {
            $rootScope.$on(AUTH_EVENTS.loginCancelled, updateDash);
            $rootScope.$on(AUTH_EVENTS.loginConfirmed, updateDash);

            common.activateController(getPromises(), controllerId)
                .then(function () {
                    log('Activated Dashboard View');
                });
        }

        function getPromises() {
            if (tokenStorageService.isLoggedIn()) {
                currentUserId = tokenStorageService.getUserId();
                //todo uow.ready promise - nested
                return [getCourses() /*, getUserSummaryData(), getUserSummaryData */];
            }
            currentUserId = null;
            setDummyData();
            return [];
        }

        function getCourses() {
            var now = new Date();
            return uow.courses.find({
                where: Predicate.create('startTime', '>', now),
                orderBy: 'startTime',
                take: 5,
                expand: 'courseParticipants'
            }).then(function (data) {
                if (data.length) {
                    uow.ready().then(function () {
                        vm.nextUserCourse = null;
                        vm.upcomingCourses = data.map(function (c) {
                            var department = uow.departments.getByKey(c.departmentId);
                            var returnVar = {
                                id: c.id,
                                date: dateShortFormatter(c.startTime),
                                location: c.location,
                                isUserInvolved: c.courseParticipants.some(function (cp) {
                                    return cp.participantId == currentUserId;
                                }),
                                departmentName: department.name
                            };
                            if (vm.nextUserCourse === null && returnVar.isUserInvolved) {
                                vm.nextUserCourse = returnVar;
                                angular.extend(returnVar, dateLongFormatter(c.startTime));
                                returnVar.institutionName = department.institution.name
                            }

                            return returnVar;
                        });
                    });
                }

            });
        }

        function getUserSummaryData() {
            return datacontext.getUserCourses().then(function (data) {
                return vm.userSummaryData = data;
            });
        }

        function getUpcomingCourses() {
            return uow.staffingResourceListItems.find().then(function (data) {
                return vm.userCourses = data;
            });
        }

        function gotoCourse(course) {
            if (course && course.id) {
                // '#/course/71'
                //$route.transition(url)
                $location.path('/course/' + course.id);
            }
        }

        function updateDash() {
            $q.all([getPromises()]).then(function (eventArgs) {
                log.info({ msg: 'Dashboard updated' });
            });
        }

        function dateLongFormatter(date) {
            if (typeof date === 'undefined') {
                throw new TypeError('date not defined');
            }
            var m = moment(date);
            return {
                longDate: m.format("Do MMMM YYYY h:mm a"),
                interval: m.fromNow()
            }
        }

        function dateShortFormatter(date){
            if (typeof date === 'undefined') {
                throw new TypeError('date not defined');
            }
            var m = moment(date);
            return m.format("DD MMM YYYY HH:mm");
        }

        function setDummyData() {
            vm.nextUserCourse = {
                longDate: 'sign in to find out when',
                interval:'(hopefully very soon)',
                departmentName: '[your department]',
                institution: '[your institution]'
            };
        }
    }
})();