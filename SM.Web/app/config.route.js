(function () {
    'use strict';

    var app = angular.module('app');

    var userRoles = {
        all: '*',
        siteAdmin: 'siteAdmin',
        institutionAdmin: 'institutionAdmin',
        faculty: 'faculty',
        participant: 'participant'
    };

    // Collect the routes
    app.constant('USER_ROLES', userRoles)
        .constant('routes', getRoutes());
    
    // Configure the routes and route resolvers
    app.config(['$routeProvider', 'routes', routeConfigurator]);
    function routeConfigurator($routeProvider, routes) {

        routes.forEach(function (r) {
            $routeProvider.when(r.url, r.config);
        });
        $routeProvider.otherwise({ redirectTo: '/' });
    }

    // Define the routes 
    function getRoutes() {
        return [
            {
                url: '/',
                config: {
                    templateUrl: 'app/dashboard/dashboard.html',
                    title: 'Dashboard',
                    settings: {
                        nav: 1,
                        content: '<i class="fa fa-dashboard"></i> Dashboard'
                    }
                }
            }, {
                url: '/admin',
                config: {
                    title: 'Admin',
                    templateUrl: 'app/admin/admin.html',
                    settings: {
                        nav: 2,
                        content: '<i class="fa fa-lock"></i> Admin'
                    },
                    access: {
                        allowedRoles: userRoles.siteAdmin
                    }
                }
            }, {
                url: '/course/:id',
                config: {
                    title: 'Courses',
                    templateUrl: 'app/course/course.html',
                    settings: {
                        nav: 3,
                        content: 'Course' //<i class="fa fa-"></i> 
                    },
                    access: {
                        allowedRoles: userRoles.all
                    }
                }
            }, {
                url: '/all-courses',
                config: {
                    title: 'All Courses',
                    templateUrl: 'app/course/courses.html',
                    settings: {
                        nav: 4,
                        content: 'All Courses' //<i class="fa fa-"></i> 
                    },
                    access: {
                        allowedRoles: userRoles.all
                    }
                }
            }, {
                url: '/courseRoles/:id',
                config: {
                    title: 'Courses',
                    templateUrl: 'app/course/courseRoles.html',
                    access: {
                        allowedRoles: userRoles.all
                    }
                }
            }, {
                url: '/courseTypes',
                config: {
                    title: 'Course Type',
                    templateUrl: 'app/courseType/courseTypes.html',
                    settings: {
                        nav: 5,
                        content: 'Course Types' //<i class="fa fa-"></i> 
                    },
                    access: {
                        allowedRoles: userRoles.all
                    }
                }
            }, {
                url: '/courseType/:id',
                config: {
                    title: 'Course Type',
                    templateUrl: 'app/courseType/courseType.html',
                    access: {
                        allowedRoles: userRoles.all
                    }
                }
            }, {
                url: '/Departments',
                config: {
                    title: 'Department',
                    templateUrl: 'app/department/departments.html',
                    settings: {
                        nav: 6,
                        content: 'Departments' //<i class="fa fa-"></i> 
                    },
                    access: {
                        allowedRoles: userRoles.all
                    }
                }
            }, {
                url: '/Department/:id',
                config: {
                    title: 'Department',
                    templateUrl: 'app/department/department.html',
                    access: {
                        allowedRoles: userRoles.all
                    }
                }
            }, {
                url: '/Scenario/:id',
                config: {
                    title: 'Scenario',
                    templateUrl: 'app/scenario/scenario.html',
                    access: {
                        allowedRoles: userRoles.all
                    }
                }
            }, {
                url: '/Manequin/:id',
                config: {
                    title: 'Manequin',
                    templateUrl: 'app/manequin/manequin.html',
                    access: {
                        allowedRoles: userRoles.all
                    }
                }
            }
        ];
    }
})();