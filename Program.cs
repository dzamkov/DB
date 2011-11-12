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
            Type teststruct = Type.Struct("TestStruct", new Field[]
            {
                new Field("A", Type.Int),
                new Field("B", Type.Int),
                new Field("C", Type.Int),
                new Field("D", Type.Int)
            });

            Handle testvalue = teststruct.Construct();
        }
    }
}
