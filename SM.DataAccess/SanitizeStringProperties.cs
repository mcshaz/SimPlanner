using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Ganss.XSS;
namespace SM.DataAccess
{
    public class SanitizeStringProperties
    {
        private HtmlSanitizer _sanitizer;
        private HtmlSanitizer Sanitizer
        {
            get { return _sanitizer ?? (_sanitizer = new HtmlSanitizer()); }
        }

        public void ForEntities(DbChangeTracker tracker)
        {
            foreach (var e in tracker.Entries())
            {
                if (e.State == EntityState.Added || e.State == EntityState.Modified)
                {
                    foreach(var p in e.Entity.GetType().GetProperties())
                    {
                        if (p.PropertyType == typeof(string))
                        {
                            p.SetValue(e,Sanitizer.Sanitize((string)p.GetValue(e)));
                        }
                    }
                }

            }
        }
    }
}
