(function () {
    'use strict';
    var controllerId = 'scenarioRoles';
    angular
        .module('app')
        .controller(controllerId, controller);

    controller.$inject = ['controller.abstract', '$routeParams', 'common', 'datacontext', '$scope'];

    function controller(abstractController, $routeParams, common, datacontext, $scope, breeze, $aside) {
        /* jshint validthis:true */
        var vm = this;
        abstractController.constructor.call(this, {
            controllerId: controllerId,
            $scope: $scope,
        })
        var courseTypeId = $routeParams.id;

        vm.addRole = addRole;
        vm.courseType = {};
        vm.newRoleName = '';
        vm.unused = [];
        vm.used = []
        vm.sortableOptions = {
            connectWith:'.faculty'
        }

        vm.title = 'Course Type Scenario Roles';

        activate();

        function activate() {
            datacontext.ready().then(function () {
                datacontext.ready().then(function () {
                    var allRoles;
                    var promises = [
                        datacontext.facultyScenarioRoles.all().then(function (data) {
                            allRoles = data;
                        }),
                        datacontext.courseTypes.fetchByKey(courseTypeId, { expand: 'courseTypeScenarioRoles' }).then(function (data) {
                            vm.courseType = data;
                            vm.used = data.courseTypeScenarioRoles.map(function (el) {
                                return el.facultyScenarioRole;
                            });
                            if (!vm.courseType) {
                                vm.log.warning('Could not find courseType id = ' + courseTypeId);
                                return;
                                //gotoCourses();
                            }
                        })];
                    common.activateController(promises, controllerId)
                        .then(function () {
                            mapRoles(allRoles);
                            vm.log('Activated Scenario Role View');
                        });
                });
            });
        }

        function mapRoles(allRoles) {

            vm.unused = allRoles.filter(function (el) {
                return vm.used.indexOf(el) === -1;
            });

            $scope.$watchCollection(function () { return vm.used; }, addUsed);
        }

        function addRole() {
            var formEl = $scope.scenarioRolesForm.newRoleName;
            if (vm.used.some(isRepeated) || vm.unused.some(isRepeated)) {
                formEl.$setValidity('repeat', false);
            } else {
                formEl.$setValidity('repeat', true);
                var pr = datacontext.facultyScenarioRoles.create({ description: vm.newRoleName, order: vm.used.length });
                vm.used.push(pr);
                vm.newRoleName = '';
            }

            function isRepeated(el) {
                return common.alphaNumericEqual(el.description,vm.newRoleName);
            }
        }
        
        function addUsed(newVals, oldVals) {
            oldVals.forEach(function (o) {
                if (!newVals.some(function (n) {
                    return n.courseTypeScenarioRoles === o.courseTypeScenarioRoles;
                })) {
                    var ctsr = datacontext.courseTypeScenarioRoles.getByKey({
                        facultyScenarioRoleId: o.id,
                        courseTypeId: courseTypeId
                    });
                    if (ctsr) {
                        ctsr.entityAspect.setDeleted();
                    } else {
                        log.debug({
                            msg: 'courseTypeScenarioRole looks to be deleted in viewmodel, but cannot be found by key',
                            data: {
                                oldVals: oldVals,
                                newVals: newVals
                            }
                        });
                    }
                    vm.isEntityStateChanged = true;
                }
            });

            newVals.forEach(function (n, i) {
                if (n.order !== i) {
                    n.order = i;
                    vm.isEntityStateChanged = true;
                }
                if (!oldVals.some(function (o) {
                    return n.courseTypeScenarioRoles === o.courseTypeScenarioRoles;
                })) {
                    var key = {
                        facultyScenarioRoleId: n.id,
                        courseTypeId: courseTypeId
                    };
                    var ctsr = datacontext.courseTypeScenarioRoles.getByKey(key, true);
                    if (ctsr) {
                        if (ctsr.entityAspect.entityState.isDeleted()) {
                            ctsr.entityAspect.setUnchanged();
                        } else if (!ctsr.entityAspect.entityState.isUnchanged()) {
                            vm.log.debug({ msg: 'courseTypeScenarioRole found in cache & to be added but in state other than deleted', data: ctd });
                        }
                    } else {
                        datacontext.courseTypeScenarioRoles.create(key);
                    }
                    vm.isEntityStateChanged = true;
                }
            });
        }
    }
})();
