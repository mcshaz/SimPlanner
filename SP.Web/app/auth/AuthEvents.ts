import angular from 'angular';

angular.module('app').constant('AUTH_EVENTS', {
    forbidden: 'event:auth-forbidden',
    loginRequired: 'event:auth-loginRequired',
    loginConfirmed: 'event:auth-loginConfirmed',
    loginCancelled: 'event:auth-loginCancelled',
    loginWidgetReady: 'event:auth-loginWidgetReady'
})