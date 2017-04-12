using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DatabaseManagement.SQL
{
    public abstract class Statement
    {
        public void Prepare(IDbCommand command)
        {
            command.CommandText = this.ToString();
            command.Parameters.Clear();
            foreach (ParameterExpression paramExpression in enumerateExpressionsRecursive(EnumerateExpressions()).OfType<ParameterExpression>())
            {
                IDbDataParameter parameter = command.CreateParameter();
                parameter.ParameterName = paramExpression.ParameterName;
                parameter.Value = parameter.Value;
                command.Parameters.Add(parameter);
            }
        }

        internal abstract IEnumerable<Expression> EnumerateExpressions();

        private static IEnumerable<Expression> enumerateExpressionsRecursive(IEnumerable<Expression> expressions)
        {
            foreach (Expression expr in expressions)
            {
                yield return expr;
                foreach (Expression sub in enumerateExpressionsRecursive(expr.EnumerateSubExpressions()))
                {
                    yield return sub;
                }
            }
        }

        //public abstract void Enumerate

        public static InsertStatement InsertInto(ITable table, IEnumerable<IColumn> columnsToInsert, IEnumerable<ScalarExpression> values)
        {
            Utility.AssertNonNull(table, "tableExpression");
            Utility.AssertNonNull(columnsToInsert, "tableExpression");
            Utility.AssertNonNull(values, "tableExpression");
            List<IColumn> columnsList = columnsToInsert.ToList();
            List<ScalarExpression> valuesList = values.ToList();
            if (columnsList.Count == 0)
            {
                throw new ArgumentException("Cannot be empty.", "columnsToInsert");
            }
            if (valuesList.Count == 0)
            {
                throw new ArgumentException("Cannot be empty.", "values");
            }
            if (columnsList.Count != valuesList.Count)
            {
                throw new ArgumentException("Number of values does not match number of provided columns.");
            }
            return new InsertStatement
            {
                Table = table,
                InsertColumns = columnsToInsert.ToList(),
                Values = values.ToList()
            };
        }

        public static UpdateStatement Update(ITable table, IEnumerable<KeyValuePair<IColumn, ScalarExpression>> columnsAndValues, ConditionalExpression whereExpression)
        {
            Utility.AssertNonNull(table, "table");
            Utility.AssertNonNull(columnsAndValues, "columnsAndValues");
            Utility.AssertNonNull(whereExpression, "whereExpression");
            return new UpdateStatement
            {
                TableToUpdate = table,
                ColumnsAndValues = columnsAndValues.ToList().AsReadOnly(),
                WhereExpression = whereExpression
            };
        }

        public static SelectStatement SelectFrom(TableValuedExpression tableExpression)
        {
            Utility.AssertNonNull(tableExpression, "tableExpression");
            return new SelectStatement
            {
                FromExpression = tableExpression
            };
        }
        public static SelectStatement SelectFrom(TableValuedExpression tableExpression, IEnumerable<Expression> selections)
        {
            Utility.AssertNonNull(tableExpression, "tableExpression");
            Utility.AssertNonNull(selections, "selections");
            return new SelectStatement
            {
                FromExpression = tableExpression,
                SelectedFields = selections.ToList()
            };
        }

        public static SelectStatement SelectFrom(TableValuedExpression tableExpression, IEnumerable<Expression> selections, ConditionalExpression whereExpression)
        {
            Utility.AssertNonNull(tableExpression, "tableExpression");
            Utility.AssertNonNull(selections, "selections");
            Utility.AssertNonNull(whereExpression, "whereExpression");
            return new SelectStatement
            {
                FromExpression = tableExpression,
                SelectedFields = selections.ToList(),
                WhereExpression = whereExpression
            };
        }

        public static SelectStatement SelectFrom(TableValuedExpression tableExpression, IEnumerable<Expression> selections, int maxRows, ConditionalExpression whereExpression)
        {
            Utility.AssertNonNull(tableExpression, "tableExpression");
            Utility.AssertNonNull(selections, "selections");
            Utility.AssertNonNull(whereExpression, "whereExpression");
            if (maxRows < 1)
            {
                throw new ArgumentOutOfRangeException("maxRows", "Must be greater than zero.");
            }
            return new SelectStatement
            {
                FromExpression = tableExpression,
                SelectedFields = selections.ToList(),
                MaxRows = maxRows,
                WhereExpression = whereExpression
            };
        }

        public static DeleteStatement DeleteFrom(ITable table, ConditionalExpression where)
        {
            return new DeleteStatement
            {
                Table = table,
                WhereExpression = where
            };
        }
    }
}
