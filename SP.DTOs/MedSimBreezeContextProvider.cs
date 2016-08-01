using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using Newtonsoft.Json.Linq;
using SP.DataAccess;
using System;

namespace SP.Dto
{
    class MedSimBreezeContextProvider : EFContextProvider<MedSimDbContext>
    {
        protected override EntityInfo CreateEntityInfoFromJson(JTokenReader reader, Type entityType)
        {
            EntityInfo ent = base.CreateEntityInfoFromJson(reader, entityType);
            

        }

        static object MapFromDto(object obj)
        {
            
        }
    }
}
