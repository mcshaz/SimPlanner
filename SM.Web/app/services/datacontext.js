(function() {
    'use strict';

    angular.module('app')
        .service('datacontext', ['$rootScope', 'entityManagerFactory', 'repository', service]);

    function service ($rootScope, entityManagerFactory, repository) {
        var provider = entityManagerFactory.manager;

        this.ready = entityManagerFactory.ready;

        this.rejectChanges = provider.rejectChanges;
                
        this.save = function () {
            var saveOptions = new breeze.SaveOptions({ resourceName: 'savechanges' });

            return provider.saveChanges(null, saveOptions)
                .then(function(saveResult) {
                    $rootScope.$broadcast('saved', saveResult.entities);
                    logSuccess({ msg: 'Saved data', data: result, showToast: true });
                }).catch(saveFailed);
        };


        this.courses = repository.create(provider, 'CourseDto', 'Courses');
        this.departments = repository.create(provider, 'DepartmentDto', 'Departments', breeze.FetchStrategy.FromLocalCache);
        this.institutions = repository.create(provider, 'InstitutionDto', 'Institutions', breeze.FetchStrategy.FromLocalCache);
        this.courseTypes = repository.create(provider, 'CourseTypeDto', 'CourseTypes', breeze.FetchStrategy.FromLocalCache);
        this.professionalRoles = repository.create(provider, 'ProfessionalRoleDto', 'ProfessionalRoles', breeze.FetchStrategy.FromLocalCache);
        this.courseParticipants = repository.create(provider, 'CourseParticipantDto', 'CourseParticipants'/* 'Courses' */);

        function saveFailed(error) {
            var msg = config.appErrorPrefix + 'Save failed: ' +
                breeze.saveErrorMessageService.getErrorMessage(error);
            error.message = msg;
            logError(msg, error);
            throw error;
        }
    }
})();