using SP.Dto.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SP.Dto
{
    [MetadataType(typeof(RoleDtoMetadata))]
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<UserRoleDto> UserRoles { get; set; }
    }
}
