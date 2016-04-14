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
            watchedEntityNames: ['courseType', 'courseType.courseFormats', 'courseType.courseFormats.courseActivities'],
            $scope: $scope
        })
        var id = $routeParams.id;
        var isNew = id === 'new';

        vm.activeFormatIndex = -1;
        vm.activitySelected = activitySelected;
        vm.clone = clone;
        vm.courseType = {};

        vm.createSlot = createSlot;
        vm.removeSlot = removeSlot;
        vm.instructorCourses = [];
        vm.isScenarioChanged = isScenarioChanged;
        vm.editSlot = editSlot;
        vm.editChoices = editChoices;
        vm.emersionCategories = common.getEnumValues().emersion;
        vm.selectedSlot = null;
        var baseSave = vm.save;
        vm.save = saveOverride;
        vm.title = 'Course Format';

        vm.sortableOptions = {
            handle: '.handle',
            stop: function (e, ui) {
                // this callback has the changed model
                vm.courseFormat.courseSlots.forEach(function (el,indx) {
                    if (el.order !== indx) {
                        el.order = indx;
                    }
                });
            }
        };

        activate();

        function activate() {
            datacontext.ready().then(function () {
                var promises = [
                    datacontext.courseActivities.findServerIfCacheEmpty({
                        where: breeze.Predicate.create('courseTypeId', '==', id)
                    }),
                    datacontext.courseTypes.find({where: breeze.Predicate.create('instructorCourseId','==',null).and('id','!=',id)}).then(function (data) {
                        vm.instructorCourses = data;
                    })
                ];
                if (isNew) {
                    vm.courseType = datacontext.courseTypes.create();
                    datacontext.courseFormats.create({ courseType: vm.courseType });
                    vm.notifyViewModelPropChanged();
                } else {
                    //promises.push(datacontext.courseTypes.fetchByKey(id, { expand: 'courseFormats.courseSlots' }).then(function (data) { - if the courseFormats were not already loaded from the server
                    vm.courseType = datacontext.courseTypes.getByKey(id);
                    if (!vm.courseType) {
                        vm.log.warning({ msg: 'could not find courseType Id: ' + id });
                    }
                    promises.push(datacontext.courseSlots.findServerIfCacheEmpty({ withParameters: { courseFormatId: vm.courseType.courseFormats.map(function (el) { return el.id; }) } }).then(function (data) {
                        vm.courseType.courseFormats.forEach(function (el) {
                            el.courseSlots.sort(common.sortOnPropertyName('order'));
                        });
                        vm.activeFormatIndex = vm.courseType.courseFormats.findIndex(function (el) {
                            return el.id === $routeParams.formatId;
                        });

                        vm.notifyViewModelPropChanged();
                    }));
                }
                common.activateController(promises, controllerId)
                    .then(function () {
                        vm.log('Activated Course Format View');
                    });
            });
        }

        //logic is - can't delete slots after course has been run, as they may have associated tables such as participants
        //-altering the name of a slot - don't allow reassignment of activity, as course participants etc will be all mucked up
        //-only show typeahead if a new slot (otherwise simple input)
        //-if a new slot/activity and activity selected, delete new activity, replace with selected
        function activitySelected(activity) {
            removeActivity();
            if (!activity.courseSlots.length) {
                activity.isActive = true;
            }
            
            /*
            vm.selectedSlot.activity = datacontext.courseActivities.findInCache({
                withParameters: {
                    name: activityName,
                    courseTypeId: vm.courseFormat.courseTypeId
                }
            })[0];
            */
        }

        function createActivity() {
            vm.selectedSlot.activity = datacontext.courseActivities.create({
                courseTypeId: vm.courseType.Id
            });
        }

        function removeActivity() {
            var ca = vm.selectedSlot.activity
            if (ca && ca.entityAspect.entityState.isAdded()) {
                ca.entityAspect.setDeleted();
            }
            vm.selectedSlot.activity = null;
        }

        function getCourseActivityNames(name) {
            name = name.toLowerCase();
            var returnVar = [];
            if (vm.courseType.courseActivities) {
                vm.courseType.courseActivities.forEach(function (el) {
                    if (el.name.toLowerCase().indexOf(name) !== -1) {
                        returnVar.push(el.name);
                    }
                });
            }
            return returnVar;
        }

        function saveOverride($event) {
            //vm.log.debug($event);
            baseSave().then(function () {
                vm.selectedSlot = null;
            });
        }//;

        function updateCourseActivities() {
            if (vm.selectedSlot) {
                //rerun query in case we have just changed or edited
                var activities = datacontext.courseActivities.findInCache(courseActivitiesPredicate);
                mapCourseActivities(activities);
            }
        }

        function createSlot(courseFormat) {
            updateCourseActivities();
            courseFormat.selectedSlot = datacontext.courseSlots.create({
                courseFormatId: courseFormat.id,
                order: (courseFormat.courseActivities || []).length,
                isActive: true
            });
            courseFormat.selectedSlot.isScenario = false;
            createActivity();
        }

        function removeSlot(courseSlot) {
            if (courseSlot.courseActivity && courseSlot.courseActivity.entityAspect.entityState.isAdded()) {
                courseSlot.courseActivity.entityAspect.setDeleted();
            }
            if (courseSlot.entityAspect.entityState.isAdded()) {
                courseSlot.entityAspect.setDeleted();
            } else {
                courseSlot.isActive = false;
            }
        }

        function editChoices() {
            var modal = getModalInstance();
            modal.$scope.courseActivity = vm.selectedSlot.activity;
            modal.$promise.then(modal.show);
        }

        function editSlot(courseSlot) {
            updateCourseActivities();
            courseSlot.isScenario = courseSlot.activity === null;
        }

        function isScenarioChanged() {
            if (vm.selectedSlot.isScenario) {
                removeActivity();
            } else {
                createActivity();
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
                    controllerAs:'ar'

                });
                scope.asideInstance = _modalInstance;
            }
            return _modalInstance;
        }

        function clone(cf) {
            var newFormat = datacontext.cloneItem(cf, ['courseSlots']);
            newFormat.description += " - copy";
            //?notify I believe not but test
            activeFormatIndex = vm.courseType.courseFormats.length - 1;
        }
    }
})();
