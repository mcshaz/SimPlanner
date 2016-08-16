(function () {
    'use strict';
    var controllerId = 'courses';
    angular
        .module('app')
        .controller(controllerId, courseTypesCtrl);

    courseTypesCtrl.$inject = ['common', 'datacontext', 'breeze', '$scope', 'uiGridConstants', 'uiGridGroupingConstants', 'tokenStorageService'];
    //changed $uibModalInstance to $scope to get the events

    function courseTypesCtrl(common, datacontext, breeze, $scope, uiGridConstants, uiGridGroupingConstants, tokenStorageService) {
        /* jshint validthis:true */
        var vm = this;
        var filterPredicate = null;
        //var orderBy = 'start desc';
        var filterHeaderGrpTemplate = "<div class=\"ui-grid-filter-container\">" + 
                                        "<select class=\"ui-grid-filter ui-grid-filter-select\" ng-model=\"col.filters[0].term\" ng-attr-placeholder=\"{{colFilter.placeholder || aria.defaultFilterLabel}}\" aria-label=\"{{colFilter.ariaLabel || ''}}\" ng-options=\"option.value as option.label group by option.group for option in col.filter.selectOptions\">" +
                                        "		<option value=\"\"></option>" +
                                        "</select></div>";
                                        //"	<div role=\"button\" class=\"ui-grid-filter-button-select\" ng-click=\"removeFilter(colFilter, $index)\" ng-if=\"!colFilter.disableCancelFilterButton\" ng-disabled=\"colFilter.term === undefined || colFilter.term === null || colFilter.term === ''\" ng-show=\"colFilter.term !== undefined && colFilter.term != null\">" +
                                        //"		<i class=\"ui-grid-icon-cancel\" ui-grid-one-bind-aria-label=\"aria.removeFilter\">&nbsp;</i>" +
                                        //"	</div>";
        var today = new Date();
        var startDate = new Date(today);
        var gridApi;
        var aggregateColumns;
        startDate.setFullYear(today.getFullYear() - 1);

        vm.aggregateChange = aggregateChange;
        vm.aggregateTypes = uiGridGroupingConstants.aggregation;
        vm.aggregateType = vm.aggregateTypes.SUM;
        vm.gridOptions = {
            paginationPageSizes: [10, 25, 100],
            paginationPageSize: 10,
            enableFiltering: true,
            useExternalFiltering: true,
            //useExternalPagination: true,
            //useExternalSorting: true,
            treeRowHeaderAlwaysVisible: false,
            columnDefs: [
                {
                    name: 'Department', field: 'department',
                    filterHeaderTemplate: filterHeaderGrpTemplate,
                    filter: { /* type: uiGridConstants.filter.SELECT */ },
                    grouping: { groupPriority: 0 }
                },
            {
                name: 'Course', field: 'course',
                filterHeaderTemplate: filterHeaderGrpTemplate,
                filter: { /* type: uiGridConstants.filter.SELECT */ },
                sort: { priority: 0, direction: 'asc' },
                grouping: { groupPriority: 1 }
            },
            {
                name: 'Outreaching Dpt', field: 'outreachingDepartment',
                filterHeaderTemplate: filterHeaderGrpTemplate,
                filter: { /* type: uiGridConstants.filter.SELECT */ },
            },
            {
                name: 'Hours', field: 'totalDurationMins', 
                treeAggregationType: vm.aggregateType,
                enableFiltering: false,
                enableSorting: false
            },
            {
                name: 'Participant No.', field: 'participantCount',
                treeAggregationType: vm.aggregateType,
                enableFiltering: false,
                enableSorting: false
            },
            {
                name: 'Faculty No.', field: 'facultyCount',
                treeAggregationType: vm.aggregateType,
                enableFiltering: false,
                enableSorting: false
            },
            {
                name: 'Date/Time', field: 'start', cellFilter: "date:'short'", type: 'date',
                filterHeaderTemplate: '<div class="ui-grid-filter-container">' +
                    '<input  class="ui-grid-filter-input" bs-datepicker type="text" ng-model="col.filters[0].term" placeholder="from" container="body"/>' +
                    '<input class="ui-grid-filter-input" bs-datepicker type="text" ng-model="col.filters[1].term" placeholder="to" container="body"/></div>',
                filters: [{term: startDate}, {term:today}]
                /*sort: {
                    direction: uiGridConstants.DESC,
                    priority: 0
                }*/
            },
              {
                  name: 'Edit', field:'id',
                  cellTemplate: '<div class="ui-grid-cell-contents" ng-if="!row.groupHeader"><a href="#course/{{COL_FIELD}}" class="btn-link"><i class="fa fa-edit"></i><a></div>',
                  enableFiltering: false,
                  enableSorting:false
              }
            ],
            onRegisterApi: function (gApi) {
                gridApi = gApi;
                //gridApi.core.on.sortChanged($scope, sortChanged);
                gridApi.core.on.filterChanged($scope, filterChanged);
                //gridApi.pagination.on.paginationChanged($scope, updateData);
            }
        };
        aggregateColumns = [3, 4, 5].map(function (i) { return vm.gridOptions.columnDefs[i].name; });
        activate();

        function activate() {
            datacontext.ready().then(function () {
                common.activateController([
                    datacontext.institutions.all().then(function (data) {
                        var opts = [];
                        data.forEach(function (i) {
                            opts.push({ value: 'i:' + i.id, label: 'All ' + i.abbreviation, group: i.name });
                            i.departments.forEach(function (d) {
                                opts.push({ value: 'd:' + d.id, label: d.abbreviation, group:i.name });
                            });
                        });
                        vm.gridOptions.columnDefs[0].filter.term = 'd:' + tokenStorageService.getUserDepartmentId();
                        vm.gridOptions.columnDefs[0].filter.selectOptions = opts,
                        //gridApi.grid.refresh();
                        vm.gridOptions.columnDefs[2].filter.selectOptions = opts;
                    }),
                    datacontext.courseTypes.all().then(function (data) {
                        var opts = [];
                        data.forEach(function (t) {
                            opts.push({ value: 't:' + t.id, label: 'ALL ' + t.abbreviation, group:t.description });
                            t.courseFormats.forEach(function (f) {
                                if (f.description) {
                                    opts.push({ value: 'f:' + f.id, label: f.description, group:t.description });
                                }
                            });
                        });
                        vm.gridOptions.columnDefs[1].filter.selectOptions = opts;
                    })]);
            });
        }


        function aggregateChange() {
            aggregateColumns.forEach(function (name) {
                gridApi.grouping.aggregateColumn(name, vm.aggregateType);
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
            var term = grid.columns[7].filters[0].term;
            if (term) {
                predicates.push(breeze.Predicate.create('start', '>=', term));
            }
            term = grid.columns[7].filters[1].term;
            if (term) {
                predicates.push(breeze.Predicate.create('start', '<=', term));
            }
            var dptHash = { d: 'departmentId', i: 'department.institutionId' };
            createIdPredicate(dptHash, grid.columns[1].filters[0].term);
            createIdPredicate(dptHash, grid.columns[3].filters[0].term);
            createIdPredicate({f: 'courseFormatId', t:'courseFormat.courseTypeId'}, grid.columns[2].filters[0].term);

            filterPredicate = predicates.length ? breeze.Predicate.and(predicates) : null;
                    
            return updateData();
        }
        /*
        function sortChanged(grid, sortColumns) {
            if (sortColumns.length === 0) {
                orderBy = null;
            } else {
                orderBy = sortColumns.map(function (el) {
                    var dir = el.sort.direction === "desc" ? " desc" : "";
                    switch (el.colDef.field) {
                        case "course":
                            return "courseFormat.courseType.abbreviation" + dir + "," +
                                "courseFormat.description" + dir;
                        case "department":
                            return "department.institution.abbreviation" + dir + "," +
                                "deparment.abbreviation" + dir;
                        case "outreachingDepartment":
                            return "outreachingDepartment.institution.abbreviation" + dir + "," +
                                "outreachingDeparment.abbreviation" + dir;
                        default:
                            throw new Error("sort on property not accounted for: " + el.colDef.field);
                            return el.colDef.field + dir;
                    }
                    
                }).join(',');

            }
            updateData();
        }
        */
        function updateData() {
            var options = {
                //take: vm.gridOptions.paginationPageSize,
                inlineCount: true,
                //skip: (vm.gridOptions.paginationCurrentPage || 1)-1
            };
            if (filterPredicate) { options.where = filterPredicate; }
            //if (orderBy) { options.orderBy = orderBy;}
            return datacontext.courses.find(options).then(function (data) {
                vm.gridOptions.data = data.map(function (el) {
                    return {
                        department: el.department.institutionDptDescriptor,
                        course: el.courseFormat.typeFormatDescriptor,
                        outreachingDepartment: el.outreachingDepartment 
                            ?el.outreachingDepartment.institutionDptDescriptor
                            :'',
                        totalDurationMins: el.totalDurationMins,
                        participantCount: el.participantCount,
                        facultyCount: el.facultyCount,
                        start: el.start,
                        id: el.id
                    };
                });
                vm.gridOptions.totalItems = data.inlineCount;
            });
        }
    }

})();