using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace SP.Dto.Utilities
{
    public class BasicPrincipalImplementation : IPrincipal
    {
        public BasicPrincipalImplementation(string userName, IEnumerable<string> roleNames)
        {
            _roleNames = roleNames.ToArray();
            Identity = new BasicIdentityImplementation
            {
                Name = userName
            };
        }

        private readonly string[] _roleNames;
        public IIdentity Identity { get; private set; }
        public bool IsInRole(string role) { return _roleNames.Contains(role); }
    }

    internal class BasicIdentityImplementation : IIdentity
    {
        public string Name { get; set; }
        public string AuthenticationType { get { return nameof(BasicIdentityImplementation);  } }
        public bool IsAuthenticated { get { return true; } }
    }
}