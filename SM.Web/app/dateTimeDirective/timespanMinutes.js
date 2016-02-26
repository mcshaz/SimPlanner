(function () {
    window.timespanMinutes = function () {

        var returnVar = new TimespanMinutes();
        if (arguments.length) {
            if (typeof arguments[0] === 'string') {
                returnVar.parse(arguments[0]);
            } else {
                returnVar.setMinutes(arguments[0]);
            }

        }
        return returnVar;
    }
    function TimespanMinutes() {
        //var self = this;
        //self.minutes;
        //self.isValid;
        //self.invalidReason;
        //self.parsed;
        this.allowMinuteOnly = false;
        this.maxMins = 1439;//24 * 60 - 1;
    }
    TimespanMinutes.prototype.parse = parse;
    TimespanMinutes.prototype.toString = toString;
    TimespanMinutes.prototype.setMinutes = setMinutes;
    TimespanMinutes.prototype.setTime = setTime;
    function parse(span) {
        if (!typeof span === 'string') {
            throw new TypeError('string argument in format minutes or h:mm required');
        }
        this.parsed = span = span.trim();

        var vals = /^(\d\d?):(\d\d)\s*([ap]?)\.?m?\.?$/i.exec(span);
        if (vals) {
            if (vals[2] > 59) {
                setInvalid(this, "minutes");
                return;
            }
            if (setMins(this, parseFloat(vals[2]) + 60 * parseFloat(vals[1]) + ((vals[3] && vals[3].toLowerCase() === 'p') ? 12 : 0))) {
                return;
            }
        } else if (this.allowMinuteOnly) {
            vals = /\d+/.exec(span);
            if (vals) {
                if (setMins(parseInt(vals[0], 10))) {
                    return;
                }
            }
        }

        setInvalid(this, "unparsable");
    }

    function toString() {
        var h = Math.floor(this.minutes / 60);
        var m = '00' + (this.minutes - 60*h);

        return h + ':' + m.substr(m.length - 2);
    };

    function setMinutes(arg) {
        if (arg instanceof Date) {
            if (setMins(this, arg.getHours() * 60 + arg.getMinutes())) {
                return;
            }
        } else if (arg instanceof TimespanMinutes) {
            if (!arg.isValid) {
                setInvalid(this, "invalidTimespan");
                return;
            }
            if (setMins(this, arg.minutes)) {
                return;
            }
        } else {
            var num = parseInt(arg, 10);
            if (isNaN(num) || !isFinite(num)) {
                setInvalid(this, "unparsable");
                return;
            }
            setMins(this, num);
        }

    }

    function setTime(dateArg) {
        if (!dateArg instanceof Date) {
            throw new TypeError("settime sets the time on a date");
        }
        return new Date(dateArg.setHours(0, this.minutes, 0, 0));
    }

    function setMins(obj, mins) {
        if (mins > this.maxMins) {
            setInvalid(this, "maxMinutes");
            return false;
        }
        obj.isValid = true;
        obj.minutes = mins;
        obj.invalidReason = '';
        return true;
    }

    function setInvalid(obj, reason) {
        obj.isValid = false;
        obj.minutes = null;
        obj.invalidReason = reason;
    }
})();