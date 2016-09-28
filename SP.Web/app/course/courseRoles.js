(function () {
    'use strict';
    var controllerId = 'courseRoles';
    angular
        .module('app')
        .controller(controllerId, controller);

    controller.$inject = ['controller.abstract', '$routeParams', 'common', 'datacontext', '$aside', 'breeze', '$scope', '$http'];

    function controller(abstractController, $routeParams, common, datacontext,  $aside, breeze, $scope, $http) {
        /* jshint validthis:true */
        var vm = this;
        abstractController.constructor.call(this, {
            controllerId: controllerId,
            $scope: $scope
        });
        var id = $routeParams.id;
        vm.changeActivityScenario = changeActivityScenario;
        vm.course = {};
        vm.scenarios = [];

        vm.title = 'course roles';

        activate();

        function activate() {
            datacontext.ready().then(function () {
                var priorExposure;
                common.activateController([$http({ method: 'GET', url: '/api/ActivitySummary/PriorExposure?courseId=' + id }).then(function (response) {
                    priorExposure = response.data;
                }, vm.log.error),
                datacontext.courses.fetchByKey(id, {
                    expand: ['courseParticipants.participant',
                        'courseFormat.courseSlots.activity.activityChoices',
                        'courseSlotActivities',
                        'courseSlotPresenters',
                        'courseSlotManikins',
                        'courseScenarioFacultyRoles',
                        'department.scenarios',
                        'courseFormat.courseType.courseTypeScenarioRoles.facultyScenarioRole']
                }).then(function (data) {
                    if (!data) {
                        vm.log.warning('Could not find course id = ' +id);
                        return;
                        //gotoCourses();
                    }
                    var slotTime = data.startUtc;
                    var count = 0;

                    vm.course = data;
                    vm.scenarios = data.department.scenarios;

                    data.courseFormat.courseSlots.sort(common.sortOnPropertyName('order'));
                    data.courseParticipants.sort(common.sortOnChildPropertyName('participant', 'fullName'));

                    //map faculty
                    var faculty = [];

                    data.courseParticipants.forEach(function (el) {
                        if (el.isFaculty) {
                            //add the icon property so we can access it in the other categories
                            faculty.push(angular.extend(calculateCourseRoles(el.participantId),
                                                {
                                                    participantId: el.participantId,
                                                    fullName: el.participant.fullName,
                                                    isConfirmed:el.isConfirmed,
                                                    icon: common.getRoleIcon(el.participant.professionalRole.category),
                                                    departmentName: el.participant.department.abbreviation
                                                }));
                        }
                    });
                    //end region map faculty

                    vm.map = data.courseFormat.courseSlots
                        .filter(function (cs) { return cs.isActive;})
                        .map(function (cs) {
                            var start = slotTime;
                            var isThisSlot = function (el) { return el.courseSlotId === cs.id; };
                            var returnVar = {
                                id: cs.id,
                                start: start,
                                groupClass: 'grp' + count++,
                                availableFaculty: faculty.slice(),
                                activityScenario: data.courseSlotActivities.find(isThisSlot)
                            };
                            var sortableOptions = {
                                connectWith: '.' + returnVar.groupClass
                            };
                            returnVar.availableFacultyOptions = angular.extend({
                                update: updateSortable
                            },sortableOptions);
                            slotTime = new Date(start.getTime() + cs.minutesDuration * 60000);
                            if (cs.activity) {
                                var assignedFaculty = [];
                                data.courseSlotPresenters.forEach(function (csp) {
                                    if (csp.courseSlotId === cs.id) {
                                        var indx = returnVar.availableFaculty.findIndex(function (f) {
                                            return f.participantId === csp.participantId;
                                        });
                                        assignedFaculty.push(returnVar.availableFaculty[indx]);
                                        returnVar.availableFaculty.splice(indx, 1);
                                    }
                                });
                                sortableOptions.update = updateSortableRepo.bind(null, {
                                    courseSlotId: cs.id,
                                    courseId: data.id
                                }, datacontext.courseSlotPresenters);

                                return angular.extend(returnVar, {
                                    name: cs.activity.name,
                                    start: start,
                                    choices: cs.activity.activityChoices,
                                    selectedActivity: (returnVar.activityScenario || {}).activity,
                                    isScenario: false,
                                    assignedFaculty: assignedFaculty,
                                    sortableOptions: sortableOptions
                                });
                            }
                            //extra properties to allow duplication
                            returnVar.availableFacultyOptions.start = startSortable;
                            returnVar.availableFaculty.availableFaculty = true;

                            var slotRoles = cs.courseScenarioFacultyRoles.filter(isThisSlot);
                            data.courseFormat.courseType.courseTypeScenarioRoles.sort(common.sortOnChildPropertyName('facultyScenarioRole', 'order'));
                            return angular.extend(returnVar, {
                                name: 'Simulation',
                                selectedActivity: (returnVar.activityScenario || {}).scenario,
                                isScenario: true,
                                courseSlotManikins: data.courseSlotManikins.filter(isThisSlot),
                                roles: data.courseFormat.courseType.courseTypeScenarioRoles.map(function (ctsr) {
                                    var assignedFaculty = [];
                                    slotRoles.forEach(function (sr) {
                                        if (ctsr.facultyScenarioRoleId === sr.facultyScenarioRoleId) {
                                            var findMatch = function (f) {
                                                return f.participantId === sr.participantId;
                                            };
                                            var indx = returnVar.availableFaculty.findIndex(findMatch);
                                            if (indx === -1) {
                                                assignedFaculty.push(faculty.find(findMatch));
                                            } else {
                                                assignedFaculty.push(returnVar.availableFaculty[indx]);
                                                returnVar.availableFaculty.splice(indx, 1);
                                            }
                                        }
                                    });

                                    return {
                                        description: ctsr.facultyScenarioRole.description,
                                        id: ctsr.facultyScenarioRoleId,
                                        assignedFaculty: assignedFaculty,
                                        sortableOptions: angular.extend({
                                            update: updateSortableRepo.bind(null, {
                                                courseSlotId: cs.id,
                                                courseId: data.id,
                                                facultyScenarioRoleId: ctsr.facultyScenarioRoleId
                                            }, datacontext.courseScenarioFacultyRoles),
                                            start: startSortable
                                        }, sortableOptions)
                                    };
                                })
                            });
                        });
                    vm.notifyViewModelLoaded();
                }), //end datacontext.courses.findbykey
                datacontext.manikins.all()
                ], controllerId).then(function () { //all loaded
                    var manikins = [];
                    //at the moment, department and institution are loaded at datacontex.ready,
                    //so the following will work here
                    //if the institution and department data were to be loaded by a server call, this should go in the
                    //activateController.then method
                    datacontext.institutions.all().then(function (institutions) {
                        institutions.forEach(function (inst) {
                            var dpts = departmentMap(inst.departments);
                            if (dpts.length) {
                                manikins.push({
                                    id: inst.id,
                                    checked: false,
                                    open: inst.id === vm.course.department.institution.id,
                                    name: inst.name,
                                    abbrev: inst.abbreviation,
                                    children: dpts
                                });
                            }
                        });

                        function departmentMap(ds) {
                            var returnVar = [];
                            ds.forEach(function (d) {
                                if (d.manikins.length) {
                                    returnVar.push({
                                        id: d.id,
                                        checked: false,
                                        open: d.id === vm.course.department.id,
                                        name: d.name,
                                        abbrev: d.abbreviation,
                                        children: d.manikins.map(manikinMap)
                                    });
                                }
                            });
                            return returnVar;
                        }

                        function manikinMap(m) {
                            return {
                                id: m.id,
                                checked: false,
                                description: m.description,
                                booked: priorExposure.BookedManikins.indexOf(m.id) !== -1
                            };
                        }
                        vm.map.forEach(function (el, indx) {
                            if (el.courseSlotManikins) {
                                el.manikins = angular.copy(manikins);
                                el.manikins.forEach(function (i) {
                                    i.children.forEach(function (m) {
                                        m.checked = el.courseSlotManikins.some(function (csm) {
                                            return csm.manikinId === m.id;
                                        });
                                    });
                                });
                                el.selectedManikins = [];
                                $scope.$watchCollection(function () {
                                    return el.selectedManikins;
                                }, common.manageCollectionChange(datacontext.courseSlotManikins, 'id',
                                    function (member) {
                                        return {
                                            manikinId: member.id,
                                            courseSlotId: vm.map[indx].id,//slight hack because the collection is replaced by the isteven multiselect - not really sure why this works, but otherwise digest in progress error on change
                                            courseId: vm.course.id
                                        };
                                    }));
                            }
                        });
                    });
                });
            }); // end datacontext.ready
        }
        function changeActivityScenario(m) {
            var fk = m.isScenario ?'scenarioId':'activityId';
            //console.log(m.scenario.briefDescription);
            if (m.activityScenario && m.activityScenario.entityAspect.entityState.isDetached()) {
                m.activityScenario = null;
            }
            if (!m.activityScenario) {
                if (m.selectedActivity) {
                    m.activityScenario = datacontext.courseSlotActivities.create({
                        courseSlotId: m.id,
                        courseId: vm.course.id
                    });
                    m.activityScenario[fk] = m.selectedActivity.id;
                }
            } else if (m.selectedActivity) {
                if (m.activityScenario.entityAspect.entityState.isDeleted()) {
                    if (m.activityScenario[fk] === m.selectedChoice.id) {
                        m.activityScenario.entityAspect.setUnchanged();
                        //could notify change to controller.abstract here
                    } else {
                        m.activityScenario.entityAspect.setModified();
                    }
                }
                m.activityScenario[fk] = m.selectedActivity.id;
            } else { //chosenTeachingResource exists, but activity is set to null
                m.activityScenario.entityAspect.setDeleted();
            }
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
        //duplicate ctrl key windows/ubuntu, option[alt] key mac, 
        function startSortable(event, ui) {
            if (event.ctrlKey) {
                //ui.placeholder.css('visibility', 'visible').text('[copying]');
                ui.item.sortable.isCopy = true;
            }
        }
        function updateSortable(event, ui) {
            var sortable = ui.item.sortable;
            if (ui.sender) {
                return; 
            } //sender has a value only when the receiving table. in order to cancel both sending and receiving events, cancel must be called from the sending table;
            var participantId = sortable.model.participantId;
            if (sortable.droptargetModel.some(function (el) {
                    return el.participantId === participantId;
            })) { //if the sender, ui.sender will be null - only cancel if the sender
                sortable.cancel();
                return;
            }
            if (ui.item.sortable.isCopy) { //?? should be in stop - add the item back in if in copy mode
                sortable.sourceModel.splice(sortable.index,0,sortable.model);
            }
        }

        function updateSortableRepo(key, repo, event, ui) {
            updateSortable(event, ui);
            var sortable = ui.item.sortable;
            var cancelled = sortable.isCanceled();
            //at the moment this works because the only reason to cancel is if there is a duplicate - beware if cancelling for other reasons
            var removingDuplicate = sortable.droptargetModel.availableFaculty && cancelled;
            if (removingDuplicate) {
                 //? should be in stop
                sortable.sourceModel.splice(sortable.index, 1);
            }

            key.participantId = sortable.model.participantId;
            var ent;
            if (ui.sender && !cancelled) { //receiving = adding 
                ent = repo.getByKey(key);
                if (ent) {
                    if (ent.entityAspect.entityState.isDeleted()) {
                        ent.entityAspect.setUnchanged();
                    } else {
                        log.debug({msg:'attempted to readd existing entity in unknown state', data:ent});
                    }
                } else {
                    repo.create(key);
                    //this is not user changed (Ids only) - probably could look at how this is implemented 
                    //but for now just set manually
                    //vm.isEntityStateChanged = true;
                }
                
            } else if (!ui.item.sortable.isCopy && (!cancelled || removingDuplicate)) { //sending = deleting
                ent = repo.getByKey(key);
                ent.entityAspect.setDeleted();
            }

            angular.extend(sortable.model, calculateCourseRoles(key.participantId));
        }
    }
    function sortManikins(man1, man2){
        if (man1.department.name > man2.department.name)
            return 1;
        if (man1.department.name < man2.department.name)
            return -1;

        if (man1.description > man2.description)
            return 1;
        if (man1.description < man2.description)
            return -1;

        return 0;
    }
})();
