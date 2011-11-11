using System;
using System.Collections.Generic;
using System.Linq;

namespace DB
{
    /// <summary>
    /// Contains program related functions.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Program main entry-point.
        /// </summary>
        public static void Main(string[] Args)
        {
            Type mytype = Type.Struct("Stuff", new Field[]
            {
                new Field("Frequency", Type.Int),
                new Field("Gender", Type.Variant("Gender", new Option[]
                {
                    new Option("Male", Type.Void),
                    new Option("Female", Type.Void),
                    new Option("Undecided", Type.Void),
                })),
                new Field("Name", Type.String),
                new Field("Owner", Type.Int.Reference),
                new Field("Friends", Type.String.Set),
                new Field("Log", Type.String.List),
                new Field("IsHappy", Type.Bool)
            });

            Handle a = mytype.Default();
            a["Frequency"] = 4;
            a["Gender"].Set("Male");
            a["Name"] = "Something";
            a["Owner"].Target = 4;

            a["Friends"].Clear();
            a["Friends"].Insert("Some Guy");

            a["Log"].Clear();
            a["Log"].Append("Cause blah blah");

            a["IsHappy"] = true;
        }
    }
}
