(function () {
    'use strict';
    var controllerId = 'dashboard';
    angular.module('app').controller(controllerId, ['common', 'errorhandler', 'datacontext','breeze', 'tokenStorageService','$rootScope','AUTH_EVENTS','$q','$location',dashboard]);

    function dashboard(common, errorhandler, datacontext, breeze, tokenStorageService, $rootScope, AUTH_EVENTS,$q,$location) {
        var log = common.logger.getLogFn(controllerId);
        var Predicate = breeze.Predicate;
        var FilterQueryOp = breeze.FilterQueryOp;
        var vm = this;
        vm.currentUserId = null;
        vm.gotoCourse = gotoCourse;
        vm.userCourses = [];
        vm.upcomingCourses = [];
        vm.userSummaryData = {};
        vm.title = 'Dashboard';
        vm.loginState = '';

        vm.dateIntervalFormatter = common.dateUtilities.dateIntervalFormatter;
        vm.dateLongFormatter = common.dateUtilities.dateTimeLongFormatter;
        vm.dateShortFormatter = common.dateUtilities.dateTimeShortFormatter;

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
                vm.currentUserId = tokenStorageService.getUserId();
                //todo uow.ready promise - nested
                return [getCourses() /*, getUserSummaryData(), getUserSummaryData */];
            }
            vm.loginState = 'notLoggedIn';
            vm.currentUserId = null;
            return [];
        }

        function getCourses() {
            var now = new Date();
            return datacontext.briefCourses.find({
                where: Predicate.create('startTime', '>', now),
                orderBy: 'startTime',
                take: 5
                //expand: 'courseParticipants'
            }).then(function (data) {
                if (data.length) {
                    datacontext.ready().then(function () {
                        vm.upcomingCourses = data;
                        vm.nextUserCourse = data.find(function (course) {
                            return course.includesUser(vm.currentUserId);
                        });
                        vm.loginState = (vm.nextUserCourse ? 'active' : 'inactive');
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

    }
})();