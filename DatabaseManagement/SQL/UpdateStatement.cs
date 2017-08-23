using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseManagement.SQL
{
    public class UpdateStatement : Statement
    {
        public ITable TableToUpdate { get; internal set; }

        public IList<KeyValuePair<IColumn, ScalarExpression>> ColumnsAndValues { get; internal set; }

        public ConditionalExpression WhereExpression { get; internal set; }

        internal override Statement Parameterize(Parameterizer p)
        {
            if (_parameterized)
            {
                return this;
            }
            IList<KeyValuePair<IColumn, ScalarExpression>> initialList = ColumnsAndValues;
            List<KeyValuePair<IColumn, ScalarExpression>> updatedList = new List<KeyValuePair<IColumn, ScalarExpression>>();
            foreach (var kvp in initialList)
            {
                updatedList.Add(new KeyValuePair<IColumn, ScalarExpression>(kvp.Key, p.VisitScalar(kvp.Value)));
            }
            return new UpdateStatement
            {
                TableToUpdate = this.TableToUpdate,
                ColumnsAndValues = updatedList.AsReadOnly(),
                WhereExpression = p.VisitConditional(WhereExpression),
                _parameterized = true
            };
        }

        public override string ToString()
        {
            string ret = "update " + TableToUpdate + " set " + string.Join(", ", ColumnsAndValues.Select(kvp => kvp.Key + "=" + kvp.Value));
            if (WhereExpression != null)
            {
                ret += " where " + WhereExpression;
            }
            return ret;
        }
    }
}
