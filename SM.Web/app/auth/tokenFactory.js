(function(){
    'use strict';
    var serviceId = 'tokenFactory';
    angular.module('app')
        .factory(serviceId, ['$http', '$httpParamSerializerJQLike', 'authService', tokenFactory]);

    function tokenFactory($http, $httpParamSerializerJQLike, authService) {

        var service = {
            login : login, 
            logout: logout,
            registerExternal: registerExternal
        }
	
        return service;
	
        function login(credentials) {
            var loginData = {
                grant_type: 'password',
                username: credentials.username,
                password: credentials.password
            };
            return $http({
                            method:'POST',
                            url:'/Token', 
                            data:$httpParamSerializerJQLike(loginData), // Make sure to inject the service you choose to the controller
                            headers: {
                                'Content-Type': 'application/x-www-form-urlencoded' // Note the appropriate header
                            }
            }).then(function (response) {
                authService.loginConfirmed(response.data);
                return response.data;
            });
        }

        function logout() {
            $http({
                method: 'POST',
                url: 'api/Account/Logout',
                ignoreAuthModule: true // if we had already timed out, a 401 will be returned
            });
            authService.loginCancelled();
        };

	
	    //check if the user is authorized to access the next route
	    //this function can be also used on element level
	    //e.g. <p ng-if="isAuthorized(authorizedRoles)">show this only to admins</p>
	    function isAuthorized(authorizedRoles) {
	        if (!angular.isArray(authorizedRoles)) {
	            authorizedRoles = [authorizedRoles];
	        }
	        /* return (isAuthenticated() && authorizedRoles.some(function(el) { **roles.indexOf(el) !== -1 })); */
	    };

	    function registerExternal(registerExternalData) {
	        authService.loginConfirmed(registerExternalData);
	    };

    };
})();