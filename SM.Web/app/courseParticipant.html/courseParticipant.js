(function () {
    'use strict';
    var controllerId = 'courseParticipant';
    angular
        .module('app')
        .controller(controllerId, course);

    courseParticipantCtrl.$inject = ['$uibModalInstance', 'common', 'courseParticipant', 'dataContext', 'courseParticipantIds'];

    function courseParticipantCtrl($uibModalInstance, common, courseParticipant, datacontext, courseParticipantIds) {
        /* jshint validthis:true */
        var vm = this;
        var log = common.logger.getLogFn(controllerId);

        vm.departments = [];
        vm.professionalRoles = [];
        vm.save = save;
        vm.close = close;

        vm.canSave = false;
        $rootScope.$on('hasChanges', function () {
            vm.canSave = datacontext.courseParticipants.hasChanges();
        });
        activate();

        function activate() {
            datacontext.ready().then(function () {
                var promises = [datacontext.departments.all().then(function (data) {
                    vm.courseTypes = data;
                }),
                datacontext.professionalRoles.all().then(function (data) {
                    vm.professionalRoles = data;
                })];
                if (courseParticipantId && courseParticipantId!=null) {
                    promises.push(datacontext.courseParticipants.fetchByKey(courseParticipantId, "courseParticipant.Participant").then(function (data) {
                        if (!data) {
                            log.warning('Could not find session id = ' +courseParticipantId);
                            return;
                            //gotoCourses();
                    }
                    vm.courseParticipant = data;
                    }));
                }
                common.activateController(promises, controllerId)
                    .then(function () {
                        log('Activated Course View');
                    });
                });
        }
        function close() {
            $uibModalInstance.close();
        }
        function save() {
            log({ msg: 'saved date: ' + vm.course.startTime });
        }//datacontext.save;

        function getPeople(val) {
            return datacontext.participants.find({
                where: Predicate.create('fullName', 'startsWith', val),
                orderBy: 'fullName',
                take: 10,
                select:'id,fullName'
            }).then(function (results) {
                return results;
            });
        }

    }
})();
