(function () {
    "use strict";

    
    //makeRequest('dist/js/main-423da1eb50.min.js');
    var _progressDiv=document.querySelector(".progress > .progress-bar");;
    var levels = ["danger", "warning", "info", "success"];
    updateOver(1000);

    function updatePercent(percent) {
        var newLevel = levels[Math.trunc(levels.length * percent/100)];
        //updating before jquery is loaded

        _progressDiv.style.width = percent + '%';
        _progressDiv.setAttribute('aria-valuenow', percent);
        levels.forEach(function (l) {
            _progressDiv.classList.remove("progress-bar-" + l);
        });
        if (percent === 100) {
            _progressDiv.classList.remove("active");
        }
    }

    function updateOver(milliSecs) {
        var timerId;
        var percent = 0;
        timerId = setInterval(function () {
            // increment progress bar
            percent += 10;
            
            updatePercent(percent);
            // complete
            if (percent >= 100) {
                clearInterval(timerId);
            }

        }, milliSecs/10);
    }

    function makeRequest(url) {
        var oReq = new XMLHttpRequest();
        
        oReq.addEventListener("progress", updateProgress);
        oReq.addEventListener("load", transferComplete);
        oReq.addEventListener("error", transferFailed);
        oReq.addEventListener("abort", transferCanceled);
        oReq.addEventListener("loadend", cleanup);

        oReq.open('GET', url, true);
        oReq.send();

        function updateProgress(oEvent) {
            var percent = oEvent.lengthComputable
                ? 10 + 80 * oEvent.loaded / oEvent.total //that is by getting here we are ~10% of the way there, and even after finished loading we are ~90% there (have to execute the script)
                : 10; //first update comes in before length computable - assume 10% of the way there
            //console.log(percent, oEvent);
            updatePercent(percent);
        }

        function transferFailed(evt) {
            //console.log("failed to load " + url, evt);
            alert("Failed to load required file. Please try and reload the page. If the problem continues, please notify brentm@adhb.govt.nz");
        }

        function transferCanceled(evt) {
            //console.log(url + " canceled by the user.");
        }


        function transferComplete(evt) {
            updatePercent(100);
            /*
            var script = document.createElement("script");
            script.innerHTML = oReq.responseText;
            script.src = '';//in case strict
            document.body.appendChild(script);
            */

            //console.log(url + " transfer is complete.");
            eval(oReq.responseText);
        }

        function cleanup() {
            _progressDiv = oReq = null;
        }
    }

})();