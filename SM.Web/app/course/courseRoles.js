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
            $scope: $scope
        })
        var id = $routeParams.id;

        vm.course = {};

        vm.title = 'course roles';

        vm.sortableOptions = {};

        vm.dragableOptions = {
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
                    data.courseFormat.courseSlots.sort(common.sortOnPropertyName('order'));
                    vm.map = data.courseFormat.courseSlots.map(function(cs){
                        var isThisSlot = function(el) {
                            return el.courseSlotId === cs.id;
                        };
                        var startTime = slotTime;
                        slotTime = new Date(startTime.getTime() + cs.minutesDuration * 60000);;
                        if (cs.activity) {
                            var faculty = data.courseSlotPresenters.filter(isThisSlot);
                            faculty.addFaculty = function (participantId) {
                                return datacontext.courseSlotPresenters.create({ participantId: participantId, courseSlotId: cs.id, courseId: data.id });
                            };
                            faculty.removeFaculty = removeFaculty.bind(faculty);
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
                        data.courseFormat.courseType.facultySimRoles.sort(common.sortOnPropertyName('order'));
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
                                    return datacontext.courseScenarioFacultyRoles.create({ participantId: participantId, courseSlotId: cs.id, courseId: data.id, facultySimRoleId: fr.id });
                                };
                                faculty.removeFaculty = removeFaculty.bind(faculty);
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
                    data.courseParticipants.sort(common.sortOnChildPropertyName('participant','fullName'));
                    data.courseParticipants.forEach(function (el) {
                        if (el.isFaculty) {
                            //add the icon property so we can access it in the other categories
                            if (!el.participant.icon) {
                                el.participant.icon = common.getRoleIcon(el.participant.professionalRole.category);
                            }
                            vm.faculty.push(angular.extend(
                                calculateCourseRoles(el.participantId),
                                {
                                    participantId: el.participantId,
                                    fullName: el.participant.fullName,
                                    icon: el.participant.icon,
                                    department: el.participant.department.abbreviation
                                }));
                        }
                    });
                    vm.notifyViewModelPropChanged();
                }), controllerId).then(function () {
                        vm.log('Activated Course Roles View');
                });
            });
        }

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

        function removeFaculty(item) {
            var indx = this.indexOf(item);
            item.entityAspect.setDeleted();
            this.splice(indx, 1);
        }

        function updateSortable(event, ui) {
            // on cross list sortings recieved is not true
            // during the first update
            // which is fired on the source sortable
            var sortable = ui.item.sortable;
            var participantId = sortable.model.participantId;
            sortable.cancel();
            if (sortable.droptarget[0] === sortable.source[0] ||
                sortable.droptargetModel.some(function (el) {
                    return el.participantId === participantId
            })) {
                return;
            }

            var model = sortable.droptargetModel.addFaculty(participantId);
            angular.extend(sortable.model, calculateCourseRoles(participantId));

            // restore the removed item
            //unless it has been moved out of a faculty area
            //not necessary now calling cancel
            //sortable.sourceModel.push(sortable.model);

            // clone the original model to restore the removed item is another option if wishing to keep list order the same
            //vm.sourceScreens = originalScreens.slice();

            //$scope.$apply(function () {
                sortable.droptargetModel.splice(
                  sortable.dropindex, 0,
                  model);
            //});
            //options 
        }
    }
})();
