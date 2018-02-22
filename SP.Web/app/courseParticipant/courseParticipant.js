"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
var controllerId = 'courseParticipant';
angular_1.default
    .module('app')
    .controller(controllerId, courseParticipantCtrl);
courseParticipantCtrl.$inject = ['common', 'datacontext', 'breeze', '$scope', 'participantBase.abstract', 'selectOptionMaps'];
function courseParticipantCtrl(common, datacontext, breeze, $scope, participantBaseAbstract, selectOptionMaps) {
    var vm = this;
    participantBaseAbstract.constructor.call(this, {
        controllerId: controllerId,
        $scope: $scope
    });
    vm.createCourseParticipant = createCourseParticipant;
    vm.createNewPerson = createNewPerson;
    vm.disableAdd = disableAdd;
    vm.getPeople = getPeople;
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
    activate();
    function activate() {
        datacontext.ready().then(function () {
            var promises = vm.getPromises();
            promises.push(datacontext.institutions.all({ expand: 'culture' }).then(function (data) {
                vm.institutions = data.sort(common.sortOnPropertyName('name'));
                vm.institutions.forEach(function (i) {
                    i.departments.sort(common.sortOnPropertyName('name'));
                });
                vm.institution = vm.isNew
                    ? $scope.course.department.institution
                    : $scope.courseParticipant.department.institution;
            }));
            common.activateController(promises, controllerId)
                .then(function () {
                institutionChanged();
                vm.notifyViewModelLoaded();
                vm.log('Activated Course Participant Dialog');
            });
        });
    }
    var _lastVal;
    var _lastLookupResult;
    var _lastLookupPromise;
    function createCourseParticipant() {
        if (!validateSaveParticipant()) {
            return;
        }
        var forSave;
        vm.participant.adminApproved = true;
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
                clearCurrent();
            });
        }
        else {
            forSave = $scope.courseParticipant;
            forSave.professionalRoleId = vm.participant.defaultProfessionalRoleId;
            forSave.departmentId = vm.participant.defaultDepartmentId;
            vm.save([forSave, forSave.participant]).then(function () {
                vm.log.success(vm.participant.fullName + ' updated');
                clearCurrent();
            });
            vm.isNew = true;
        }
    }
    function clearCurrent() {
        vm.participant = datacontext.participants.create(breeze.EntityState.Detached);
        _lastLookupResult = _lastVal = _lastLookupPromise = null;
    }
    function createNewPerson() {
        var partRx = new RegExp("^\\s*" + vm.participant.fullName, 'i');
        if (_lastLookupResult) {
            create(_lastLookupResult);
        }
        else {
            (_lastLookupPromise || getPeople(vm.participant.fullName)).then(create);
        }
        function create(result) {
            var match = result.find(function (ld) {
                return partRx.test(ld.fullName);
            });
            if (match) {
                if (!confirm("Are you sure you want to create a new person rather than select " + match.fullName)) {
                    onParticipantSelected(match);
                    return;
                }
            }
            partRx = new RegExp(partRx.source + "\\s*$", "i");
            match = $scope.course.courseParticipants.find(function (cp) {
                return partRx.test(cp.participant.fullName);
            });
            if (match) {
                if (!confirm(match.participant.fullName + " is already " + (match.isFaculty ? "faculty for" : "a participant in") + " this course. Do you wish to add someone else with the same name?")) {
                    clearCurrent();
                    return;
                }
            }
            vm.participant.department = $scope.course.department;
            datacontext.addEntity(vm.participant);
        }
    }
    function disableAdd() {
        var ent = vm.participant.entityAspect;
        return ent.entityState.isDetached() || ent.hasValidationErrors || vm.isSaving;
    }
    function validateSaveParticipant() {
        var origName = vm.participant.entityAspect.originalValues.fullName;
        if (origName && origName !== vm.participant.fullName) {
            if (!confirm("Are you sure you wish to change the name of " + origName + " to " + vm.participant.fullName + "?\n\nYou should only click yes if:\n-This person's name was originally mispelt [sic]\n-You are adding something to differentiate from others with a similar name, e.g. John 'tall' Smith\n-The person has changed their name, e.g. after marriage")) {
                vm.participant.entityAspect.rejectChanges();
            }
        }
        return true;
    }
    var notThisCourse = breeze.Predicate.create('courseParticipants', 'any', 'courseId', '==', $scope.course.id).not();
    var _pred;
    function institutionChanged() {
        _pred = notThisCourse.and('department.institutionId', '==', vm.institution.id);
        _lastVal = _lastLookupResult = _lastLookupPromise = null;
        vm.filterRoles();
    }
    var baseArgs = {
        orderBy: 'fullName',
        take: $scope.nameLimit,
        select: 'id,fullName,professionalRole.category,department.abbreviation'
    };
    function getPeople(val) {
        if (_lastVal && _lastVal.length < baseArgs.take && val.toLowerCase().startsWith(_lastVal)) {
            val = val.toLowerCase();
            return _lastLookupResult.filter(function (el) { return el.fullName.toLowerCase().startsWith(val); });
        }
        baseArgs.where = breeze.Predicate.create('fullName', 'startsWith', val).and(_pred);
        return _lastLookupPromise = datacontext.participants.find(baseArgs).then(function (results) {
            results.forEach(function (el) {
                el.class = selectOptionMaps.getRoleIcon(el.professionalRole_Category);
            });
            _lastVal = val.toLowerCase();
            delete baseArgs.where;
            return _lastLookupResult = results;
        });
    }
    function onParticipantSelected(item) {
        datacontext.participants.fetchByKey(item.id).then(function (part) {
            vm.participant = part;
        });
    }
    function isValidParticipantName() {
        return vm.participant.entityAspect.validateProperty("fullName");
    }
}
//# sourceMappingURL=courseParticipant.js.map