using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Dto
{
    public class HotDrinkDto
    {
        public Guid Id { get;  set; }
        public string Description { get; set; }

        public virtual ICollection<ParticipantDto> Participants { get; set; }
    }
}
