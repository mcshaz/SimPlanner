(function () {
    'use strict';
    var controllerId = 'videos';
    angular
        .module('app')
        .controller(controllerId, videoCtrl);

    videoCtrl.$inject = ["$sce", "common"];
    //changed $uibModalInstance to $scope to get the events

    function videoCtrl($sce, common) {
        var vm = this;

        vm.currentVideo = null;
        vm.setVideo = setVideo;
        vm.videos = {
            '208395872' : 'How to register on the Sim-Planner website'
        }
        activate();

        function activate() {
            common.activateController([], controllerId);
            setVideo('208395872')
        }

        function setVideo(vimeoKey) {
            vm.currentVideo = $sce.trustAsResourceUrl("https://player.vimeo.com/video/" + vimeoKey);
        }
    }
})();