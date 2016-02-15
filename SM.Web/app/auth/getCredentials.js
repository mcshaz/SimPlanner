(function () {
    'use strict';
    var controllerId = 'getCredentials';
    angular.module('app')
            .controller(controllerId, ['$scope', 'loginFactory', 'common', 'ngAuthSettings', '$location', '$window', getCredentials]);

    function getCredentials($scope, loginFactory, common, ngAuthSettings, $location, $window) {
        var vm = this;
        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(controllerId);

        vm.credentials = {
            username: '',
            password: ''
        };

        vm.errors = '';

        vm.login = function (credentials) {
            vm.errors = '';
            loginFactory.login(credentials).then(function (user) {
                log("logged in as " + user.fullName,user,true)
            }, function (response) {
                if (!response.data || !response.data.error_description) {
                    getLogFn(controllerId,'error')("unhandled data returned after attempted login", response, true);
                }
                vm.errors = response.data.error_description;
            });
        };

        vm.authExternalProvider = function (provider) {
            var port = $location.port();
            var redirectUri = $location.protocol() + '://' + $location.host() + (port?(':' + port):'') + '/app/auth/authcomplete.html';
            var externalProviderUrl = ngAuthSettings.apiServiceBaseUri + "api/Account/ExternalLogin?provider=" + provider
                                                                        + "&response_type=token&client_id=" + ngAuthSettings.clientId
                                                                        + "&redirect_uri=" + redirectUri;
            $window.$windowScope = $scope;
            var oauthWindow = $window.open(externalProviderUrl, "Authenticate Account", 'resizeable=1,status=0,' + windowSize($window,0.8,300)); //,width=600,height=750,location=0
        };

        $scope.authCompletedCB = function (fragment) {
            $scope.$apply(function () {
                loginFactory.registerExternal(fragment);
                log("logged in as " + fragment.fullName, fragment, true)
                if (fragment.fullName) {
                    $location.path('/dashBoard');
                }
                else {
                    $location.path('/associate');
                }

            });
        }
    }

    function windowSize($window, fraction, min) {
        var invFrac = 1 - fraction;
        var winHeight = $window.innerHeight * fraction;
        var winWidth = $window.innerWidth * fraction;
        var top, left;
        if (winHeight < min){
            winHeight = min;
            top = 0;
        } else {
            top = winHeight * invFrac / 2;
        }
        if (winWidth < min){
            winWidth = min;
            left = 0;
        } else {
            left = winWidth * invFrac / 2;
        }
        return "height=" + winHeight +
            ",width=" + winWidth +
            ",top=" + top +
            ",left=" + left ;
    }
})();