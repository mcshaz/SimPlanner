using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using Ganss.XSS;
using System;

namespace SP.DataAccess
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
                    Type t = e.Entity.GetType();
                    foreach (var p in t.GetProperties())
                    {
                        if (p.PropertyType == typeof(string))
                        {
                            string val = (string)p.GetValue(e.Entity);
                            if (!string.IsNullOrWhiteSpace(val))
                            {
                                p.SetValue(e.Entity, Sanitizer.Sanitize(val));
                            }
                        }
                    }
                    if(t == typeof(Course))
                    {
                        var course = (Course)e.Entity;
                        var now = DateTime.UtcNow;
                        course.LastModified = now;
                        if (e.State == EntityState.Added)
                        {
                            course.Created = now;
                        }
                    }
                }

            }
        }
    }
}
