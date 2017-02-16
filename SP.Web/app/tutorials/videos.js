(function () {
    'use strict';
    var controllerId = 'videos';
    angular
        .module('app')
        .controller(controllerId, videoCtrl);

    videoCtrl.$inject = ["$sce", "$timeout", "common"];
    //changed $uibModalInstance to $scope to get the events

    function videoCtrl($sce, $timeout, common) {
        var vm = this;
        var api = null;
        vm.state = null;
        vm.currentVideo = 0;

        vm.onPlayerReady = function (API) {api = API;};

        vm.onCompleteVideo = onCompletedVideo;
        vm.videoNames = ["How to register on sim-planner website"];
        vm.videos = createSources(vm.videoNames);
        vm.config = {
            preload: "none",
            autoHide: false,
            autoHideTime: 3000,
            autoPlay: false,
            sources: vm.videos[0].sources,
            theme: {
                url: "css/videogular.min.css"
            }
            /*,
            plugins: {
                poster: "http://www.videogular.com/assets/images/videogular.png"
            }
            */
        };

        vm.setVideo = setVideo;
        activate();

        function activate() {
            common.activateController([], controllerId);
        }
        
        function setVideo(index) {
            if (isNaN(index)) {
                index = vm.videoNames.indexOf(index);
                if (index === -1) {
                    throw new Error("index " + index + "not found");
                }
            }
            api.stop();
            vm.currentVideo = index;
            vm.config.sources = vm.videos[index].sources;
            $timeout(api.play.bind(api), 100);
        }

        function createSources(sourceArray) {
            return sourceArray.map(function (el) {
                var src = "api/videos/stream/" + encodeURIComponent(el) + "?ext=";
                return {
                    sources: ["mp4"].map(function(vf){
                        return { src: $sce.trustAsResourceUrl(src + vf), type: "video/" + vf }
                    })
                };
            });
        }

        function onCompletedVideo() {
            vm.isCompleted = true;

            vm.currentVideo++;

            if (vm.currentVideo >= vm.videos.length) {
                vm.currentVideo = 0;
            }

            vm.setVideo(vm.currentVideo);
        }
    }
})();