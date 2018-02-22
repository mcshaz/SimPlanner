"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
var controllerId = 'course';
angular_1.default
    .module('app')
    .controller(controllerId, controller);
controller.$inject = ['controller.abstract', '$routeParams', 'common', 'datacontext', '$aside', '$scope'];
function controller(abstractController, $routeParams, common, datacontext, $aside, $scope) {
    var vm = this;
    abstractController.constructor.call(this, {
        controllerId: controllerId,
        watchedEntityNames: 'course',
        $scope: $scope
    });
    var id = $routeParams.id;
    var isNew = id === 'new';
    vm.course = {};
    vm.courseFormats = [];
    vm.dateChanged = dateChanged;
    vm.dateFormat = '';
    vm.deleteCourseParticipant = deleteCourseParticipant;
    vm.dpPopup = { isOpen: false };
    vm.departments = [];
    vm.hasChanges = false;
    vm.maxDate = new Date();
    vm.maxDate.setFullYear(vm.maxDate.getFullYear() + 1);
    vm.minDate = new Date(2007, 0);
    vm.openDp = openDp;
    vm.openCourseParticipant = openCourseParticipant;
    vm.rooms = [];
    vm.title = 'course';
    activate();
    function activate() {
        datacontext.ready().then(function () {
            var promises = [datacontext.courseFormats.all().then(function (data) {
                    vm.courseFormats = data;
                }), datacontext.departments.all().then(function (data) {
                    vm.departments = data;
                }), datacontext.rooms.all().then(function (data) {
                    vm.rooms = data;
                })];
            if (isNew) {
                vm.course = datacontext.courses.create();
                vm.notifyViewModelLoaded();
            }
            else {
                promises.push(datacontext.courses.fetchByKey(id, { expand: 'courseParticipants.participant' }).then(function (data) {
                    if (!data) {
                        vm.log.warning('Could not find course id = ' + id);
                        return;
                    }
                    vm.course = data;
                    vm.notifyViewModelLoaded();
                }));
            }
            common.activateController(promises, controllerId)
                .then(function () {
                vm.log('Activated Course View');
            });
        });
    }
    function openDp() {
        this.dpPopup.isOpen = true;
    }
    function openCourseParticipant(participantId) {
        var modal = getModalInstance();
        var scope = modal.$scope;
        var isNew = participantId.startsWith('new');
        scope.courseParticipant = isNew
            ? null
            : getCourseParticipant(participantId);
        scope.isFaculty = isNew
            ? participantId.endsWith('Faculty')
            : scope.courseParticipant.isFaculty;
        modal.$promise.then(modal.show);
    }
    var _modalInstance;
    function getModalInstance() {
        if (!_modalInstance) {
            var scope = $scope.$new();
            _modalInstance = $aside({
                templateUrl: 'app/courseParticipant/courseParticipant.html',
                controller: 'courseParticipant',
                show: false,
                id: 'cpModal',
                placement: 'left',
                animation: 'am-slide-left',
                scope: scope,
                controllerAs: 'cp'
            });
            scope.course = vm.course;
        }
        return _modalInstance;
    }
    function dateChanged(propName) {
        var dateInst = vm.course[propName];
        if (dateInst.getHours() === 0 && dateInst.getMinutes() === 0) {
            dateInst.setHours(8);
        }
    }
    function deleteCourseParticipant(participantId) {
        var cp = getCourseParticipant(participantId);
        var name = cp.participant.fullName;
        cp.entityAspect.setDeleted();
        datacontext.save(cp).then(function () { vm.log('removed ' + name + ' from course'); }, function (response) { vm.log.error({ msg: 'failed to remove ' + name + ' from course', data: response }); });
    }
    function getCourseParticipant(participantId) {
        return vm.course.courseParticipants.find(function (cp) {
            return cp.participantId === participantId;
        });
    }
}
//# sourceMappingURL=contactUs.js.map