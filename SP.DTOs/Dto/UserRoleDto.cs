using SP.Dto.Metadata;
using System;
using System.ComponentModel.DataAnnotations;

namespace SP.Dto
{
    [MetadataType(typeof(UserRoleDtoMetadata))]
    public class UserRoleDto
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }

        public virtual ParticipantDto User { get; set; }
        public virtual RoleDto Role { get; set; }
    }
}
