"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var angular_1 = require("angular");
var controllerId = 'videos';
angular_1.default
    .module('app')
    .controller(controllerId, videoCtrl);
videoCtrl.$inject = ["$sce", "common"];
function videoCtrl($sce, common) {
    var vm = this;
    vm.currentVideoSrc = null;
    vm.currentVideoId = '';
    vm.setVideo = setVideo;
    vm.videos = [{
            id: '208395872', title: 'How to register on the Sim-Planner website'
        }];
    activate();
    function activate() {
        common.activateController([], controllerId);
        setVideo(vm.videos[0].id);
    }
    function setVideo(vimeoId) {
        vm.currentVideoId = vimeoId;
        vm.currentVideoSrc = $sce.trustAsResourceUrl("https://player.vimeo.com/video/" + vimeoId);
    }
}
//# sourceMappingURL=videos.js.map