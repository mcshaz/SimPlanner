(function () {
    'use strict';

    var app = angular.module('app');

    var userRoles = {
        all: '*',
        accessAllData:	'03fe7856-7b58-46b4-a1a5-1d70cf03bab2',
        accessDepartment: '75a4d6c3-9e20-4567-8b49-5d791db8f110',
        accessInstitution:	'2adedaf3-b215-4cc7-8692-1a8e58584306',
        siteAdmin:	'e5fffe70-76ef-4cfd-8d58-089ba2198dc0'
    };

    // Collect the routes
    app.constant('USER_ROLES', userRoles)
        .constant('routes', getRoutes());
    
    // Configure the routes and route resolvers
    app.config(['$routeProvider', 'routes' /*, '$locationProvider' */, routeConfigurator]);
    function routeConfigurator($routeProvider, routes /*, $locationProvider */) {

        routes.forEach(function (r) {
            $routeProvider.when(r.url, r.config);
        });
        $routeProvider.otherwise({ redirectTo: '/' });

        // use the HTML5 History API
        //$locationProvider.html5Mode(true);
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
                },
                {
                    url: '/calendar',
                    config: {
                        title: 'Calendar',
                        templateUrl: 'app/calendar/calendar.html',
                        settings: {
                            nav: 2,
                            content: '<i class="fa fa-calendar"></i> Calendar'
                        },
                        access: {
                            allowedRoles: userRoles.all
                        }
                    }
                },
                {
                    url: '/admin',
                    config: {
                        title: 'Admin',
                        templateUrl: 'app/admin/admin.html',
                        settings: {
                            nav: 3,
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
                            nav: 4,
                            content: 'Course' //<i class="fa fa-"></i> 
                        },
                        reloadOnSearch:false,
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
                            nav: 5,
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
                            nav: 6,
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
                    url: '/institutions',
                    config: {
                        title: 'Institutions',
                        templateUrl: 'app/institution/institutions.html',
                        settings: {
                            nav: 7,
                            content: 'Institutions' //<i class="fa fa-"></i> 
                        },
                        access: {
                            allowedRoles: userRoles.all
                        }
                    }
                }, {
                    url: '/institution/:id',
                    config: {
                        title: 'Institutions',
                        templateUrl: 'app/institution/institution.html',
                        access: {
                            allowedRoles: userRoles.all
                        }
                    }
                }, {
                    url: '/department/:id',
                    config: {
                        title: 'Department',
                        templateUrl: 'app/department/department.html',
                        access: {
                            allowedRoles: userRoles.all
                        }
                    }
                }, {
                    url: '/manequinModels',
                    config: {
                        title: 'Manequin Models',
                        templateUrl: 'app/manequinModels/manequinModels.html',
                        settings: {
                            nav: 8,
                            content: 'Manequin Models' //<i class="fa fa-"></i> 
                        },
                        access: {
                            allowedRoles: userRoles.all
                        }
                    }
                }, {
                    url: '/scenario/:id',
                    config: {
                        title: 'Scenario',
                        templateUrl: 'app/scenario/scenario.html',
                        access: {
                            allowedRoles: userRoles.all
                        }
                    }
                }, {
                    url: '/manequin/:id',
                    config: {
                        title: 'Manequin',
                        templateUrl: 'app/manequin/manequin.html',
                        access: {
                            allowedRoles: userRoles.all
                        }
                    }
                }, {
                    url: '/room/:id',
                    config: {
                        title: 'Room',
                        templateUrl: 'app/room/room.html',
                        access: {
                            allowedRoles: userRoles.all
                        }
                    }
                }, {
                    url: '/professionalRoles/:id',
                    config: {
                        title: 'Professional Roles',
                        templateUrl: 'app/professionalRoles/professionalRoles.html',
                        access: {
                            allowedRoles: userRoles.all
                        }
                    }
                }, {
                    url: '/scenarioRoles/:id',
                    config: {
                        title: 'Course Roles',
                        templateUrl: 'app/scenarioRoles/scenarioRoles.html',
                        access: {
                            allowedRoles: userRoles.all
                        }
                    }
                }, {
                    url: '/users',
                    config: {
                        title: 'All Users',
                        templateUrl: 'app/user/users.html',
                        settings: {
                            nav: 9,
                            content: 'Users' //<i class="fa fa-"></i> 
                        },
                        access: {
                            allowedRoles: userRoles.all
                        }
                    }
                }, {
                    url: '/user/:id',
                    config: {
                        title: 'User',
                        templateUrl: 'app/user/updateUser.html',
                        access: {
                            allowedRoles: userRoles.all
                        }
                    }
                }, {
                    url: '/updateDetails',
                    config: {
                        title: 'Update My Details',
                        templateUrl: 'app/account/updateMyDetails.html',
                        access: {
                            allowedRoles: userRoles.all
                        }
                    }
                }, {
                    url: '/changePassword',
                    config: {
                        title: 'Change Password',
                        templateUrl: 'app/account/changePassword.html',
                        access: {
                            allowedRoles: userRoles.all
                        }
                    }
                }, {
                    url: '/forgotPassword',
                    config: {
                        title: 'Forgot Password',
                        templateUrl: 'app/account/forgotPassword.html'
                    }
                }, {
                    url: '/resetPassword',
                    config: {
                        title: 'Reset Password',
                        templateUrl: 'app/account/resetPassword.html'
                    }
                }, {
                    url: '/rsvp',
                    config: {
                        title: 'RSVP',
                        templateUrl: 'app/rsvp/rsvp.html'
                    }
                }
        ];
    }
})();