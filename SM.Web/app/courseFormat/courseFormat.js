(function () {
    'use strict';
    var controllerId = 'courseFormat';
    angular
        .module('app')
        .controller(controllerId, controller);

    controller.$inject = ['controller.abstract', '$routeParams', 'common', 'datacontext', '$scope', 'breeze', '$location'];

    function controller(abstractController, $routeParams, common, datacontext, $scope, breeze, $location) {
        /* jshint validthis:true */
        var vm = this;
        abstractController.constructor.call(this, {
            controllerId: controllerId,
            watchedEntityName: 'courseFormat',
            $scope: $scope
        })
        var id = $routeParams.id;
        var isNew = id == 'new';
        var courseActivitiesPredicate = {
            where: breeze.Predicate.create('courseType.courseFormats', 'any', 'id', '==', id)
        }

        vm.activitySelected = activitySelected;
        vm.clone = clone;
        vm.courseFormat = {};
        vm.courseActivities = [];
        vm.createSlot = createSlot;
        vm.deleteSlot = deleteSlot;
        vm.isScenarioChanged = isScenarioChanged;
        vm.saveSlot = saveSlot;
        vm.editSlot = editSlot;
        vm.editChoices = editChoices;
        vm.selectedSlot = null;

        vm.save = save;
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
                var promises;
                if (isNew) {
                    promises = [
                        datacontext.courseActivities.findServerIfCacheEmpty({
                            where: breeze.Predicate.create('courseTypeId', '==', $routeParams.cloneId || $routeParams.courseTypeId)
                        }).then(mapCourseActivities)
                    ];
                    vm.courseFormat = datacontext.courseFormats.create();
                    vm.courseFormat.courseTypeId = $routeParams.courseTypeId;
                } else {
                    promises = [
                        datacontext.courseActivities.findServerIfCacheEmpty(courseActivitiesPredicate).then(mapCourseActivities),
                         datacontext.courseFormats.fetchByKey(id, { expand: 'courseSlots,courseType' }).then(function (data) {
                             if (!data) {
                                 vm.log.warning({msg:'could not find courseFormat Id: ' + id})
                             }
                             vm.courseFormat = data;
                             data.courseSlots = data.courseSlots.sort(function (a, b) {
                                 if (a.order > b.order) {
                                     return 1;
                                 }
                                 if (a.order < b.order) {
                                     return -1;
                                 }
                                 // a must be equal to b
                                 return 0;
                             });
                         })
                    ];
                }
                common.activateController(promises, controllerId)
                    .then(function () {
                        vm.log('Activated Course Format View');
                    });
            });
        }

        function activitySelected(activityName) {
            removeActivity();
            vm.selectedSlot.activity = datacontext.courseActivities.findInCache()
                .find(function (el) {
                    return el.name === activityName && el.courseTypeId === vm.courseFormat.courseTypeId;
                });
            /*
            vm.selectedSlot.activity = datacontext.courseActivities.findInCache({
                withParameters: {
                    name: activityName,
                    courseTypeId: vm.courseFormat.courseTypeId
                }
            })[0];
            */
        }

        function mapCourseActivities(data) {
            //vm.courseActivities = data;
            vm.courseActivities = data.map(function(el) { return el.name; });
        }

        function createActivity() {
            vm.selectedSlot.activity = datacontext.courseActivities.create({
                courseTypeId: vm.courseFormat.courseTypeId
            });
        }

        function removeActivity() {
            var ca = vm.selectedSlot.activity
            if (ca && ca.entityAspect.entityState.isAdded()) {
                ca.entityAspect.setDeleted();
            }
            vm.selectedSlot.activity = null;
        }

        function save($event) {
            //vm.log.debug($event);
            datacontext.save().then(function () { vm.selectedSlot = null; });
        }//;

        function updateCourseActivities() {
            if (vm.selectedSlot) {
                //rerun query in case we have just changed or edited
                var activities = datacontext.courseActivities.findInCache(courseActivitiesPredicate);
                mapCourseActivities(activities);
            }
        }

        function createSlot() {
            updateCourseActivities();
            vm.selectedSlot = datacontext.courseSlots.create({
                courseFormatId: vm.courseFormat.id,
                order: (vm.courseActivities || []).length,
                day:1 //**todo** remove this & allow for multi day courses
            });
            vm.selectedSlot.isScenario = false;
            createActivity();

        }

        function deleteSlot(courseSlot) {
            courseSlot.entityAspect.setDeleted();
        }

        function editChoices() {

        }

        function editSlot(courseSlot) {
            updateCourseActivities();
            courseSlot.isScenario = courseSlot.activity === null;
            vm.selectedSlot = courseSlot;
        }

        function isScenarioChanged() {
            if (vm.selectedSlot.isScenario) {
                removeActivity();
            } else {
                createActivity();
            }
        }

        function saveSlot() {
            datacontext.save([vm.selectedSlot]);
            vm.selectedSlot = null;
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

                });
                scope.asideInstance = _modalInstance;
                scope.course = vm.course;
            }
            return _modalInstance;
        }

        function clone() {
            var currentLocation = $location.path();
            var slashPos = currentLocation.lastIndexOf('/');

            var newFormat = datacontext.cloneItem(vm.courseFormat, ['courseSlots']);
            newFormat.description += " - copy";
            //datacontext.save();
            $location.path(currentLocation.substr(0, slashPos + 1) + newFormat.id);
        }
    }
})();
