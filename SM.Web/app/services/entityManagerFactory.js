(function() {
    'use strict';
    var serviceId = 'entityManagerFactory';
    angular.module('app')
        .factory(serviceId, ['breeze', 'common', 'AUTH_EVENTS', '$rootScope', '$q',factory]);

    function factory(breeze, common, AUTH_EVENTS, $rootScope, $q) {
        breeze.NamingConvention.camelCase.setAsDefault();
        //can set identity as default here
        var serviceName = 'breeze/MedSimApi';
        var copies = [];
        var hasLookupData = false;
        // define the Breeze `DataService` for this app
        var dataService = new breeze.DataService({
            serviceName: serviceName,
            hasServerMetadata: false  // don't ask the server for metadata
        });
        // create the metadataStore 
        var metadataStore = new breeze.MetadataStore(); /*{namingConvention: camelCaseConvention // if you use this convention)*/

        // initialize it from the application's metadata variable
        metadataStore.importMetadata(window.app.metadata);

        var masterManager = new breeze.EntityManager({
            dataService: dataService,
            metadataStore: metadataStore
        });
        //var log = common.logger.getLogFn(serviceId);

        var EntityManagerFactory = (function () {

            var entityManagerFactory = function () {
                var manager;

                this.manager = function () {
                    if (!manager) {
                        manager = masterManager.createEmptyCopy();

                        // Populate with lookup data
                        if (hasLookupData) {
                            manager.importEntities(masterManager.exportEntities());
                        } else {
                            copies.push(manager);
                        }

                        // Subscribe to events
                        manager.hasChangesChanged.subscribe(function (args) {
                            $rootScope.$broadcast('hasChanges');
                        });
                    }

                    return manager;
                };
            };

            return entityManagerFactory;
        })();

        var self = {
            create: create,
            ready: ready
        };

        var defer;
        
        $rootScope.$on(AUTH_EVENTS.loginConfirmed, importEntities);

        return self;

        function create() {
            var returnVar = new EntityManagerFactory();
            //memory leak needs to be dealt with, but could track array of managers here.
            return returnVar;
        }

        function ready() {
            if (!defer) { defer = $q.defer(); }
            if (hasLookupData) {
                defer.resolve();
            }
            return defer.promise;
        }

        function importEntities(data) {
            var query = breeze.EntityQuery.from('Lookups');

            return masterManager.executeQuery(query).then(function () {
                if (self.modelBuilder) {
                    self.modelBuilder(masterManager.metadataStore);
                }

                if (copies.length) {
                    var entities = masterManager.exportEntities();
                }
                while (copies.length) {
                    copies.pop().importEntities(entities); //getting rid of reference to allow GC to manage memory efficiently
                }
                hasLookupData = true;
                if (defer) { defer.resolve(); }
            }, function () {
                if (defer) { defer.reject.apply(this, arguments); }
            });
        };

    }

})();