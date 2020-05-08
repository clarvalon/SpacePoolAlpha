// Module_Time - Type "override" followed by space to see list of C# methods to implement
using static SpacePoolAlpha.GlobalBase;
using System.Diagnostics;
using static SpacePoolAlpha.Module_Time;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MessagePack;
using Clarvalon.XAGE.Global;

namespace SpacePoolAlpha
{
    public partial class Module_Time
    {
        // Fields

        // Methods
        public String get_time_string(int time)
        {
            int minutes = time / (60 * 40);
            int seconds = (time / 40) - (60 * minutes);
            int tenths = (time - (40 * seconds) - (40 * 60 * minutes)) / 4;

            String timeString = StringFormatAGS("%02d.%02d.%d", minutes, seconds, tenths);

            return timeString;
        }

    }

    #region Globally Exposed Items

    public partial class GlobalBase
    {
        // Expose Time methods so they can be used without instance prefix
        public static String get_time_string(int time)
        {
            return Time.get_time_string(time);
        }


    }

    #endregion

}
