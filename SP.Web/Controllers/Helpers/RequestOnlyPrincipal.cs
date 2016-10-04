using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace SP.Web.Controllers.Helpers
{
    public class RequestOnlyPrincipal : IPrincipal
    {
        public RequestOnlyPrincipal(string userName, IEnumerable<string> roleNames)
        {
            _roleNames = roleNames.ToArray();
            Identity = new RequestOnlyIdentity
            {
                Name = userName
            };
        }

        private readonly string[] _roleNames;
        public IIdentity Identity { get; private set; }
        public bool IsInRole(string role) { return _roleNames.Contains(role); }
    }

    internal class RequestOnlyIdentity : IIdentity
    {
        public string Name { get; set; }
        public string AuthenticationType { get { return "CustomRequestOnly";  } }
        public bool IsAuthenticated { get { return true; } }
    }
}