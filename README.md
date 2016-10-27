# SimPlanner
####Medical Simulation Course Planning, Participant Confirmation and Reporting
A web based application to coordinate running a hospital simulation program - development phase
uses BreezeJs & Angular, based on John Papa's Hot-Towel template

##About
Simulation Planner is a tool originally designed by and for the Starship Hospital Simulation Program to simplify
organising medical simulation courses and keeping track of institution wide data, including:
* Faculty and participants attending a course
  * (including auto-notification and tracking reply confirmation by email ± text message)
* Developing course timetables
* Which scenarios are to be run during a course 
  * And checking which scenarios participants have previously completed, to avoid repeating scenarios
* Faculty member roles - including lectures, activities and scenario roles (e.g. debriefing, sim director)
* Producing annual reports of simulation activity, as a whole institution and by department
* Track total hours simulation time (and thereby lifespan) for each manequin, including hours spent away for service and service costs

**It is free to use for institutions such as hospitals and universities** (a small fee may be charged per text messaging reminder if you choose to use the text messaging service)

###To Do
- [ ] update time if course type set after date in course page [bug]
- [ ] update dto to db without reloading user details not mapped to DTO
- [ ] Sim Team to give current version a full test
- [ ] Add Contact us page
- [ ] Add How To Use This Site page
- [ ] Add list of all participants, filerable by course participation
- [ ] Pretty up draggable interaction 1) css class for colour change on receiving element 2)giving and receiving elements to have the same width
- [ ] Apply for different levels of administrator privileges
- [ ] enable sign in with other social media (more than just facebook)
- [ ] reset facebook clientId key

###Desired Features
- [ ] Courses with 'faculty present throughout' & 'faculty only present for slots listed'. I guess being a course organiser in the latter would mean he/she present throughout - perhaps 'visitor checkbox'
- [ ] Department/Institution Reports
- [ ] multi-stream courses with faculty/participants split into different rooms (tabbed on course roles form)
- [ ] discuss with potential users re ways of splitting eg red/blue groups & ways of presenting in workable UI
- [ ] sort out flags into sprite & classes - [see here for details of tools](https://css-tricks.com/css-sprites/)
- [ ] Allow manequin & room booking without specific course
