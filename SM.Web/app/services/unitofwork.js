(function() {
    'use strict';

    angular.module('app')
        .factory('unitofwork', ['$rootScope', 'entityManagerFactory', 'repository', factory]);

    function factory ($rootScope, entityManagerFactory, repository) {
        var refs = {};
        var UnitOfWork = (function () {

            var unitofwork = function () {
                var provider = entityManagerFactory.create();

                this.ready = entityManagerFactory.ready;

                this.hasChanges = function() {
                    return provider.manager().hasChanges();
                };
                
                this.commit = function () {
                    var saveOptions = new breeze.SaveOptions({ resourceName: 'savechanges' });

                    return provider.manager().saveChanges(null, saveOptions)
                        .then(function(saveResult) {
                            $rootScope.$broadcast('saved', saveResult.entities);
                        });
                };

                this.rollback = function () {
                    provider.manager().rejectChanges();
                };

                this.courses = repository.create(provider, 'Course', 'Courses'/* 'Courses' */);

                this.departments = repository.create(provider, 'Department', 'Departments', breeze.FetchStrategy.FromLocalCache);
                this.institutions = repository.create(provider, 'Institution', 'Institutions', breeze.FetchStrategy.FromLocalCache);
                this.courseTypes = repository.create(provider, 'CourseType', 'CourseTypes', breeze.FetchStrategy.FromLocalCache);
            };

            return unitofwork;
        })();

        var SmartReference = (function () {

            var ctor = function () {
                var value = null;

                this.referenceCount = 0;

                this.value = function() {
                    if (value === null) {
                        value = new UnitOfWork();
                    }

                    this.referenceCount++;
                    return value;
                };

                this.clear = function() {
                    value = null;
                    this.referenceCount = 0;

                    clean();
                };
            };

            ctor.prototype.release = function () {
                this.referenceCount--;
                if (this.referenceCount === 0) {
                    this.clear();
                }
            };

            return ctor;
        })();

        return {
            create: create,
            get : get
        };
        
        function create() {
            return new UnitOfWork();
        }
        
        function get(key) {
            if (!refs[key]) {
                refs[key] = new SmartReference();
            }

            return refs[key];
        }
        
        function clean() {
            for (key in refs) {
                if (refs[key].referenceCount == 0) {
                    delete refs[key];
                }
            }
        }
    }

})();