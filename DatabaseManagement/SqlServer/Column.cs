using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseManagement.SQL;

namespace DatabaseManagement.SqlServer
{
    public class Column : IColumn
    {
        public Table Table { get; internal set; }

        public string Name { get; internal set; }

        public SqlDbType DBType { get; internal set; }

        public bool IsNullable { get; internal set; }

        public bool IsIdentity { get; internal set; }

        public int OrdinalPosition { get; internal set; }

        public int CharacterMaxLength { get; internal set; }

        public int NumericPrecision { get; internal set; }

        public int NumericScale { get; internal set; }

        DbType IColumn.DbType
        {
            get { return TypeConverter.GetDbType(DBType); }
        }

        ITable IColumn.Table
        {
            get { return Table; }
        }

        public string FullyQualifiedName
        {
            get
            {
                return Table.FullyQualifiedName + ".[" + Name + "]";
            }
        }

        public ScalarExpression DefaultValue { get; internal set; }

        public override string ToString()
        {
            return FullyQualifiedName;
        }
    }
}
