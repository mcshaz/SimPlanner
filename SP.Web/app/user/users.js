(function () {
    'use strict';
    var controllerId = 'users';
    angular
        .module('app')
        .controller(controllerId, courseTypesCtrl);

    courseTypesCtrl.$inject = ['common', 'datacontext', 'breeze', '$scope', 'uiGridConstants', 'USER_ROLES'];
    //changed $uibModalInstance to $scope to get the events

    function courseTypesCtrl(common, datacontext, breeze, $scope, uiGridConstants, USER_ROLES) {
        /* jshint validthis:true */
        var vm = this;
        var filterPredicate = null;
        var filterHeaderGrpTemplate = "<div class=\"ui-grid-filter-container\">" + "" +
                                "<select class=\"ui-grid-filter ui-grid-filter-select\" ng-model=\"col.filter.term\" ng-attr-placeholder=\"{{colFilter.placeholder || aria.defaultFilterLabel}}\" aria-label=\"{{colFilter.ariaLabel || ''}}\" ng-options=\"option.value as option.label group by option.group for option in col.filter.selectOptions track by option.value\">" +
                                "		<option value=\"\"></option>" +
                                "</select></div>";
                                //"	<div role=\"button\" class=\"ui-grid-filter-button-select\" ng-click=\"removeFilter(colFilter, $index)\" ng-if=\"!colFilter.disableCancelFilterButton\" ng-disabled=\"colFilter.term === undefined || colFilter.term === null || colFilter.term === ''\" ng-show=\"colFilter.term !== undefined && colFilter.term != null\">" +
                                //"		<i class=\"ui-grid-icon-cancel\" ui-grid-one-bind-aria-label=\"aria.removeFilter\">&nbsp;</i>" +
                                //"	</div>";
        var orderBy = 'fullName asc';

        var _roles = getRoles();

        vm.gridOptions = {
            paginationPageSizes: [10, 25, 100],
            paginationPageSize: 10,
            enableFiltering: true,
            useExternalFiltering: true,
            useExternalPagination: true,
            useExternalSorting: true,
            columnDefs: [
                {
                    name: 'Name', field: 'fullName',
                    //cellTemplate: '<div class="ui-grid-cell-contents"><a href="#course/{{COL_FIELD}}" class="btn-link"><i class="fa fa-edit"></i><a></div>'
                    filter: { type: uiGridConstants.filter.CONTAINS }
                },
                {
                    name: 'Department', field: 'department',
                    filterHeaderTemplate: filterHeaderGrpTemplate,
                    filter: { }
                },
                {
                    name: 'Professional Role', field: 'professionalRole',
                    filterHeaderTemplate: filterHeaderGrpTemplate,
                    filter: {  }
                },
                {
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
                }
            ],
            onRegisterApi: function (gridApi) {
                gridApi.core.on.sortChanged($scope, sortChanged);
                gridApi.core.on.filterChanged($scope, filterChanged);
                gridApi.pagination.on.paginationChanged($scope, updateData);
            }
        };
        activate();

        function activate() {
            datacontext.ready().then(function () {
                common.activateController([
                    updateData(),
                    datacontext.institutions.all().then(function (data) {
                        var opts = [];
                        data.forEach(function (i) {
                            opts.push({ value: 'i:' + i.id, label: 'All ' + i.abbreviation, group: i.name });
                            i.departments.forEach(function (d) {
                                opts.push({ value: 'd:' + d.id, label: d.abbreviation, group: i.name });
                            });
                        });
                        vm.gridOptions.columnDefs[1].filter.selectOptions = opts;
                    }),
                    datacontext.professionalRoles.all().then(function (data) {
                        var opts = [];
                        for (var key in selectOptionMaps.roleSymbols) {
                            if (key) {
                                opts.push({
                                    label: 'All ' + key,
                                    value:'c:' + key,
                                    group: key
                                });
                            }
                        }
                        opts = opts.concat(data.map(function (pr) {
                            return { value: 'p:' + pr.id, label: pr.description, group: pr.category };
                        }));
                        vm.gridOptions.columnDefs[2].filter.selectOptions = opts;
                    })], controllerId);
            });
        }


        function filterChanged() {
            var grid = this.grid;
            var predicates = [];
            var createIdPredicate = function (propNames, id) {
                if (id) {
                    var propName = propNames[id[0]];
                    predicates.push(breeze.Predicate.create(propName, '==', id.substring(2)));
                }
            };
            var term = grid.columns[0].filters[0].term;
            if (term) {
                predicates.push(breeze.Predicate.create('fullName', 'contains', term));
            }

            createIdPredicate({ d: 'defaultDepartmentId', i: 'department.institutionId' }, grid.columns[1].filters[0].term);

            createIdPredicate({ p: 'defaultProfessionalRoleId', c: 'professionalRole.category' }, grid.columns[2].filters[0].term);

            term = grid.columns[3].filters[0].term;
            if (typeof term === 'boolean') {
                var pred = breeze.Predicate.create('roles', 'any', 'roleId', '==', USER_ROLES.siteAdmin);
                if (!term) { pred = pred.not(); }
                predicates.push(pred);
            }

            term = grid.columns[4].filters[0].term;
            if (term) {
                predicates.push(breeze.Predicate.create('roles', 'any', 'roleId', '==', term));
            }

            filterPredicate = predicates.length ? breeze.Predicate.and(predicates) : null;

            updateData();
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

        function sortChanged(grid, sortColumns) {
            if (sortColumns.length === 0) {
                orderBy = null;
            } else {
                orderBy = sortColumns.map(function (el) {
                    var dir = el.sort.direction === "desc" ? " desc" : "";
                    var field;
                    switch (el.colDef.field){
                        case 'department':
                            field = 'department.abbreviation';
                            break;
                        case 'professionalRole':
                            field = 'professionalRole.description';
                            break;
                            /*
                        //too dificult for now
                        case 'access':
                            field='roles.'
                            break;
                            */
                        default:
                            field = el.colDef.field;
                            break;
                    }
                    return field + dir;
                }).join(',');

            }
            updateData();
        }

        function updateData() {
            var options = {
                take: vm.gridOptions.paginationPageSize,
                inlineCount: true,
                skip: (vm.gridOptions.paginationCurrentPage || 1) - 1,
                expand: 'roles'
            };
            if (filterPredicate) { options.where = filterPredicate; }
            if (orderBy) { options.orderBy = orderBy; }
            return datacontext.participants.find(options).then(function (data) {
                vm.gridOptions.data = data.map(map);
                vm.gridOptions.totalItems = data.inlineCount;
            });
        }

        function map(user) {
            var ur = user.roles.find(function (ur) { return ur.roleId !== USER_ROLES.siteAdmin; });

            return {
                id: user.id,
                fullName: user.fullName,
                email: user.email,
                alternateEmail: user.alternateEmail,
                department: user.department.abbreviation,
                professionalRole: user.professionalRole.description,
                siteAdmin: user.roles.some(function (ur) { return ur.roleId === USER_ROLES.siteAdmin; }),
                access: ur
                    ? _roles.find(function (r) { return r.value === ur.roleId; }).label
                    : ''
            };
        }
    }

})();