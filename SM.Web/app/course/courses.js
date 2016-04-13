(function () {
    'use strict';
    var controllerId = 'courses';
    angular
        .module('app')
        .controller(controllerId, courseTypesCtrl);

    courseTypesCtrl.$inject = ['common', 'datacontext', 'breeze', '$scope', 'uiGridConstants'];
    //changed $uibModalInstance to $scope to get the events

    function courseTypesCtrl(common, datacontext, breeze, $scope, uiGridConstants) {
        /* jshint validthis:true */
        var vm = this;
        var filterPredicate = null;
        var orderBy = 'startTime desc';
        vm.gridOptions = {
            paginationPageSizes: [10, 25, 100],
            paginationPageSize: 10,
            enableFiltering: true,
            useExternalFiltering: true,
            useExternalPagination: true,
            useExternalSorting: true,
            columnDefs: [
              {
                  name: 'Date/Time', field: 'startTime', cellFilter: "date:'short'", type: 'date',
                  filterHeaderTemplate: '<div class="ui-grid-filter-container">' +
                    '<input  class="ui-grid-filter-input" bs-datepicker type="text" ng-model="col.filters[0].term" placeholder="from" container="body"/>' +
                    '<input class="ui-grid-filter-input" bs-datepicker type="text" ng-model="col.filters[1].term" placeholder="to" container="body"/></div>',
                  filters: [{}, {}],
                  /*sort: {
                      direction: uiGridConstants.DESC,
                      priority: 0
                  }*/
              },
              {
                  name: 'Department', field: 'department.abbreviation',
                  filter: { type: uiGridConstants.filter.SELECT },
              },
              {
                  name: 'Outreaching Department', field: 'outreachingDepartment.abbreviation',
                  filter: { type: uiGridConstants.filter.SELECT },
              },
              {
                  name: 'Course Type', field: 'courseFormat.typeFormatDescriptor',
                  filter: { type: uiGridConstants.filter.SELECT }
              },
              {
                  name: 'Edit', field:'id',
                  cellTemplate: '<div class="ui-grid-cell-contents"><a href="#course/{{COL_FIELD}}" class="btn-link"><i class="fa fa-edit"></i><a></div>',
                  enableFiltering: false,
                  enableSorting:false
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
                    datacontext.departments.all().then(function (data) {
                        var opts = data.map(function (el) {
                            return { value: el.id, label: el.abbreviation }
                        });
                        vm.gridOptions.columnDefs[1].filter.selectOptions = opts;
                        vm.gridOptions.columnDefs[2].filter.selectOptions = opts;
                    }),
                    datacontext.courseFormats.all().then(function (data) {
                        var opts = data.map(function (el) {
                            return { value: el.id, label: el.typeFormatDescriptor }
                        });
                        vm.gridOptions.columnDefs[3].filter.selectOptions = opts;
                    })], controllerId)
            });
        }

        function filterChanged() {
            var grid = this.grid;
            var predicates = [];
            var createIdPredicate = function (propName, id) {
                if (id) {
                    predicates.push(breeze.Predicate.create(propName, '==', id));
                }
            }
            var term = grid.columns[0].filters[0].term;
            if (term) {
                predicates.push(breeze.Predicate.create('startTime', '>=', term));
            }
            term = grid.columns[0].filters[1].term;
            if (term) {
                predicates.push(breeze.Predicate.create('startTime', '<=', term));
            }
            createIdPredicate('departmentId', grid.columns[1].filters[0].term);
            createIdPredicate('outreachingDepartmentId', grid.columns[2].filters[0].term);
            createIdPredicate('courseFormatId', grid.columns[3].filters[0].term);

            filterPredicate = predicates.length ? breeze.Predicate.and(predicates) : null;
                    
            updateData();
        }

        function sortChanged(grid, sortColumns) {
            if (sortColumns.length == 0) {
                orderBy = null;
            } else {
                orderBy = sortColumns.map(function (el) {
                    var dir = el.sort.direction === "desc" ? " desc" : "";
                    if (el.colDef.field === "courseFormat.typeFormatDescriptor") {
                        return "courseFormat.courseType.abbrev" + dir + "," +
                            "courseFormat.description" + dir;
                    }
                    return el.colDef.field + dir;
                }).join(',');

            }
            updateData();
        }

        function updateData() {
            var options = {
                take: vm.gridOptions.paginationPageSize,
                inlineCount: true,
                skip: (vm.gridOptions.paginationCurrentPage || 1)-1
            };
            if (filterPredicate) { options.where = filterPredicate; }
            if (orderBy) { options.orderBy = orderBy;}
            return datacontext.courses.find(options).then(function (data) {
                vm.gridOptions.data = data;
                vm.gridOptions.totalItems = data.inlineCount;
            });
        }
    }

})();