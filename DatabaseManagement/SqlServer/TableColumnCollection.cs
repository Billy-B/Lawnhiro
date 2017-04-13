using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SqlServer
{
    public class TableColumnCollection : IReadOnlyList<Column>
    {
        public Table Table { get; private set; }

        private Dictionary<string, Column> _nameIndexed;

        private Column[] _columns;

        public TableColumnCollection(Table table, IEnumerable<Column> columns)
        {
            this.Table = table;
            _columns = columns.ToArray();
            for (int i = 0; i < _columns.Length; i++)
            {
                if (_columns[i].OrdinalPosition != (i + 1))
                {
                    throw new DatabaseIntegrityException("Missing / unordered columns detected for table " + Table);
                }
            }
            _nameIndexed = columns.ToDictionary(c => c.Name.ToLowerInvariant());
        }

        public int Count
        {
            get { return _columns.Length; }
        }

        public Column GetColumnOrNull(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            string[] splitIntoParts = name.SplitQualifiedNameIntoParts();
            string columnName;
            if (splitIntoParts.Length == 1)
            {
                columnName = splitIntoParts[0];
            }
            else
            {
                columnName = splitIntoParts.Last();
                string qualifier = string.Join(".", splitIntoParts.Take(splitIntoParts.Length - 2));
                Table qualifiedTable = Table.Schema.Database.GetObjectOrNull(qualifier) as Table;
                if (qualifiedTable != Table)
                {
                    throw new ArgumentException("The qualifier provided on argument \"" + name + "\" does not match table " + Table + ".");
                }
            }
            Column ret;
            _nameIndexed.TryGetValue(columnName, out ret);
            return ret;
        }

        public Column this[string name]
        {
            get
            {
                Column ret = GetColumnOrNull(name);
                if (ret == null)
                { 
                    throw new ArgumentException("No column " + name + " exists on table " + Table + ".");
                }
                return ret;
            }
        }

        public Column this[int ordinal]
        {
            get
            {
                if ((uint)ordinal >= this.Count)
                {
                    throw new ArgumentOutOfRangeException("position", "The value exceeds the number of columns in table " + Table + " (" + this.Count + ").");
                }
                return _columns[ordinal - 1];
            }
        }

        /*public bool TryGetColumn(string name, out Column col)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            string[] split = name.Split('.');
            int nQualifiers = split.Length;
            if (nQualifiers == 1)
            {
                return _nameIndexed.TryGetValue(name.TrimBracketNotation(), out col);
            }
            else
            {
                Table qualifiedTable
                switch (split.Length)
                {
                    case 2:
                        {
                            Table qualifiedTable;
                            if (Table.Schema.Database.Tables.TryGetValue(split[0], out qualifiedTable) && qualifiedTable == Table)
                            {
                                return _nameIndexed.TryGetValue(split[1].TrimBracketNotation(), out col);
                            }
                            else
                            {
                                col = null;
                                return false;
                            }
                        }
                    case 3:
                        {
                            Table qualifiedTable;
                            if (Table.Schema.Database.Tables.TryGetValue(string.Concat(split[0], split[1]), out qualifiedTable) && qualifiedTable == Table)
                            {
                                return _nameIndexed.TryGetValue(split[2].TrimBracketNotation().ToLowerInvariant(), out col);
                            }
                            else
                            {
                                col = null;
                                return false;
                            }
                        }
                    default:
                        col = null;
                        return false;
                }
            }
        }*/

        public Column GetByOrdinalPosition(int position)
        {
            if ((uint)position >= this.Count)
            {
                throw new ArgumentOutOfRangeException("position", "The value exceeds the number of columns in table " + Table + " (" + this.Count + ").");
            }
            return this[position - 1];
        }

        IEnumerator<Column> IEnumerable<Column>.GetEnumerator()
        {
            return ((IEnumerable<Column>)_columns).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Column>)_columns).GetEnumerator();
        }
    }
}
