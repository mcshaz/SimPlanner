(function () {
    'use strict';
    var controllerId = 'users';
    angular
        .module('app')
        .controller(controllerId, courseTypesCtrl);

    courseTypesCtrl.$inject = ['common','$scope','users.abstract', 'uiGridConstants', 'USER_ROLES', 'breeze'];
    //changed $uibModalInstance to $scope to get the events

    function courseTypesCtrl(common, $scope, abstractUserDetails, uiGridConstants, USER_ROLES, breeze) {
        /* jshint validthis:true */
        var vm = this;

        abstractUserDetails.constructor.call(this, $scope);
        var _roles = getRoles();

        vm.expand = 'roles';
        vm.additionalFilters = filterChanged;
        vm.gridOptions.columnDefs.push({
            name: 'Site Admin', field: 'siteAdmin',
                type: 'boolean',
                cellTemplate: '<input type="checkbox" disabled ng-checked="COL_FIELD">',
                filter: {
                    type: uiGridConstants.filter.SELECT,
                    selectOptions: [
                        { label: 'Yes', value: true },
                        { label: 'No', value: false }
                    ]
                },
                enableSorting: false
            },
            {
                name: 'Access', field: 'access',
                filter: {
                    type: uiGridConstants.filter.SELECT,
                    selectOptions: _roles
                },
                enableSorting: false
            },
            {
                name: 'Edit', field: 'id',
                cellTemplate: '<div class="ui-grid-cell-contents"><a href="#/user/{{COL_FIELD}}" class="btn-link"><i class="fa fa-edit"></i><a></div>',
                enableFiltering: false,
                enableSorting: false
            });
        vm.map = map;
        activate();

        function activate() {
            common.activateController([vm.baseReady, vm.updateData()], controllerId);
        }

        function filterChanged(cols) {
            var predicates = [];
            var term = cols[3].filters[0].term;

            if (typeof term === 'boolean') {
                var pred = breeze.Predicate.create('roles', 'any', 'roleId', '==', USER_ROLES.siteAdmin);
                if (!term) { pred = pred.not(); }
                predicates.push(pred);
            }

            term = cols[4].filters[0].term;
            if (term) {
                predicates.push(breeze.Predicate.create('roles', 'any', 'roleId', '==', term));
            }

            return predicates;
        }

        function getRoles() {
            var roles=[];
            var skip = ['all', 'siteAdmin'];
            var _access = /^access/i;
            var _Separate = /(\B[A-Z])/g;
            
            for (var key in USER_ROLES){
                if (key && skip.indexOf(key) === -1) {
                    roles.push({
                        label: key.replace(_access, '').replace(_Separate, ' $1'),
                        value: USER_ROLES[key]
                    });
                }
            }
            return roles;
        }

        function map(user) {
            var ur = user.roles.find(function (ur) { return ur.roleId !== USER_ROLES.siteAdmin; });

            return {
                siteAdmin: user.roles.some(function (ur) { return ur.roleId === USER_ROLES.siteAdmin; }),
                access: ur
                    ? _roles.find(function (r) { return r.value === ur.roleId; }).label
                    : ''
            };
        }
    }
})();