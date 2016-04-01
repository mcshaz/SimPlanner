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

        vm.clone = clone;
        vm.courseFormat = {};
        vm.courseActivities = [];
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
                        datacontext.courseActivities.find({
                            where: breeze.Predicate.create('courseTypeId', '==', $routeParams.cloneId || $routeParams.courseTypeId)
                        }).then(courseActivitiesReady)
                    ];
                    if ($routeParams.cloneId){
                        promises.push(datacontext.courseFormats.fetchByKey($routeParams.cloneId, { expand: 'courseSlots' }).then(function (data) {
                            vm.courseFormat = datacontext.cloneItem(data);
                            vm.courseFormat.desctiption += " - copy"
                        }));
                    } else {
                        vm.courseFormat = datacontext.courseFormats.create();
                        vm.courseFormat.courseTypeId = $routeParams.courseTypeId;
                    }
                } else {
                    promises = [
                        datacontext.courseActivities.findServerIfCacheEmpty({
                            where: breeze.Predicate.create('courseType.courseFormats', 'any', 'id', '==', id)
                         }).then(courseActivitiesReady),
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

        function courseActivitiesReady(data) {
            vm.courseActivities = data;
        }

        function save($event) {
            //vm.log.debug($event);
            datacontext.save();
        }//;


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
