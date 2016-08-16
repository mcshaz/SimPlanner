(function () {
    'use strict';
    var controllerId = 'courseType';
    angular
        .module('app')
        .controller(controllerId, controller);

    controller.$inject = ['controller.abstract', '$routeParams', 'common', 'datacontext', '$scope', 'breeze', '$aside'];

    function controller(abstractController, $routeParams, common, datacontext, $scope, breeze, $aside) {
        /* jshint validthis:true */
        var vm = this;
        abstractController.constructor.call(this, {
            controllerId: controllerId,
            watchedEntityNames: ['courseType', 'courseType.courseFormats', 'courseType.courseFormats.courseSlots', 'courseType.courseFormats.courseSlots.activity', 'courseType.courseTypeDepartments'],
            $scope: $scope,
            $watchers: [$scope.$on('$destroy', destroy)]
        });
        var id = $routeParams.id;
        var isNew = id === 'new';

        vm.activeFormatIndex = -1;
        vm.activitySelected = activitySelected;
        vm.alterDayMarkers = alterDayMarkers;
        vm.clone = clone;
        vm.courseType = {};
        vm.createSlot = createSlot;
        vm.createNewFormat = createNewFormat;
        vm.deleteFormat = deleteFormat;
        vm.departments = [];
        vm.editSlot = editSlot;
        vm.editChoices = editChoices;
        vm.emersionCategories = common.getEnumValues().emersion;
        vm.getCourseActivityNames = getCourseActivityNames;
        vm.instructorCourses = [];
        vm.isScenarioChanged = isScenarioChanged;
        vm.removeSlot = removeSlot;
        vm.resetExampleTimes = resetExampleTimes;

        vm.selectedDepartments = [];
        vm.title = 'Course Format';

        vm.sortableOptions = {
            handle: '.handle',
            stop: function (e, ui) {
                // this callback has the changed model
                var format = vm.courseType.courseFormats[vm.activeFormatIndex];
                var nextSlot = -1;
                console.log(format.sortableSlots.map(el=> ({ day:el.day, activity:el.activity?el.activity.name:el.isDayMarker?'day':'sim' })));
                format.sortableSlots.forEach(function (cs) {
                    if (!cs.isDayMarker && cs.order !== ++nextSlot) {
                        cs.order = nextSlot;
                    }
                });
                alterDayMarkers(format);
            },
            items: 'tr:not(.not-sortable)'
        };

        activate();

        function activate() {
            var departments;
            var promises = [
                    datacontext.ready(),
                    datacontext.courseTypes.find({where: breeze.Predicate.create('instructorCourseId','==',null).and('id','!=',id)}).then(function (data) {
                        vm.instructorCourses = data;
                    }),
                    datacontext.institutions.all({expand:'departments'}).then(function(data){
                        departments = data;
                    })
                ];
            if (isNew) {
                vm.courseType = datacontext.courseTypes.create();
                datacontext.courseFormats.create({ courseType: vm.courseType });
            } else {
                //promises.push(datacontext.courseTypes.fetchByKey(id, { expand: 'courseFormats.courseSlots' }).then(function (data) { - if the courseFormats were not already loaded from the server
                promises.push(datacontext.courseTypes.fetchByKey(id,
                {
                    expand: ["courseFormats.courseSlots.activity", "courseTypeDepartments"]
                }).then(function (data) {
                    vm.courseType = data;
                    vm.courseType.courseFormats.forEach(function (cf) {
                        resetExampleTimes(cf);
                        cf.sortableSlots = createCourseSlotSortableArray(cf.courseSlots);
                    });
                    vm.activeFormatIndex = vm.courseType.courseFormats.findIndex(function (cf) {
                        return cf.id === $routeParams.formatId;
                    });
                }));
            }
            common.activateController(promises, controllerId)
                .then(function () {
                    vm.notifyViewModelLoaded();
                    var ctds = vm.courseType.courseTypeDepartments;
                    departments.forEach(function (el) {
                        vm.departments.push({ name: '<strong>' + el.name + '</strong>', dptGroup: true });
                        el.departments.forEach(iterateDpt);
                        vm.departments.push({ dptGroup: false });
                    });
                    function iterateDpt(d) {
                        var checked = ctds.some(function(c){return c.departmentId === d.id;});
                        vm.departments.push({ name: d.name, abbreviation: d.abbreviation, dptId:d.id, checked:checked });
                    }
                    $scope.$watchCollection(function () { return vm.selectedDepartments; }, common.manageCollectionChange(datacontext.courseTypeDepartments, 'dptId',
                        function (member) {
                            return {
                                departmentId: member.dptId,
                                courseTypeId: vm.courseType.id
                            };
                        }));
                    if (vm.courseType.courseFormats.length === 1) {
                        vm.activeFormatIndex = 0;
                    }
                    vm.log('Activated Course Format View');
            });
        }
        
        //logic is - can't delete slots after course has been run, as they may have associated tables such as participants
        //-altering the name of a slot - don't allow reassignment of activity, as course participants etc will be all mucked up
        //-only show typeahead if a new slot (otherwise simple input)

        //-if a new slot/activity and activity selected, delete new activity, replace with selected
        function activitySelected(activityName, slot) {
            removeActivity(slot);
            slot.activity = vm.courseType.courseActivities.find(function (el) {
                return el.name === activityName;
            });
            reinstateInactive(slot.courseFormat);
        }

        function createActivity(slot) {
            slot.activity = datacontext.courseActivities.create({
                courseTypeId: vm.courseType.id
            });
        }

        //we have fiddled with the entity model rather than create a view model (naughty/lazy) - tear down those mods here
        function destroy() {
            if (vm.courseType) {
                vm.courseType.courseFormats.forEach(function (cf) {
                    delete cf.selectedSlot;
                    delete cf.sortableSlots;
                    delete cf.exampleStart;
                    cf.courseSlots.forEach(function (cs) {
                        delete cs.exampleFinish;
                    });
                });
            }
        }

        function removeActivity(slot) {
            var ca = slot.activity;
            if (ca && ca.entityAspect.entityState.isAdded()) {
                ca.entityAspect.setDeleted();
            }
            slot.activity = null;
        }

        function getCourseActivityNames(name) {
            name = name.toLowerCase();
            var returnVar =[];
            if (vm.courseType.courseActivities) {
                vm.courseType.courseActivities.forEach(function (el) {
                    if (el.name.toLowerCase().indexOf(name) !== -1) {
                        returnVar.push(el.name);
                }
            });
            }
            return returnVar;
        }
        
        /*
        function saveOverride() {
            //vm.log.debug($event);
            baseSave().then(destroy);
        }//;
        */

        function createSlot(courseFormat) {
            courseFormat.selectedSlot = datacontext.courseSlots.create({
                courseFormatId: courseFormat.id,
                order: (courseFormat.courseSlots || []).length,
                day: courseFormat.daysDuration
            });
            courseFormat.sortableSlots.push(courseFormat.selectedSlot);
            createActivity(courseFormat.selectedSlot);
        }

        function removeSlot(courseSlot) {
            removeActivity(courseSlot);
            var sortableSlots = courseSlot.courseFormat.sortableSlots;
            var indx = sortableSlots.indexOf(courseSlot);
            sortableSlots.splice(indx, 1);
            //delete courseSlot.courseFormat.selectedSlot; - not necessarily selected
            if (courseSlot.entityAspect.entityState.isAdded()) {
                courseSlot.entityAspect.setDeleted();
            } else {
                courseSlot.isActive = false;
            }
            resetExampleTimes(courseSlot.courseFormat);
        }

        function editChoices(slot) {
            var modal = getModalInstance();
            modal.$scope.courseActivity = slot.activity;
            modal.$promise.then(modal.show);
        }

        function editSlot(courseSlot) {
            courseSlot.courseFormat.selectedSlot = courseSlot;
            courseSlot.isScenario = !courseSlot.activity;
        }

        function isScenarioChanged(slot) {
            if (slot.isScenario) {
                removeActivity(slot);
                reinstateInactive(slot.courseFormat);
            } else {
                createActivity(slot);
            }
        }

        function reinstateInactive(courseFormat) {
            var selectedSlot = courseFormat.selectedSlot;
            if (!selectedSlot.entityAspect.entityState.isAdded()) {
                return;
            }
            var emptyScenarioSlot = courseFormat.courseSlots.some(function (el) {
                return !el.isActive && el !== selectedSlot && el.activity === selectedSlot.activity;
            });
            if (emptyScenarioSlot) { 
                selectedSlot.setDeleted();
                courseFormat.selectedSlot = emptyScenarioSlot;
                courseFormat.selectedSlot.isActive = true;
            }
        }

        var _modalInstance;
        function getModalInstance() {
            if (!_modalInstance) {
                var scope = $scope.$new();
                _modalInstance = $aside({
                        templateUrl: 'app/activityResources/activityResource.html',
                        controller: 'activityResource',
                    show: false,
                    id: 'cpModal',
                    placement: 'left',
                        animation: 'am-slide-left',
                    scope: scope,
                    controllerAs: 'ar'
                });
            }
            return _modalInstance;
        }

        function clone(cf) {
            var newFormat = datacontext.cloneItem(cf, ['courseSlots']);
            newFormat.description += " - copy";
            newFormat.sortableSlots = createCourseSlotSortableArray(newFormat.courseSlots);
            vm.activeFormatIndex = vm.courseType.courseFormats.length -1;
        }

        function createNewFormat() {
            var cf = datacontext.courseFormats.create({ courseTypeId: vm.courseType.id });
            cf.sortableSlots = createCourseSlotSortableArray(cf.courseSlots);
            vm.activeFormatIndex = vm.courseType.courseFormats.length - 1;
        }

        function deleteFormat(cf) {
            cf.courseSlots.forEach(function (el) {
                el.entityAspect.setDeleted();
            });
            cf.entityAspect.setDeleted();
        }

        function resetExampleTimes(cf) {
            var currentDay;
            var startIndx;
            if (!cf.exampleStart) {
                cf.exampleStart = new Date(0);
                cf.exampleStart.setHours(8);
            }
            cf.courseSlots.sort(common.sortOnPropertyName('order'));
            cf.courseSlots.forEach(function (cs) {
                if (cs.isActive) {
                    if (cs.day !== currentDay) {
                        startIndx = cf.exampleStart.getTime();
                        currentDay = cs.day;
                    }
                    startIndx += cs.minutesDuration * 60000;
                    cs.exampleFinish = new Date(startIndx);
                }
            });
        }

        function createCourseSlotSortableArray(slots) {
            var returnVar = [];
            var currentDay = -1;

            slots.forEach(function (cs) {
                if (cs.isActive) {
                    if (currentDay !== cs.day) {
                        returnVar.push({ isDayMarker: true, day: cs.day, locked: cs.day === 1, isActive: true });
                        currentDay = cs.day;
                    }
                    returnVar.push(cs);
                }
            });
            return returnVar;
        }

        function alterDayMarkers(cf) {
            if (cf.daysDuration <= 0) { return; }
            var currentDay = 0;
            var i;
            for (i = 0; i < cf.sortableSlots.length; i++) {
                if (cf.sortableSlots[i].isDayMarker && ++currentDay > cf.daysDuration) {
                    cf.sortableSlots.splice(i, 1);
                    --currentDay;
                    --i;
                } else {
                    cf.sortableSlots[i].day = currentDay;
                }
            }
            for (i = currentDay; i < cf.daysDuration; i++) {
                cf.sortableSlots.push({ isDayMarker: true, day: i+1, locked: false, isActive:true });
            }
            resetExampleTimes(cf);
        }
    }
})();
