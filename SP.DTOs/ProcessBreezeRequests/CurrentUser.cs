using SP.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace SP.Dto.ProcessBreezeRequests
{
    internal enum AdminLevels { None = 0, DepartmentAdmin=1, InstitutionAdmin = 2, AllData = 3 };
    public class CurrentUser : IDisposable
    { 
        private IEnumerable<Guid> _userDepartmentAdminIds;
        private Guid? _userInstitutionId;
        private Participant _user;
        private MedSimDbContext _context;
        private bool _contextOwner;

        public CurrentUser(IPrincipal user, MedSimDbContext context = null)
        {
            _context = context;
            _contextOwner = context == null;
            if (user.Identity.IsAuthenticated)
            {
                if (user.IsInRole(RoleConstants.AccessAllData))
                {
                    AdminLevel = AdminLevels.AllData;
                }
                else if (user.IsInRole(RoleConstants.AccessInstitution))
                {
                    AdminLevel = AdminLevels.InstitutionAdmin;
                }
                else if (user.IsInRole(RoleConstants.AccessDepartment))
                {
                    AdminLevel = AdminLevels.DepartmentAdmin;
                }
                UserName = user.Identity.Name;
                IsSiteAdmin = user.IsInRole(RoleConstants.SiteAdmin);
            }
        }

        public virtual MedSimDbContext Context { get { return _context ?? (_context = new MedSimDbContext()); } }
        internal virtual string UserName { get; private set; }
        internal virtual bool IsSiteAdmin { get; private set; }
        internal virtual Guid UserInstitutionId
        {
            get
            {
                if (!_userInstitutionId.HasValue)
                {
                    _userInstitutionId = User.Department.InstitutionId;
                }
                return _userInstitutionId.Value;
            }
        }
        internal virtual Participant User
        {
            get
            {
                return _user ??
                    (_user = Context.Users.Local.FirstOrDefault(u => u.UserName == UserName)
                        ?? Context.Users.First(u => u.UserName == UserName));
            }
        }
        internal virtual IEnumerable<Guid> UserDepartmentAdminIds
        {
            get
            {
                if (_userDepartmentAdminIds == null)
                {
                    switch (AdminLevel)
                    {
                        case AdminLevels.None:
                            _userDepartmentAdminIds = new Guid[0];
                            break;
                        case AdminLevels.DepartmentAdmin:
                            _userDepartmentAdminIds = new Guid[] {
                                (from u in Context.Users
                                 where u.UserName == UserName
                                 select u.DefaultDepartmentId).First()
                            };
                            break;
                        case AdminLevels.InstitutionAdmin:
                            _userDepartmentAdminIds = (from u in Context.Users
                                                       where u.UserName == UserName
                                                       from d in u.Department.Institution.Departments
                                                       select d.Id).ToList();
                            break;
                        default:
                            throw new NotImplementedException("UserDepartmentAccess property only impleented for none, InstAdmin and DptAdmin");
                    }

                }
                return _userDepartmentAdminIds;
            }
        }

        internal virtual AdminLevels AdminLevel { get; private set; }

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~CurrentUser() { Dispose(false); }
        void Dispose(bool disposing)
        { // would be protected virtual if not sealed 
            if (disposing && _context != null && _contextOwner)
            { // only run this logic when Dispose is called
                _context.Dispose();
            }
        }
        #endregion //IDisposable
    }
}
