using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BB
{
    internal class StringColumnPropertyManager : ColumnPropertyManager
    {
        public int MaxLength { get; set; }
        public bool AllowNull { get; set; }

        internal override void Initialize()
        {
            base.Initialize();
            if (ColumnAttribute.AllowNullDeclared)
            {
                AllowNull = ColumnAttribute.AllowNull;
            }
            else
            {
                AllowNull = Column.IsNullable;
            }
            if (ColumnAttribute.MaxLengthDeclared)
            {
                int attMaxLength = ColumnAttribute.MaxLength;
                if (attMaxLength < 1)
                {
                    throw new ArgumentOutOfRangeException("MaxLength", "Value cannot be less than 1.");
                }
            }
            else
            {
                int colMax = Column.CharacterMaxLength;
                if (colMax == -1)
                {
                    MaxLength = int.MaxValue;
                }
                else
                {
                    MaxLength = colMax;
                }
            }
        }

        internal override void Validate(object value)
        {
            string valAsString = (string)value;

            if (valAsString == null)
            {
                if (!AllowNull)
                {
                    throw new ArgumentNullException("value", "Value cannot be null because column " + Column + " does not allow null.");
                }
            }
            else if (valAsString.Length > MaxLength)
            {
                throw new ArgumentException("String value is invalid because its Length exceeds the maximum length (" + MaxLength + ")");
            }
        }
    }
}
