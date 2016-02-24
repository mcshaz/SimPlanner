(function () {
    'use strict';
    var controllerId = 'course';
    angular
        .module('app')
        .controller(controllerId, course);

    course.$inject = ['$routeParams','common','unitofwork']; 

    function course($routeParams,common, unitofwork) {
        /* jshint validthis:true */
        var vm = this;
        var log = common.logger.getLogFn(controllerId);
        var ref = unitofwork.get('courses');
        var uow = ref.value();
        var id = $routeParams.id;

        vm.course = {};
        //TODO - move nullOs out of controller

        vm.maxDate = new Date();
        vm.maxDate.setFullYear(vm.maxDate.getFullYear() + 1);
        vm.minDate = new Date(2007,1);
        vm.timeString = '08:00';

        vm.timeChange = timeChange;
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
            uow.ready().then(function () {
                var promises = [
                    uow.courseTypes.all().then(function (data) {
                        vm.courseTypes = data;
                        if (data.length === 1 && !id) {
                            vm.course.courseType = data[0];
                        }
                    }),
                    uow.institutions.all().then(function (data) {
                        vm.institutions = data;
                        if (data.length === 1 && !id) {
                            vm.institution = data[0];
                        }
                    })];
                if (id) {
                    promises.push(uow.courses.find({
                        where: breeze.Predicate.create('id', '==', id),
                        expand: 'courseParticipants'
                    }).then(function (data) {
                        if (data.length !== 1) {
                            log.warning('Could not find session id = ' + val);
                            //gotoCourses();
                        }
                        vm.course = data[0];
                        vm.institution = vm.course.department.institution;
                        setTime();
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
            log.success('New Course Saved');
        }

        var timeFormats = ['H:m', 'h:m a'];

        function timeChange(value) {
            var m = moment(value, timeFormats);
            if (m.isValid()) {
                vm.course.startTime.setHours(m.hour(), m.minute(), 0, 0);
            }
        }

        function setTime() {
            var m = moment(vm.course.startTime);
            vm.timeString = m.format('HH:m');
        }

    }
})();
