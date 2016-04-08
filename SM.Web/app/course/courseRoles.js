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

        vm.sortableOptions = {
            placeholder: "faculty here",
            connectWith: ".faculty",
            update: updateSortable
        };

        activate();

        function activate() {
            datacontext.ready().then(function () {
                common.activateController(datacontext.courses.fetchByKey(id, {
                    expand: ['courseParticipants.participant',
                        'courseFormat.courseSlots.activity.activityChoices',
                        'courseSlotScenarios',
                        'courseSlotPresenters',
                        'courseScenarioFacultyRoles',
                        'chosenTeachingResources',
                        'courseFormat.courseType.scenarios',
                        'courseFormat.courseType.facultySimRoles'],
                }).then(function (data) {
                    if (!data) {
                        vm.log.warning('Could not find course id = ' +id);
                        return;
                        //gotoCourses();
                    }
                    var slotTime = data.startTime;
                    data.courseFormat.courseSlots.sort(vm.sortOnOrderProperty);
                    vm.map = data.courseFormat.courseSlots.map(function(cs){
                        var isThisSlot = function(el) {
                            return el.courseSlotId === cs.id;
                        };
                        var startTime = slotTime;
                        slotTime = new Date(startTime.getTime() + cs.minutesDuration * 60000);;
                        if (cs.activity) {
                            var faculty = data.courseSlotPresenters.filter(isThisSlot);
                            faculty.addFaculty = function (participantId) {
                                datacontext.courseSlotPresenters.create({ participantId: participantId, courseSlotId: cs.id, courseId: data.id });
                            };
                            return {
                                id: cs.id,
                                name: cs.activity.name,
                                startTime: startTime,
                                choices: cs.activity.activityChoices,
                                selectedChoice: (data.chosenTeachingResources.find(isThisSlot)||{}).activityTeachingResource,
                                isSim: false,
                                faculty: faculty
                                
                            }
                        }
                        var slotRoles = cs.courseScenarioFacultyRoles.filter(isThisSlot);
                        return {
                            id:cs.id,
                            name: 'Simulation',
                            startTime: startTime,
                            choices: data.courseFormat.courseType.scenarios,
                            selectedChoice: (data.courseSlotScenarios.find(isThisSlot) || {}).scenario,
                            isSim: true,
                            roles: data.courseFormat.courseType.facultySimRoles.map(function (fr) {
                                var faculty = slotRoles.filter(function (el) { return fr.id === el.facultySimRoleId; });
                                faculty.addFaculty = function (participantId) {
                                    datacontext.courseScenarioFacultyRoles.create({ participantId: participantId, courseSlotId: cs.id, courseId: data.id, facultySimRoleId: fr.id });
                                };
                                return {
                                    description: fr.description,
                                    id: fr.id,
                                    faculty:faculty
                                }
                            })
                        };
                    });
                    vm.course = data;
                    vm.faculty = [];
                    data.courseParticipants.forEach(function (el) {
                        if (el.isFaculty) {
                            vm.faculty.push(angular.extend(
                                calculateCourseRoles(el.participantId),
                                {
                                    participantId: el.participantId,
                                    name: el.participant.fullName, 
                                }));
                        }
                    });
                }), controllerId).then(function () {
                        vm.log('Activated Course Roles View');
                });
            });
        }

        function save($event) {
            //vm.log.debug($event);
            datacontext.save();
        }//;

        function calculateCourseRoles(participantId) {
            var returnVar = {
                slotCount: 0,
                scenarioCount: 0
            };
            vm.course.courseSlotPresenters.forEach(function (el) {
                if (el.participantId===participantId) {
                    returnVar.slotCount++;
                }
            });
            vm.course.courseScenarioFacultyRoles.forEach(function (el) {
                if (el.participantId === participantId) {
                    returnVar.scenarioCount++;
                }
            });
            return returnVar;
        }

        function updateSortable(event, ui) {
            // on cross list sortings recieved is not true
            // during the first update
            // which is fired on the source sortable
            if (!ui.item.sortable.received) {
                var sourceModel = ui.item.sortable.sourceModel;
                var itemModel = ui.item.sortable.model;
                var targetModel = ui.item.sortable.droptargetModel;
                //todo - check for id or participantId

                //no duplicates
                if (targetModel.some(function (el) {
                    return el.participantId === itemModel.participantId;
                })) {
                    ui.item.sortable.cancel();
                    return;
                }

                targetModel.addFaculty(itemModel.participantId);
                console.log(vm.map[0].faculty.map(function (el) { return el.participantId }).join(','));
                // restore the removed item
                //unless it has been moved out of a faculty area
                sourceModel.push(itemModel);

                // clone the original model to restore the removed item is another option if wishing to keep list order the same
                //vm.sourceScreens = originalScreens.slice();

                //options 
            }
        }
    }
})();
