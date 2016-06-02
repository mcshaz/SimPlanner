(function () {
    'use strict';

    angular
        .module('app')
        .controller('instructorCourse', instructorCourse);

    instructorCourse.$inject = ['datacontext'];

    function instructorCourse(datacontext) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = "New Instructor Course"

        activate();

        function activate() {

        }

        function getHospitals(forceRefresh) {
            return datacontext.hospital.getAll(forceRefresh)
				.then(function (data) {
				    vm.hospitals = data;
				    return data;
				});
        }

        function getParticipants(forceRefresh) {
            return datacontext.participant.getAll(forceRefresh)
				.then(function (data) {
				    vm.participants = data;
				    return data;
				});
        }
    }
})();
