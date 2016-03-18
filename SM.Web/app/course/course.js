(function () {
    'use strict';
    var controllerId = 'course';
    angular
        .module('app')
        .controller(controllerId, controller);

    controller.$inject = ['$routeParams','common','datacontext', '$rootScope', '$uibModal', 'breeze']; 

    function controller($routeParams, common, datacontext, $rootScope, $uibModal, breeze) {
        /* jshint validthis:true */
        var vm = this;
        var log = common.logger.getLogFn(controllerId);
        var id = $routeParams.id;

        vm.canSave = false;
        vm.course = {};
        vm.courseTypes = [];
        vm.dateFormat = '';
        vm.deleteCourseParticipant = deleteCourseParticipant;
        vm.dpPopup = { isOpen: false };
        vm.departments = [];
        vm.maxDate = new Date();
        vm.maxDate.setFullYear(vm.maxDate.getFullYear() + 1);
        vm.minDate = new Date(2007, 1);
        vm.openDp = openDp;
        vm.openCourseParticipant = openCourseParticipant;
        vm.rooms = [];
        vm.save = save;
        vm.title = 'course';

        $rootScope.$on('hasChanges', function () {
            vm.canSave = datacontext.courses.hasChanges();
        });
        activate();

        function activate() {
            datacontext.ready().then(function () {
                var promises =[ datacontext.courseTypes.all().then(function (data) {
                        vm.courseTypes = data;
                        if (data.length === 1 && isNew()) {
                            vm.course.courseType = data[0];
                    }
                }),datacontext.departments.all().then(function (data) {
                        vm.departments = data;
                        if (data.length === 1 && isNew()) {
                            vm.course.department = data[0];
                        }
                }), datacontext.rooms.all().then(function (data) {
                    vm.rooms = data;
                    if (data.length === 1 && isNew()) {
                        vm.course.room = data[0];
                    }
                })];
                if (isNew()) {
                    vm.course = datacontext.courses.create();
                }else{
                    promises.push(datacontext.courses.fetchByKey(id, {expand:'courseParticipants.participant'}).then(function (data) {
                        if (!data) {
                            log.warning('Could not find course id = ' +id);
                            return;
                            //gotoCourses();
                        }
                        vm.course = data;
                    }));
                }
                vm.dateFormat = moment().localeData().longDateFormat('L').replace(/D/g, "d").replace(/Y/g, "y");
                common.activateController(promises, controllerId)
                    .then(function () {
                        log('Activated Course View');
                    });
            });
        }

        function isNew() {
            return id == 'new';
        }

        function openDp() {
            this.dpPopup.isOpen = true;
        }

        function save() {
            log({ msg: 'saved date: ' + vm.course.startTime });
        }//datacontext.save;

        function openCourseParticipant(participantId) {
            var modalInstance = $uibModal.open({
                templateUrl: 'app/courseParticipant/courseParticipant.html',
                controller: 'courseParticipant',
                controllerAs: 'cp',
                //size: 'lg',
                resolve: {
                    courseParticipantIds: function () {
                        return { courseId: vm.course.id, participantId:participantId };
                    }
                }
            });
        }

        function deleteCourseParticipant(participantId) {
            var cp = vm.course.courseParticipants.find(function (el) {
                return el.participantId == participantId;
            });
            if (!cp) { log.warning({ msg: 'could not delete course participant', data: 'failed delete participant:' + participantId }); }
            cp.entityAspect.setDeleted();

        }
    }
})();
