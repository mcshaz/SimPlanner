"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
var controllerId = 'courseRoles';
angular_1.default
    .module('app')
    .controller(controllerId, controller);
controller.$inject = ['controller.abstract', '$routeParams', 'common', 'datacontext', 'breeze', '$scope', '$http', '$q', 'selectOptionMaps', 'tokenStorageService', '$httpParamSerializerJQLike', 'moment'];
function controller(abstractController, $routeParams, common, datacontext, breeze, $scope, $http, $q, selectOptionMaps, tokenStorageService, $httpParamSerializerJQLike, moment) {
    var vm = this;
    var id = $routeParams.id;
    abstractController.constructor.call(this, {
        controllerId: controllerId,
        $scope: $scope
    });
    vm.changeActivityScenario = changeActivityScenario;
    vm.course = {};
    vm.departments = [];
    vm.departmentSelected = departmentSelected;
    vm.manikins = [];
    vm.manikinDptIds = [];
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
                        'courseSlotManikins.manikin',
                        'courseScenarioFacultyRoles',
                        'department.scenarios',
                        'courseDays',
                        'courseFormat.courseType.courseTypeScenarioRoles.facultyScenarioRole']
                }).then(function (data) {
                    if (!data) {
                        vm.log.warning('Could not find course id = ' + id);
                        return;
                    }
                    vm.course = data;
                    data.courseFormat.courseSlots.sort(common.sortOnPropertyName('order'));
                    data.courseParticipants.sort(common.sortOnChildPropertyName('participant', 'fullName'));
                    data.courseParticipants.forEach(function (el) {
                        if (el.isFaculty) {
                            faculty.push(angular_1.default.extend(calculateCourseRoles(el.participantId), {
                                participantId: el.participantId,
                                fullName: el.participant.fullName,
                                isConfirmed: el.isConfirmed,
                                icon: selectOptionMaps.getRoleIcon(el.participant.professionalRole.category),
                                departmentName: el.participant.department.abbreviation
                            }));
                        }
                    });
                    vm.manikinDptIds = Array.from(new Set(vm.course.courseSlotManikins.map(function (csm) { return csm.manikin.departmentId; }).concat([vm.course.departmentId, tokenStorageService.getUserDepartmentId()])));
                    departmentSelected(vm.manikinDptIds);
                }),
                datacontext.departments.all().then(function (data) {
                    vm.departments = selectOptionMaps.sortAndMapDepartment(data.filter(selectOptionMaps.filterLocalDepartments()));
                })], controllerId).then(function () {
                var slotTime = vm.course.startFacultyUtc;
                var currentDay = 1;
                var slotCount = 0;
                var scenarioCount = 0;
                vm.scenarios = vm.course.department.scenarios.map(function (s) {
                    return {
                        name: s.briefDescription,
                        additional: createBsOptionHtml(participantIdsToName(priorExposure.ScenarioParticipants[s.id])),
                        id: s.id
                    };
                });
                vm.map = vm.course.courseFormat.courseSlots
                    .filter(function (cs) { return cs.isActive; })
                    .map(function (cs) {
                    var start = cs.day === currentDay
                        ? slotTime
                        : vm.course.courseDays.find(function (cd) {
                            return cd.day === cs.day;
                        }).startFacultyUtc;
                    var returnVar = {
                        id: cs.id,
                        start: start,
                        newDay: cs.day !== currentDay,
                        groupClass: 'grp' + slotCount++,
                        name: cs.activity
                            ? cs.activity.name
                            : 'Simulation ' + ++scenarioCount,
                        track: cs.trackParticipants
                    };
                    currentDay = cs.day;
                    slotTime = new Date(start.getTime() + cs.minutesDuration * 60000);
                    if (!cs.trackParticipants) {
                        return returnVar;
                    }
                    var isThisSlot = function (el) { return el.courseSlotId === cs.id; };
                    var sortableOptions = {
                        connectWith: '.' + returnVar.groupClass
                    };
                    returnVar.activityScenario = vm.course.courseSlotActivities.find(isThisSlot);
                    returnVar.availableFaculty = faculty.slice();
                    returnVar.availableFacultyOptions = angular_1.default.extend({
                        update: updateSortable
                    }, sortableOptions);
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
                        return angular_1.default.extend(returnVar, {
                            choices: cs.activity.activityChoices.map(mapChoice),
                            selectedActivityId: returnVar.activityScenario
                                ? returnVar.activityScenario.activityId
                                : null,
                            isScenario: false,
                            assignedFaculty: assignedFaculty,
                            sortableOptions: sortableOptions
                        });
                    }
                    var slotRoles = vm.course.courseScenarioFacultyRoles.filter(isThisSlot);
                    var createManikinKey = function (manikinId) {
                        return {
                            manikinId: manikinId,
                            courseSlotId: returnVar.id,
                            courseId: id
                        };
                    };
                    returnVar.availableFacultyOptions.start = startSortable;
                    returnVar.availableFaculty.availableFaculty = true;
                    returnVar.courseSlotManikinIds = vm.course.courseSlotManikins.filter(isThisSlot).map(mapId);
                    returnVar.manikinRemoved = common.removeCollectionItem.bind(null, datacontext.courseSlotManikins, createManikinKey);
                    returnVar.manikinSelected = common.addCollectionItem.bind(null, datacontext.courseSlotManikins, createManikinKey);
                    vm.course.courseFormat.courseType.courseTypeScenarioRoles.sort(common.sortOnChildPropertyName('facultyScenarioRole', 'order'));
                    return angular_1.default.extend(returnVar, {
                        selectedActivityId: returnVar.activityScenario
                            ? returnVar.activityScenario.scenarioId
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
                                    }
                                    else {
                                        assignedFaculty.push(returnVar.availableFaculty[indx]);
                                        returnVar.availableFaculty.splice(indx, 1);
                                    }
                                }
                            });
                            return {
                                description: ctsr.facultyScenarioRole.description,
                                id: ctsr.facultyScenarioRoleId,
                                assignedFaculty: assignedFaculty,
                                sortableOptions: angular_1.default.extend({
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
                });
                vm.notifyViewModelLoaded();
                function mapChoice(c) {
                    return {
                        id: c.id,
                        name: c.description,
                        additional: createBsOptionHtml(participantIdsToName(priorExposure.ActivityParticipants[c.id]))
                    };
                }
            });
        });
    }
    function changeActivityScenario(m) {
        var fk = m.isScenario ? 'scenarioId' : 'activityId';
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
        }
        else if (m.selectedActivityId) {
            if (m.activityScenario.entityAspect.entityState.isDeleted()) {
                if (m.activityScenario[fk] === m.selectedActivityId) {
                    m.activityScenario.entityAspect.setUnchanged();
                }
                else {
                    m.activityScenario.entityAspect.setModified();
                }
            }
            m.activityScenario[fk] = m.selectedActivityId;
        }
        else {
            m.activityScenario.entityAspect.setDeleted();
        }
    }
    var _activeSlots;
    function getActiveSlots() {
        if (!_activeSlots) {
            _activeSlots = vm.course.courseFormat.courseSlots
                .filter(function (cs) { return cs.isActive; })
                .map(function (cs) { return cs.id; });
        }
        return _activeSlots;
    }
    function calculateCourseRoles(participantId) {
        var returnVar = {
            slotCount: 0,
            scenarioCount: 0
        };
        vm.course.courseSlotPresenters.forEach(function (el) {
            if (el.participantId === participantId && getActiveSlots().indexOf(el.courseSlotId) > -1) {
                returnVar.slotCount++;
            }
        });
        vm.course.courseScenarioFacultyRoles.forEach(function (el) {
            if (el.participantId === participantId && getActiveSlots().indexOf(el.courseSlotId) > -1) {
                returnVar.scenarioCount++;
            }
        });
        return returnVar;
    }
    function departmentSelected(departmentIds) {
        var pred, bookings;
        if (departmentIds === void 0) {
            return $q.when([]);
        }
        if (!Array.isArray(departmentIds)) {
            departmentIds = [departmentIds];
        }
        departmentIds = departmentIds.filter(function (dId) { return !vm.manikins.some(function (m) { return m.departmentId === dId; }); });
        switch (departmentIds.length) {
            case 0:
                return $q.when([]);
            case 1:
                pred = breeze.Predicate.create('departmentId', '==', departmentIds[0]);
                break;
            default:
                pred = breeze.Predicate.create({ 'departmentId': { 'in': departmentIds } });
                break;
        }
        $q.all([datacontext.manikins.find({ where: pred, expand: 'manikinServices' }).then(function (data) {
                vm.manikins = vm.manikins.concat(data.map(function (m) {
                    var serviceDate = m.manikinServices.find(function (ms) { return !ms.returned; });
                    return {
                        id: m.id,
                        description: m.description,
                        departmentAbbreviation: m.department.abbreviation,
                        institutionAbbreviation: m.department.institution.abbreviation,
                        inService: serviceDate
                            ? "manikin sent for service on " + moment(serviceDate).format('l')
                            : null
                    };
                }));
            }), $http({ method: 'GET', url: '/api/ActivitySummary/ManikinBookings?' + $httpParamSerializerJQLike({
                    courseId: id,
                    departmentIds: departmentIds
                })
            }).then(function (response) {
                bookings = response.data;
            })]).then(function () {
            vm.manikins.forEach(function (m) {
                var courses = bookings[m.id];
                if (courses) {
                    m.bookings = "booked for " + courses.map(bookingInfoString).join(',\r\n');
                }
            });
        });
        function bookingInfoString(c) {
            return c.courseFormat.courseType.abbreviation + ' '
                + c.courseFormat.abbreviation + ' '
                + c.department.abbreviation + ' '
                + moment(c.startFacultyUtc).format('lll') + ' - '
                + moment(c.lastDay.finish).format('lll');
        }
    }
    function startSortable(event, ui) {
        if (event.ctrlKey) {
            ui.item.sortable.isCopy = true;
        }
    }
    function updateSortable(_event, ui) {
        var sortable = ui.item.sortable;
        if (ui.sender) {
            return;
        }
        var participantId = sortable.model.participantId;
        if (sortable.droptargetModel.some(function (el) {
            return el.participantId === participantId;
        })) {
            sortable.cancel();
            return;
        }
        if (ui.item.sortable.isCopy) {
            sortable.sourceModel.splice(sortable.index, 0, sortable.model);
        }
    }
    function updateSortableRepo(key, repo, event, ui) {
        updateSortable(event, ui);
        var sortable = ui.item.sortable;
        var cancelled = sortable.isCanceled();
        var removingDuplicate = sortable.droptargetModel.availableFaculty && cancelled;
        if (removingDuplicate) {
            sortable.sourceModel.splice(sortable.index, 1);
        }
        key.participantId = sortable.model.participantId;
        var ent;
        if (ui.sender && !cancelled) {
            ent = repo.getByKey(key);
            if (ent) {
                if (ent.entityAspect.entityState.isDeleted()) {
                    ent.entityAspect.setUnchanged();
                }
                else {
                    vm.log.debug({ msg: 'attempted to readd existing entity in unknown state', data: ent });
                }
            }
            else {
                repo.create(key);
            }
        }
        else if (!ui.item.sortable.isCopy && (!cancelled || removingDuplicate)) {
            ent = repo.getByKey(key);
            ent.entityAspect.setDeleted();
        }
        angular_1.default.extend(sortable.model, calculateCourseRoles(key.participantId));
    }
    function participantIdsToName(ids) {
        if (!ids) {
            return ids;
        }
        var names = ids.map(function (id) {
            return datacontext.participants.getByKey(id).fullName;
        });
        return names.join(', ');
    }
}
function createBsOptionHtml(priorExp) {
    return priorExp
        ? "repeat for participant(s) " + priorExp + "."
        : null;
}
function mapId(el) {
    return el.manikinId;
}
//# sourceMappingURL=courseRoles.js.map