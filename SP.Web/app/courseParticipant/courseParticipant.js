(function () {
    'use strict';
    var controllerId = 'courseParticipant';
    angular
        .module('app')
        .controller(controllerId, courseParticipantCtrl);

    courseParticipantCtrl.$inject = ['common', 'datacontext', 'breeze', '$scope','controller.abstract', 'selectOptionMaps'];
    //changed $uibModalInstance to $scope to get the events

    function courseParticipantCtrl(common, datacontext, breeze, $scope, abstractController, selectOptionMaps) {
        /* jshint validthis:true */
        var vm = this;
        
        abstractController.constructor.call(this, {
            controllerId: controllerId,
            watchedEntityNames: 'participant',
            $scope: $scope
        });
        
        vm.createCourseParticipant = createCourseParticipant;
        vm.createNewPerson = createNewPerson;
        vm.disableAdd = disableAdd;
        vm.getPeople = getPeople;
        vm.institution = {};
        vm.institutionChanged = institutionChanged;
        vm.institutions = [];
        vm.isFaculty = $scope.isFaculty;
        vm.isNew = !$scope.courseParticipant;
        vm.isValidParticipantName = isValidParticipantName;
        vm.nameLimit = 10;
        vm.onParticipantSelected = onParticipantSelected;
        vm.participant = vm.isNew
            ? datacontext.participants.create(breeze.EntityState.Detached)
            : $scope.courseParticipant.participant;
        vm.professionalRoles = [];

        activate();

        function activate() {
            datacontext.ready().then(function () {
                var promises = [datacontext.institutions.all({expand:'culture'}).then(function (data) {
                    vm.institutions = data;
                    vm.institution = vm.isNew
                        ? $scope.course.department.institution
                        : $scope.courseParticipant.department.institution;
                    institutionChanged();
                    /*
                    vm.institutions.forEach(function (el) {
                        if (!el.culture.flagUrl) {
                            el.culture.flagUrl = common.getFlagUrlFromLocaleCode(el.localeCode);
                        }
                    });
                    */
                }),
                datacontext.professionalRoles.all().then(function (data) {
                    vm.professionalRoles = data;
                })];
                common.activateController(promises, controllerId)
                    .then(function () {
                        vm.notifyViewModelLoaded();
                        vm.log('Activated Course Participant Dialog');
                    });
                });
        }

        var _lastVal;
        var _lastLookup;
        function createCourseParticipant($event) {
            if (!validateSaveParticipant()) { return; }
            var forSave;
            if (vm.isNew) {
                forSave = datacontext.courseParticipants.create({
                    participant: vm.participant,
                    isFaculty: vm.isFaculty,
                    course: $scope.course,
                    professionalRoleId: vm.participant.defaultProfessionalRoleId,
                    departmentId: vm.participant.defaultDepartmentId
                });
                vm.save([forSave, forSave.participant]).then(function () {
                    vm.log.success(vm.participant.fullName + ' added to course ' + (vm.isFaculty ? 'faculty' : 'participants'));
                    afterSave();
                });
            } else {
                forSave = $scope.courseParticipant;
                forSave.professionalRoleId = vm.participant.defaultProfessionalRoleId;
                forSave.departmentId = vm.participant.defaultDepartmentId;
                vm.save([forSave, forSave.participant]).then(function () {
                    vm.log.success(vm.participant.fullName + ' updated');
                    afterSave();
                });
                vm.isNew = true;
            }

            function afterSave() {
                vm.participant = datacontext.participants.create(breeze.EntityState.Detached);
                _lastLookup = _lastVal = null;
            }
        }

        function createNewPerson() {
            var match = _lastLookup.find(function (ld) {
                return ld.fullName.startsWith(name);
            });
            if (match) {
                if (!confirm("Are you sure you want to create a new person rather than select " + match.fullName)) {
                    onParticipantSelected(match);
                    return;
                }
            }
            datacontext.addEntity(vm.participant);
            //todo check event fires
        }

        function disableAdd() {
            var ent = vm.participant.entityAspect;
            return ent.entityState.isDetached() || ent.hasValidationErrors;
        }

        function validateSaveParticipant() {
            var origName = vm.participant.entityAspect.originalValues.fullName;
            if (origName && origName !== vm.participant.fullName) {
                if (!confirm("Are you sure you wish to change the name of " + origName + " to " + vm.participant.fullName +"?\n\nYou should only click yes if:\n-This person's name was originally mispelt [sic]\n-You are adding something to differentiate from others with a similar name, e.g. John 'tall' Smith\n-The person has changed their name, e.g. after marriage")) {
                    vm.participant.entityAspect.rejectChanges();
                    /*
                    var oldVal = vm.participant;
                    vm.participant = datacontext.cloneItem(oldVal);
                    oldval.entityAspect.rejectChanges();
                    ["email", "alternateEmail", "phoneNumber"].foreach(function(propName){
                        if (vm.participant[propName] === oldVal[propName]) { vm.participant["propName"] = null; }
                    });
                    return false
                    */
                }
            }
            return true;
        }

        var notThisCourse = breeze.Predicate.create('courseParticipants', 'any', 'courseId', '==', $scope.course.id).not();
        var _pred;
        function institutionChanged() {
            _pred = notThisCourse.and('department.institutionId', '==', vm.institution.id);
            _lastVal = _lastLookup = null;

        }

        var baseArgs = {
            orderBy: 'fullName',
            take: $scope.nameLimit,
            select: 'id,fullName,professionalRole.category,department.abbreviation'
        };

        function getPeople(val) {
            if (_lastVal && _lastVal.length < baseArgs.take && val.toLowerCase().startsWith(_lastVal)) {
                val = val.toLowerCase();
                //I think the uib - typeahead handles either promises or objects, but seems cleaner to have a function return one or tother
                return _lastLookup.filter(function (el) { return el.fullName.toLowerCase().startsWith(val); });
            }
            baseArgs.where = breeze.Predicate.create('fullName', 'startsWith', val).and(_pred);
            return datacontext.participants.find(baseArgs).then(function (results) {
                results.forEach(function (el) {
                    el.class = selectOptionMaps.getRoleIcon(el.professionalRole_Category);
                });
                _lastVal = val.toLowerCase();
                delete baseArgs.where;
                return _lastLookup = results;
            });
        }

        function onParticipantSelected(item) {
            datacontext.participants.fetchByKey(item.id).then(function (part) {
                vm.participant = part;
            });
        }

/* strap lookup
        function onParticipantSelected(event, value, index, elem) {
            if (elem.$id === "getFullName") {
                datacontext.participants.fetchByKey(_lastCacheLookup[index].id).then(function (part) {
                    vm.participant = part;
                });
            }
        }
*/

        function isValidParticipantName() {
            return vm.participant.entityAspect.validateProperty("fullName");
        }

    }
})();
