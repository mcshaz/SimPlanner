(function () {
    'use strict';
    var controllerId = 'courseTypes';
    angular
        .module('app')
        .controller(controllerId, courseTypesCtrl);

    courseTypesCtrl.$inject = ['common', 'datacontext'];
    //changed $uibModalInstance to $scope to get the events

    function courseTypesCtrl(common, datacontext) {
        /* jshint validthis:true */
        var vm = this;
        vm.gridOptions = {
            paginationPageSizes: [10, 25, 100],
            paginationPageSize: 10,
            useExternalPagination: true,
            useExternalSorting: true,
            columnDefs: [
              { name: 'date' },
              { name: 'department' },
              { name: 'department' },
              { name: 'format' }
            ],
            onRegisterApi: function (gridApi) {
                $scope.gridApi = gridApi;
                $scope.gridApi.core.on.sortChanged($scope, function (grid, sortColumns) {
                    if (sortColumns.length == 0) {
                        paginationOptions.sort = null;
                    } else {
                        paginationOptions.sort = sortColumns[0].sort.direction;
                    }
                    getPage();
                });
                $scope.gridApi.core.on.filterChanged($scope, function () {
                    var grid = this.grid;
                    if (grid.columns[1].filters[0].term === 'male') {
                        $http.get('/data/100_male.json')
                        .success(function (data) {
                            $scope.gridOptions.data = data;
                        });
                    }
                });
                gridApi.pagination.on.paginationChanged($scope, function (newPage, pageSize) {
                    paginationOptions.pageNumber = newPage;
                    paginationOptions.pageSize = pageSize;
                    getPage();
                });
            }
        };
        activate();

        function activate() {
            datacontext.ready().then(function () {
                common.activateController([
                    datacontext.departments.all().then(function (data) {
                        vm.departments = data;
                        if (data.length === 1) {
                            vm.selectedDepartment = departments[0];
                        }
                    })], controllerId)
            });
        }
    }

})();