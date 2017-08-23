using DatabaseManagement;
using DatabaseManagement.SQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace BB
{
    internal class TableEnumManager : EnumManager
    {
        private TableAttribute _tableAttribute;

        public ITable Table { get; private set; }

        public IColumn IdColumn { get; private set; }

        public IColumn NameColumn { get; private set; }

        private SelectStatement _selectStatement;

        private DatabaseRepository _repository;

        internal override IObjectRepository Repository
        {
            get { return _repository; }
        }

        internal TableEnumManager(TableAttribute att)
        {
            _tableAttribute = att;
        }

        public override IEnumerable EnumerateValues()
        {
            throw new NotImplementedException();
        }

        internal override void OnInitialize()
        {
            string tableName = _tableAttribute.Name ?? Type.Name;
            ITable table = AssemblyManager.GetTable(tableName);
            if (table == null)
            {
                throw new ArgumentException("Cannot find table \"" + tableName + "\".");
            }
            _repository = DatabaseRepository.GetRepository(table.Database);
            string idColName = _tableAttribute.IdColumn;
            string nameColName = _tableAttribute.NameColumn;
            IColumn idColumn, nameColumn;
            IUniqueConstraint primaryKey = table.PrimaryKey;
            if (primaryKey == null)
            {
                throw bindingError(Type, table, "Table has no primary key.");
            }
            if (primaryKey.ConstrainedColumns.Count != 1)
            {
                throw bindingError(Type, table, "Primary key " + primaryKey + " is on more than one column.");
            }
            idColumn = primaryKey.ConstrainedColumns[0];
            if (!isValidIdColumn(idColumn))
            {
                throw bindingError(Type, table, "Column " + idColumn + " holds data type that cannot be converted to enum's underlying type.");
            }
            if (nameColName == null)
            {
                IColumn[] candidateColumns = table.Columns.Where(c => c != idColumn && isValidDescriptionColumn(c)).ToArray();
                if (candidateColumns.Length == 1)
                {
                    nameColumn = candidateColumns[0];
                }
                else if (candidateColumns.Length == 0)
                {
                    throw bindingError(Type, table, "No candidate columns found to supply enum name. Column must be non-nullable and hold data type which can be converted to System.String.");
                }
                else
                {
                    IColumn[] uniqueColumns = candidateColumns.Where(col => table.UniqueConstraints.Any(u => u.ConstrainedColumns.Count == 1 && u.ConstrainedColumns[0] == col)).ToArray();
                    if (uniqueColumns.Any())
                    {
                        nameColumn = uniqueColumns.OrderBy(c => c.OrdinalPosition).First();
                    }
                    else
                    {
                        nameColumn = candidateColumns.OrderBy(c => c.OrdinalPosition).First();
                    }
                }
            }
            else
            {
                nameColumn = table.GetColumnByName(nameColName);
                if (nameColumn == null)
                {
                    throw bindingError(Type, table, "No column " + nameColName + " in table " + table + ".");
                }
                if (!isValidDescriptionColumn(nameColumn))
                {
                    throw bindingError(Type, table, "Column " + nameColumn + " is not valid for enum name. Column must be non-nullable and hold data type which can be converted to System.String.");
                }
            }
            IdColumn = idColumn;
            NameColumn = nameColumn;
            ScalarExpression[] selectExpressions = 
            {
                Expression.Column(idColumn),
                Expression.Column(nameColumn)
            };
            _selectStatement = Statement.SelectFrom(Expression.Table(table), selectExpressions);
            Refresh();
        }

        public void Refresh()
        {
            throw new NotImplementedException();
            /*IColumn idColumn = IdColumn;
            IColumn nameColumn = NameColumn;
            DatabaseDataRow[] rows = _repository.EnumerateRows(_selectStatement).ToArray();
            ulong[] values = new ulong[rows.Length];
            string[] names = new string[rows.Length];
            for (int i = 0; i < rows.Length; i++)
            {
                values[i] = Convert.ToUInt64(rows[i][idColumn]);
                names[i] = (string)rows[i][nameColumn];
            }
            SetValuesAndNames(values, names);*/
        }

        private static Exception bindingError(Type type, ITable table, string message)
        {
            return new InvalidOperationException("Cannot bind enum " + type + " to table " + table + ". " + message);
        }

        private static bool isValidIdColumn(IColumn col)
        {
            switch (col.DbType)
            {
                case DbType.Byte:
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                case DbType.SByte:
                case DbType.UInt16:
                case DbType.UInt32:
                case DbType.UInt64:
                    return true;
                case DbType.AnsiStringFixedLength:
                case DbType.StringFixedLength:
                    return col.CharacterMaxLength == 1;
                default:
                    return false;
            }
        }

        private static bool isValidDescriptionColumn(IColumn col)
        {
            if (col.IsNullable)
            {
                return false;
            }
            switch (col.DbType)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                    return true;
                default:
                    return false;
            }
        }
    }
}
