// Module_Random - Type "override" followed by space to see list of C# methods to implement
using static SpacePoolAlpha.GlobalBase;
using System.Diagnostics;
using static SpacePoolAlpha.Module_Random;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MessagePack;
using Clarvalon.XAGE.Global;

namespace SpacePoolAlpha
{
    public partial class Module_Random
    {
        // Fields
        public int x;

        // Methods
        public void Random2_Seed(int seed)
        {
            x = seed;
        }

        public int Random2(int max)
        {
            x = (9821*x + 6925) % 65535;
            return x % (max + 1);
        }

    }

    #region Globally Exposed Items

    public partial class GlobalBase
    {
        // Expose Random methods so they can be used without instance prefix
        public static void Random2_Seed(int seed)
        {
            RandomInstance.Random2_Seed(seed);
        }

        public static int Random2(int max)
        {
            return RandomInstance.Random2(max);
        }


    }

    #endregion

}
