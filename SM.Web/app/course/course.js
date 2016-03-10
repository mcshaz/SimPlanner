(function () {
    'use strict';
    var controllerId = 'course';
    angular
        .module('app')
        .controller(controllerId, controller);

    controller.$inject = ['$routeParams','common','datacontext', '$rootScope', '$uibModal']; 

    function controller($routeParams, common, datacontext, $rootScope, $uibModal) {
        /* jshint validthis:true */
        var vm = this;
        var log = common.logger.getLogFn(controllerId);
        var id = $routeParams.id;

        vm.canSave = false;
        vm.course = {};
        vm.courseTypes = [];
        vm.dateFormat = '';
        vm.dpPopup = { isOpen: false };
        vm.institutions = [];
        vm.institution = {};
        vm.maxDate = new Date();
        vm.maxDate.setFullYear(vm.maxDate.getFullYear() + 1);
        vm.minDate = new Date(2007, 1);
        vm.openDp = openDp;
        vm.openCourseParticipant = openCourseParticipant;
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
                        if (data.length === 1 && !id) {
                            vm.course.courseType = data[0];
                    }
                }),
                    datacontext.institutions.all().then(function (data) {
                        vm.institutions = data;
                        if (data.length === 1 && !id) {
                            vm.institution = data[0];
                        }
                })];
                if (id && id!='new') {
                    promises.push(datacontext.courses.fetchByKey(id, {expand:'courseParticipants.participant'}).then(function (data) {
                        if (!data) {
                            log.warning('Could not find course id = ' +id);
                            return;
                            //gotoCourses();
                        }
                        vm.course = data;
                        vm.institution = vm.course.department.institution;
                    }));
                }
                vm.dateFormat = moment().localeData().longDateFormat('L').replace(/D/g, "d").replace(/Y/g, "y");
                common.activateController(promises, controllerId)
                    .then(function () {
                        log('Activated Course View');
                    });
            });
        }
        function openDp() {
            this.dpPopup.isOpen = true;
        }

        function save() {
            log({ msg: 'saved date: ' + vm.course.startTime });
        }//datacontext.save;

        function openCourseParticipant(participantId) {
            var modalInstance = $uibModal.open({
                templateUrl: 'app/auth/getCredentials.html',
                controller: 'getCredentials',
                size: 'lg',
                resolve: {
                    courseParticipantIds: function () {
                        return participantId?[id, participantId]:'new';
                    }
                }
            });
        }
    }
})();
