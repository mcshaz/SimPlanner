using SP.DataAccess;
using SP.Dto.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;

namespace SP.Dto.ProcessBreezeRequests
{
    internal enum AdminLevels { None = 0, DepartmentAdmin=1, InstitutionAdmin = 2, AllData = 3 };
    public class CurrentPrincipal : IDisposable
    { 
        private IEnumerable<Guid> _userDepartmentAdminIds;
        private Guid? _userInstitutionId;
        private Participant _user;
        private MedSimDbContext _context;
        private bool _contextOwner;

        public CurrentPrincipal(IPrincipal principal, MedSimDbContext context = null)
        {
            _context = context;
            _contextOwner = context == null;
            if (principal.Identity.IsAuthenticated)
            {
                AdminLevel = GetLevel(principal);
                PrincipalName = principal.Identity.Name;
                IsSiteAdmin = principal.IsInRole(RoleConstants.SiteAdmin);
            }
        }

        internal AdminLevels GetAdminLevelForUser(Guid userId)
        {
            var usr = Context.Users.Include(u => u.Roles).First(u => u.Id == userId);
            var p = new BasicPrincipalImplementation(usr.UserName, usr.Roles.Select(r => RoleConstants.RoleNames[r.RoleId]));
            return GetLevel(p);
        }

        private static AdminLevels GetLevel(IPrincipal principal){
            if (principal.IsInRole(RoleConstants.AccessAllData))
            {
                return AdminLevels.AllData;
            }
            else if (principal.IsInRole(RoleConstants.AccessInstitution))
            {
                return AdminLevels.InstitutionAdmin;
            }
            else if (principal.IsInRole(RoleConstants.AccessDepartment))
            {
                return AdminLevels.DepartmentAdmin;
            }
            return AdminLevels.None;
        }

        public virtual MedSimDbContext Context { get { return _context ?? (_context = new MedSimDbContext()); } }
        internal virtual string PrincipalName { get; private set; }
        internal virtual bool IsSiteAdmin { get; private set; }
        internal virtual Guid UserInstitutionId
        {
            get
            {
                if (!_userInstitutionId.HasValue)
                {
                    _userInstitutionId = Principal.Department.InstitutionId;
                }
                return _userInstitutionId.Value;
            }
        }
        internal virtual Participant Principal
        {
            get
            {
                return _user ??
                    (_user = GetUser(PrincipalName));
            }
        }
        internal virtual Participant GetUser(string userName)
        {
            return Context.Users.Local.FirstOrDefault(u => u.UserName == userName)
                        ?? Context.Users.First(u => u.UserName == userName);
        }
        internal virtual IEnumerable<Guid> UserDepartmentAdminIds
        {
            get
            {
                return _userDepartmentAdminIds ?? (_userDepartmentAdminIds = GetDepartmentAdminIdsForUser(Principal.Id, AdminLevel));
            }
        }
        internal IEnumerable<Guid> GetDepartmentAdminIdsForUser(Guid userId, AdminLevels? admin = null)
        {
            switch (admin ?? GetAdminLevelForUser(userId))
            {
                case AdminLevels.None:
                    return new Guid[0];
                case AdminLevels.DepartmentAdmin:
                    return new Guid[] {Context.Users.Find(userId).DefaultDepartmentId };
                case AdminLevels.InstitutionAdmin:
                    return (from u in Context.Users
                            where u.Id == userId
                            from d in u.Department.Institution.Departments
                            select d.Id).ToList();
                default:
                    throw new NotImplementedException("UserDepartmentAccess property only implemented for none, InstAdmin and DptAdmin");
            }
        }
        internal virtual AdminLevels AdminLevel { get; private set; }

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~CurrentPrincipal() { Dispose(false); }
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
