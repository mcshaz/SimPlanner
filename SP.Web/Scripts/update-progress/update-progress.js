(function() {
    "use strict";
    makeRequest('/');
    function makeRequest(url) {
        var oReq = new XMLHttpRequest();
        var _progressDiv;

        oReq.addEventListener("progress", updateProgress);
        oReq.addEventListener("load", transferComplete);
        oReq.addEventListener("error", transferFailed);
        oReq.addEventListener("abort", transferCanceled);
        oReq.addEventListener("loadend", cleanup);

        oReq.open('GET',url,true);
  
        function updateProgress(oEvent) {
            var width = oEvent.lengthComputable
                ? 0.1 + 0.8 * oEvent.loaded / oEvent.total //that is by getting here we are ~10% of the way there, and even after finished loading we are ~90% there (have to execute the script)
                : 0.5;
            var levels = ["danger", "warning", "info", "success"];
            var newLevel = levels[Math.trunc(levels.length * width)];
            //updating before jquery is loaded
            if (!_progressDiv) {
                _progressDiv = document.querySelector(".progress > .progress-bar");
            }
            progressDiv.style.width = width + '%';
            progressDiv.setAttribute('aria-valuenow', width);
            levels.forEach(function (l) {
                progressDiv.classList.remove("progress-bar-" + l);
            });
            progressDiv.classList.add("progress-bar-" + newLevel);
        }

        function transferFailed(evt) {
            console.log("failed to load " + fileName, evt);
            alert("Failed to load required file. Please try and reload the page. If the problem continues, please notify brentm@adhb.govt.nz");
        }

        function transferCanceled(evt) {
            console.log(fileName + " canceled by the user.");
        }


        function transferComplete(evt) {
            updateProgress({ lengthComputable: true, loaded: 1, total: 1 });
            var script = document.createElement("script");
            script.innerHTML = evt.data;

            document.body.appendChild(script);

            console.log(fileName + " transfer is complete.");
        }

        function cleanup() {
            _progressDiv  = oReq = null;
        }
    }

})()