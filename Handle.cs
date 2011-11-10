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
        /// Gets or sets a handle to the object pointed to by this reference. 
        /// </summary>
        /// <remarks>This may only be used for references.</remarks>
        public abstract Handle Target { get; set; }

        /// <summary>
        /// Gets the size of this collection.
        /// </summary>
        /// <remarks>This may only be used for lists, sets or tuples.</remarks>
        public abstract int Size { get; }

        /// <summary>
        /// Gets the index of the current form of this variant.
        /// </summary>
        /// <remarks>This may only be used for variants.</remarks>
        public abstract int Option { get; }

        /// <summary>
        /// Gets or sets the contents of a field, option or element of the object at this handle. When used with a struct or
        /// variant, this will act like the named field indexer with the index of the field or option. When used with a tuple or list, this can
        /// be used to lookup or replace an element at a certain index.
        /// </summary>
        /// <remarks>This may only be used for structs, variants, tuples or lists.</remarks>
        public abstract Handle this[int Index] { get; set; }

        /// <summary>
        /// Gets or sets the contents of a field or option of the object at this handle. When used with a struct, this can
        /// be used to lookup or replace an individual field. When used with a variant, this can set the current form and associated data or
        /// get the associated data for the given form (which will be null if that is not the current form).
        /// </summary>
        /// <remarks>This may only be used for structs or variants.</remarks>
        public abstract Handle this[string Name] { get; set; }

        /// <summary>
        /// Replaces the contents of this handle with the object at the given handle.
        /// </summary>
        public abstract void Replace(Handle Object);

        /// <summary>
        /// Inserts an object into the given index in the list at this handle.
        /// </summary>
        /// <remarks>This may only be used for lists.</remarks>
        public abstract void Insert(int Index, Handle Object);

        /// <summary>
        /// Inserts an object to the set at this handle.
        /// </summary>
        /// <remarks>This may only be used for sets.</remarks>
        public abstract void Insert(Handle Object);

        /// <summary>
        /// Removes the object at the given index in the list at this handle.
        /// </summary>
        /// <remarks>This may only be used for lists.</remarks>
        public abstract void Remove(int Index);

        /// <summary>
        /// Removes an object from the set at this handle.
        /// </summary>
        /// <remarks>This may only be used for sets.</remarks>
        public abstract void Remove(Handle Object);

        /// <summary>
        /// Appends an object to the list at this handle.
        /// </summary>
        /// <remarks>This may only be used for lists.</remarks>
        public virtual void Append(Handle Object)
        {
            this.Insert(this.Size, Object);
        }

        /// <summary>
        /// Prepends an object to the list at this handle.
        /// </summary>
        /// <remarks>This may only be used for lists.</remarks>
        public virtual void Prepend(Handle Object)
        {
            this.Insert(0, Object);
        }
    }
}