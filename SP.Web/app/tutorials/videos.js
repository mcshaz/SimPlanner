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

        //http://stackoverflow.com/questions/9582463/vlc-how-to-convert-from-mp4-to-webm#28629749
        //https://msdn.microsoft.com/en-us/library/dn551368(v=vs.85).aspx
        //admin 
        //cd C:\Program Files\GPAC
        //https://www.radiantmediaplayer.com/guides/working-with-ffmpeg.html#ffmpeg-h264
        //
        //https://www.radiantmediaplayer.com/guides/working-with-mp4box.html
        //"C:\Users\OEM\Documents\Visual Studio 2015\Projects\SimPlanner\SP.Web\VideoStreaming\How to register on sim-planner website.mp4"
        // -profile dashavc264:onDemand
        function createSources(sourceArray) {
            return sourceArray.map(function (el) {
                var encoded = encodeURIComponent(el);
                var src = "api/videos/stream/" + encoded + "?ext=";
                return {
                    sources: //[{ src: "videos/" + encoded + "_dash.mpd" }].concat(
                        ["mp4","webm","ogv"].map(function(vf){
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