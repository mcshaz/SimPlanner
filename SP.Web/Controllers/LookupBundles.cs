using SP.DataAccess;
using SP.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Web.Controllers
{
    public class LookupBundle
    {
        //commented out because departments are now sent as collection with associated institution
        //public IEnumerable<Department> Departments { get; set; }
        public IEnumerable<CourseTypeDto> CourseTypes { get; set; }
        public IEnumerable<InstitutionDto> Institutions { get; set; }
        public IEnumerable<ProfessionalRoleDto> ProfessionalRoles { get; set; }
        public IEnumerable<ManequinManufacturerDto> ManequinManufacturers {get; set;}
        public IEnumerable<ManequinDto> Manequins { get; set; }
    }

}
