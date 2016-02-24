(function() {
    'use strict';
    
    var serviceId = 'entityManagerFactory';
    angular.module('app').factory(serviceId, ['breeze', 'config', 'model', emFactory]);

    function emFactory(breeze, config, model) {
        // Convert server-side PascalCase to client-side camelCase property names
        breeze.NamingConvention.camelCase.setAsDefault();
        // Do not validate when we attach a newly created entity to an EntityManager.
        // We could also set this per entityManager
        new breeze.ValidationOptions({ validateOnAttach: false }).setAsDefault();

        // backingStore works for Angular (ES5 property 'ready')
        breeze.config.initializeAdapterInstance('modelLibrary', 'backingStore', true);
        
        var serviceName = config.remoteServiceName;
        var metadataStore = createMetadataStore();

        var provider = {
            metadataStore: metadataStore,
            newManager: newManager
        };
        
        return provider;

        function createMetadataStore() {
            var store = new breeze.MetadataStore();
            model.configureMetadataStore(store);
            if (model.useManualMetadata) {
                store.addDataService(new breeze.DataService({ serviceName: serviceName }));
            }
            return store;
        }

        function newManager() {
            var mgr = new breeze.EntityManager({
                serviceName: serviceName,
                metadataStore: metadataStore
            });
            return mgr;
        }
    }
})();