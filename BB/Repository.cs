using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    public abstract class Repository
    {

        public static IQueryable<T> Query<T>()
        {
            Type type = typeof(T);
            TypeManager mgr = TypeManager.GetManager(type);
            if (mgr == null)
            {
                throw new NotSupportedException("Type " + type + " is not a managed type.");
            }
            mgr.EnsureInitialized();
            return new Queryable<T> { Provider = new QueryProvider(mgr) };
        }

        public static void Add(System.Collections.IEnumerable objects)
        {

        }

        public static void Add(params object[] objects)
        {

        }

        public static void Add(object obj)
        {
            AssertNonNullAndManaged(obj);
            ObjectExtender extender = ObjectExtender.GetExtender(obj);
            if (extender.State != ObjectState.New)
            {
                throw new ArgumentException("Object has already been added.");
            }
            ObjectContext.Current.Add(obj);
        }

        public static void Delete(object obj)
        {
            AssertNonNullAndManaged(obj);
            ObjectExtender extender = ObjectExtender.GetExtender(obj);
            if (extender.State != ObjectState.Attached)
            {
                throw new ArgumentException();
            }
            ObjectContext.Current.Delete(obj);
        }

        public static void CommitChanges()
        {
            ObjectContext.Current.CommitChanges();
        }

        public static void DiscardChanges()
        {
            ObjectContext.Current.DisgardChanges();
        }

        public static object GetPrimaryKey(object obj)
        {
            throw new NotImplementedException();
        }

        public static int GetId(object obj)
        {
            throw new NotImplementedException();
        }

        internal static void AssertNonNullAndManaged(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException();
            }
            Type type = obj.GetType();
            if (!TypeManager.IsManaged(type))
            {
                throw new ArgumentException("Type " + type + " is not a managed type.");
            }
        }
    }
}
