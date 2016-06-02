using System;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SP.Metadata
{

    public class FixedLengthAttribute : Attribute
    {
        public int Length { get; set; }
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
