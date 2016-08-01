using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using Ganss.XSS;
using System;
using System.Reflection;
using System.Linq;

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
            foreach (var e in tracker.Entries().Where(te => te.State == EntityState.Added || te.State == EntityState.Modified).ToLookup(te=>te.Entity.GetType()))
            {
                foreach (var p in e.Key.GetProperties(BindingFlags.DeclaredOnly |
                                        BindingFlags.Public |
                                        BindingFlags.Instance).Where(p=>p.PropertyType == typeof(string)))
                {
                    foreach (var ent in e)
                    {
                        string val = ent.CurrentValues.GetValue<string>(p.Name);
                        if (val != null)
                        {
                            val = val.Trim();
                            if (val != string.Empty)
                            {
                                val = Sanitizer.Sanitize(val);
                            }
                            ent.CurrentValues[p.Name] = val;
                        }
                    }
                }

            }
        }
    }
}
