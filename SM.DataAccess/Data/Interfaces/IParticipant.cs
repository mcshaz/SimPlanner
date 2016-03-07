using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SM.DataAccess.Data.Interfaces
{
    public interface IParticipant
    {
        Guid Id { get; set; }
        string PhoneNumber { get; set; }
    }
}
