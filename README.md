# SimManager 
####Medical Simulation Course Planning, Participant Confirmation and Reporting
A web based application to coordinate running a hospital simulation program - development phase
uses BreezeJs & Angular, based on John Papa's Hot-Towel template

##About
Simulation Manager is a tool originally designed by and for the Starship Hospital Simulation Program to simplify
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
- [ ] Come up with appropriate icon for site (currently using hot towel icon!)
- [ ] Sim Team to give current version a full test
- [ ] Enable Migrations and move to live data
- [ ] Test saving and subsequently changing/saving different combination of scenario & manequin for a given course slot
- [ ] Add Contact us page
- [ ] Add How To Use This Site page
- [ ] Add Edit My Details page
- [ ] Add Change My Password page
- [ ] Add Manequin service dates/cost table
- [ ] Add list of all participants, filerable by course participation
- [ ] ?Add ISO currency field to culture record (on create) until supported in Angular localisation
- [ ] Enable bower as per [this SO post](http://stackoverflow.com/questions/31872622/using-grunt-bower-gulp-npm-with-visual-studio-2015-for-a-asp-net-4-5-project)
- [ ] Pretty up draggable interaction 1) css class for colour change on receiving element 2)giving and receiving elements to have the same width
- [ ] Apply for/grant different levels of administrator priveleges
- [ ] Print format of course roles page &/or use migradoc to allow user to edit RTF file
- [ ] Check availability of manequins in course roles
- [ ] Check previous scenario & resource exposure for participants in course roles
- [ ] enable sign in with other social media (more than just facebook)
- [ ] reset facebook clientId key
- [ ] Course participant/faculty notification chron & method (email/text)
- [ ] keep a record of participant notifications
- [ ] Zip & store resource files for courses

###Desired Features
- [ ] Courses with 'faculty present throughout' & 'faculty only present for slots listed'. I guess being a course organiser in the latter would mean he/she present throughout - perhaps 'visitor checkbox'
- [ ] Department/Institution Reports
- [ ] multi-stream courses with faculty/participants split into different rooms (tabbed on course roles form)
- [ ] discuss with potential users re ways of splitting eg red/blue groups & ways of presenting in workable UI
- [ ] sort out flags into sprite & classes - [see here for details of tools](https://css-tricks.com/css-sprites/)
- [ ] Allow manequin room booking without specific course
