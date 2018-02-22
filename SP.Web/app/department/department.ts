import angular from 'angular';

    var controllerId = 'department';
export default angular
        .module('app')
        .controller(controllerId, courseTypesCtrl).name;

    (courseTypesCtrl as any).$inject = ['common', 'datacontext', '$routeParams', 'controller.abstract', '$scope', 'tokenStorageService', '$location', 'USER_ROLES'];
    //changed $uibModalInstance to $scope to get the events

    function courseTypesCtrl(common, datacontext, $routeParams, abstractController, $scope, tokenStorageService, $location, USER_ROLES) {
        /* jshint validthis:true */
        var vm = this;
        abstractController.constructor.call(this, {
            controllerId: controllerId,
            watchedEntityNames: 'department',
            $scope: $scope
        });
        var id = $routeParams.id;
        var isNew = id === 'new';

        vm.department = {};
        //vm.institutionChanged = institutionChanged;
        vm.institutions = [];
        vm.isLoggedIn = tokenStorageService.isLoggedIn();
        vm.submit = submit;

        activate();

        function activate() {
            //current logic - users who are not logged in can only create. Might fall down if we wanted to give people a chance to edit
            //before approval
            var promises;
            if (isNew) {
                vm.department = datacontext.departments.create({ adminApproved: $routeParams.institutionId
                    ? userCanApprove($routeParams.institutionId)
                    : false });
                if (vm.isLoggedIn) {
                    //institution will have already be downloaded
                    vm.department.institutionId = $routeParams.institutionId;
                } else {
                    promises = [datacontext.institutions.fetchByKey($routeParams.institutionId).then(function(data){
                        vm.institutions = [data];
                        vm.department.institution = data;
                    })];
                }
            }
            if (vm.isLoggedIn) {
                promises = [datacontext.ready().then(function () {
                    datacontext.institutions.all().then(function (data) {
                        vm.institutions = data;
                    });
                    if (!isNew) {
                        datacontext.departments.fetchByKey(id).then(function (data) {
                            vm.department = data;
                            if (!vm.department) {
                                vm.log.warning('Could not find department id = ' + id);
                                return;
                            }
                        });
                    }
                })];
            }
            common.activateController(promises, controllerId)
                    .then(function() {
                        vm.notifyViewModelLoaded();
                        vm.log('Activated Department View');
                    });
        }

        function userCanApprove(institutionId){
            return tokenStorageService.isAuthorized(USER_ROLES.accessAllData) ||
                tokenStorageService.isAuthorized(USER_ROLES.accessInstitution) 
                    && datacontext.getByKey(tokenStorageService.getUserDepartmentId()).institutionId === institutionId;
        }

        function submit() {
            vm.save().then(function () {
                if (!vm.isLoggedIn) {
                    $location.path('/updateDetails/new').search({departmentId: vm.department.id });
                }
            });
        }
    }


