(function() {
    'use strict';
    var serviceId = 'entityManagerFactory';
    angular.module('app')
        .factory(serviceId, ['breeze', 'common', 'AUTH_EVENTS', '$rootScope', '$q',factory]);

    function factory(breeze, common, AUTH_EVENTS, $rootScope, $q) {
        breeze.NamingConvention.camelCase.setAsDefault();
        //can set identity as default here
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
        
        $rootScope.$on(AUTH_EVENTS.loginConfirmed, importEntities);

        return self;

        function ready() {
            return defer.promise;
        }

        function importEntities() {
            
            if (self.modelBuilder) {
                self.modelBuilder(masterManager.metadataStore);
            }

            var query = breeze.EntityQuery.from('Lookups');
            return masterManager.executeQuery(query).then(function () {

                defer.resolve(); 
            }, function () {
                defer.reject.apply(this, arguments); 
            });
        };

    }

})();