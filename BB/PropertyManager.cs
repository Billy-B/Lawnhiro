﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mono.Reflection;
using System.Reflection.Emit;

namespace BB
{
    internal abstract class PropertyManager
    {
        private static readonly Dictionary<PropertyInfo, PropertyManager> _mapper = new Dictionary<PropertyInfo, PropertyManager>();

        private long _getCounter;

        private bool _initialized = false;
        protected string _initErrorMessage;

        private Func<object, object> _fieldValueGetter;
        private Action<object, object> _fieldValueSetter;

        public long GetCounter
        {
            get { return _getCounter; }
        }

        public double UsageRate { get; internal set; }

        public bool IsValid { get; internal set; }
        public PropertyInfo Property { get; internal set; }
        public FieldInfo BackingField { get; internal set; }
        public TypeManager TypeManager { get; internal set; }
        public PropertyValidationMode ValidationMode { get; internal set; }

        internal abstract void Initialize();

        public static PropertyManager GetManager(PropertyInfo prop)
        {
            PropertyManager ret;
            _mapper.TryGetValue(prop, out ret);
            return ret;
        }

        public static PropertyManager Create(PropertyInfo prop)
        {
            ManagedPropertyBaseAttribute att = prop.GetCustomAttribute<ManagedPropertyBaseAttribute>();
            PropertyManager ret = att.CreateManager(prop);
            ret.Property = prop;
            FieldInfo backingField = prop.GetBackingField();
            ret.BackingField = backingField;
            _mapper.Add(prop, ret);
            DynamicMethod fieldValueGetterMethod = new DynamicMethod("getField_" + backingField, typeof(object), new[] { typeof(object) }, prop.DeclaringType, true);
            ILGenerator ilGen = fieldValueGetterMethod.GetILGenerator();
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Castclass, prop.DeclaringType);
            ilGen.Emit(OpCodes.Ldfld, backingField);
            if (prop.PropertyType.IsValueType)
            {
                ilGen.Emit(OpCodes.Box, prop.PropertyType);
            }
            ilGen.Emit(OpCodes.Ret);
            ret._fieldValueGetter = (Func<object, object>)fieldValueGetterMethod.CreateDelegate(typeof(Func<object, object>));
            DynamicMethod fieldValueSetterMethod = new DynamicMethod("setField_" + backingField, typeof(void), new[] { typeof(object), typeof(object) }, prop.DeclaringType, true);
            ilGen = fieldValueSetterMethod.GetILGenerator();
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Castclass, prop.DeclaringType);
            ilGen.Emit(OpCodes.Ldarg_1);
            if (prop.PropertyType.IsValueType)
            {
                ilGen.Emit(OpCodes.Unbox_Any, prop.PropertyType);
            }
            else
            {
                ilGen.Emit(OpCodes.Castclass, prop.PropertyType);
            }
            ilGen.Emit(OpCodes.Stfld, backingField);
            ilGen.Emit(OpCodes.Ret);
            ret._fieldValueSetter = (Action<object, object>)fieldValueSetterMethod.CreateDelegate(typeof(Action<object, object>));

            return ret;
        }

        internal object GetFieldValue(object obj)
        {
            return _fieldValueGetter(obj);
        }

        internal void SetFieldValue(object obj, object value)
        {
            _fieldValueSetter(obj, value);
        }

        public object GetValue(object obj)
        {
            if (!IsValid)
            {
                throw new PropertyInitializationException(Property, _initErrorMessage);
            }
            Interlocked.Increment(ref _getCounter);
            ObjectExtender extender = ObjectExtender.GetExtender(obj);
            lock (extender)
            {
                switch (extender.State)
                {
                    case ObjectState.Attached:
                        object changedValue;
                        if (ObjectContext.Current.HasChangedPropertyValue(obj, this, out changedValue))
                        {
                            return changedValue;
                        }
                        else if (!extender.InitializedProperties.Contains(this))
                        {
                            extender.InitializedProperties.Add(this);
                            object ret = FetchValue(extender.DataSource);
                            SetFieldValue(obj, ret);
                            return ret;
                        }
                        break;
                    case ObjectState.Deleted:
                        throw new InvalidOperationException("Cannot access property " + Property + " because the object has been deleted.");
                }
                return GetFieldValue(obj);
            }
        }

        public void SetValue(object obj, object value)
        {
            if (!IsValid)
            {
                throw new PropertyInitializationException(Property, _initErrorMessage);
            }
            if (ValidationMode == PropertyValidationMode.Immediate)
            {
                Validate(value);
            }
            ObjectExtender extender = ObjectExtender.GetExtender(obj);
            lock (extender)
            {
                switch (extender.State)
                {
                    case ObjectState.New:
                    case ObjectState.AwaitingInsert:
                        SetFieldValue(obj, value);
                        break;
                    case ObjectState.Attached:
                        ObjectContext.Current.SetPropertyValue(obj, this, value);
                        break;
                    case ObjectState.Deleted:
                        throw new InvalidOperationException("Cannot access property " + Property + " because the object has been deleted.");
                }
            }
        }

        internal virtual void Validate(object value) { }

        public abstract object FetchValue(IObjectDataSource dataSource);
    }
}
