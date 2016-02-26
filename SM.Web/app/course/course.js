(function () {
    'use strict';
    var controllerId = 'course';
    angular
        .module('app')
        .controller(controllerId, course);

    course.$inject = ['$routeParams','common','datacontext', '$rootScope']; 

    function course($routeParams, common, datacontext, $rootScope) {
        /* jshint validthis:true */
        var vm = this;
        var log = common.logger.getLogFn(controllerId);

        var id = $routeParams.id;

        vm.course = {};
        //TODO - move nullOs out of controller

        vm.maxDate = new Date();
        vm.maxDate.setFullYear(vm.maxDate.getFullYear() + 1);
        vm.minDate = new Date(2007, 1);

        vm.title = 'course';

        vm.institutions = [];
        vm.courseTypes = [];
        vm.institution = {};

        vm.faculty = [];
        vm.participants = [];

        vm.save = function () {
            log({ msg:'saved date: ' + vm.course.startTime });
        }//datacontext.save;
        vm.canSave = true;
        $rootScope.$on('hasChanges', function () {
            vm.canSave = datacontext.courses.hasChanges() || datacontext.courseParticipants.hasChanges();
        });
        activate();

            function activate() {
            datacontext.ready().then(function () {
                var promises =[
                    datacontext.courseTypes.all().then(function (data) {
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

    }
})();
