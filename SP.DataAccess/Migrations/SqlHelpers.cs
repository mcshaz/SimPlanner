using SP.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SP.DataAccess.Migrations
{
    public static class SqlHelpers
    {
        internal static string DropConstraintIfExists(string tableName, string constraintName )
        {
            return $"IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = '{constraintName}') "
                + $"ALTER TABLE {tableName} DROP CONSTRAINT [{constraintName}];\r\n"
                + $"IF EXISTS (SELECT 1 FROM sys.indexes WHERE name='{constraintName}' AND object_id = OBJECT_ID('[dbo].[{constraintName}]')) "
                + $"DROP INDEX [{constraintName}] ON [dbo].[{tableName}];";
        }

        public static string CreateUniqueConstraint<T>(string tableName, params Expression<Func<T,object>>[] expressions)
        {
            List<string> nullPropNames = new List<string>();
            List<string> notNullPropNames = new List<string>();
            foreach (var expr in expressions)
            {
                var un = expr.Body as UnaryExpression;
                var me = un==null
                    ?(MemberExpression)expr.Body
                    :(MemberExpression)un.Operand;
                //string name = me.Member.Name;
                var pi = (PropertyInfo)me.Member;
                var type = pi.PropertyType;
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    nullPropNames.Add(pi.Name);
                } 
                else if (type==typeof(string))
                {
                    if (Attribute.IsDefined(pi, typeof(RequiredAttribute)))
                    {
                        notNullPropNames.Add(pi.Name);
                    }
                    else
                    {
                        var metaAttr = typeof(T).GetCustomAttributes(typeof(MetadataTypeAttribute), true).Cast<MetadataTypeAttribute>().FirstOrDefault();
                        if (metaAttr==null)
                        {
                            nullPropNames.Add(pi.Name);
                        }
                        else
                        {
                            var metaPi = metaAttr.MetadataClassType.GetProperty(pi.Name);
                            if (metaPi != null
                                && (Attribute.IsDefined(metaPi, typeof(RequiredAttribute))
                                    || Attribute.IsDefined(metaPi, typeof(FixedLengthAttribute))))
                            {
                                notNullPropNames.Add(pi.Name);
                            }
                            else
                            {
                                nullPropNames.Add(pi.Name);
                            }
                        }
                    }
                }
                else
                {
                    notNullPropNames.Add(pi.Name);
                }
            }
            return CreateUniqueConstraint(tableName, notNullPropNames, nullPropNames);
        }

        internal static string CreateUniqueConstraint(string tableName, IEnumerable<string> notNullPropertyNames, IEnumerable<string> nullPropertyNames)
        {
            var propertyNames = notNullPropertyNames.Concat(nullPropertyNames);
            string constraintName = $"Unique_{ tableName}_{string.Join(string.Empty, propertyNames)}";
            string columns = '[' + string.Join("],[", propertyNames) + ']';
            if (nullPropertyNames.Any())
            {
                string nullClause = string.Join(" AND ", nullPropertyNames.Select(p => $"[{p}] IS NOT NULL"));
                return $"CREATE UNIQUE NONCLUSTERED INDEX {constraintName} ON dbo.[{tableName}]({columns}) WHERE { nullClause };";
            }
            return $"ALTER TABLE dbo.[{tableName}] ADD CONSTRAINT {constraintName} UNIQUE({columns});";
        }

        internal static string GetTableName(Type type, DbContext context)
        {
            var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;

            // Get the part of the model that contains info about the actual CLR types
            var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

            // Get the entity type from the model that maps to the CLR type
            var entityType = metadata
                    .GetItems<EntityType>(DataSpace.OSpace)
                    .Single(e => objectItemCollection.GetClrType(e) == type);

            // Get the entity set that uses this entity type
            var entitySet = metadata
                .GetItems<EntityContainer>(DataSpace.CSpace)
                .Single()
                .EntitySets
                .Single(s => s.ElementType.Name == entityType.Name);

            // Find the mapping between conceptual and storage model for this entity set
            var mapping = metadata.GetItems<EntityContainerMapping>(DataSpace.CSSpace)
                    .Single()
                    .EntitySetMappings
                    .Single(s => s.EntitySet == entitySet);

            // Find the storage entity set (table) that the entity is mapped
            var table = mapping
                .EntityTypeMappings.Single()
                .Fragments.Single()
                .StoreEntitySet;

            // Return the table name from the storage entity set
            return (string)table.MetadataProperties["Table"].Value ?? table.Name;
        }
    }
}
