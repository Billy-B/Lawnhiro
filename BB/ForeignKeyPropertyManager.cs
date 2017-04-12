using DatabaseManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal class ForeignKeyPropertyManager : JoinedPropertyManager
    {
        public IForeignKeyConstraint ForeignKey { get; private set; }

        internal ForeignKeyAttribute FkAttribute { get; set; }

        internal override void Initialize()
        {
            base.Initialize();
            if (IsValid)
            {
                bool isValid;
                IForeignKeyConstraint constraint = null;
                string fkName = FkAttribute.Name;
                TableBoundClassManager other = TypeManager.GetManager(Property.PropertyType) as TableBoundClassManager;
                other.EnsureInitialized();
                ITable referencedTable = other.Table;
                if (fkName == null)
                {
                    if (referencedTable == null)
                    {
                        throw new InvalidOperationException("Table is null on initialized manager?");
                    }
                    IForeignKeyConstraint[] possibleConstraints = Table.Constraints.OfType<IForeignKeyConstraint>().Where(fk => fk.ReferencedTable == referencedTable).ToArray();
                    if (possibleConstraints.Length == 0)
                    {
                        isValid = false;
                        _initErrorMessage = "Cannot determine foreign key to use. No foreign keys on table " + Table + " reference table " + referencedTable + ".";
                    }
                    else if (possibleConstraints.Length == 1)
                    {
                        constraint = possibleConstraints[0];
                        isValid = true;
                    }
                    else
                    {
                        constraint = possibleConstraints.SingleOrDefault(c => c.Name == Property.Name);
                        if (constraint == null)
                        {
                            isValid = false;
                            _initErrorMessage = "Multiple foreign key constraints on table " + Table + " reference table " + referencedTable + ", none of which are named " + Property.Name + ". Must provide name of foreign key in the ForeignKeyAttribute.";
                        }
                        else
                        {
                            isValid = true;
                        }
                    }
                }
                else
                {
                    constraint = Table.Database.GetObjectByName(fkName) as IForeignKeyConstraint;
                    if (constraint == null)
                    {
                        isValid = false;
                        _initErrorMessage = "No foreign key constraint \"" + fkName + "\" in database " + Table.Database + ".";
                    }
                    else if (constraint.Table == Table)
                    {
                        if (constraint.ReferencedTable == referencedTable)
                        {
                            isValid = true;
                        }
                        else
                        {
                            isValid = false;
                            _initErrorMessage = "Foreign key constraint " + constraint + " does not reference table " + referencedTable + ".";
                        }
                    }
                    else
                    {
                        isValid = false;
                        _initErrorMessage = "Foreign key constraint " + constraint + " is not on table " + Table + ".";
                    }
                }
                if (isValid)
                {
                    ForeignKeyColumns = constraint.ForeignKeyColumns.ToList();
                    ReferencedColumns = constraint.ReferencedColumns.ToList();
                    ColumnsAllowNull = AllowNull = ReferencedColumns.Any(c => c.IsNullable);
                }
                IsValid = isValid;
            }
        }
    }

    /*internal abstract class ForeignKeyPropertyManager<TProperty> : PropertyManager<TProperty>
        where TProperty : class
    {
        public IForeignKeyConstraint ForeignKeyConstraint { get; private set; }

        public IUniqueConstraint ReferencingConstraint { get; private set; }

        public DatabaseTableBoundTypeManager<TProperty> Manager { get; private set; }

        public override TProperty FetchValue(object dataSource)
        {
            DatabaseDataRow row = (DatabaseDataRow)dataSource;
            DatabaseDataRow joinedRow = row.GetJoinedRow(ForeignKeyConstraint);
            if (joinedRow == null)
            {
                IReadOnlyList<IColumn> foreignKeyColumns = ForeignKeyConstraint.ForeignKeyColumns;
                object[] primaryKey = new object[foreignKeyColumns.Count];
                for (int i = 0; i < primaryKey.Length; i++)
                {
                    primaryKey[i] = row[foreignKeyColumns[i]];
                }
                return Manager.LookupByUniqueKey(ReferencingConstraint, primaryKey);
            }
            throw new NotImplementedException();
        }
    }*/
}
