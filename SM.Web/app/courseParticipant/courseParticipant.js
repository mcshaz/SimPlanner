(function () {
    'use strict';
    var controllerId = 'courseParticipant';
    angular
        .module('app')
        .controller(controllerId, courseParticipantCtrl);

    courseParticipantCtrl.$inject = ['common', 'datacontext', '$rootScope', 'breeze', 'courseParticipantIds'];

    function courseParticipantCtrl(common, datacontext, $rootScope, breeze, courseParticipantIds) {
        /* jshint validthis:true */
        var vm = this;
        var log = common.logger.getLogFn(controllerId);

        vm.canSave = false;
        vm.close = close;
        vm.dialCode = '';
        vm.departments = [];
        vm.getPeople = getPeople;
        vm.participantSelected = participantSelected;
        vm.professionalRoles = [];
        vm.save = save;

        $rootScope.$on('hasChanges', function () {
            vm.canSave = datacontext.courseParticipants.hasChanges();
        });
        activate();

        function activate() {
            datacontext.ready().then(function () {
                var promises = [datacontext.departments.all().then(function (data) {
                    vm.departments = data;
                    vm.dialCode = data[0].institution.country.dialCode;
                }),
                datacontext.professionalRoles.all().then(function (data) {
                    vm.professionalRoles = data;
                })];
                if (!/^new/.test(courseParticipantIds[1])) {
                    promises.push(datacontext.courseParticipants.fetchByKey(courseParticipantIds, { expand: "participant" }).then(function (data) {
                        if (!data) {
                            log.warning('Could not find session id = ' + courseParticipantIds.join(' , '));
                            return;
                            //gotoCourses();
                        }
                        vm.courseParticipant = data;
                    }));
                } 
                common.activateController(promises, controllerId)
                    .then(function () {
                        log('Activated CourseParticipant Dialog');
                    });
                });
        }
        function close() {
            $modalInstance.close();
        }
        function save() {
            log({ msg: 'saved date: ' + vm.course.startTime });
        }//datacontext.save;

        var notThisCourse = breeze.Predicate.create('courseParticipants', 'any', 'courseId', '==', courseParticipantIds[0]).not();

        var baseArgs = {
            orderBy: 'fullName',
            take: 10,
            select: 'id,fullName,professionalRole.category,department.abbreviation'
        };
        function getPeople(val) {
            baseArgs.where = breeze.Predicate.create('fullName', 'startsWith', val).and(notThisCourse);
            return datacontext.participants.find(baseArgs).then(function (results) {
                results.forEach(function (el) {
                    el.description = el.fullName + ' (' + el.department_Abbreviation + ' ' + common.toSeperateWords(el.professionalRole_Category) + ')';
                });
                return results;
            });
            delete baseArgs.where;
        }

        function participantSelected(item) {
            //for now assuming we make courseParticipant equivalent of 'immutable'
            var course = datacontext.courses.getByKey(courseParticipantIds[0]);
            vm.courseParticipant = course.addParticipant(item.id);
            vm.courseParticipant.isFaculty = courseParticipantIds[1]=='newFaculty';
            datacontext.participants.fetchByKey(item.id).then(function (part) {
                vm.courseParticipant.participant = part;
            });

        }

    }
})();
