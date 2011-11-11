using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DB
{
    /// <summary>
    /// Identifies a possible kind for a type.
    /// </summary>
    public enum Kind
    {
        Primitive,
        Dynamic,
        Tuple,
        Struct,
        Variant,
        List,
        Set,
        Reference
    }

    /// <summary>
    /// Identifies and describes the type of a database object. Note that types are compared by reference; two types that have the same members
    /// and definition will not be considered equivalent unless they have the same reference.
    /// </summary>
    public abstract class Type
    {
        internal Type(string Name, Kind Kind)
        {
            this._Name = Name;
            this._Kind = Kind;
        }

        internal Type(Kind Kind)
        {
            this._Kind = Kind;
        }

        /// <summary>
        /// Gets the user-friendly name of this type.
        /// </summary>
        public string Name
        {
            get
            {
                if (this._Name == null)
                {
                    TupleType tt = this as TupleType;
                    if (tt != null)
                        return this._Name = TupleType.GetName(tt.ElementTypes);

                    ListType lt = this as ListType;
                    if (lt != null)
                        return this._Name = ListType.GetName(lt.ElementType);

                    SetType st = this as SetType;
                    if (st != null)
                        return this._Name = SetType.GetName(st.ElementType);

                    ReferenceType rt = this as ReferenceType;
                    if (rt != null && rt._TargetType != null)
                        return this._Name = ReferenceType.GetName(rt.TargetType);
                }
                return this._Name;
            }
        }

        /// <summary>
        /// Gets the kind of this type.
        /// </summary>
        public Kind Kind
        {
            get
            {
                return this._Kind;
            }
        }

        /// <summary>
        /// A primitive type with only one value, and no data.
        /// </summary>
        public static readonly PrimitiveType Void = new PrimitiveType("void", 0, false);

        /// <summary>
        /// The primitive type for an unsigned byte of data.
        /// </summary>
        public static readonly PrimitiveType Byte = new PrimitiveType("byte", 1, false);

        /// <summary>
        /// The primitive type for a character.
        /// </summary>
        public static readonly PrimitiveType Char = new PrimitiveType("char", 2, false);

        /// <summary>
        /// The primitive type for a signed 2-byte integer.
        /// </summary>
        public static readonly PrimitiveType Short = new PrimitiveType("short", 2, true);

        /// <summary>
        /// The primitive type for an unsigned 2-byte integer.
        /// </summary>
        public static readonly PrimitiveType UShort = new PrimitiveType("ushort", 2, false);

        /// <summary>
        /// The primitive type for a signed 4-byte integer.
        /// </summary>
        public static readonly PrimitiveType Int = new PrimitiveType("int", 4, true);

        /// <summary>
        /// The primitive type for a unsigned 4-byte integer.
        /// </summary>
        public static readonly PrimitiveType UInt = new PrimitiveType("uint", 4, false);

        /// <summary>
        /// The primitive type for a signed 8-byte integer.
        /// </summary>
        public static readonly PrimitiveType Long = new PrimitiveType("long", 8, true);

        /// <summary>
        /// The primitive type for an unsigned 8-byte integer.
        /// </summary>
        public static readonly PrimitiveType ULong = new PrimitiveType("ulong", 8, false);

        /// <summary>
        /// The primitive type for a single-precision floating point number.
        /// </summary>
        public static readonly PrimitiveType Float = new PrimitiveType("float", 4, true);

        /// <summary>
        /// The primitive type for a double-precision floating point number.
        /// </summary>
        public static readonly PrimitiveType Double = new PrimitiveType("double", 8, true);

        /// <summary>
        /// The primitive type for a decimal number.
        /// </summary>
        public static readonly PrimitiveType Decimal = new PrimitiveType("decimal", 16, true);

        /// <summary>
        /// The type for a string of character.
        /// </summary>
        public static readonly ListType String = Type.Char.List;

        /// <summary>
        /// The type for a boolean value.
        /// </summary>
        public static readonly VariantType Bool = Type.Variant("bool", new Option[] 
        { 
            new Option("false", Type.Void), 
            new Option("true", Type.Void) 
        });

        /// <summary>
        /// The type for arbitrary data.
        /// </summary>
        public static readonly ListType Data = Type.Byte.List;

        /// <summary>
        /// The type for a dynamic value with an indefinite type.
        /// </summary>
        public static readonly DynamicType Dynamic = new DynamicType();

        /// <summary>
        /// Constructs a tuple type with elements of the given types.
        /// </summary>
        public static TupleType Tuple(Type[] ElementTypes)
        {
            return new TupleType(ElementTypes);
        }

        /// <summary>
        /// Constructs a struct type with the given name and fields.
        /// </summary>
        public static StructType Struct(string Name, Field[] Fields)
        {
            return new StructType(Name, Fields);
        }

        /// <summary>
        /// Constructs a variant type with the given name and options.
        /// </summary>
        public static VariantType Variant(string Name, Option[] Options)
        {
            return new VariantType(Name, Options);
        }

        /// <summary>
        /// Constructs a struct type that has a reference to itself by calling the given function with a reference type to the type it returns. Some operations
        /// (such as .Name) may not be used withint he Construct function.
        /// </summary>
        public static StructType Fix(Func<ReferenceType, StructType> Construct)
        {
            ReferenceType rt = new ReferenceType();
            StructType t = Construct(rt);
            rt._TargetType = t;
            return t;
        }

        /// <summary>
        /// Gets the type for a list of elements of this type.
        /// </summary>
        public ListType List
        {
            get
            {
                if (this._List == null)
                    this._List = new ListType(this);
                return this._List;
            }
        }

        /// <summary>
        /// Gets the type for a set of elements of this type.
        /// </summary>
        public SetType Set
        {
            get
            {
                if (this._Set == null)
                    this._Set = new SetType(this);
                return this._Set;
            }
        }

        /// <summary>
        /// Gets the type for a reference to a value of this type.
        /// </summary>
        public ReferenceType Reference
        {
            get
            {
                if (this._Reference == null)
                    this._Reference = new ReferenceType(this);
                return this._Reference;
            }
        }

        /// <summary>
        /// Creates a handle to the default value of this type.
        /// </summary>
        public virtual Handle Default()
        {
            return null;
        }

        private string _Name;
        private Kind _Kind;
        private ListType _List;
        private SetType _Set;
        private ReferenceType _Reference;
    }

    /// <summary>
    /// A type for a simple numerical value.
    /// </summary>
    public sealed class PrimitiveType : Type
    {
        internal PrimitiveType(string Name, int Size, bool Signed)
            : base(Name, Kind.Primitive)
        {
            this.Signed = Signed;
            this.Size = Size;
        }

        /// <summary>
        /// The size, in bytes, of the values of this type.
        /// </summary>
        public readonly int Size;

        /// <summary>
        /// Indicates wether this primitive type allows for negative values.
        /// </summary>
        public readonly bool Signed;
    }

    /// <summary>
    /// A type that stores a value of an indefinite type.
    /// </summary>
    public sealed class DynamicType : Type
    {
        internal DynamicType()
            : base("dynamic", Kind.Dynamic)
        {

        }
    }

    /// <summary>
    /// A type for an ordered, heterogeneously-typed collection.
    /// </summary>
    public sealed class TupleType : Type
    {
        internal TupleType(Type[] ElementTypes)
            : base(Kind.Tuple)
        {
            this.ElementTypes = ElementTypes;
        }

        /// <summary>
        /// Gets the name for the tuple with the given element types.
        /// </summary>
        public static string GetName(Type[] ElementTypes)
        {
            StringBuilder str = new StringBuilder();
            str.Append("(");
            if (ElementTypes.Length > 0)
            {
                string name = ElementTypes[0].Name;
                if (name == null)
                    return null;
                str.Append(name);
            }
            for (int t = 1; t < ElementTypes.Length; t++)
            {
                str.Append(", ");
                string name = ElementTypes[0].Name;
                if (name == null)
                    return null;
                str.Append(name);
            }
            str.Append(")");
            return str.ToString();
        }

        /// <summary>
        /// The types for the elements of the tuple.
        /// </summary>
        public readonly Type[] ElementTypes;
    }

    /// <summary>
    /// A collection of named elements, or fields.
    /// </summary>
    public sealed class StructType : Type
    {
        internal StructType(string Name, Field[] Fields)
            : base(Name, Kind.Struct)
        {
            this.Fields = Fields;
            this.FieldByName = new Dictionary<string, int>();
            for (int t = 0; t < this.Fields.Length; t++)
            {
                this.FieldByName[this.Fields[t].Name] = t;
            }
        }

        /// <summary>
        /// The fields for this struct type.
        /// </summary>
        public readonly Field[] Fields;

        /// <summary>
        /// A mapping of names to fields.
        /// </summary>
        public readonly Dictionary<string, int> FieldByName;
    }

    /// <summary>
    /// Contains information about a field in a struct type.
    /// </summary>
    public struct Field
    {
        public Field(string Name, Type Type)
        {
            this.Name = Name;
            this.Type = Type;
        }

        /// <summary>
        /// The type of this field.
        /// </summary>
        public Type Type;

        /// <summary>
        /// The name of this field.
        /// </summary>
        public string Name;
    }

    /// <summary>
    /// A type for a value that can be one of many different named forms.
    /// </summary>
    public sealed class VariantType : Type
    {
        internal VariantType(string Name, Option[] Options)
            : base(Name, Kind.Variant)
        {
            this.Options = Options;
            this.OptionByName = new Dictionary<string, int>();
            for (int t = 0; t < this.Options.Length; t++)
            {
                this.OptionByName[this.Options[t].Name] = t;
            }
        }

        /// <summary>
        /// The options for this variant type.
        /// </summary>
        public readonly Option[] Options;

        /// <summary>
        /// A mapping of names to options.
        /// </summary>
        public readonly Dictionary<string, int> OptionByName;
    }

    /// <summary>
    /// Contains information about an option (or form) for a variant type.
    /// </summary>
    public struct Option
    {
        public Option(string Name, Type Type)
        {
            this.Name = Name;
            this.Type = Type;
        }

        /// <summary>
        /// The type of the additional information included with this form.
        /// </summary>
        public readonly Type Type;

        /// <summary>
        /// The name of this form.
        /// </summary>
        public readonly string Name;
    }

    /// <summary>
    /// A type for a variable-size ordered collection of homogeneously-typed elements.
    /// </summary>
    public sealed class ListType : Type
    {
        internal ListType(Type ElementType)
            : base("[" + ElementType.Name + "]", Kind.List)
        {
            this.ElementType = ElementType;
        }

        /// <summary>
        /// Gets the name of a list type with the given element type.
        /// </summary>
        public static string GetName(Type ElementType)
        {
            string name = ElementType.Name;
            if (name != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("[");
                sb.Append(ElementType.Name);
                sb.Append("]");
                return sb.ToString();
            }
            return null;
        }

        /// <summary>
        /// The type for elements of this list type.
        /// </summary>
        public readonly Type ElementType;
    }

    /// <summary>
    /// A type for a variable-size unordered collection of homogeneously-typed elements where elements can
    /// not occur more than once.
    /// </summary>
    public sealed class SetType : Type
    {
        internal SetType(Type ElementType)
            : base(GetName(ElementType), Kind.Set)
        {
            this.ElementType = ElementType;
        }

        /// <summary>
        /// Gets the name of a set type with the given element type.
        /// </summary>
        public static string GetName(Type ElementType)
        {
            string name = ElementType.Name;
            if (name != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                sb.Append(ElementType.Name);
                sb.Append("}");
                return sb.ToString();
            }
            return null;
        }

        /// <summary>
        /// The type for elements of this set type.
        /// </summary>
        public readonly Type ElementType;
    }

    /// <summary>
    /// A type for a value that contains a reference to another value within the same database.
    /// </summary>
    public sealed class ReferenceType : Type
    {
        internal ReferenceType(Type TargetType)
            : base(Kind.Reference)
        {
            this._TargetType = TargetType;
        }

        internal ReferenceType()
            : base(Kind.Reference)
        {

        }

        /// <summary>
        /// Gets the name of a reference type with the given target type.
        /// </summary>
        public static string GetName(Type TargetType)
        {
            string name = TargetType.Name;
            if (name != null)
                return name + "*";
            else
                return null;
        }

        /// <summary>
        /// Gets the target type of this reference.
        /// </summary>
        public Type TargetType
        {
            get
            {
                return this._TargetType;
            }
        }

        internal Type _TargetType;
    }
}
