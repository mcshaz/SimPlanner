(function() {
    'use strict';
    var serviceId = 'entityManagerFactory';
    angular.module('app')
        .factory(serviceId, ['breeze', 'common', 'AUTH_EVENTS', '$rootScope', '$q', 'tokenStorageService', factory]);

    function factory(breeze, common, AUTH_EVENTS, $rootScope, $q, tokenStorageService) {
        breeze.NamingConvention.camelCase.setAsDefault();
        //can set identity as default here
        var log = common.logger.getLogFn(serviceId);
        var serviceName = 'breeze/MedSimApi';

        // define the Breeze `DataService` for this app
        var dataService = new breeze.DataService({
            serviceName: serviceName,
            hasServerMetadata: false  // don't ask the server for metadata
        });
        // create the metadataStore 
        var metadataStore = new breeze.MetadataStore(); /*{namingConvention: camelCaseConvention // if you use this convention)*/

        // initialize it from the application's metadata variable
        metadataStore.importMetadata(window.medsimMetadata);
        console.log(metadataStore.exportMetadata());

        var masterManager = new breeze.EntityManager({
            dataService: dataService,
            metadataStore: metadataStore
        });
        //var log = common.logger.getLogFn(serviceId);
        masterManager.hasChangesChanged.subscribe(function () {
            $rootScope.$broadcast('hasChanges', arguments[0]);
        });

        var self = {
            manager: masterManager,
            ready: ready
        };

        var defer = $q.defer();
        
        var unwatchImport = $rootScope.$on(AUTH_EVENTS.loginConfirmed, importEntities);
        //to do empty values on logout;

        return self;

        function ready() {
            return defer.promise;
        }

        function importEntities(args) {
            if (args.recredentialled) {
                return;
            }
            if (self.modelBuilder) {
                self.modelBuilder(masterManager.metadataStore);
            }

            var query = breeze.EntityQuery.from('Lookups');
            return masterManager.executeQuery(query).then(function () {
                defer.resolve();
            }, function (arg) {
                log.error(arg);
                defer.reject(arg);
            });
            resolved = true;
            unwatchImport(); //only run once - will be called on every recredential
            unwatchImport = null;
        };

    }

})();