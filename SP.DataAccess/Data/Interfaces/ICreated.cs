using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.DataAccess.Data.Interfaces
{
    public interface ICreated
    {
        DateTime CreatedUtc { get; set; }
    }
}
