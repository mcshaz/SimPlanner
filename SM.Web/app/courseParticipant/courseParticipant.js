(function () {
    'use strict';
    var controllerId = 'courseParticipant';
    angular
        .module('app')
        .controller(controllerId, courseParticipantCtrl);

    courseParticipantCtrl.$inject = ['common', 'datacontext', '$rootScope', 'breeze', '$uibModalInstance','courseParticipantIds'];

    function courseParticipantCtrl(common, datacontext, $rootScope, breeze, $modalInstance,courseParticipantIds) {
        /* jshint validthis:true */
        var vm = this;
        var log = common.logger.getLogFn(controllerId);
        var lastData = [];

        vm.canSave = false;
        vm.close = close;
        vm.courseParticipant = { participant: {fullName:''} };
        vm.dialCode = '';
        vm.departments = [];
        vm.getPeople = getPeople;
        vm.isNullCourseParticipant = courseParticipantIds.participantId.startsWith('new');
        vm.noResults;
        vm.createAddParticipant = createAddParticipant;
        vm.participantSelected = participantSelected;
        vm.professionalRoles = [];
        vm.save = save;

        $rootScope.$on('hasChanges', function () {
            vm.canSave = datacontext.courseParticipants.hasChanges();
        });
        activate();

        function activate() {
            datacontext.ready().then(function () {
                var promises = [datacontext.departments.all().then(function (data) {
                    vm.departments = data;
                    vm.dialCode = data[0].institution.country.dialCode;
                }),
                datacontext.professionalRoles.all().then(function (data) {
                    vm.professionalRoles = data;
                })];
                if (!vm.isNullCourseParticipant) {
                    promises.push(datacontext.courseParticipants.fetchByKey(courseParticipantIds, { expand: "participant" }).then(function (data) {
                        if (!data) {
                            log.warning({ msg: 'Could not find specified course participant', data: courseParticipantIds });
                            return;
                            //gotoCourses();
                        }
                        vm.courseParticipant = data;
                    }));
                } 
                common.activateController(promises, controllerId)
                    .then(function () {
                        log('Activated CourseParticipant Dialog');
                    });
                });
        }
        function close() {
            destroy();
            $modalInstance.close();
        }
        function save() {
            log({ msg: 'saved date: ' + vm.course.startTime });
        }//datacontext.save;

        var notThisCourse = breeze.Predicate.create('courseParticipants', 'any', 'courseId', '==', courseParticipantIds.courseId).not();

        function createAddParticipant() {
            var name = vm.courseParticipant.participant.fullName;
            var match;
            if (!vm.isNullCourseParticipant) {
                throw new Error('createAddParticipant called but courseParticipant is not empty')
            }
            if (match = lastData.find(function (el) { return el.startsWith(name); })
                    && !confirm("Are you SURE this person is not the existing " + match.description)) {
                return;
            }
            var participant = datacontext.participants.create(breeze.EntityState.Detached);
            participant.fullName =name;
            assignParticipant(participant.id);
        }

        var baseArgs = {
            orderBy: 'fullName',
            take: 10,
            select: 'id,fullName,professionalRole.category,department.abbreviation'
        };
        var lastVal;
        function getPeople(val) {
            if (lastVal && lastVal.length < baseArgs.take && val.toLowerCase().startsWith(lastVal)) {
                val = val.toLowerCase();
                //I think the uib - typeahead handles either promises or objects, but seems cleaner to have a function return one or tother
                return lastData.filter(function (el) { return el.fullName.toLowerCase().startsWith(val); });
            }
            baseArgs.where = breeze.Predicate.create('fullName', 'startsWith', val).and(notThisCourse);
            return datacontext.participants.find(baseArgs).then(function (results) {
                results.forEach(function (el) {
                    el.description = el.fullName + ' (' + el.department_Abbreviation + ' ' + common.toSeperateWords(el.professionalRole_Category) + ')';
                });
                lastVal = val.toLowerCase();
                return (lastData = results);
            });
            delete baseArgs.where;
        }


        function participantSelected(item) {
            //for now assuming we make courseParticipant equivalent of 'immutable'
            assignParticipant(item.id);
            datacontext.participants.fetchByKey(item.id).then(function (part) {
                vm.courseParticipant.participant = part;
            });
        }

        function assignParticipant(participantId) {
            if (vm.courseParticipant.entityAspect) {
                throw new Error('should not be calling assignParticipant if courseParticipant is already an entity');
            }
            vm.courseParticipant = datacontext.courseParticipants.create({courseId: courseParticipantIds.courseId, participantId: participantId}, breeze.EntityState.Detached);
            vm.courseParticipant.isFaculty = /faculty$/i.test(courseParticipantIds.participantId);
            
            vm.isNullCourseParticipant = false;
        }

        function destroy($event) {
            deleteAdded([vm.courseParticipant.participant, vm.courseParticipant], $event);
        }

        function deleteAdded(entArray, $event) {
            if (!entArray) { return; }
            if (!Array.isArray(entArray)) { entArray = [entArray]; }
            var hasChecked = !($event && angular.isFunction($event.preventDefault));
            for (var i = 0; i < entArray.length; i++) {
                if (entArray[i].entityAspect && ent.entityAspect.entityState.isDetached()) {
                    if (!hasChecked) {
                        if (!(hasChecked = confirm('Are you sure you want to discard changes?'))) {
                            $event.preventDefault();
                            return;
                        }
                    }
                    entArray[i].entityAspect.setDeleted();
                }
            }

        }
    }
})();
