(function () {
    'use strict';
    var controllerId = 'courseParticipant';
    angular
        .module('app')
        .controller(controllerId, courseParticipantCtrl);

    courseParticipantCtrl.$inject = ['common', 'datacontext', 'breeze', '$scope', 'currentCourse', 'controller.abstract'];
    //changed $uibModalInstance to $scope to get the events

    function courseParticipantCtrl(common, datacontext, breeze, $scope, currentCourse, abstractController) {
        /* jshint validthis:true */
        var vm = this;
        abstractController.constructor.call(this, {
            controllerId: controllerId,
            watchedEntityName: 'participant',
            $scope: $scope
        })

        var isNewCourseParticipant = vm.isNew = (currentCourse.participantId === null);

        vm.canSave = false;
        vm.close = close;
        vm.createCourseParticipant = createCourseParticipant;
        vm.createNewPerson = createNewPerson;
        vm.dialCode = '';
        vm.departments = [];
        vm.isFaculty = currentCourse.isFaculty;
        vm.isValidParticipantName = isValidParticipantName;
        vm.getPeople = getPeople;
        vm.participant = { fullName: '' };
        vm.onParticipantSelected = onParticipantSelected;
        vm.professionalRoles = [];
        vm.saveParticipant = saveParticipant;

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
                if (!isNewCourseParticipant) {
                    promises.push(datacontext.participants.fetchByKey(currentCourse.participantId, { expand: "participant" }).then(function (data) {
                        if (!data) {
                            vm.log.warning({ msg: 'Could not find specified course participant', data: currentCourse.participantId });
                            return;
                            //gotoCourses();
                        }
                        vm.participant = data;
                    }));
                } 
                common.activateController(promises, controllerId)
                    .then(function () {
                        vm.log('Activated Course Participant Dialog');
                    });
                });
        }
        function close() {
            $scope.$close('closeButton');
        }

        function createCourseParticipant($event) {
            var entState = vm.participant.entityAspect.entityState;
            if (entState.isAdded()) {
                datacontext.save(vm.participant);
            } else if (entState.isModified()) {
                //TODO compare names and show alert
            }
            var cp = currentCourse.createCourseParticipant(vm.participant);
            vm.log.success(cp.participant.fullName + ' added to course ' + (cp.isFaculty ? 'faculty' : 'participants'));
            vm.isNew = true;
            vm.participant = { fullName: '' };
        }

        function createNewPerson() {
            var match;
            if (match = _lastData.find(function (ld) {
                return ld.fullName.startsWith(name);
            })) {
                if (!confirm("Are you sure you want to create a new person rather than select " + match.fullName)) {
                    return;
                }
            }
            vm.participant = datacontext.participants.create();

            //todo check event fires
        }

        function saveParticipant() {
            var origName = vm.participant.entityAspect.originalValues.fullName;
            if (origName !== vm.participant.fullName) {
                if (!confirm("Are you sure you wish to change the name of " + origName + " to " + vm.participant.fullName +"?\nYou should only click yes if\n-This person's name was mispelt\n-You are adding something to differentiate from others with a similar name, e.g. John 'tall' Smith\n-The person has changed their name, e.g. after marriage")) {

                }
            }
            datacontext.save(vm.participant);
        }

        var notThisCourse = breeze.Predicate.create('courseParticipants', 'any', 'courseId', '==', currentCourse.courseId).not();
        var baseArgs = {
            orderBy: 'fullName',
            take: 10,
            select: 'id,fullName,professionalRole.category,department.abbreviation'
        };
        var _lastVal;
        var _lastData;
        function getPeople(val) {
            if (_lastVal && _lastVal.length < baseArgs.take && val.toLowerCase().startsWith(_lastVal)) {
                val = val.toLowerCase();
                //I think the uib - typeahead handles either promises or objects, but seems cleaner to have a function return one or tother
                return _lastData.filter(function (el) { return el.fullName.toLowerCase().startsWith(val); });
            }
            baseArgs.where = breeze.Predicate.create('fullName', 'startsWith', val).and(notThisCourse);
            return datacontext.participants.find(baseArgs).then(function (results) {
                results.forEach(function (el) {
                    el.description = el.fullName + ' (' + el.department_Abbreviation + ' ' + common.toSeperateWords(el.professionalRole_Category) + ')';
                });
                _lastVal = val.toLowerCase();
                return (_lastData = results);
            });
            delete baseArgs.where;
        }

        function onParticipantSelected(item) {
            datacontext.participants.fetchByKey(item.id).then(function (part) {
                vm.participant = part;
            });
        }

        function isValidParticipantName() {

        }

    }
})();
