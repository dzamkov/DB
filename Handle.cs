using System;
using System.Collections.Generic;
using System.Linq;

namespace DB
{
    /// <summary>
    /// A reference to a database or memory location that can store objects of a certain type.
    /// </summary>
    public abstract class Handle
    {
        public Handle(Type Type)
        {
            this.Type = Type;
        }

        /// <summary>
        /// The expected type of object at this handle.
        /// </summary>
        public readonly Type Type;

        /// <summary>
        /// Performs an operation on this handle.
        /// </summary>
        /// <remarks>The operation should be checked for type compatibility before using this method.</remarks>
        /// <param name="Argument">The argument of the operation, when applicable.</param>
        /// <param name="Index">The index of the operation, when applicable.</param>
        public abstract void Perform(HandleOperation Operation, Handle Argument, int Index);

        /// <summary>
        /// Performs a query on this handle.
        /// </summary>
        /// <remarks>The query should be checked for type compatibility before using this method.</remarks>
        /// <param name="Index">The index of the operation, when applicable.</param>
        public abstract object Perform(HandleQuery Query, int Index);

        /// <summary>
        /// Performs an operation on this handle.
        /// </summary>
        public void Perform(HandleOperation Operation, Handle Argument)
        {
            this.Perform(Operation, Argument, 0);
        }

        /// <summary>
        /// Performs an operation on this handle.
        /// </summary>
        public void Perform(HandleOperation Operation, int Index)
        {
            this.Perform(Operation, null, Index);
        }

        /// <summary>
        /// Performs an operation on this handle.
        /// </summary>
        public void Perform(HandleOperation Operation)
        {
            this.Perform(Operation, null, 0);
        }

        /// <summary>
        /// Performs a query on this handle.
        /// </summary>
        public object Perform(HandleQuery Query)
        {
            return this.Perform(Query, 0);
        }

        /// <summary>
        /// Gets or sets a handle to the target object for this handle. For references, this will get or set the handle for the object
        /// the reference points to. For dynamic values, this will get or set a typed version of the object.
        /// </summary>
        /// <remarks>This may only be used for references and dynamic values.</remarks>
        public Handle Target
        {
            get
            {
                if (this.Type is ReferenceType)
                    return (Handle)this.Perform(HandleQuery.GetTargetReference);
                if (this.Type is DynamicType)
                    return (Handle)this.Perform(HandleQuery.GetTargetDynamic);
                throw new TypeIncompatibleException();
            }
            set
            {
                if (this.Type is ReferenceType)
                {
                    this.Perform(HandleOperation.SetTargetReference, value);
                    return;
                }
                if (this.Type is DynamicType)
                {
                    this.Perform(HandleOperation.SetTargetDynamic, value);
                    return;
                }
                throw new TypeIncompatibleException();
            }
        }

        /// <summary>
        /// Gets the size of this collection.
        /// </summary>
        /// <remarks>This may only be used for lists, sets or tuples.</remarks>
        public int Size
        {
            get
            {
                if (this.Type is ListType)
                    return (int)this.Perform(HandleQuery.GetListSize);
                if (this.Type is SetType)
                    return (int)this.Perform(HandleQuery.GetSetSize);
                TupleType tt = this.Type as TupleType;
                if (tt != null)
                {
                    return tt.ElementTypes.Length;
                }
                throw new TypeIncompatibleException();
            }
        }

        /// <summary>
        /// Gets or sets the index of the current form of this variant. When setting the form, the form's information will be set to the default
        /// value of the form type.
        /// </summary>
        /// <remarks>This may only be used for variants.</remarks>
        public int Option
        {
            get
            {
                if (this.Type is VariantType)
                    return (int)this.Perform(HandleQuery.GetOptionIndex);
                throw new TypeIncompatibleException();
            }
            set
            {
                VariantType vt = this.Type as VariantType;
                if (vt != null)
                {
                    this.Perform(HandleOperation.SetOption, vt.Options[value].Type.Default(), value);
                    return;
                }
                throw new TypeIncompatibleException();
            }
        }

        /// <summary>
        /// Gets or sets the contents of a field, option or element of the object at this handle. When used with a struct or
        /// variant, this will act like the named field indexer with the index of the field or option. When used with a tuple or list, this can
        /// be used to lookup or replace an element at a certain index.
        /// </summary>
        /// <remarks>This may only be used for structs, variants, tuples or lists.</remarks>
        public Handle this[int Index]
        {
            get
            {
                if (this.Type is TupleType)
                    return (Handle)this.Perform(HandleQuery.GetElementTuple, Index);
                if (this.Type is ListType)
                    return (Handle)this.Perform(HandleQuery.GetElementList, Index);
                if (this.Type is StructType)
                    return (Handle)this.Perform(HandleQuery.GetField, Index);
                if (this.Type is VariantType)
                    return (Handle)this.Perform(HandleQuery.GetOption, Index);
                throw new TypeIncompatibleException();
            }
            set
            {
                if (this.Type is TupleType)
                {
                    this.Perform(HandleOperation.SetElementTuple, value, Index);
                    return;
                }
                if (this.Type is ListType)
                {
                    this.Perform(HandleOperation.SetElementList, value, Index);
                    return;
                }
                if (this.Type is StructType)
                {
                    this.Perform(HandleOperation.SetField, value, Index);
                    return;
                }
                if (this.Type is VariantType)
                {
                    this.Perform(HandleOperation.SetOption, value, Index);
                    return;
                }
                throw new TypeIncompatibleException();
            }
        }

        /// <summary>
        /// Gets or sets the contents of a field or option of the object at this handle. When used with a struct, this can
        /// be used to lookup or replace an individual field. When used with a variant, this can set the current form and associated data or
        /// get the associated data for the given form (which will be null if that is not the current form).
        /// </summary>
        /// <remarks>This may only be used for structs or variants.</remarks>
        public Handle this[string Name]
        {
            get
            {
                StructType st = this.Type as StructType;
                if (st != null)
                    return (Handle)this.Perform(HandleQuery.GetField, st.FieldByName[Name]);

                VariantType vt = this.Type as VariantType;
                if (vt != null)
                    return (Handle)this.Perform(HandleQuery.GetOption, vt.OptionByName[Name]);

                throw new TypeIncompatibleException();
            }
            set
            {
                StructType st = this.Type as StructType;
                if (st != null)
                {
                    this.Perform(HandleOperation.SetField, value, st.FieldByName[Name]);
                    return;
                }

                VariantType vt = this.Type as VariantType;
                if (vt != null)
                {
                    this.Perform(HandleOperation.SetOption, value, vt.OptionByName[Name]);
                    return;
                }

                throw new TypeIncompatibleException();
            }
        }

        /// <summary>
        /// Replaces the contents of this handle with the object at the given handle.
        /// </summary>
        public void Set(Handle Object)
        {
            if (this.Type != Object.Type)
                throw new TypeIncompatibleException();
            this.Perform(HandleOperation.Replace, Object);
        }

        /// <summary>
        /// Replaces the contents of this handle with the given string, or sets the current option for
        /// this variant.
        /// </summary>
        /// <remarks>The may only be used for strings and variants.</remarks>
        public void Set(string String)
        {
            if (this.Type == Type.String)
            {
                this.Perform(HandleOperation.Replace, String);
                return;
            }

            VariantType vt = this.Type as VariantType;
            if (vt != null)
            {
                int optind = vt.OptionByName[String];
                this.Perform(HandleOperation.SetOption, vt.Options[optind].Type.Default(), optind);
                return;
            }
            throw new TypeIncompatibleException();
        }

        /// <summary>
        /// Removes all items in this collection.
        /// </summary>
        /// <remarks>This may only be used for lists or sets.</remarks>
        public void Clear()
        {
            if (this.Type is ListType)
            {
                this.Perform(HandleOperation.ClearList);
                return;
            }

            if (this.Type is SetType)
            {
                this.Perform(HandleOperation.ClearSet);
                return;
            }

            throw new TypeIncompatibleException();
        }

        /// <summary>
        /// Inserts an object into the given index in the list at this handle.
        /// </summary>
        /// <remarks>This may only be used for lists.</remarks>
        public void Insert(int Index, Handle Object)
        {
            ListType lt = this.Type as ListType;
            if (lt != null && lt.ElementType == Object.Type)
            {
                this.Perform(HandleOperation.InsertList, Object, Index);
                return;
            }
            throw new TypeIncompatibleException();
        }

        /// <summary>
        /// Inserts an object to the set at this handle.
        /// </summary>
        /// <remarks>This may only be used for sets.</remarks>
        public void Insert(Handle Object)
        {
            SetType st = this.Type as SetType;
            if (st != null && st.ElementType == Object.Type)
            {
                this.Perform(HandleOperation.InsertSet, Object);
                return;
            }
            throw new TypeIncompatibleException();
        }

        /// <summary>
        /// Removes the object at the given index in the list at this handle.
        /// </summary>
        /// <remarks>This may only be used for lists.</remarks>
        public void Remove(int Index)
        {
            if (this.Type is ListType)
            {
                this.Perform(HandleOperation.RemoveList, Index);
                return;
            }
            throw new TypeIncompatibleException();
        }

        /// <summary>
        /// Removes an object from the set at this handle.
        /// </summary>
        /// <remarks>This may only be used for sets.</remarks>
        public void Remove(Handle Object)
        {
            SetType st = this.Type as SetType;
            if (st != null && st.ElementType == Object.Type)
            {
                this.Perform(HandleOperation.RemoveSet, Object);
                return;
            }
            throw new TypeIncompatibleException();
        }


        /// <summary>
        /// Appends an object to the list at this handle.
        /// </summary>
        /// <remarks>This may only be used for lists.</remarks>
        public void Append(Handle Object)
        {
            if (this.Type is ListType)
            {
                this.Perform(HandleOperation.Append, Object);
                return;
            }
            throw new TypeIncompatibleException();
        }

        /// <summary>
        /// Prepends an object to the list at this handle.
        /// </summary>
        /// <remarks>This may only be used for lists.</remarks>
        public void Prepend(Handle Object)
        {
            if (this.Type is ListType)
            {
                this.Perform(HandleOperation.Prepend, Object);
                return;
            }
            throw new TypeIncompatibleException();
        }

        public static implicit operator Handle(bool Value)
        {
            throw new NotImplementedException();
        }

        public static implicit operator Handle(string Value)
        {
            throw new NotImplementedException();
        }

        public static implicit operator Handle(byte Value)
        {
            throw new NotImplementedException();
        }

        public static implicit operator Handle(char Value)
        {
            throw new NotImplementedException();
        }

        public static implicit operator Handle(short Value)
        {
            throw new NotImplementedException();
        }

        public static implicit operator Handle(ushort Value)
        {
            throw new NotImplementedException();
        }

        public static implicit operator Handle(int Value)
        {
            throw new NotImplementedException();
        }

        public static implicit operator Handle(uint Value)
        {
            throw new NotImplementedException();
        }

        public static implicit operator Handle(long Value)
        {
            throw new NotImplementedException();
        }

        public static implicit operator Handle(ulong Value)
        {
            throw new NotImplementedException();
        }

        public static implicit operator Handle(float Value)
        {
            throw new NotImplementedException();
        }

        public static implicit operator Handle(double Value)
        {
            throw new NotImplementedException();
        }

        public static implicit operator Handle(decimal Value)
        {
            throw new NotImplementedException();
        }

        public static implicit operator byte(Handle Handle)
        {
            if (Handle.Type != Type.Byte)
                throw new TypeIncompatibleException();
            return (byte)Handle.Perform(HandleQuery.GetPrimitiveByte, 0);
        }

        public static implicit operator char(Handle Handle)
        {
            if (Handle.Type != Type.Char)
                throw new TypeIncompatibleException();
            return (char)Handle.Perform(HandleQuery.GetPrimitiveChar, 0);
        }

        public static implicit operator short(Handle Handle)
        {
            if (Handle.Type != Type.Short)
                throw new TypeIncompatibleException();
            return (short)Handle.Perform(HandleQuery.GetPrimitiveShort, 0);
        }

        public static implicit operator ushort(Handle Handle)
        {
            if (Handle.Type != Type.UShort)
                throw new TypeIncompatibleException();
            return (ushort)(short)Handle.Perform(HandleQuery.GetPrimitiveShort, 0);
        }

        public static implicit operator int(Handle Handle)
        {
            if (Handle.Type != Type.Int)
                throw new TypeIncompatibleException();
            return (int)Handle.Perform(HandleQuery.GetPrimitiveInt, 0);
        }

        public static implicit operator uint(Handle Handle)
        {
            if (Handle.Type != Type.UInt)
                throw new TypeIncompatibleException();
            return (uint)(int)Handle.Perform(HandleQuery.GetPrimitiveInt, 0);
        }

        public static implicit operator long(Handle Handle)
        {
            if (Handle.Type != Type.Long)
                throw new TypeIncompatibleException();
            return (long)Handle.Perform(HandleQuery.GetPrimitiveLong, 0);
        }

        public static implicit operator ulong(Handle Handle)
        {
            if (Handle.Type != Type.ULong)
                throw new TypeIncompatibleException();
            return (ulong)(long)Handle.Perform(HandleQuery.GetPrimitiveLong, 0);
        }

        public static implicit operator float(Handle Handle)
        {
            if (Handle.Type != Type.Float)
                throw new TypeIncompatibleException();
            return (float)Handle.Perform(HandleQuery.GetPrimitiveFloat, 0);
        }

        public static implicit operator double(Handle Handle)
        {
            if (Handle.Type != Type.Double)
                throw new TypeIncompatibleException();
            return (double)Handle.Perform(HandleQuery.GetPrimitiveDouble, 0);
        }

        public static implicit operator decimal(Handle Handle)
        {
            if (Handle.Type != Type.Decimal)
                throw new TypeIncompatibleException();
            return (decimal)Handle.Perform(HandleQuery.GetPrimitiveDecimal, 0);
        }

        public static implicit operator bool(Handle Handle)
        {
            if (Handle.Type != Type.Bool)
                throw new TypeIncompatibleException();
            throw new NotImplementedException();
        }

        public static implicit operator string(Handle Handle)
        {
            if (Handle.Type != Type.String)
                throw new TypeIncompatibleException();
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// An exception from a handle operation or query that indicates that the operation is not applicable for
    /// a handle of that type.
    /// </summary>
    public class TypeIncompatibleException : Exception
    {

    }

    /// <summary>
    /// Identifies a possible operation to be performed on a handle.
    /// </summary>
    public enum HandleOperation
    {
        SetTargetReference,
        SetTargetDynamic,
        SetOption,
        SetField,
        SetElementTuple,
        SetElementList,
        Replace,
        ClearList,
        ClearSet,
        InsertList,
        InsertSet,
        RemoveList,
        RemoveSet,
        Append,
        Prepend
    }

    /// <summary>
    /// Identifies a possible query to be performed on a handle.
    /// </summary>
    public enum HandleQuery
    {
        GetTargetReference,
        GetTargetDynamic,
        GetListSize,
        GetSetSize,
        GetOptionIndex,
        GetOption,
        GetField,
        GetElementTuple,
        GetElementList,
        GetPrimitiveByte,
        GetPrimitiveChar,
        GetPrimitiveShort,
        GetPrimitiveInt,
        GetPrimitiveLong,
        GetPrimitiveFloat,
        GetPrimitiveDouble,
        GetPrimitiveDecimal
    }
}