(function () {
    'use strict';

    angular.module('common').factory('dateUtilities', [factory]);
    
    function factory() {
        var universalDateInputs = ["D MMM YYYY", "Do MMM YYYY", "Do MMMM YYYY", "D MMMM YYYY", "YYYY-MM-DD"];
        var dateFirstInputs = ['D-M-YYYY', 'D/M/YYYY', 'D.M.YYYY'];
        var service = {
            dateFirstCulture: true, //set to true or false
            dateIntervalFormatter: dateIntervalFormatter,
            dateLongFormat: "Do MMMM YYYY",
            dateLongFormatter:dateLongFormatter,
            dateParseFormats: {
                dateFirst: universalDateInputs.slice(0).push(dateFirstInputs),
                monthFirst: universalDateInputs.slice(0).push(dateFirstInputs.map(function (el) {
                    el = el.replace(/M/g, "~");
                    el = el.replace(/D/g, "M");
                    el = el.replace(/~/g, "D");
                }))
            },
            dateShortFormat: "D MMM YYYY",
            dateShortFormatter: dateShortFormatter,
            dateTimeLongFormatter: dateTimeLongFormatter,
            dateTimeShortFormatter: dateTimeShortFormatter,
            parseDate: parseDate,
            parseTime: parseTime,
            timeFormatter: timeFormatter,
            timeOutputFormat: 'HH:m',
            timeParseFormats: ['H:m', 'h:m a']
        };

        service.dateTimeLongFormat = service.dateLongFormat + " h:mm a";
        service.dateTimeShortFormat = service.dateLongFormat + " HH:mm";
        return service;


        function dateLongFormatter(date) {
            var m = moment(date);
            if (m.isValid()) {
                return m.format(service.dateLongFormat);
            }
            return null;
        }

        function dateShortFormatter(date) {
            var m = moment(date);
            if (m.isValid()) {
                return m.format(service.dateShortFormat);
            }
            return null;
        }

        function dateTimeLongFormatter(date) {
            var m = moment(date);
            if (m.isValid()) {
                return m.format(service.dateTimeLongFormat);
            }
            return null;
        }

        function dateTimeShortFormatter(date) {
            var m = moment(date);
            if (m.isValid()) {
                return m.format(service.dateTimeShortFormat);
            }
            return null;
        }

        function dateIntervalFormatter(date) {
            var m = moment(date);
            if (m.isValid()) {
                return m.fromNow();
            }
            return null;
        }


        function parseDate(dateString) {
            if (service.dateFirstCulture) {
                return moment(dateString, service.dateParseFormats.dateFirst)
            }
            return moment(value, service.dateParseFormats.monthFirst);
        }

        function parseTime(timeString, beginDate) {
            var m = moment(timeString, service.timeParseFormats);
            if (m.isValid()) {
                return beginDate.setHours(m.hour(), m.minute(), 0, 0);
            }
            return null;
        }

        function timeFormatter(date) {
            var m = moment(date);
            if (m.isValid()) {
                return m.format(service.timeOutputFormat);
            }
        }

    }

})();