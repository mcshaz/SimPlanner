using System;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SM.Metadata
{

    public class FixedLengthAttribute : Attribute
    {
        public ushort Length { get; set; }
    }

    public class FixedLengthAttributeConvention
        : PrimitivePropertyAttributeConfigurationConvention<FixedLengthAttribute>
    {
        public override void Apply(ConventionPrimitivePropertyConfiguration configuration,
            FixedLengthAttribute attribute)
        {
            configuration.IsFixedLength();
            configuration.HasMaxLength(attribute.Length);
        }
    }


}
