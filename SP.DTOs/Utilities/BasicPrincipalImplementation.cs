using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace SP.Dto.Utilities
{
    public class BasicPrincipalImplementation : GenericPrincipal
    {
        public BasicPrincipalImplementation(string userName, IEnumerable<string> roleNames) 
            : base(new GenericIdentity(userName), roleNames.ToArray())
        {
        }
    }
}