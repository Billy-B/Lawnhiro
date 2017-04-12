using DatabaseManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mono.Reflection;

namespace BB
{
    /*internal class DatabaseTableBoundTypeManager<T> : TypeManager<T>
        where T : class
    {
        public ITable Table { get; internal set; }

        public DatabaseTableBoundTypeManager(ITable table)
        {
            Table = table;
            Type type = typeof(T);
            PropertyInfo[] allProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            List<PropertyManager> managedProperties = new List<PropertyManager>();
            foreach (PropertyInfo prop in allProperties)
            {
                FieldInfo backingField = prop.GetBackingField();
                ColumnAttribute colAtt = prop.GetCustomAttribute<ColumnAttribute>();
                if (backingField != null) // property is an auto - property
                {
                    if (colAtt != null)
                    {
                        //Tab
                        IColumn column = table.GetColumnByName(colAtt.Name);
                        //if()
                        
                    }
                }
            }
        }

        public UniqueConstraint GetConstraintByDbConstraint(IUniqueConstraint dbConstraint)
        {
            throw new NotImplementedException();
        }

        

        public T LookupByUniqueKey(IUniqueConstraint constraint, object uniqueKey)
        {
            Cache<T> cache = Cache;
            if (cache == null)
            {

            }
            else
            {
                UniqueConstraint sysConstraint = GetConstraintByDbConstraint(constraint);
                CacheUniqueIndex<T> index = cache.GetOrCreateUniqueIndex(sysConstraint);
                return index.GetValue(uniqueKey);
            }
            //Array
            //IStructualEquatable
            throw new NotImplementedException();
        }
    }*/
}
