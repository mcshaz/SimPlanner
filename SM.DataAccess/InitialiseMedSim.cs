using System;
using System.Collections;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.DataAccess
{
    class InitialiseMedSim : DropCreateDatabaseAlways<MedSimDbContext>// DropCreateDatabaseIfModelChanges<MedSimDbContext>
    {
        protected override void Seed(MedSimDbContext context)
        {
            var nz = new Country { Code = "NZ", Name = "New Zealand" };
            context.Countries.Add(nz);
            var starship = new Institution { Country = nz, Name="Starship" };
            context.Hospitals.Add(starship);
            var ced = new Department { Hospital = starship, Name = "CED" };
            context.Departments.Add(ced);
            var simProgram = new Department { Hospital = starship, Name = "Simulation Programme" };
            context.Departments.Add(simProgram);

            var picu = new Department { Hospital = starship, Name = "PICU" };
            context.Departments.Add(ced);

            var consultantRole = new ProfessionalRole { Category = ProfessionalCategory.Medical, Description = "Consulant" };
            context.ProfessionalRoles.Add(consultantRole);
            var nursingRole = new ProfessionalRole { Category = ProfessionalCategory.Medical, Description = "Clinical Charge Nurse" };
            context.ProfessionalRoles.Add(nursingRole);
            var techRole = new ProfessionalRole { Category = ProfessionalCategory.Medical, Description = "Simulation Technician" };
            context.ProfessionalRoles.Add(techRole);

            nz.ProfessionalRoles.Add(consultantRole);
            nz.ProfessionalRoles.Add(nursingRole);
            nz.ProfessionalRoles.Add(techRole);


            //todo add Mike,Denish,Becks & check trish phone no and 2nd email 

            var trish = new Participant { Id = Guid.NewGuid(), Email = "trishw@adhb.govt.nz", FullName = "Trish Wood", PhoneNumber = "999 9999 99", ProfessionalRole = nursingRole, Department=ced };
            context.Users.Add(trish);

            var brent = new Participant { Id = Guid.NewGuid(), Email = "brentm@adhb.govt.nz", AlternateEmail = "mcshagery@yahoo.com.au",FullName = "Brent McSharry", PhoneNumber = "999 9999 99", ProfessionalRole = consultantRole, Department=picu };
            context.Users.Add(brent);

            context.SaveChanges();
            var laerdal = new ManequinManufacturer { Name = "Laerdal" };
            context.ManequinManufacturers.Add(laerdal);
            var junior = new Manequin { Department = ced, Description = "Sim Junior", Manufacturer=laerdal };
            context.Manequins.Add(junior);
            var crm = new CourseType { Abbrev = "CRM", DaysDuration = 1, Description = "Crisis Resourse Managment", EmersionCategory = Emersion.Emersive, IsInstructorCourse = false };
            context.CourseTypes.Add(crm);
            crm.Departments.Add(ced);

            //NB alter so that resources belong to each department!!
            var slides = new CourseTeachingResource { Name = "Slide Set", ResourceFilename = @"C:\whatever\Slides.ppt" };
            context.CourseTeachingResources.Add(slides);
            var didactic = new CourseSlot { Day = 1, MinutesDuration = 20, MaximumFaculty = 1, MinimumFaculty = 1, Name = "Didactic Lecture", Order = 1 };
            didactic.DefaultResources.Add(slides);
            didactic.CourseTypes.Add(crm);
            context.CourseEvents.Add(didactic);

            var sim1 = new ScenarioSlot {  Day = 1, MinutesDuration = 40, Order = 2 };
            sim1.CourseTypes.Add(crm);
            context.CourseScenarios.Add(sim1);

            var ld = new ScenarioRoleDescription { Description = "Lead Debrief" };
            var ad = new ScenarioRoleDescription { Description = "Assistant Debrief" };
            var lt = new ScenarioRoleDescription { Description = "Lead Tech" };
            var at = new ScenarioRoleDescription { Description = "Assistant Tech" };
            var d = new ScenarioRoleDescription { Description = "Director" };
            var r = new ScenarioRoleDescription { Description = "Runner" };
            crm.ScenarioRoles.Add(ld);
            crm.ScenarioRoles.Add(ad);
            crm.ScenarioRoles.Add(lt);
            crm.ScenarioRoles.Add(at);
            crm.ScenarioRoles.Add(d);
            crm.ScenarioRoles.Add(r);

            var coffee = new CourseSlot { Day = 1, MinutesDuration =20, MaximumFaculty = 0, MinimumFaculty = 0, Name = "Coffee break", Order = 3 };
            coffee.CourseTypes.Add(crm);
            context.CourseEvents.Add(coffee);

            var c = new Course { CourseType = crm, Department = ced, FacultyNoRequired = 5, StartTime = new DateTime(2016, 3, 3, 8, 0, 0) };
            context.Courses.Add(c);

            var p1 = new CourseParticipant { Participant = trish, Confirmed = true, Faculty = true, Course = c, DepartmentId = trish.DefaultDepartmentId, ProfessionalRoleId=trish.DefaultProfessionalRoleId };
            context.CourseParticipants.Add(p1);
            c.Participants.Add(p1);

            var p2 = new CourseParticipant { Participant = brent, Confirmed = true, Faculty = false, Course = c, DepartmentId = brent.DefaultDepartmentId, ProfessionalRoleId = brent.DefaultProfessionalRoleId };
            context.CourseParticipants.Add(p2);
            c.Participants.Add(p2);

            var s = new Scenario { Complexity = Difficulty.Easy, Description = "boy falls down well", EmersionCategory = Emersion.Emersive, CourseType = crm, Manequin = junior,
                Department = ced };

            var proforma = new ScenarioResource { Name = "proforma", ResourceFilename = @"C:\whatever.doc", Scenario=s };

            c.Scenarios.Add(s);

            context.SaveChanges();

            var pres = new CourseSlotPresenter { Course = c, CourseSlot = didactic, Presenter = trish };

            var role = new ScenarioFacultyRole { Course = c, FacultyMember = trish, Role = ld, Scenario = s };

            context.CourseSlotPresenters.Add(pres);

            context.ScenarioFacultyRoles.Add(role);

            context.SaveChanges();

        }
    }
}
