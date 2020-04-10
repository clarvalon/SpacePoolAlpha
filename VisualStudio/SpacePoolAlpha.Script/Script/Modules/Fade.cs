// Module_Fade - Type "override" followed by space to see list of C# methods to implement
using static SpacePoolAlpha.GlobalBase;
using System.Diagnostics;
using static SpacePoolAlpha.Module_Fade;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MessagePack;
using Clarvalon.XAGE.Global;

namespace SpacePoolAlpha
{
    public partial class Module_Fade
    {
        // Fields
        DynamicSprite ds;

        // Methods
        public void ExtensionMethod_Fade(DrawingSurface  thisItem, int stateTime)
        {
            if (ds == null)
                ds = DynamicSprite.Create(1, 1);
            DrawingSurface spriteSurf = ds.GetDrawingSurface();
            spriteSurf.Clear(Game.GetColorFromRGB(0, 0, 0));
            spriteSurf.Release();
            thisItem.DrawImage(0, 0, ds.Graphic, 100 - 2*stateTime, 320, 200);
            //ds.Delete();
        }

    }

    #region Globally Exposed Items

    public partial class GlobalBase
    {

    }

    #endregion

    #region Extension Methods Wrapper (AGS workaround)

    public static partial class ExtensionMethods
    {
        public static void Fade(this DrawingSurface  thisItem, int stateTime)
        {
            GlobalBase.Fade.ExtensionMethod_Fade(thisItem, stateTime);
        }

    }

    #endregion

}
