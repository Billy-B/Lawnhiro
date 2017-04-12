using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement
{
    public interface IColumn
    {
        ITable Table { get; }

        string Name { get; }

        DbType DbType { get; }

        bool IsNullable { get; }

        bool IsIdentity { get; }

        int OrdinalPosition { get; }

        string DefaultValue { get; }

        int CharacterMaxLength { get; }

        int NumericPrecision { get; }

        int NumericScale { get; }
    }
}
