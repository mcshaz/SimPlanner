using SM.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.Web.Controllers
{
    public class LookupBundle
    {
        //commented out because departments are now sent as collection with associated institution
        //public IEnumerable<Department> Departments { get; set; }
        public IEnumerable<CourseType> CourseTypes { get; set; }
        public Institution UserInstitution { get; set; }
        public IEnumerable<ProfessionalRole> ProfessionalRoles { get; set; }
    }
}
