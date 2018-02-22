import angular from 'angular';
export default angular.module('app')
    .filter('fileDate',['moment',fileDate])
    .filter('sizeKib', ['$filter', sizeKib]);

    function fileDate(moment) {
        return function (date) {
            if (!date) { return ''; }
            var dt = moment(date);
            return dt.format('L') + ' ' + dt.format('LT');
        };
    }
    
    function sizeKib($filter) {
        return function (bytes) {
            if (!bytes) { return ''; }
            return $filter('number')(bytes / 1024, 1);
        };
    }

