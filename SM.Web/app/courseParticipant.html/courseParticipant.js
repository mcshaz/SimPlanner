(function () {
    'use strict';
    var controllerId = 'courseParticipant';
    angular
        .module('app')
        .controller(controllerId, course);

    courseParticipantCtrl.$inject = ['$uibModalInstance', 'common', 'courseParticipant', dataContext];

    function courseParticipantCtrl($uibModalInstance, common, courseParticipant, datacontext) {
        /* jshint validthis:true */
        var vm = this;
        var log = common.logger.getLogFn(controllerId);

        vm.save = function () {
            log({ msg:'saved date: ' + vm.course.startTime });
        }//datacontext.save;
        vm.canSave = false;
        $rootScope.$on('hasChanges', function () {
            vm.canSave = datacontext.courseParticipants.hasChanges();
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
                if (id) {
                    promises.push(datacontext.courses.fetchByKey(id).then(function (data) {
                        if (!data) {
                            log.warning('Could not find session id = ' +id);
                            return;
                            //gotoCourses();
                    }
                    vm.course = data;
                    vm.institution = vm.course.department.institution;
                    }));
                }
                common.activateController(promises, controllerId)
                    .then(function () {
                        log('Activated Course View');
                    });
                });
            }
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
