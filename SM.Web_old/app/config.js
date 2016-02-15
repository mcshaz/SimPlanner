(function () {
    'use strict';

    var app = angular.module('app');

    // Configure Toastr
    toastr.options.timeOut = 4000;
    toastr.options.positionClass = 'toast-bottom-right';
    toastr.options.showMethod = 'slideDown';
    toastr.options.hideMethod = 'slideUp';

    var keyCodes = {
        backspace: 8,
        tab: 9,
        enter: 13,
        esc: 27,
        space: 32,
        pageup: 33,
        pagedown: 34,
        end: 35,
        home: 36,
        left: 37,
        up: 38,
        right: 39,
        down: 40,
        insert: 45,
        del: 46
    };

    var imageSettings = {
        imageBasePath: '../content/images/photos/',
        unknownPersonImageSource: 'unknown_person.jpg'
    };

    var remoteServiceName = 'breeze/Breeze';

    var events = {
        controllerActivateSuccess: 'controller.activateSuccess',
        entitiesChanged: 'datacontext.entitiesChanged',
        entitiesImported: 'datacontext.entitiesImported',
        hasChangesChanged: 'datacontext.hasChangesChanged',
        spinnerToggle: 'spinner.toggle',
        storage: {
            error: 'store.error',
            storeChanged: 'store.changed',
            wipChanged: 'wip.changed'
        }
    };

    var config = {
        appErrorPrefix: '[SM Error] ', //Configure the exceptionHandler decorator
        busyIndicator: 'overlay', // 2 options: spinner or overlay
        docTitle: 'SM: ',
        events: events,
        imageSettings: imageSettings,
        keyCodes: keyCodes,
        remoteServiceName: remoteServiceName,
        version: '0.0.1'
    };

    app.value('config', config);
    
    app.config(['$logProvider', function ($logProvider) {
        // turn debugging off/on (no info or warn)
        if ($logProvider.debugEnabled) {
            $logProvider.debugEnabled(true);
        }
    }]);
    
    //#region Configure the common services via commonConfig
    app.config(['commonConfigProvider', function (cfg) {
        cfg.config.controllerActivateSuccessEvent = config.events.controllerActivateSuccess;
        cfg.config.spinnerToggleEvent = config.events.spinnerToggle;
    }]);
    //#endregion

    //#region Configure the zStorage and zStorageWip services via zStorageConfig
    app.config(['zStorageConfigProvider', function (cfg) {
        cfg.config = {
            // zStorage
            enabled: false,
            key: 'SMAngularBreeze',
            events: events.storage,

            // zStorageWip
            wipKey: 'SMAngularBreeze.wip',
            appErrorPrefix: config.appErrorPrefix,
            newGuid: breeze.core.getUuid,

            // zStorageCore
            version: config.version
        };
    }]);
    //#endregion

    //#region Configure the Breeze Validation Directive
    app.config(['zDirectivesConfigProvider', function (cfg) {
        cfg.zValidateTemplate =
                     '<span class="invalid"><i class="fa fa-warning-sign"></i>' +
                     'Inconceivable! %error%</span>';
        //cfg.zRequiredTemplate =
        //    '<i class="fa fa-asterisk fa-asterisk-invalid z-required" title="Required"></i>';
    }]);

    // Learning Point:
    // Can configure during config or app.run phase
    //app.run(['zDirectivesConfig', function(cfg) {
    //    cfg.zValidateTemplate =
    //                 '<span class="invalid"><i class="fa fa-warning-sign"></i>' +
    //                 'Inconceivable! %error%</span>';
    //}]);
    //#endregion
})();