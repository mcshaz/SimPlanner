(function () {
    'use strict';
    var controllerId = 'courseRoles';
    angular
        .module('app')
        .controller(controllerId, controller);

    controller.$inject = ['controller.abstract', '$routeParams', 'common', 'datacontext', '$aside', 'breeze', '$scope'];

    function controller(abstractController, $routeParams, common, datacontext,  $aside, breeze, $scope) {
        /* jshint validthis:true */
        var vm = this;
        abstractController.constructor.call(this, {
            controllerId: controllerId,
            watchedEntityNames: 'coursesSlotPresenter',
            $scope: $scope
        })
        var id = $routeParams.id;

        vm.course = {};

        vm.title = 'course roles';

        activate();

        function activate() {
            datacontext.ready().then(function () {
                common.activateController(datacontext.courses.fetchByKey(id, {
                    expand: ['courseParticipants',
                        'courseFormat.courseSlots.activity.activityChoices',
                        'courseSlotScenarios',
                        'courseSlotPresenters',
                        'courseScenarioFacultyRoles',
                        'chosenTeachingResources',
                        'courseFormat.courseType.scenarios'],
                }).then(function (data) {
                    if (!data) {
                        vm.log.warning('Could not find course id = ' +id);
                        return;
                        //gotoCourses();
                    }
                    vm.map = vm.course.courseFormat.courseSlots.map(function(cs){
                        var isThisSlot = function(el) {
                            return el.courseSlotId === cs.id;
                        };
                        if (cs.courseActivity){
                            return {
                                name: cs.courseActivity.name,
                                choices: cs.courseActivity.activityChoices,
                                selectedChoice: data.courseSlotActivities.find(isThisSlot),
                                isSim: false,
                                faculty: data.courseSlotPresenters.filter(isThisSlot)
                            }
                        }
                        return {
                            name:'Simulation',
                            choices: data.courseFormat.courseType.scenarios,
                            selectedChoice: data.courseslotScenarios.find(isThisSlot),
                            isSim: true,
                            roles: data.courseFormat.courseType.facultyRoles.map(function (fr) {
                                return {
                                    name: fr.name,
                                    id: fr.id,
                                    presenters: cs.courseSlotScenarioPresenterRoles.filter(isThisSlot)
                                }
                            })
                        };
                    });

                }), controllerId).then(function () {
                        vm.log('Activated Course View');
                });
            });
        }

        function openDp() {
            this.dpPopup.isOpen = true;
        }

        function save($event) {
            //vm.log.debug($event);
            datacontext.save();
        }//;

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
                scope.asideInstance = _modalInstance;
                scope.course = vm.course;
            }
            return _modalInstance;
        }

        function deleteCourseParticipant(participantId) {
            var cp = getCourseParticipant(participantId);
            var name = cp.participant.fullName;
            cp.entityAspect.setDeleted();
            datacontext.save(cp).then(function (data) { vm.log('removed ' + name + ' from course');},
                function (error) { log.error({ msg: 'failed to remove ' + name + ' from course' }) })
        }

        function getCourseParticipant(participantId) {
            return vm.course.courseParticipants.find(function (cp) {
                return cp.participantId === participantId;
            })
        }
    }
})();
