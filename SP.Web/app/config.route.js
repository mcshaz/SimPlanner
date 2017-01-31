(function () {
    'use strict';

    var app = angular.module('app');

    var userRoles = {
        authenticated: '*',
        anonymous: '?',
        accessAllData:	'03fe7856-7b58-46b4-a1a5-1d70cf03bab2',
        accessDepartment: '75a4d6c3-9e20-4567-8b49-5d791db8f110',
        accessInstitution:	'2adedaf3-b215-4cc7-8692-1a8e58584306',
        siteAdmin: 'e5fffe70-76ef-4cfd-8d58-089ba2198dc0',
        dptRoomBookings: '1a04cee2-f3f0-4965-a223-a2eae7937637',
        dptManikinBookings: 'e8149241-e33b-4510-a698-41b26b4b5d48'
    };

    // Collect the routes
    app.constant('USER_ROLES', userRoles)
        .constant('routes', getRoutes());
    
    // Configure the routes and route resolvers
    app.config(['$routeProvider', 'routes' ,'$locationProvider' , routeConfigurator]);
    function routeConfigurator($routeProvider, routes, $locationProvider ) {

        routes.forEach(function (r) {
            $routeProvider.when(r.url, r.config);
        });
        $routeProvider.otherwise({ redirectTo: '/' });

        // use the HTML5 History API
        //$locationProvider.html5Mode(true);
        $locationProvider.hashPrefix('!');
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
                            allowedRoles: userRoles.authenticated
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
                            allowedRoles: userRoles.authenticated
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
                            allowedRoles: userRoles.authenticated
                        }
                    }
                }, {
                    url: '/courseRoles/:id',
                    config: {
                        title: 'Courses',
                        templateUrl: 'app/course/courseRoles.html',
                        access: {
                            allowedRoles: userRoles.authenticated
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
                            allowedRoles: userRoles.authenticated
                        }
                    }
                }, {
                    url: '/courseType/:id',
                    config: {
                        title: 'Course Type',
                        templateUrl: 'app/courseType/courseType.html',
                        access: {
                            allowedRoles: userRoles.authenticated
                        }
                    }
                }, {
                    url: '/resources',
                    config: {
                        title: 'Resources',
                        templateUrl: 'app/institution/institutions.html',
                        settings: {
                            nav: 7,
                            content: 'Resources <i class="fa fa-info-circle" title="Update or Create Institutions, Departments or department resources - comprising Manikins, Scenarios & Rooms"></i>'
                        },
                        access: {
                            allowedRoles: userRoles.authenticated
                        }
                    }
                }, {
                    url: '/institution/:id',
                    config: {
                        title: 'Institutions',
                        templateUrl: 'app/institution/institution.html',
                        access: {
                            allowedRoles: [userRoles.anonymous, userRoles.accessAllData, userRoles.accessInstitution]
                        }
                    }
                }, {
                    url: '/department/:id',
                    config: {
                        title: 'Department',
                        templateUrl: 'app/department/department.html',
                        access: {
                            allowedRoles: [userRoles.anonymous, userRoles.accessAllData, userRoles.accessDepartment,userRoles.accessInstitution]
                        }
                    }
                }, {
                    url: '/manikinModels',
                    config: {
                        title: 'Manikin Models',
                        templateUrl: 'app/manikinModels/manikinModels.html',
                        settings: {
                            nav: 8,
                            content: 'Manikin Models' //<i class="fa fa-"></i> 
                        },
                        access: {
                            allowedRoles: userRoles.authenticated
                        }
                    }
                }, {
                    url: '/scenario/:id',
                    config: {
                        title: 'Scenario',
                        templateUrl: 'app/scenario/scenario.html',
                        access: {
                            allowedRoles: userRoles.authenticated
                        }
                    }
                }, {
                    url: '/manikin/:id',
                    config: {
                        title: 'Manikin',
                        templateUrl: 'app/manikin/manikin.html',
                        access: {
                            allowedRoles: userRoles.authenticated
                        }
                    }
                }, {
                    url: '/room/:id',
                    config: {
                        title: 'Room',
                        templateUrl: 'app/room/room.html',
                        access: {
                            allowedRoles: userRoles.authenticated
                        }
                    }
                }, {
                    url: '/professionalRoles/:id',
                    config: {
                        title: 'Professional Roles',
                        templateUrl: 'app/professionalRoles/professionalRoles.html',
                        access: {
                            allowedRoles: userRoles.authenticated
                        }
                    }
                }, {
                    url: '/scenarioRoles/:id',
                    config: {
                        title: 'Course Roles',
                        templateUrl: 'app/scenarioRoles/scenarioRoles.html',
                        access: {
                            allowedRoles: userRoles.authenticated
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
                            allowedRoles: userRoles.authenticated
                        }
                    }
                }, {
                    url: '/user/:id',
                    config: {
                        title: 'User',
                        templateUrl: 'app/user/updateUser.html',
                        access: {
                            allowedRoles: userRoles.authenticated
                        }
                    }
                }, {
                    url: '/updateDetails',
                    config: {
                        title: 'Update My Details',
                        templateUrl: 'app/account/updateMyDetails.html',
                        access: {
                            allowedRoles: userRoles.authenticated
                        }
                    }
                }, {
                    url: '/changePassword',
                    config: {
                        title: 'Change Password',
                        templateUrl: 'app/account/changePassword.html',
                        access: {
                            allowedRoles: userRoles.authenticated
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
                }, {
                    url: '/reviewRegistrationRequest/:id',
                    config: {
                        title: 'Review Request',
                        templateUrl: 'app/approval/reviewRegistrationRequest.html',
                        access: {
                            allowedRoles: userRoles.siteAdmin
                        }
                    }
                }, {
                    url: '/finishedSubmission',
                    config: {
                        title: 'Finished Submission',
                        templateUrl: 'app/approval/finishedSubmission.html',
                        access: {
                            allowedRoles: userRoles.anonymous
                        }
                    }
                }, {
                    url: '/register',
                    config: {
                        title: 'Register',
                        templateUrl: 'app/approval/beginSubmission.html', settings: {
                            nav: 10,
                            content: '<i class="fa fa-user-plus"></i> Register'
                        },
                        access: {
                            allowedRoles: userRoles.anonymous
                        }
                    }
                }, {
                    url: '/facultyInvites',
                    config: {
                        title: 'Invite Faculty',
                        templateUrl: 'app/facultyInvites/facultyInvites.html',
                        settings: {
                            nav: 11,
                            content: '<i class="fa fa-binoculars"></i> Find Faculty'
                        },
                        access: {
                            allowedRoles: [userRoles.accessAllData, userRoles.accessDepartment, userRoles.accessInstitution]
                        }
                    }
                }, {
                    url: '/myInvites',
                    config: {
                        title: 'My Invitations',
                        templateUrl: 'app/myCourseInvites/myCourseInvites.html',
                        settings: {
                            nav: 12,
                            content: '<i class="fa fa-envelope">Pending Invites'
                        },
                        access: {
                            allowedRoles: userRoles.authenticated
                        }
                    }
                }
        ];
    }
})();