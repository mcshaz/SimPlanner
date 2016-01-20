using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.DataAccess
{
    class InitialiseMedSim : DropCreateDatabaseAlways<MedSimDbContext>
    {
        protected override void Seed(MedSimDbContext context)
        {
            var country = new Country { Code = "NZ", Name = "New Zealand" };
            context.Countries.Add(country);
            var starship = new Hospital { Country = country, Name="Starship" };
            context.Hospitals.Add(starship);
            var ced = new Department { Hospital = starship, Name = "CED" };
            context.Departments.Add(ced);
            var consultantRole = new ProfessionalRole { Category = ProfessionalCategory.Medical, Description = "Consulant" };
            context.ProfessionalRoles.Add(consultantRole);
            var nursingRole = new ProfessionalRole { Category = ProfessionalCategory.Medical, Description = "Clinical Charge Nurse" };
            context.ProfessionalRoles.Add(nursingRole);
            var techRole = new ProfessionalRole { Category = ProfessionalCategory.Medical, Description = "Simulation Technician" };
            context.ProfessionalRoles.Add(techRole);
            //todo add Mike,Denish,Becks & check trish phone no and 2nd email 
            var trish = new Participant { Email = "trishw@adhb.govt.nz", Name = "Trish Wood", PhoneNumber = "999 9999 99", ProfessionalRole = nursingRole };
            context.Participants.Add(trish);
            var course1 = new InstructorCourse { Description = "Dummy Course", StartDate = DateTime.Now, MaximumParticipants = 20 };
            context.InstructorCourses.Add(course1);

            //to add manequins
        }
    }
}
