(function () {
    'use strict';
    var controllerId = 'course';
    angular
        .module('app')
        .controller(controllerId, course);

    course.$inject = ['$routeParams','common','datacontext']; 

    function course($routeParams,common, datacontext) {
        /* jshint validthis:true */
        var vm = this;
        var log = common.logger.getLogFn(controllerId);

        var id = $routeParams.id;

        vm.course = {};
        //TODO - move nullOs out of controller

        vm.maxDate = new Date();
        vm.maxDate.setFullYear(vm.maxDate.getFullYear() + 1);
        vm.minDate = new Date(2007,1);


        vm.dpPopup = { isOpen: false };
        vm.openDp = openDp;

        vm.title = 'course';

        vm.institutions = [];
        vm.courseTypes = [];
        vm.institution = {};

        vm.faculty = [];
        vm.participants = [];

        vm.save = save;
        vm.canSave = false;

        activate();

        function activate() {
            datacontext.ready().then(function () {
                var promises = [
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
                            log.warning('Could not find session id = ' + id);
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

        function openDp() {
            vm.dpPopup.isOpen = true;
        }

        function save() {
            log.success({ msg: 'New Course Saved', data: vm.course.startTime });
        }

    }
})();
