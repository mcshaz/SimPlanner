using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.DataAccess.Migrations
{
    internal static class DropConstraintHelper
    {
        internal enum ConstraintType { ForeignKey, Check, CheckUniqueFkPK }
        internal static string DropIfExists(string tableName, string constraintName, ConstraintType constraint = ConstraintType.CheckUniqueFkPK)
        {
            string constraintSchema;
            switch (constraint)
            {
                case ConstraintType.Check:
                    constraintSchema = "CHECK_CONSTRAINTS";
                    break;
                case ConstraintType.ForeignKey:
                    constraintSchema = "REFERENTIAL_CONSTRAINTS";
                    break;
                default:
                    constraintSchema = "TABLE_CONSTRAINTS";
                    break;
            }
            return $"IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.{constraintSchema} WHERE CONSTRAINT_NAME = '{constraintName}') "
                + $" ALTER TABLE {tableName} DROP CONSTRAINT {constraintName}";
        }
    }
}
