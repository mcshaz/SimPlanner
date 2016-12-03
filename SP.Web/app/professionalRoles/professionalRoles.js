(function () {
    'use strict';
    var controllerId = 'professionalRoles';
    angular
        .module('app')
        .controller(controllerId, controller);

    controller.$inject = ['controller.abstract', '$routeParams', 'common', 'datacontext', '$scope', 'selectOptionMaps'];

    function controller(abstractController, $routeParams, common, datacontext, $scope, selectOptionMaps) {
        /* jshint validthis:true */
        var vm = this;
        abstractController.constructor.call(this, {
            controllerId: controllerId,
            $scope: $scope
        });
        var institutionId = $routeParams.id;

        vm.categories = [];

        vm.institution = {};
        vm.title = 'Course Type Professional Roles';

        activate();

        function activate() {
            datacontext.ready().then(function () {
                datacontext.ready().then(function () {
                    var allRoles;
                    var promises = [
                        datacontext.professionalRoles.all().then(function (data) {
                            allRoles = data;
                        }),
                        datacontext.institutions.fetchByKey(institutionId, { expand: 'professionalRoleInstitutions' }).then(function (data) {
                            vm.institution = data;
                            if (!vm.institution) {
                                vm.log.warning('Could not find institution id = ' + institutionId);
                                return;
                                //gotoCourses();
                            }
                        })];
                    common.activateController(promises, controllerId)
                        .then(function () {
                            mapRoles(allRoles, vm.institution.professionalRoleInstitutions.map(function (el) { return el.professionalRole; }));

                            vm.log('Activated Professional Role View');
                        });
                });
            });
        }

        function mapRoles(allRoles, departmentRoles) {
            var dict = {};
            var categoryNames = selectOptionMaps.getEnumValues().professionalCategory;
            var keyName;
            vm.categories = [];
            categoryNames.forEach(function (el) { dict[el] = { used: [], unused: [] }; });
            departmentRoles.forEach(function (pr) { dict[pr.category].used.push(pr); });
            allRoles.forEach(function (pr) {
                if (dict[pr.category].used.indexOf(pr) === -1) {
                    dict[pr.category].unused.push(pr);
                }
            });
            for (keyName in dict) {
                var returnVar = {
                    name: keyName,
                    used: dict[keyName].used.sort(common.sortOnPropertyName('order')),
                    unused: dict[keyName].unused.sort(common.sortOnPropertyName('order')),
                    newRoleName: '',
                    sortableOptions: {
                        connectWith: '.' + keyName
                    }
                };
                $scope.$watchCollection(function () { return this.used; }.bind(returnVar), addUsed);
                returnVar.addRole = addRole.bind(returnVar);
                vm.categories.push(returnVar);
            }
        }

        function addRole() {
            var self = this;
            var formEl = $scope.courseFormatForm[self.name];
            if (self.used.some(isRepeated) || self.unused.some(isRepeated)) {
                formEl.$setValidity('repeat', false);
            } else {
                formEl.$setValidity('repeat', true);
                var pr = datacontext.professionalRoles.create({ description: self.newRoleName, order:self.used.length, category: self.name });
                this.used.push(pr);
                this.newRoleName = '';
            }

            function isRepeated(el) {
                return common.alphaNumericEqual(el.description,self.newRoleName);
            }
        }
        
        function addUsed(newVals, oldVals) {
            oldVals.forEach(function (o) {
                if (!newVals.some(function (n) {
                    return n.professionalRoleInstitutions === o.professionalRoleInstitutions;
                })) {
                    var pri = datacontext.professionalRoleInstitutions.getByKey({
                        professionalRoleId: o.id,
                        institutionId: institutionId
                    });
                    if (pri) {
                        pri.entityAspect.setDeleted();
                    } else {
                        log.debug({
                            msg: 'courseTpeDpt looks to be deleted in viewmodel, but cannot be found by key',
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
                    return n.professionalRoleInstitutions === o.professionalRoleInstitutions;
                })) {
                    var key = {
                        professionalRoleId: n.id,
                        institutionId: institutionId
                    };
                    var pri = datacontext.professionalRoleInstitutions.getByKey(key, true);
                    if (pri) {
                        if (pri.entityAspect.entityState.isDeleted()) {
                            pri.entityAspect.setUnchanged();
                        } else if (!pri.entityAspect.entityState.isUnchanged()) {
                            vm.log.debug({ msg: 'professionalRoleInstitution found in cache & to be added but in state other than deleted', data: ctd });
                        }
                    } else {
                        datacontext.professionalRoleInstitutions.create(key);
                    }
                    vm.isEntityStateChanged = true;
                }
            });
        }
    }
})();
