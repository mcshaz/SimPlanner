(function (undefined) {
    'use strict';

    var serviceId = 'users.abstract';
    angular.module('app').factory(serviceId,
        ['common', 'datacontext', 'breeze',  'uiGridConstants', 'selectOptionMaps', '$q', abstractUsers]);

    function abstractUsers(common, datacontext, breeze, uiGridConstants, selectOptionMaps, $q) {

        return {
            constructor: Ctor
        };

        function Ctor($scope) {
            var vm = this;
            var filterHeaderGrpTemplate = "<div class=\"ui-grid-filter-container\">" + "" +
                        "<select class=\"ui-grid-filter ui-grid-filter-select\" ng-model=\"col.filter.term\" ng-attr-placeholder=\"{{colFilter.placeholder || aria.defaultFilterLabel}}\" aria-label=\"{{colFilter.ariaLabel || ''}}\" ng-options=\"option.value as option.label group by option.group for option in col.filter.selectOptions track by option.value\">" +
                        "		<option value=\"\"></option>" +
                        "</select></div>";
            //"	<div role=\"button\" class=\"ui-grid-filter-button-select\" ng-click=\"removeFilter(colFilter, $index)\" ng-if=\"!colFilter.disableCancelFilterButton\" ng-disabled=\"colFilter.term === undefined || colFilter.term === null || colFilter.term === ''\" ng-show=\"colFilter.term !== undefined && colFilter.term != null\">" +
            //"		<i class=\"ui-grid-icon-cancel\" ui-grid-one-bind-aria-label=\"aria.removeFilter\">&nbsp;</i>" +
            //"	</div>";
            var defer = $q.defer();
            var filterPredicate = null;

            vm.additionalFilters = null;
            vm.filterChanged = filterChangedNotBound;

            vm.orderBy = 'fullName asc';
            vm.baseReady = defer.promise;

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
                        //cellTemplate: '<div class="ui-grid-cell-contents"><a href="#!/course/{{COL_FIELD}}" class="btn-link"><i class="fa fa-edit"></i><a></div>'
                        filter: { type: uiGridConstants.filter.CONTAINS }
                    },
                    {
                        name: 'Department', field: 'department',
                        filterHeaderTemplate: filterHeaderGrpTemplate,
                        filter: {}
                    },
                    {
                        name: 'Professional Role', field: 'professionalRole',
                        filterHeaderTemplate: filterHeaderGrpTemplate,
                        filter: {}
                    }
                ],
                onRegisterApi: function (gridApi) {
                    gridApi.core.on.sortChanged($scope, sortChanged);
                    gridApi.core.on.filterChanged($scope, filterChanged);
                    gridApi.pagination.on.paginationChanged($scope, updateData);
                    vm.filterChanged = filterChanged.bind(gridApi);
                }
            };
            vm.map = null;
            vm.updateData = updateData;
            activate();

            function activate() {
                datacontext.ready().then(function () {
                    $q.all([datacontext.institutions.all().then(function (data) {
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
                                    value: 'c:' + key,
                                    group: key
                                });
                            }
                        }
                        opts = opts.concat(data.map(function (pr) {
                            return { value: 'p:' + pr.id, label: pr.description, group: pr.category };
                        }));
                        vm.gridOptions.columnDefs[2].filter.selectOptions = opts;
                    })]).then(defer.resolve, defer.reject);
                });
            }

            function filterChangedNotBound() {
                throw new Error("filterChanged called before grid is ready");
            }

            function filterChanged() {
                var cols = this.grid.columns;
                var predicates = [];
                var createIdPredicate = function (propNames, id) {
                    if (id) {
                        var propName = propNames[id[0]];
                        predicates.push(breeze.Predicate.create(propName, '==', id.substring(2)));
                    }
                };
                var term = cols[0].filters[0].term;
                if (term) {
                    predicates.push(breeze.Predicate.create('fullName', 'contains', term));
                }

                createIdPredicate({ d: 'defaultDepartmentId', i: 'department.institutionId' }, cols[1].filters[0].term);

                createIdPredicate({ p: 'defaultProfessionalRoleId', c: 'professionalRole.category' }, cols[2].filters[0].term);

                if (vm.additionalFilters) {
                    predicates = predicates.concat(vm.additionalFilters(cols));
                }

                filterPredicate = predicates.length ? breeze.Predicate.and(predicates) : null;

                updateData();
            }

            function sortChanged(grid, sortColumns) {
                if (sortColumns.length === 0) {
                    orderBy = null;
                } else {
                    orderBy = sortColumns.map(function (el) {
                        var dir = el.sort.direction === "desc" ? " desc" : "";
                        var field;
                        switch (el.colDef.field) {
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
                    skip: (vm.gridOptions.paginationCurrentPage || 1) - 1
                };
                if (filterPredicate) { options.where = filterPredicate; }
                if (vm.expand) { options.expand = vm.expand; }
                if (vm.orderBy) { options.orderBy = vm.orderBy; }
                return datacontext.participants.find(options).then(function (data) {
                    vm.gridOptions.data = data.map(privateMap);
                    vm.gridOptions.totalItems = data.inlineCount;
                });
            }

            function privateMap(user) {
                var returnVar = {
                    id: user.id,
                    fullName: user.fullName,
                    email: user.email,
                    alternateEmail: user.alternateEmail,
                    department: user.department.abbreviation,
                    professionalRole: user.professionalRole.description
                };
                if (vm.map) {
                    angular.extend(returnVar, vm.map(user));
                }
                return returnVar;
            }
        }
    }
})();