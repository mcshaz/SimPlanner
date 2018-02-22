"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
var controllerId = 'dashboard';
angular_1.default.module('app').controller(controllerId, ['common', 'datacontext', 'breeze', 'tokenStorageService', '$rootScope', 'AUTH_EVENTS', '$q', '$http', dashboard]);
function dashboard(common, datacontext, breeze, tokenStorageService, $rootScope, AUTH_EVENTS, $q, $http) {
    var log = common.logger.getLogFn(controllerId);
    var vm = this;
    vm.currentUserId = null;
    vm.summaryData = {};
    vm.userCourses = [];
    vm.upcomingCourses = [];
    vm.userSummaryData = {};
    vm.title = 'Dashboard';
    vm.loginState = '';
    activate();
    function activate() {
        if (!tokenStorageService.isLoggedIn()) {
            $rootScope.$on(AUTH_EVENTS.loginConfirmed, updateDash);
        }
        common.activateController(getPromises(), controllerId)
            .then(function () {
            log('Activated Dashboard View');
        });
    }
    function getPromises() {
        if (tokenStorageService.isLoggedIn()) {
            vm.currentUserId = tokenStorageService.getUserId();
            return [getCourses(), getUserSummaryData()];
        }
        vm.loginState = 'notLoggedIn';
        vm.currentUserId = null;
        return [];
    }
    function getCourses() {
        var now = new Date();
        return $q.all([
            datacontext.ready(),
            datacontext.courses.find({
                where: breeze.Predicate.create('startFacultyUtc', '>', now),
                orderBy: 'startFacultyUtc',
                take: 5,
                expand: 'courseParticipants'
            }).then(function (data) { vm.upcomingCourses = data; })
        ])
            .then(function () {
            vm.upcomingCourses.forEach(function (course) {
                course.includesCurrentUser = course.includesUser(vm.currentUserId);
                if (!vm.nextUserCourse && course.includesCurrentUser) {
                    vm.nextUserCourse = course;
                }
            });
            vm.loginState = vm.nextUserCourse ? 'active' : 'inactive';
        });
    }
    function getUserSummaryData() {
        return $http({ method: 'GET', url: '/api/ActivitySummary/UserInfo' }).then(function (response) {
            vm.summaryData = common.mapToCamelCase(response.data);
        }, log.error);
    }
    function updateDash() {
        $q.all([getPromises()]).then(function () {
            log.info({ msg: 'Dashboard updated' });
        });
    }
}
//# sourceMappingURL=dashboard.js.map