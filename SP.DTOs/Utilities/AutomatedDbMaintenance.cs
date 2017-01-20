using SP.DataAccess;
using System;
using System.Linq;

namespace SP.DTOs.Utilities
{
    public static class AutomatedDbMaintenance
    {
        public static void DeleteOrphans()
        {
            using (var db = new MedSimDbContext())
            {
                var threeWeeksPrior = DateTime.UtcNow.AddDays(-21);
                var dpts = db.Departments.Include("Participants").Where(d => !d.AdminApproved && d.CreatedUtc < threeWeeksPrior && !d.Participants.Any()).ToList();
                db.Departments.RemoveRange(dpts);
                db.SaveChanges();
                var insts = db.Institutions.Where(i => !i.AdminApproved && i.CreatedUtc < threeWeeksPrior && !i.Departments.Any());
                db.Institutions.RemoveRange(insts);
                db.SaveChanges();
            }
        }
    }
}
