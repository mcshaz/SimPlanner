import angular from 'angular';

    var controllerId = 'myCourseInvites';
export default angular
        .module('app')
        .controller(controllerId, controller).name;

    (controller as any).$inject = ['common', 'datacontext', 'tokenStorageService', 'breeze', '$interval', '$scope', '$location', '$browser'];

    function controller(common, datacontext, tokenStorageService, breeze, $interval, $scope, $location, $browser) {
        /* jshint validthis:true */
        var vm = this;
        var user = {};
        var trackedCoursesPred;
        var userId;

        vm.title = 'My Invitations';
        vm.courses = [];
        vm.sendAcceptance = sendAcceptance;
        vm.downloadFileUrl = null;

        console.log($location);
        console.log($browser);

        activate();

        function activate() {
            datacontext.ready().then(function () {
                userId = tokenStorageService.getUserId();
                var pred = breeze.Predicate.create('startFacultyUtc', '>', new Date())
                    .and('facultyInvites', 'any', 'participantId', '==', userId)
                    .and(breeze.Predicate.create('courseParticipants', 'any', 'participantId', '==', userId).not());
                common.activateController([
                    datacontext.courses.find({ where: pred, expand: 'courseParticipants', orderBy: 'startFacultyUtc' }).then(function (data) {
                        var stop;
                        vm.courses = data;
                        if (data.length) {
                            trackedCoursesPred = { where: breeze.Predicate.create('courseId', 'in', data.map(function (el) { return el.id; })) };
                            stop = $interval(updateCourseParticipants, 60000);
                            $scope.$on('$destroy', function () {
                                $interval.cancel(stop);
                                removeAccepts();
                            });
                        }
                    }),
                datacontext.participants.fetchByKey(userId).then(function (u) {
                    user = u;
                })], controllerId);
            });
        }

        function sendAcceptance() {
            var selectedCourses = vm.courses.filter(function (c) {
                return c.accept && c.facultyCount >= c.facultyNoRequired;
            });
            var courseParticipants = selectedCourses.map(function (c) {
                var cp = c.addParticipant(user, {isConfirmed:true, isFaculty: true});
                return cp;
            });
            datacontext.save(courseParticipants).then(function () {
                common.arrayUtils.removeFromArray.apply(null, [vm.courses].concat(vm.selectedCourses));
                selectedCourses.forEach(removeAccept);

                vm.downloadFileUrl = common.windowOrigin() + '/api/CoursePlanning/MyCalendar/' + userId + '.ics';
            });
        }

        function updateCourseParticipants() {
            datacontext.courses.find(trackedCoursesPred);
        }

        function removeAccepts(){
            vm.courses.forEach(removeAccept);
        }

    }
    function removeAccept(c) {
        delete c.accept;
    }

