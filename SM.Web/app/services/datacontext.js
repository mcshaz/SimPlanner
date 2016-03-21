(function() {
    'use strict';
    var serviceId = 'datacontext';
    angular.module('app')
        .service(serviceId, ['$rootScope', 'entityManagerFactory', 'repository', 'common', 'config',service]);

    function service ($rootScope, entityManagerFactory, repository, common, config) {
        var provider = entityManagerFactory.manager;
        var self = this;
        var log = common.logger.getLogFn(serviceId);

        self.ready = entityManagerFactory.ready;

        self.rejectChanges = provider.rejectChanges;
                
        self.save = function (/*entitiesToSave*/) {
            var saveOptions = new breeze.SaveOptions({ resourceName: 'savechanges' });
            var entititiesToSave;
            switch(arguments.length) {
                case 0:
                    entititiesToSave = null; // = save all;
                    break;
                case 1:
                    entititiesToSave = Array.isArray(arguments[0])
                        ? arguments[0]
                        : [arguments[0]];
                    break;
                default:
                    entititiesToSave = Array.prototype.slice.call(arguments);
            }
            return provider.saveChanges(entititiesToSave, saveOptions)
                .then(function(saveResult) {
                    $rootScope.$broadcast('saved', saveResult.entities);
                    log.success({ msg: 'Saved data', data: saveResult, showToast: true });
                }).catch(saveFailed);
        };

        self.courses = repository.create(provider, 'CourseDto', 'Courses');
        self.countries = repository.create(provider, 'CountryDto', 'Countries', breeze.FetchStrategy.FromLocalCache);
        self.courseParticipants = repository.create(provider, 'CourseParticipantDto', 'CourseParticipants'/* 'Courses' */);
        self.courseTypes = repository.create(provider, 'CourseTypeDto', 'CourseTypes', breeze.FetchStrategy.FromLocalCache);
        self.departments = repository.create(provider, 'DepartmentDto', 'Departments', breeze.FetchStrategy.FromLocalCache);
        self.institutions = repository.create(provider, 'InstitutionDto', 'Institutions', breeze.FetchStrategy.FromLocalCache);
        self.participants = repository.create(provider, 'ParticipantDto', 'Participants');
        self.professionalRoles = repository.create(provider, 'ProfessionalRoleDto', 'ProfessionalRoles', breeze.FetchStrategy.FromLocalCache);
        self.rooms = repository.create(provider, 'RoomDto', 'Rooms', breeze.FetchStrategy.FromLocalCache);

        function saveFailed(error) {
            var msg = config.appErrorPrefix + 'Save failed: ' +
                breeze.saveErrorMessageService.getErrorMessage(error);
            error.message = msg;
            log.error({ msg: msg, data: error });
            throw error;
        }
    }
})();