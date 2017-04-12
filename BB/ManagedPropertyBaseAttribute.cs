using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class ManagedPropertyBaseAttribute : Attribute
    {
        PropertyValidationMode ValidationMode { get; set; }

        internal bool AllowNullDeclared { get; private set; }
        internal bool MaxLengthDeclared { get; private set; }

        private bool _allowNull;
        private int _maxLength;

        public bool AllowNull
        {
            get { return _allowNull; }
            set 
            {
                AllowNullDeclared = true;
                _allowNull = value;
            }
        }

        public int MaxLength
        {
            get { return _maxLength; }
            set
            {
                MaxLengthDeclared = true;
                _maxLength = value;
            }
        }

        internal abstract PropertyManager CreateManager(PropertyInfo prop);
    }
}
