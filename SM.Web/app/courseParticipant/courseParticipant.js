(function () {
    'use strict';
    var controllerId = 'courseParticipant';
    angular
        .module('app')
        .controller(controllerId, courseParticipantCtrl);

    courseParticipantCtrl.$inject = ['common', 'datacontext', 'breeze', '$scope','controller.abstract'];
    //changed $uibModalInstance to $scope to get the events

    function courseParticipantCtrl(common, datacontext, breeze, $scope, abstractController) {
        /* jshint validthis:true */
        var cp = this;
        
        abstractController.constructor.call(this, {
            controllerId: controllerId,
            watchedEntityNames: 'participant',
            $scope: $scope
        })
        
        cp.createCourseParticipant = createCourseParticipant;
        cp.createNewPerson = createNewPerson;
        cp.dialCode = '';
        cp.disableAdd = disableAdd;
        cp.departments = [];
        cp.isFaculty = $scope.isFaculty;
        cp.isNew = !$scope.courseParticipant;
        cp.isValidParticipantName = isValidParticipantName;
        cp.getPeople = getPeople;
        cp.nameLimit = 10;
        cp.participant = cp.isNew
            ? datacontext.participants.create(breeze.EntityState.Detached)
            : $scope.courseParticipant.participant;
            
        cp.onParticipantSelected = onParticipantSelected;
        cp.professionalRoles = [];

        activate();

        function activate() {
            datacontext.ready().then(function () {
                var promises = [datacontext.departments.all().then(function (data) {
                    cp.departments = data;
                    cp.dialCode = data[0].institution.country.dialCode;
                }),
                datacontext.professionalRoles.all().then(function (data) {
                    cp.professionalRoles = data;
                })];
                common.activateController(promises, controllerId)
                    .then(function () {
                        cp.log('Activated Course Participant Dialog');
                    });
                });
        }

        var _lastVal;
        var _lastLookup;
        function createCourseParticipant($event) {
            if (!validateSaveParticipant()) { return; }
            var forSave;
            if (cp.isNew) {
                forSave = $scope.course.addParticipant(cp.participant, { isFaculty: cp.isFaculty });
                datacontext.save([forSave, forSave.participant]).then(function () {
                    cp.log.success(cp.participant.fullName + ' added to course ' + (cp.isFaculty ? 'faculty' : 'participants'));
                    afterSave();
                });
            } else {
                forSave = $scope.courseParticipant;
                forSave.professionalRoleId = cp.participant.defaultProfessionalRoleId;
                forSave.departmentId = cp.participant.defaultDepartmentId;
                datacontext.save([forSave, forSave.participant]).then(function () {
                    cp.log.success(cp.participant.fullName + ' updated');
                    afterSave();
                });
                cp.isNew = true;
            }

            function afterSave() {
                cp.participant = datacontext.participants.create(breeze.EntityState.Detached);
                _lastLookup = _lastVal = null;
            }
        }

        function createNewPerson() {
            var match;
            if (match = _lastLookup.find(function (ld) {
                return ld.fullName.startsWith(name);
            })) {
                if (!confirm("Are you sure you want to create a new person rather than select " + match.fullName)) {
                    onParticipantSelected(match);
                    return;
                }
            }
            datacontext.addEntity(cp.participant);

            //todo check event fires
        }

        function disableAdd() {
            var ent = cp.participant.entityAspect;
            return ent.entityState.isDetached() || ent.hasValidationErrors;
        }

        function validateSaveParticipant() {
            var origName = cp.participant.entityAspect.originalValues.fullName;
            if (origName && origName !== cp.participant.fullName) {
                if (!confirm("Are you sure you wish to change the name of " + origName + " to " + cp.participant.fullName +"?\n\nYou should only click yes if:\n-This person's name was originally mispelt [sic]\n-You are adding something to differentiate from others with a similar name, e.g. John 'tall' Smith\n-The person has changed their name, e.g. after marriage")) {
                    cp.participant.entityAspect.rejectChanges();
                    /*
                    var oldVal = cp.participant;
                    cp.participant = datacontext.cloneItem(oldVal);
                    oldval.entityAspect.rejectChanges();
                    ["email", "alternateEmail", "phoneNumber"].foreach(function(propName){
                        if (cp.participant[propName] === oldVal[propName]) { cp.participant["propName"] = null; }
                    });
                    return false
                    */
                }
            }
            return true;
        }

        var notThisCourse = breeze.Predicate.create('courseParticipants', 'any', 'courseId', '==', $scope.course.id).not();
        var baseArgs = {
            orderBy: 'fullName',
            take: $scope.nameLimit,
            select: 'id,fullName,professionalRole.category,department.abbreviation'
        };

        var icons = {
            medical: 'stethoscope',
            tech: 'wrench',
            perfusionist: 'heart-o',
            other: 'question',
            paramedic: 'ambulance',
            nursing: 'hearbeat'
        }

        function getPeople(val) {
            if (_lastVal && _lastVal.length < baseArgs.take && val.toLowerCase().startsWith(_lastVal)) {
                val = val.toLowerCase();
                //I think the uib - typeahead handles either promises or objects, but seems cleaner to have a function return one or tother
                return _lastLookup.filter(function (el) { return el.fullName.toLowerCase().startsWith(val); });
            }
            baseArgs.where = breeze.Predicate.create('fullName', 'startsWith', val).and(notThisCourse);
            return datacontext.participants.find(baseArgs).then(function (results) {
                results.forEach(function (el) {
                    el.label = '<i class="fa fa-' + icons[el.professionalRole_Category.toLowerCase()] + '"></i> '
                            + el.fullName
                            + ' <small class="small">(' + el.department_Abbreviation + ' ' + common.toSeperateWords(el.professionalRole_Category) + ')</small>';
                });
                _lastVal = val.toLowerCase();
                return (_lastLookup = results);
            });
            delete baseArgs.where;
        }

        function onParticipantSelected(item) {
            datacontext.participants.fetchByKey(item.id).then(function (part) {
                cp.participant = part;
            });
        }

/* strap lookup
        function onParticipantSelected(event, value, index, elem) {
            if (elem.$id === "getFullName") {
                datacontext.participants.fetchByKey(_lastCacheLookup[index].id).then(function (part) {
                    cp.participant = part;
                });
            }
        }
*/

        function isValidParticipantName() {
            return cp.participant.entityAspect.validateProperty("fullName");
        }

    }
})();
