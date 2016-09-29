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
                var faculty = [];
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

                    vm.course = data;
                    data.courseFormat.courseSlots.sort(common.sortOnPropertyName('order'));
                    data.courseParticipants.sort(common.sortOnChildPropertyName('participant', 'fullName'));

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
                }), //end datacontext.courses.findbykey
                datacontext.manikins.all()
                ], controllerId).then(function () { //all loaded
                    var slotTime = vm.course.startUtc;
                    var slotCount = 0;
                    var scenarioCount = 0;
                    //at the moment, department and institution are loaded at datacontex.ready,
                    //so the following will work here
                    //if the institution and department data were to be loaded by a server call, this should go in the
                    //activateController.then method
                    datacontext.institutions.all().then(function (institutions) {
                        var sortName = common.sortOnPropertyName('name');
                        var sortDescription = common.sortOnPropertyName('description');
                        institutions.sort(sortName);
                        var manikins = createMultiSelect(institutions, 'manikins', function (m) {
                            return {
                                id: m.id,
                                checked: false,
                                description: m.description,
                                booked: priorExposure.BookedManikins[m.id]
                            };
                        });
                        /*
                        vm.scenarios = createMultiSelect(institutions, 'scenarios', function (s) {
                            return {
                                id: s.id,
                                checked: false,
                                description: s.description,
                                priorExposure: participantIdsToName(priorExposure.ScenarioParticipants[s.id])
                            };
                        });
                        */
                        vm.scenarios = vm.course.department.scenarios.map(function (s) {
                            var priorExposure = participantIdsToName(priorExposure.ScenarioParticipants[s.id]);
                            return new {
                                html: createBsOptionHtml(s.briefDescription, priorExposure),
                                id: s.id
                            }
                        });

                        vm.map = vm.course.courseFormat.courseSlots
                            .filter(function (cs) { return cs.isActive; })
                            .map(function (cs) {
                                var start = slotTime;
                                var returnVar = {
                                    id: cs.id,
                                    start: start,
                                    groupClass: 'grp' + slotCount++,
                                    name: cs.activity
                                        ? cs.activity.name
                                        : 'Simulation ' + ++scenarioCount,
                                    track: cs.trackParticipants
                                }
                                if (!cs.trackParticipants) {
                                    return returnVar;
                                }
                                var isThisSlot = function (el) { return el.courseSlotId === cs.id; };
                                var sortableOptions = {
                                    connectWith: '.' + returnVar.groupClass
                                };
                                var activityScenario = vm.course.courseSlotActivities.find(isThisSlot);
                                returnVar.availableFaculty = faculty.slice();
                                returnVar.availableFacultyOptions = angular.extend({
                                    update: updateSortable
                                }, sortableOptions);
                                slotTime = new Date(start.getTime() + cs.minutesDuration * 60000);
                                if (cs.activity) {
                                    var assignedFaculty = [];
                                    vm.course.courseSlotPresenters.forEach(function (csp) {
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
                                        courseId: vm.course.id
                                    }, datacontext.courseSlotPresenters);

                                    return angular.extend(returnVar, {
                                        choices: cs.activity.activityChoices.map(mapChoice),
                                        selectedActivityId: activityScenario
                                            ? activityScenario.activityId
                                            : null,
                                        isScenario: false,
                                        assignedFaculty: assignedFaculty,
                                        sortableOptions: sortableOptions
                                    });
                                }
                                //else if isScenario {

                                returnVar.availableFacultyOptions.start = startSortable;
                                returnVar.availableFaculty.availableFaculty = true;

                                var slotRoles = cs.courseScenarioFacultyRoles.filter(isThisSlot);
                                var courseSlotManikins = vm.course.courseSlotManikins.filter(isThisSlot)

                                //manikin 
                                if (courseSlotManikins) {
                                    returnVar.manikins = angular.copy(manikins);
                                    returnVar.manikins.forEach(function (i) {
                                        i.children.forEach(function (m) {
                                            m.checked = courseSlotManikins.some(function (csm) {
                                                return csm.manikinId === m.id;
                                            });
                                        });
                                    });
                                    returnVar.selectedManikins = [];
                                    $scope.$watchCollection(function () {
                                        return returnVar.selectedManikins;
                                    }, common.manageCollectionChange(datacontext.courseSlotManikins, 'id',
                                        function (member) {
                                            return {
                                                manikinId: member.id,
                                                courseSlotId: cs.id, //vm.map[indx].id,//slight hack because the collection is replaced by the isteven multiselect - not really sure why this works, but otherwise digest in progress error on change
                                                courseId: vm.course.id
                                            };
                                        }));
                                }
                                //end manikin
                                vm.course.courseFormat.courseType.courseTypeScenarioRoles.sort(common.sortOnChildPropertyName('facultyScenarioRole', 'order'));
                                return angular.extend(returnVar, {
                                    selectedActivityId: activityScenario
                                        ? activityScenario.scenarioId
                                        : null,
                                    isScenario: true,
                                    roles: vm.course.courseFormat.courseType.courseTypeScenarioRoles.map(function (ctsr) {
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
                                                    courseId: vm.course.id,
                                                    facultyScenarioRoleId: ctsr.facultyScenarioRoleId
                                                }, datacontext.courseScenarioFacultyRoles),
                                                start: startSortable
                                            }, sortableOptions)
                                        };
                                    })
                                });
                            });//end map courseSlot
                        vm.notifyViewModelLoaded();
                        
                        function mapChoice(c) {
                            return {
                                id: c.id,
                                html: createBsOptionHtml(c.description, participantIdsToName(priorExposure.ActivityParticipants[c.id]))
                            };
                        }
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
                if (m.selectedActivityId) {
                    m.activityScenario = datacontext.courseSlotActivities.create({
                        courseSlotId: m.id,
                        courseId: vm.course.id
                    });
                    m.activityScenario[fk] = m.selectedActivityId;
                }
            } else if (m.selectedActivityId) {
                if (m.activityScenario.entityAspect.entityState.isDeleted()) {
                    if (m.activityScenario[fk] === m.selectedChoice.id) {
                        m.activityScenario.entityAspect.setUnchanged();
                        //could notify change to controller.abstract here
                    } else {
                        m.activityScenario.entityAspect.setModified();
                    }
                }
                m.activityScenario[fk] = m.selectedActivityId;
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

        function createBsOptionHtml(text, priorExposure) {
            return priorExposure
                ? '<div class="exposed" title="repeat for participants ' + priorExposure + '">' + text + '</div>' //<div data-trigger="hover" data-type="warn" data-animation="am-flip-x" data-title="xxxxxx" bs-tooltip ></div>
                : '<div>' + text + '</div>';
        }

        function participantIdsToName(ids) {
            if (!ids) {return ids;}
            var names = ids.map(function(id){
                return vm.course.participants.find(function(p){
                    return p.id === id;}).name;
            });
            return names.join(', ');
        }

        /*
        {
		text: 'B', value: 'b', id: 2, checked: 1, open: true,
		$ams_order: 2, $ams_parents_id: [], $ams_children_nodes: 0,
		$ams_children_leafs: 1,$ams_level: 0, $ams_tree_visibility: true,  $ams_checked_children: 1
	},
	{
		text: 'C', value: 'c', id: 3, checked: true, open: false,
		$ams_order: 3, $ams_parents_id: [2], $ams_children_nodes: 0,
		$ams_children_leafs: 0,$ams_level: 1, $ams_tree_visibility: true,  $ams_checked_children: 0
	},
        */
        function createMultiSelect(institutions, dptPropertyName, dptPropertyMap) {
            var returnVar = [];
            institutions.forEach(function (inst) {
                var dpts = departmentMap(inst.departments);
                if (dpts.length) {
                    returnVar.push({
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
                var dpts = [];
                ds.forEach(function (d) {
                    if (d[dptPropertyName].length) {
                        dpts.push({
                            id: d.id,
                            checked: false,
                            open: d.id === vm.course.department.id,
                            name: d.name,
                            abbrev: d.abbreviation,
                            children: d[dptPropertyName].map(dptPropertyMap)
                        });
                    }
                });
                return dpts;
            }

            return returnVar;
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
