"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
var breezeMetadata_1 = require("./../breezeMetadata");
var serviceId = 'entityManagerFactory';
angular_1.default.module('app')
    .factory(serviceId, ['breeze', 'common', 'AUTH_EVENTS', '$rootScope', '$q', factory]);
function factory(breeze, common, AUTH_EVENTS, $rootScope, $q) {
    breeze.NamingConvention.camelCase.setAsDefault();
    var log = common.logger.getLogFn(serviceId);
    var serviceName = 'breeze/MedSimApi';
    var dataService = new breeze.DataService({
        serviceName: serviceName,
        hasServerMetadata: false
    });
    var metadataStore = new breeze.MetadataStore();
    metadataStore.importMetadata(JSON.stringify(breezeMetadata_1.default));
    var masterManager = new breeze.EntityManager({
        dataService: dataService,
        metadataStore: metadataStore
    });
    masterManager.enableSaveQueuing(true);
    var self = {
        manager: masterManager,
        ready: ready,
        set modelBuilder(builder) {
            builder(this.manager.metadataStore);
        }
    };
    var defer;
    awaitLogin();
    $rootScope.$on(AUTH_EVENTS.loginCancelled, logout);
    return self;
    function ready() {
        return defer.promise;
    }
    function awaitLogin() {
        defer = $q.defer();
        var unwatchImport = $rootScope.$on(AUTH_EVENTS.loginConfirmed, importEntities);
        function importEntities(args) {
            if (args.recredentialled) {
                return;
            }
            var query = breeze.EntityQuery.from('Lookups');
            return masterManager.executeQuery(query).then(function () {
                defer.resolve();
                if (unwatchImport) {
                    unwatchImport();
                    unwatchImport = null;
                }
            }, function (arg) {
                log.error(arg);
                defer.reject(arg);
            });
        }
    }
    function logout() {
        self.manager.clear();
        awaitLogin();
    }
}
//# sourceMappingURL=entityManagerFactory.js.map