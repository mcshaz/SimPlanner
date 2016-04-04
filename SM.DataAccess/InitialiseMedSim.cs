using SM.DataAccess.Enums;
using System;
using System.Data.Entity;

namespace SM.DataAccess
{
    class InitialiseMedSim : DropCreateDatabaseAlways<MedSimDbContext>// DropCreateDatabaseIfModelChanges<MedSimDbContext>
    {
        protected override void Seed(MedSimDbContext context)
        {
            var nz = new Country { LocaleCode = "en-NZ", Name = "New Zealand", DialCode="64" };
            context.Countries.Add(nz);

            var starship = new Institution { Id = Guid.NewGuid(), Country = nz, Name="Starship" };
            context.Institutions.Add(starship);
            var ced = new Department { Id = Guid.NewGuid(), Institution = starship, Abbreviation = "CED", Name="Children's Emergency Department"};
            context.Departments.Add(ced);
            var simProgram = new Department { Id = Guid.NewGuid(), Institution = starship, Name = "Simulation Programme", Abbreviation = "Sim Team" };
            context.Departments.Add(simProgram);
            var picu = new Department {Id = Guid.NewGuid(), Institution = starship, Abbreviation = "PICU" , Name="Paediatric Intensive Care Unit"};
            context.Departments.Add(ced);

            var picuConf = new Room { Id = Guid.NewGuid(), Department = picu, ShortDescription="PICU Conf. Room",FullDescription = "PICU Conference Room (Meeting Room 252)", Directions = "Ask the Administrative assitants to let you in to the back offices. 2nd room on the left" };
            context.Rooms.Add(picuConf);
            var cedConf = new Room { Id = Guid.NewGuid(), Department = ced, ShortDescription = "CED Conf. Room", FullDescription = "CED Conference Room", Directions = "Back Coridors - enter code 9999 on keypad to enter" };
            context.Rooms.Add(cedConf);

            var consultantRole = new ProfessionalRole { Id = Guid.NewGuid(), Category = ProfessionalCategory.Medical, Description = "Consultant" };
            context.ProfessionalRoles.Add(consultantRole);
            var nursingRole = new ProfessionalRole { Id = Guid.NewGuid(), Category = ProfessionalCategory.Nursing, Description = "Clinical Charge Nurse" };
            context.ProfessionalRoles.Add(nursingRole);
            var techRole = new ProfessionalRole { Id = Guid.NewGuid(), Category = ProfessionalCategory.Tech, Description = "Simulation Technician" };
            context.ProfessionalRoles.Add(techRole);

            starship.ProfessionalRoles.Add(consultantRole);
            starship.ProfessionalRoles.Add(nursingRole);
            starship.ProfessionalRoles.Add(techRole);


            //todo add Mike,Denish,Becks & check trish phone no and 2nd email 

            var trish = new Participant { Id = Guid.NewGuid(), Email = "trishw@adhb.govt.nz", FullName = "Trish Wood", PhoneNumber = "999 9999 99", ProfessionalRole = nursingRole, Department=ced };
            context.Users.Add(trish);

            var brent = new Participant { Id = Guid.NewGuid(), Email = "brentm@adhb.govt.nz", AlternateEmail = "mcshagery@yahoo.com.au",FullName = "Brent McSharry", PhoneNumber = "999 9999 99", ProfessionalRole = consultantRole, Department=picu };
            context.Users.Add(brent);

            var denish = new Participant { Id = Guid.NewGuid(), Email = "denishk@adhb.govt.nz", FullName = "Denish Kumar", PhoneNumber = "999 999 999", ProfessionalRole = techRole, Department = simProgram };
            context.Users.Add(denish);

            context.SaveChanges();
            var laerdal = new ManequinManufacturer { Id = Guid.NewGuid(), Name = "Laerdal" };
            context.ManequinManufacturers.Add(laerdal);
            var junior = new ManequinModel { Id = Guid.NewGuid(),  Description = "Sim Junior", Manufacturer=laerdal};
            context.ManequinModels.Add(junior);

            var cedJunior = new Manequin { Id = Guid.NewGuid(), Department = ced, Description = "'charlie' (sim junior purchased 2007)", Model = junior, PurchasedNew = true, PurchaseDate = new DateTime(2008, 1, 1), LocalCurrencyPurchasePrice = 80000.00m };
            var crm = new CourseType { Id = Guid.NewGuid(), Abbrev = "CRM", Description = "Crisis Resourse Managment", EmersionCategory = Emersion.Emersive, IsInstructorCourse = false };
            context.CourseTypes.Add(crm);

            var crm2 = new CourseFormat { Id = Guid.NewGuid(), DaysDuration = 1, Description = "2 Scenario", CourseType = crm };
            crm.Departments.Add(ced);

            //eventually resource filename should belong to each department

            var didactic = new CourseActivity { Id = Guid.NewGuid(), Name = "Didactic Lecture", CourseType = crm };
            context.CourseActivities.Add(didactic);
            var slides = new ActivityTeachingResource { Id = Guid.NewGuid(), Name = "PICU 2016 version", ResourceFilename = @"C:\whatever\Slides.ppt", CourseActivity = didactic };
            context.ActivityTeachingResources.Add(slides);
            var didacticSlot = new CourseSlot { Id = Guid.NewGuid(), Day = 1, MinutesDuration = 20, Activity = didactic, Order = 0, CourseFormat = crm2, IsActive = true };
            didactic.ActivityChoices.Add(slides);
            context.CourseSlots.Add(didacticSlot);

            var teamBuilder = new CourseActivity { Id = Guid.NewGuid(), Name = "Team Building", CourseType = crm };
            context.CourseActivities.Add(teamBuilder);

            var ballGame = new ActivityTeachingResource { Id = Guid.NewGuid(), Name="Multi-sized balls", CourseActivity=teamBuilder};
            var eggGame = new ActivityTeachingResource { Id = Guid.NewGuid(), Name = "Egg, plate, ribons", CourseActivity = teamBuilder };
            var solarGame = new ActivityTeachingResource { Id = Guid.NewGuid(), Name = "Solar Blanket", CourseActivity = teamBuilder };
            context.ActivityTeachingResources.AddRange(new[] { ballGame, eggGame, solarGame });

            var teamSlot = new CourseSlot { Id = Guid.NewGuid(), Day = 1, MinutesDuration = 20, Activity = teamBuilder, Order = 1, CourseFormat = crm2, IsActive = true };
            context.CourseSlots.Add(teamSlot);

            teamBuilder.ActivityChoices.Add(ballGame);
            teamBuilder.ActivityChoices.Add(eggGame);
            teamBuilder.ActivityChoices.Add(solarGame);

            var sim1 = new CourseSlot { Id = Guid.NewGuid(), Day = 1, MinutesDuration = 40, Order = 2, CourseFormat = crm2, IsActive = true };
            context.CourseSlots.Add(sim1);

            var coffee = new CourseActivity { Id = Guid.NewGuid(), Name = "Coffee Break", CourseType = crm };
            var coffeeSlot = new CourseSlot { Id = Guid.NewGuid(), Day = 1, MinutesDuration = 20, Activity=coffee, Order = 3, CourseFormat = crm2, IsActive = true };
            context.CourseActivities.Add(coffee);
            context.CourseSlots.Add(coffeeSlot);

            var ld = new FacultySimRole { Id = Guid.NewGuid(), Description = "Lead Debrief" };
            var ad = new FacultySimRole { Id = Guid.NewGuid(), Description = "Assistant Debrief" };
            var lt = new FacultySimRole { Id = Guid.NewGuid(), Description = "Tech" };
            var d = new FacultySimRole { Id = Guid.NewGuid(), Description = "Director" };
            var r = new FacultySimRole { Id = Guid.NewGuid(), Description = "Runner" };
            crm.FacultySimRoles.Add(ld);
            crm.FacultySimRoles.Add(ad);
            crm.FacultySimRoles.Add(lt);
            crm.FacultySimRoles.Add(d);
            crm.FacultySimRoles.Add(r);



            var c = new Course { Id = Guid.NewGuid(), CourseFormat = crm2, Department = ced, FacultyNoRequired = 5, StartTime = DateTime.Now.AddDays(14), Room=cedConf };
            var c2 = new Course { Id = Guid.NewGuid(), CourseFormat = crm2, Department = picu, FacultyNoRequired = 5, StartTime = DateTime.Now.AddDays(28), Room=picuConf };
            var c0 = new Course { Id = Guid.NewGuid(), CourseFormat = crm2, Department = ced, FacultyNoRequired = 5, StartTime = DateTime.Now.AddDays(-30), Room = cedConf };
            context.Courses.AddRange(new Course[] { c, c0, c2 });

            foreach (var t in new Course[] { c, c2, c0 })
            {
                t.CalculateFinishTime();
                var cp = new CourseParticipant { Participant = trish, IsConfirmed = true, IsFaculty = true, IsOrganiser=true, Course = t, DepartmentId = trish.DefaultDepartmentId, ProfessionalRoleId = trish.DefaultProfessionalRoleId };
                context.CourseParticipants.Add(cp);
                var cp2 = new CourseParticipant { Participant = brent, IsConfirmed = false, IsFaculty = false, Course = t, DepartmentId = brent.DefaultDepartmentId, ProfessionalRoleId = brent.DefaultProfessionalRoleId };
                context.CourseParticipants.Add(cp2);
            }

            context.SaveChanges();

            var s = new Scenario { Id = Guid.NewGuid(), Complexity = Difficulty.Easy, Description = "boy falls down well", EmersionCategory = Emersion.Emersive, CourseType = crm, ManequinModel = junior,
                Department = ced };

            var s2 = new Scenario
            {
                Id = Guid.NewGuid(),
                Complexity = Difficulty.Moderate,
                Description = "boy has fever",
                EmersionCategory = Emersion.Emersive,
                CourseType = crm,
                ManequinModel = junior,
                Department = picu
            };

            var proforma = new ScenarioResource { Id = Guid.NewGuid(), Name = "proforma", ResourceFilename = @"C:\whatever.doc", Scenario=s };

            c.Scenarios.Add(s);
            c2.Scenarios.Add(s2);
            c0.Scenarios.Add(s);

            context.SaveChanges();

            var pres = new CourseSlotPresenter { Course = c, CourseSlot = didacticSlot, Presenter = trish };
            context.CourseSlotPresenters.Add(pres);

            var simScenario1 = new CourseSlotScenario { Course = c, CourseSlot=sim1, Scenario = s, Manequin=cedJunior };
            context.CourseSlotScenarios.Add(simScenario1);

            var simRole1 = new CourseScenarioFacultyRole { Course = c, CourseSlot = sim1, FacultyMember = trish, FacultySimRole = lt };
            context.CourseScenarioFacultyRoles.Add(simRole1);

            context.SaveChanges();

        }
    }
}
