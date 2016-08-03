window.updateProgress = function(width) {
    "use strict";
    if (!width) { width = 5; }
    var levels = ["danger" , "warning" , "info" , "success"];
    var newLevel = levels[Math.trunc(levels.length * width / 100)];
    //updating before jquery is loaded
    var progressDiv = document.querySelector(".progress > .progress-bar");
    progressDiv.style.width = width + '%';
    progressDiv.setAttribute('aria-valuenow', width);
    levels.forEach(function (l) {
        progressDiv.classList.remove("progress-bar-" + l);
    });
    progressDiv.classList.add("progress-bar-" + newLevel);
};