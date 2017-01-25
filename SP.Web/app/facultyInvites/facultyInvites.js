(function () {
    'use strict';
    var controllerId = 'facultyInvites';
    angular
        .module('app')
        .controller(controllerId, controller);

    controller.$inject = ['common', '$scope', 'users.abstract', 'datacontext', 'tokenStorageService', 'breeze'];

    function controller(common, $scope, abstractUserDetails, datacontext, tokenStorageService, breeze) {
        /* jshint validthis:true */
        var vm = this;

        abstractUserDetails.constructor.call(this, $scope);
        vm.title = 'Faculty Invites';
        vm.courses = [];
        vm.courseTypes = [];
        vm.filterChanged = filterChanged;

        vm.limitToInstructors = true;
        vm.selectedCourseType = {};
        activate();

        function activate() {
            datacontext.ready().then(function () {
                var pred = breeze.Predicate.create('isOrganiser', '==', true)
                    .and(breeze.Predicate.create('participantId', '==', tokenStorageService.getUserId()));
                pred = breeze.Predicate.create('courseParticipants', 'any', pred)
                    .and(breeze.Predicate.create('startUtc', '>', new Date()));
                common.activateController([vm.baseReady,
                    datacontext.courses.find({ where: pred, expand: 'courseParticipants', orderBy: 'startUtc' }).then(function (data) {
                        vm.courses = data;
                        var cts = new Set(data.map(function (c) {
                            return c.courseFormat.courseType;
                        }));
                        vm.courseTypes = Array.from(cts);
                        if (vm.courseTypes.length) {
                            vm.selectedCourseType = vm.courseTypes[0];
                            if (vm.selectedCourseType.instructorCourseId) {
                                vm.filterPredicate = filterChanged()[0];
                            } else {
                                vm.limitToInstructors = false;
                            }
                        } else {
                            vm.limitToInstructors = false;
                        }
                    })], controllerId).then(vm.updateData);
            });
        }

        function filterChanged() {
            if (vm.limitToInstructors && vm.selectedCourseType && vm.selectedCourseType.instructorCourseId) {
                var pred = breeze.Predicate.create('courseId', '==', vm.selectedCourseType.instructorCourseId)
                    .and('isConfirmed', '!=', 'false')
                    .and('course.startUtc', '<', new Date());
                return [breeze.Predicate.create('courseParticipants', 'any', pred)];
            }
            return [];
        }

    }
})();
