namespace SP.DataAccess.Migrations
{
    using Enums;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MedSimDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "SP.DataAccess.MedSimDbContext";
        }

        protected override void Seed(MedSimDbContext context)
        {
            if (!context.Cultures.Any())
            {
                var nz = new Culture { LocaleCode = "en-NZ", Name = "New Zealand", CountryCode = 64 };
                context.Cultures.Add(nz);

                var starship = new Institution { Id = Guid.NewGuid(), Culture = nz, Name = "Starship", ProfessionalRoleInstitutions = new List<ProfessionalRoleInstitution>() };
                context.Institutions.Add(starship);
                var mmh = new Institution { Id = Guid.NewGuid(), Culture = nz, Name = "Middlemore", ProfessionalRoleInstitutions = new List<ProfessionalRoleInstitution>() };
                context.Institutions.Add(mmh);
                var ced = new Department { Id = Guid.NewGuid(), Institution = starship, Abbreviation = "CED", Name = "Children's Emergency Department" };
                context.Departments.Add(ced);
                var simProgram = new Department { Id = Guid.NewGuid(), Institution = starship, Name = "Simulation Programme", Abbreviation = "Sim" };
                context.Departments.Add(simProgram);
                var picu = new Department { Id = Guid.NewGuid(), Institution = starship, Abbreviation = "PICU", Name = "Paediatric Intensive Care Unit" };
                context.Departments.Add(picu);
                var mmIcu = new Department { Id = Guid.NewGuid(), Institution = mmh, Abbreviation = "ICU", Name = "Intensive Care Unit" };
                context.Departments.Add(mmIcu);

                var picuConf = new Room { Id = Guid.NewGuid(), Department = picu, ShortDescription = "PICU Conf. Room", FullDescription = "PICU Conference Room (Meeting Room 252)", Directions = "Ask the Administrative assitants to let you in to the back offices. 2nd room on the left" };
                context.Rooms.Add(picuConf);
                var cedConf = new Room { Id = Guid.NewGuid(), Department = ced, ShortDescription = "CED Conf. Room", FullDescription = "CED Conference Room", Directions = "Back Coridors - enter code 9999 on keypad to enter" };
                context.Rooms.Add(cedConf);

                int i = 0;
                var medRoles = (new[] { "Consultant", "Fellow", "Registrar", "House Officer", "Intern", "Trainee Intern", "Medical Student" }).Select(m =>
                    new ProfessionalRole { Id = Guid.NewGuid(), Category = ProfessionalCategory.Medical, Description = m, Order = i++ });
                i = 0;
                var nurseRoles = (new[] { "Nurse Manager", "Clinical Charge Nurse", "Nurse Educator", "Clinical Nurse Specialist", "Nurse Practitioner", "New Grad" }).Select(n =>
                     new ProfessionalRole { Id = Guid.NewGuid(), Category = ProfessionalCategory.Nursing, Description = n, Order = i++ });

                var techRole = new ProfessionalRole { Id = Guid.NewGuid(), Category = ProfessionalCategory.Tech, Description = "Simulation Technician" };
                var allRoles = medRoles.Concat(nurseRoles).Concat(new[] { techRole }).ToDictionary(rl => rl.Description);

                context.ProfessionalRoles.AddRange(allRoles.Values);

                foreach (var rl in allRoles.Values)
                {
                    starship.ProfessionalRoleInstitutions.Add(new ProfessionalRoleInstitution { ProfessionalRole = rl });
                    mmh.ProfessionalRoleInstitutions.Add(new ProfessionalRoleInstitution { ProfessionalRole = rl });
                }

                var trish = new Participant { Id = Guid.NewGuid(), Email = "trishw@adhb.govt.nz", FullName = "Trish Wood", PhoneNumber = "021 764 235", ProfessionalRole = allRoles["Nurse Educator"], Department = ced };
                var brent = new Participant { Id = Guid.NewGuid(), Email = "brentm@adhb.govt.nz", AlternateEmail = "brent@focused-light.net", FullName = "Brent McSharry", PhoneNumber = "021 245 9769", ProfessionalRole = allRoles["Consultant"], Department = picu };
                var denish = new Participant { Id = Guid.NewGuid(), Email = "denishk@adhb.govt.nz", FullName = "Denish Kumar", PhoneNumber = "021 716 641", ProfessionalRole = allRoles["Simulation Technician"], Department = simProgram };
                var becs = new Participant { Id = Guid.NewGuid(), Email = "RebeccaFl@adhb.govt.nz", FullName = "Rebecca (Becs) Flanagan", PhoneNumber = "021 829 662", ProfessionalRole = allRoles["Nurse Educator"], Department = simProgram };
                var carl = new Participant { Id = Guid.NewGuid(), Email = "Carl.Horsley@cmhb.org.nz", FullName = "Carl Horsley", ProfessionalRole = allRoles["Consultant"], Department = mmIcu };
                foreach (var u in new[] { trish, brent, denish, becs, carl })
                {
                    context.Users.Add(u);
                }

                context.SaveChanges();
                var laerdal = new ManikinManufacturer { Id = Guid.NewGuid(), Name = "Laerdal" };
                context.ManikinManufacturers.Add(laerdal);
                var junior = new ManikinModel { Id = Guid.NewGuid(), Description = "Sim Junior", Manufacturer = laerdal };
                var man = new ManikinModel { Id = Guid.NewGuid(), Description = "Sim Man", Manufacturer = laerdal };
                var baby = new ManikinModel { Id = Guid.NewGuid(), Description = "Sim Baby", Manufacturer = laerdal };
                var newbie = new ManikinModel { Id = Guid.NewGuid(), Description = "Sim Newborn", Manufacturer = laerdal };
                context.ManikinModels.AddRange(new[] { junior, man, baby, newbie });

                var cedJunior = new Manikin { Id = Guid.NewGuid(), Department = ced, Description = "'burnie'", Model = junior, PurchasedNew = true };
                context.Manikins.Add(cedJunior);
                var crm = new CourseType { Id = Guid.NewGuid(), Abbreviation = "CRM", Description = "Crisis Resourse Managment", EmersionCategory = Emersion.Emersive, CourseTypeScenarioRoles = new List<CourseTypeScenarioRole>() };
                context.CourseTypes.Add(crm);

                var crm2 = new CourseFormat { Id = Guid.NewGuid(), DaysDuration = 1, Description = "2 Scenario", CourseType = crm };

                //eventually resource filename should belong to each department

                var didactic = new CourseActivity { Id = Guid.NewGuid(), Name = "Didactic Lecture", CourseType = crm };
                context.CourseActivities.Add(didactic);
                var slides = new Activity { Id = Guid.NewGuid(), Description = "PICU 2016 version", FileName = @"C:\whatever\Slides.ppt", CourseActivity = didactic };
                context.Activities.Add(slides);
                var didacticSlot = new CourseSlot { Id = Guid.NewGuid(), Day = 1, MinutesDuration = 20, Activity = didactic, Order = 0, CourseFormat = crm2, IsActive = true, SimultaneousStreams = 1 };
                didactic.ActivityChoices.Add(slides);
                context.CourseSlots.Add(didacticSlot);

                var teamBuilder = new CourseActivity { Id = Guid.NewGuid(), Name = "Team Building", CourseType = crm };
                context.CourseActivities.Add(teamBuilder);

                var ballGame = new Activity { Id = Guid.NewGuid(), Description = "Multi-sized balls", CourseActivity = teamBuilder };
                var eggGame = new Activity { Id = Guid.NewGuid(), Description = "Egg, plate, ribons", CourseActivity = teamBuilder };
                var solarGame = new Activity { Id = Guid.NewGuid(), Description = "Solar Blanket", CourseActivity = teamBuilder };
                context.Activities.AddRange(new[] { ballGame, eggGame, solarGame });

                var teamSlot = new CourseSlot { Id = Guid.NewGuid(), Day = 1, MinutesDuration = 20, Activity = teamBuilder, Order = 1, CourseFormat = crm2, IsActive = true, SimultaneousStreams = 1 };
                context.CourseSlots.Add(teamSlot);

                teamBuilder.ActivityChoices.Add(ballGame);
                teamBuilder.ActivityChoices.Add(eggGame);
                teamBuilder.ActivityChoices.Add(solarGame);

                var sim1 = new CourseSlot { Id = Guid.NewGuid(), Day = 1, MinutesDuration = 40, Order = 2, CourseFormat = crm2, IsActive = true, SimultaneousStreams = 1 };
                context.CourseSlots.Add(sim1);

                var coffee = new CourseActivity { Id = Guid.NewGuid(), Name = "Coffee Break", CourseType = crm };
                var coffeeSlot = new CourseSlot { Id = Guid.NewGuid(), Day = 1, MinutesDuration = 20, Activity = coffee, Order = 3, CourseFormat = crm2, IsActive = true, SimultaneousStreams = 1 };
                context.CourseActivities.Add(coffee);
                context.CourseSlots.Add(coffeeSlot);

                var d = new FacultyScenarioRole { Id = Guid.NewGuid(), Description = "Director", Order = 0 };
                var lt = new FacultyScenarioRole { Id = Guid.NewGuid(), Description = "Tech", Order = 1 };
                var r = new FacultyScenarioRole { Id = Guid.NewGuid(), Description = "Runner", Order = 2 };
                var ld = new FacultyScenarioRole { Id = Guid.NewGuid(), Description = "Lead Debrief", Order = 3 };
                var ad = new FacultyScenarioRole { Id = Guid.NewGuid(), Description = "Assistant Debrief", Order = 4 };
                crm.CourseTypeScenarioRoles.Add(new CourseTypeScenarioRole { FacultyScenarioRole = ld });
                crm.CourseTypeScenarioRoles.Add(new CourseTypeScenarioRole { FacultyScenarioRole = ad });
                crm.CourseTypeScenarioRoles.Add(new CourseTypeScenarioRole { FacultyScenarioRole = lt });
                crm.CourseTypeScenarioRoles.Add(new CourseTypeScenarioRole { FacultyScenarioRole = d });
                crm.CourseTypeScenarioRoles.Add(new CourseTypeScenarioRole { FacultyScenarioRole = r });
            }
            if (!context.HotDrinks.Any())
            {
                context.HotDrinks.AddRange(
                    new[] { "Flat White", "Cafe Latte", "Mocha", "Hot Chocolate", "Piccolo Latte", "Machiato" }
                        .Select(c=>new[] { c, "Trim " + c})
                        .SelectMany(c=>c)
                        .Concat(new[] { "Long Black", "Short Black", "Ristretto" })
                        .Select(c=> new HotDrink { Description=c, Id=Guid.NewGuid() }));
            }

            var roles = new[] { new ProfessionalRole { Category = ProfessionalCategory.Actor, Description = "Actor" },
                new ProfessionalRole { Category = ProfessionalCategory.Other, Description = "Other" },
                new ProfessionalRole { Category = ProfessionalCategory.Educator, Description = "Educator" },
                new ProfessionalRole { Category = ProfessionalCategory.Allied, Description = "Perfusionist" },
                new ProfessionalRole { Category = ProfessionalCategory.Administrative, Description = "Team Administrator" }
            };

            var roleNames = roles.Select(r => r.Description);
            var existingRoles = new HashSet<string>(from r in context.ProfessionalRoles where roleNames.Contains(r.Description) select r.Description);

            foreach (var r in roles.Where(r => !existingRoles.Contains(r.Description)))
            {
                r.Id = Guid.NewGuid();
                context.ProfessionalRoles.Add(r);
            }

            foreach (var r in RoleConstants.RoleNames.Select(rn=>new AspNetRole { Id = rn.Key, Name = rn.Value}))
            {
                context.Roles.AddOrUpdate(r);
            }
            //context.Roles.AddOrUpdate(new AspNetRole { Id = Guid.ParseExact(RoleConstants.AdminApprovedId,RoleConstants.IdFormat), Name = RoleConstants.AdminApproved});
            context.SaveChanges();

            context.Database.ExecuteSqlCommand("UPDATE [dbo].[Courses] SET[CourseDatesLastModified] = [CreatedUtc] ,[FacultyMeetingDatesLastModified] = [CreatedUtc] WHERE YEAR([CourseDatesLastModified]) = 1900");
        }
    }
}
