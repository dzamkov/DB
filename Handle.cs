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
        internal Handle(Type Type)
        {
            this.Type = Type;
        }

        /// <summary>
        /// The expected type of object at this handle.
        /// </summary>
        public readonly Type Type;

        /// <summary>
        /// Gets or sets a handle to the target object for this handle. For references, this will get or set the handle for the object
        /// the reference points to. For dynamic values, this will get or set a typed version of the object.
        /// </summary>
        /// <remarks>This may only be used for references and dynamic values.</remarks>
        public virtual Handle Target
        {
            get
            {
                throw new TypeIncompatibleException();
            }
            set
            {
                throw new TypeIncompatibleException();
            }
        }

        /// <summary>
        /// Gets the size of this collection.
        /// </summary>
        /// <remarks>This may only be used for lists, sets or tuples.</remarks>
        public virtual int Size
        {
            get
            {
                throw new TypeIncompatibleException();
            }
        }

        /// <summary>
        /// Gets or sets the index of the current option of this variant. When setting the option index, the additional information of the option will be
        /// undefined, and should be set before being read.
        /// </summary>
        /// <remarks>This may only be used for variants.</remarks>
        public virtual int Option
        {
            get
            {
                throw new TypeIncompatibleException();
            }
            set
            {
                throw new TypeIncompatibleException();
            }
        }

        /// <summary>
        /// Gets or sets the contents of a field, option or element of the object at this handle. When used with a struct or
        /// variant, this will act like the named field indexer with the index of the field or option. When used with a tuple or list, this can
        /// be used to lookup or replace an element at a certain index.
        /// </summary>
        /// <remarks>This may only be used for structs, variants, tuples or lists.</remarks>
        public virtual Handle this[int Index]
        {
            get
            {
                throw new TypeIncompatibleException();
            }
            set
            {

                throw new TypeIncompatibleException();
            }
        }

        /// <summary>
        /// Gets or sets the contents of a field or option of the object at this handle. When used with a struct, this can
        /// be used to lookup or replace an individual field. When used with a variant, this can set the current form and associated data or
        /// get the associated data for the given form (which will be null if that is not the current form).
        /// </summary>
        /// <remarks>This may only be used for structs or variants.</remarks>
        public virtual Handle this[string Name]
        {
            get
            {
                throw new TypeIncompatibleException();
            }
            set
            {
                throw new TypeIncompatibleException();
            }
        }

        /// <summary>
        /// Replaces the contents of this handle with the object at the given handle.
        /// </summary>
        public virtual void Set(Handle Object)
        {
            throw new TypeIncompatibleException();
        }

        /// <summary>
        /// Replaces the contents of this handle with the given value.
        /// </summary>
        /// <remarks>The type of the given value should be the native type of this handle.</remarks>
        public virtual void Set<T>(T Value)
        {
            throw new TypeIncompatibleException();
        }

        /// <summary>
        /// Gets the object at this handle as a value of the given type. Note that this will create a copy of the object at the time the
        /// method was called; no further changes can be made to the returned value using this handle.
        /// </summary>
        /// <remarks>The given type should be the native type of this handle.</remarks>
        public virtual T Get<T>()
        {
            throw new TypeIncompatibleException();
        }

        /// <summary>
        /// Removes all items in this collection.
        /// </summary>
        /// <remarks>This may only be used for lists or sets.</remarks>
        public virtual void Clear()
        {
            throw new TypeIncompatibleException();
        }

        /// <summary>
        /// Inserts an object into the given index in the list at this handle.
        /// </summary>
        /// <remarks>This may only be used for lists.</remarks>
        public virtual void Insert(int Index, Handle Object)
        {
            throw new TypeIncompatibleException();
        }

        /// <summary>
        /// Inserts an object to the set at this handle.
        /// </summary>
        /// <remarks>This may only be used for sets.</remarks>
        public virtual void Insert(Handle Object)
        {
            throw new TypeIncompatibleException();
        }

        /// <summary>
        /// Removes the object at the given index in the list at this handle.
        /// </summary>
        /// <remarks>This may only be used for lists.</remarks>
        public virtual void Remove(int Index)
        {
            throw new TypeIncompatibleException();
        }

        /// <summary>
        /// Removes an object from the set at this handle.
        /// </summary>
        /// <remarks>This may only be used for sets.</remarks>
        public virtual void Remove(Handle Object)
        {
            throw new TypeIncompatibleException();
        }

        /// <summary>
        /// Appends an object to the list at this handle.
        /// </summary>
        /// <remarks>This may only be used for lists.</remarks>
        public virtual void Append(Handle Object)
        {
            throw new TypeIncompatibleException();
        }

        /// <summary>
        /// Prepends an object to the list at this handle.
        /// </summary>
        /// <remarks>This may only be used for lists.</remarks>
        public virtual void Prepend(Handle Object)
        {
            throw new TypeIncompatibleException();
        }

        public static implicit operator Handle(bool Value)
        {
            return Type.Bool.Construct(Value);
        }

        public static implicit operator Handle(string Value)
        {
            return Type.String.Construct(Value);
        }

        public static implicit operator Handle(byte Value)
        {
            return Type.Byte.Construct(Value);
        }

        public static implicit operator Handle(char Value)
        {
            return Type.Char.Construct(Value);
        }

        public static implicit operator Handle(short Value)
        {
            return Type.Short.Construct(Value);
        }

        public static implicit operator Handle(ushort Value)
        {
            return Type.UShort.Construct(Value);
        }

        public static implicit operator Handle(int Value)
        {
            return Type.Int.Construct(Value);
        }

        public static implicit operator Handle(uint Value)
        {
            return Type.UInt.Construct(Value);
        }

        public static implicit operator Handle(long Value)
        {
            return Type.Long.Construct(Value);
        }

        public static implicit operator Handle(ulong Value)
        {
            return Type.ULong.Construct(Value);
        }

        public static implicit operator Handle(float Value)
        {
            return Type.Float.Construct(Value);
        }

        public static implicit operator Handle(double Value)
        {
            return Type.Double.Construct(Value);
        }

        public static implicit operator Handle(decimal Value)
        {
            return Type.Decimal.Construct(Value);
        }

        public static implicit operator byte(Handle Handle)
        {
            return Handle.Get<byte>();
        }

        public static implicit operator char(Handle Handle)
        {
            return Handle.Get<char>();
        }

        public static implicit operator short(Handle Handle)
        {
            return Handle.Get<short>();
        }

        public static implicit operator ushort(Handle Handle)
        {
            return Handle.Get<ushort>();
        }

        public static implicit operator int(Handle Handle)
        {
            return Handle.Get<int>();
        }

        public static implicit operator uint(Handle Handle)
        {
            return Handle.Get<uint>();
        }

        public static implicit operator long(Handle Handle)
        {
            return Handle.Get<long>();
        }

        public static implicit operator ulong(Handle Handle)
        {
            return Handle.Get<ulong>();
        }

        public static implicit operator float(Handle Handle)
        {
            return Handle.Get<float>();
        }

        public static implicit operator double(Handle Handle)
        {
            return Handle.Get<double>();
        }

        public static implicit operator decimal(Handle Handle)
        {
            return Handle.Get<decimal>();
        }

        public static implicit operator bool(Handle Handle)
        {
            return Handle.Get<bool>();
        }

        public static implicit operator string(Handle Handle)
        {
            return Handle.Get<string>();
        }
    }

    /// <summary>
    /// An exception from a handle operation or query that indicates that the operation is not applicable for
    /// a handle of that type.
    /// </summary>
    public class TypeIncompatibleException : Exception
    {

    }
}