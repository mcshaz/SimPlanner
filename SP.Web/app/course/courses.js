"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
var controllerId = 'courses';
angular_1.default
    .module('app')
    .controller(controllerId, courseTypesCtrl);
courseTypesCtrl.$inject = ['common', 'datacontext', 'breeze', '$scope', 'uiGridGroupingConstants', 'tokenStorageService'];
function courseTypesCtrl(common, datacontext, breeze, $scope, uiGridGroupingConstants, tokenStorageService) {
    var vm = this;
    var filterPredicate = null;
    var filterHeaderGrpTemplate = "<div class=\"ui-grid-filter-container\">" +
        "<select class=\"ui-grid-filter ui-grid-filter-select\" ng-model=\"col.filters[0].term\" ng-attr-placeholder=\"{{colFilter.placeholder || aria.defaultFilterLabel}}\" aria-label=\"{{colFilter.ariaLabel || ''}}\" ng-options=\"option.value as option.label group by option.group for option in col.filter.selectOptions\">" +
        "		<option value=\"\"></option>" +
        "</select></div>";
    var today = new Date();
    var startDate = new Date(today);
    var gridApi;
    startDate.setFullYear(today.getFullYear() - 1);
    vm.aggregateType = uiGridGroupingConstants.aggregation.SUM;
    vm.isExpanded = false;
    vm.expandAll = expandAll;
    vm.reverseGrouping = reverseGrouping;
    vm.gridOptions = {
        enableFiltering: true,
        useExternalFiltering: true,
        columnDefs: [
            {
                name: 'Department', field: 'department',
                filterHeaderTemplate: filterHeaderGrpTemplate,
                filter: {},
                sort: { priority: 0, direction: 'asc' },
                grouping: { groupPriority: 0 },
                groupingShowAggregationMenu: false
            },
            {
                name: 'Course', field: 'course',
                filterHeaderTemplate: filterHeaderGrpTemplate,
                filter: {},
                sort: { priority: 1, direction: 'asc' },
                grouping: { groupPriority: 1 },
                groupingShowAggregationMenu: false
            },
            {
                name: 'Outreaching Dpt', field: 'outreachingDepartment',
                filterHeaderTemplate: filterHeaderGrpTemplate,
                filter: {}
            },
            {
                name: 'Hr:Min', field: 'durationMins',
                treeAggregationType: vm.aggregateType,
                enableFiltering: false,
                enableSorting: false,
                cellFilter: "timeFilter:'m':'h:mm'",
                groupingShowGroupingMenu: false
            },
            {
                name: 'Participant No.', field: 'participantCount',
                treeAggregationType: vm.aggregateType,
                enableFiltering: false,
                enableSorting: false,
                groupingShowGroupingMenu: false
            },
            {
                name: 'Faculty No.', field: 'facultyCount',
                treeAggregationType: vm.aggregateType,
                enableFiltering: false,
                enableSorting: false,
                groupingShowGroupingMenu: false
            },
            {
                name: 'Date/Time', field: 'start', cellFilter: "date:'short'", type: 'date',
                filterHeaderTemplate: '<div class="ui-grid-filter-container">' +
                    '<input  class="ui-grid-filter-input" bs-datepicker type="text" ng-model="col.filters[0].term" placeholder="from" container="body"/>' +
                    '<input class="ui-grid-filter-input" bs-datepicker type="text" ng-model="col.filters[1].term" placeholder="to" container="body"/></div>',
                filters: [{ term: startDate }, { term: today }]
            },
            {
                name: 'Edit', field: 'id',
                cellTemplate: '<div class="ui-grid-cell-contents" ng-if="!row.groupHeader"><a href="#!/course/{{COL_FIELD}}" class="btn-link"><i class="fa fa-edit"></i><a></div>',
                enableFiltering: false,
                enableSorting: false
            }
        ],
        onRegisterApi: function (gApi) {
            gridApi = gApi;
            gridApi.core.on.filterChanged($scope, filterChanged);
            if (gridApi.grid.options.columnDefs.some(function (cd) { return !!(cd.filter && cd.filter.term); })) {
                setTimeout(filterChanged.bind(gridApi), 1);
            }
        }
    };
    activate();
    function activate() {
        datacontext.ready().then(function () {
            var userDpt = tokenStorageService.getUserDepartmentId();
            common.activateController([
                datacontext.institutions.all().then(function (data) {
                    var opts = [];
                    data.forEach(function (i) {
                        opts.push({ value: 'i:' + i.id, label: 'All ' + i.abbreviation, group: i.name });
                        i.departments.forEach(function (d) {
                            opts.push({ value: 'd:' + d.id, label: d.abbreviation, group: i.name });
                        });
                    });
                    vm.gridOptions.columnDefs[0].filter.term = 'd:' + userDpt;
                    vm.gridOptions.columnDefs[0].filter.selectOptions = opts,
                        vm.gridOptions.columnDefs[2].filter.selectOptions = opts;
                }),
                datacontext.courseTypes.all().then(function (data) {
                    var opts = [];
                    data.forEach(function (t) {
                        opts.push({ value: 't:' + t.id, label: 'ALL ' + t.abbreviation, group: t.description });
                        t.courseFormats.forEach(function (f) {
                            if (f.description) {
                                opts.push({ value: 'f:' + f.id, label: f.description, group: t.description });
                            }
                        });
                    });
                    vm.gridOptions.columnDefs[1].filter.selectOptions = opts;
                })
            ]);
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
            predicates.push(breeze.Predicate.create('startFacultyUtc', '>=', term));
        }
        term = grid.columns[7].filters[1].term;
        if (term) {
            predicates.push(breeze.Predicate.create('startFacultyUtc', '<=', term));
        }
        var dptHash = { d: 'departmentId', i: 'department.institutionId' };
        createIdPredicate(dptHash, grid.columns[1].filters[0].term);
        createIdPredicate(dptHash, grid.columns[3].filters[0].term);
        createIdPredicate({ f: 'courseFormatId', t: 'courseFormat.courseTypeId' }, grid.columns[2].filters[0].term);
        filterPredicate = predicates.length ? breeze.Predicate.and(predicates) : null;
        return updateData();
    }
    function expandAll() {
        gridApi.treeBase.expandAllRows();
    }
    function reverseGrouping() {
        var grouping = gridApi.grouping;
        grouping.clearGrouping();
        grouping.groupColumn('Course');
    }
    function updateData() {
        var options = {
            expand: 'courseParticipants'
        };
        if (filterPredicate) {
            options.where = filterPredicate;
        }
        return datacontext.courses.find(options).then(function (data) {
            vm.gridOptions.data = data.map(function (el) {
                return {
                    department: el.department.institutionDptDescriptor,
                    course: el.courseFormat.typeFormatDescriptor,
                    outreachingDepartment: el.outreachingDepartment
                        ? el.outreachingDepartment.institutionDptDescriptor
                        : '',
                    durationMins: el.totalDurationMins,
                    participantCount: el.participantCount,
                    facultyCount: el.facultyCount,
                    start: el.startFacultyUtc,
                    id: el.id
                };
            });
        });
    }
}
//# sourceMappingURL=courses.js.map