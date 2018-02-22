import angular from 'angular';

    var controllerId = 'videos';
export default angular
        .module('app')
        .controller(controllerId, videoCtrl).name;

    (videoCtrl as any).$inject = ["$sce", "common"];
    //changed $uibModalInstance to $scope to get the events

    function videoCtrl($sce, common) {
        var vm = this;

        vm.currentVideoSrc = null;
        vm.currentVideoId = '';
        vm.setVideo = setVideo;
        vm.videos = [{
            id:'208395872', title:'How to register on the Sim-Planner website'
        }];
        activate();

        function activate() {
            common.activateController([], controllerId);
            setVideo(vm.videos[0].id);
        }

        function setVideo(vimeoId) {
            vm.currentVideoId = vimeoId
            vm.currentVideoSrc = $sce.trustAsResourceUrl("https://player.vimeo.com/video/" + vimeoId);
        }
    }
