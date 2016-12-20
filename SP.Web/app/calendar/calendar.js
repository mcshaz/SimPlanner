(function (undefined) {
    'use strict';
    var controllerId = 'calendar';
    angular.module('app').controller(controllerId, ['common', 'moment', '$q', 'breeze', 'datacontext','$scope', '$location',calendar]);

    function calendar(common, moment, $q, breeze, datacontext,$scope, $location) {
        var vm = this;
        var log = common.logger.getLogFn(controllerId);
        var _retrieved = []; //a sparse array - relying on javascript engine making htis a dictionary 

        vm.calendarView = 'month';
        vm.events = [];
        vm.eventClicked = eventClicked;
        vm.isCellOpen = true;
        vm.viewDate = new Date();

        activate();

        $scope.$watch(function () { return vm.viewDate; }, viewDateChanged);
        $scope.$watch(function () { return vm.calendarView; }, viewDateChanged);


        function activate() {
            common.activateController(
                [retrieveAppointments(vm.viewDate.getFullYear(), vm.viewDate.getMonth())], controllerId) //notification of login after dashboard activated
                .then(function () {
                    log('Activated calendar View');
                });
        }

        function eventClicked(calendarEvent) {
            $location.path('/course/' + calendarEvent.id);
        }

        function viewDateChanged() {
            if (vm.calendarView === 'year') {
                retrieveAppointments(vm.viewDate.getFullYear());
            } else {
                retrieveAppointments(vm.viewDate.getFullYear(), vm.viewDate.getMonth());
            }
        }
		
		function retrieveAppointments(year, month){
			var yearData = _retrieved[year] || 0;
			var newlyRetrieved = 0;
			var requiredMonths;
			if (month === undefined){
                requiredMonths = Array.apply(null, { length: 12 }).map(Number.call, Number);
			} else {
				var startMonth = month-1;
				requiredMonths = Array.apply(null, { length: 3 }).map(function (el, indx) { return startMonth + indx; });
			}
			
			var dtRanges = createDateRanges();

			requiredMonths.forEach(function(el){
				var bitMonth = 1 << el+1;
				if ((bitMonth & yearData) === 0) {
					dtRanges.includeMonth(year, el);
					newlyRetrieved |= bitMonth;
				}
			});

            //a bit of a hack putting it befor the ajax request has returned!
			_retrieved[year] |= newlyRetrieved;

			if (!dtRanges.length){
				return $q.when();
			}
			
			var predicate = dtRanges.map(function (el) {
                return new breeze.Predicate.create('startUtc', '>=', el.start).and('startUtc','<=',el.finish);
			}).reduce(function(a,b){ 
				return a.or(b); 
			});
			
			var returnedCourses;
			return $q.all([datacontext.ready(),
                datacontext.courses.find({
                    where: predicate,
                    expand: 'courseDays'
                }).then(function (data) {
                    returnedCourses = data;
                })]).then(function () {
                    Array.prototype.push.apply(vm.events, returnedCourses.map(mapCourse).reduce(function (a, b) {
                        return a.concat(b);
                    },[]));
                });
		}
    }

    //must have months included sequentially for function to work - i.e. not for broader use without redesign
    function createDateRanges() {
        var returnVar = [];
        returnVar.includeMonth = includeMonth;

        return returnVar;

        function includeMonth(year, month) {
            if (returnVar.length) {
                var last = returnVar[returnVar.length - 1];
                if (last.finish.getMonth() === month) {
                    last.finish = new Date(year, month + 1, 1);
                    return;
                }
            }
            returnVar.push({
                start: new Date(year, month, 1),
                finish: new Date(year, month + 1, 1)
            });
        }
    }


    function mapCourse(course) {
        var day1 = {
            id: course.id,
            title: '<i class="glyphicon glyphicon-asterisk"></i> <span class="text-primary">' + course.courseFormat.courseType.abbreviation + '</span> <small>' + course.courseFormat.description + '</small>',
            color: { 
                primary: course.department.primaryColourHtml, // the primary event color (should be darker than secondary)
                secondary: course.department.secondaryColourHtml // the secondary event color (should be lighter than primary)
            },
            //safeClassName(course.department.abbreviation),
            startsAt: course.startUtc,
            endsAt: course.finish,
            draggable: true,
            resizable: true
        };
        if (course.cancelled) {
            day1.cssClass = 'cancelled';
            delete day1.color;
        }
        return [day1].concat(course.courseDays.map(function (el) {
            var clone = angular.copy(day1);
            clone.startsAt = el.startUtc;
            clone.endsAt = el.finish;
            clone.title += ' day '+ el.day;
            return clone;
        }));
    }

    /*
    function safeClassName(dptAbbrev) {
        return dptAbbrev.replace(/[^\w\d]/g, '').toLowerCase();
    }

    function createClasses(departmentList) {
        var cssId = 'departmentColourCSS';
        var style = document.getElementById(cssId);
        if (style) { return; }
        var styleDefs = [];
        style = document.createElement('style');
        style.id = cssId;
        style.type = 'text/css';
        //style.media = 
        style.innerHTML = departmentList.map(function (dpt) {
            var className = safeClassName(dpt.abbreviation);
            return '.' + className + ' { background-color: #' + dpt.colour + ' }';
        }).join('\n');

        document.getElementsByTagName('head')[0].appendChild(style);
    }
    */
})();