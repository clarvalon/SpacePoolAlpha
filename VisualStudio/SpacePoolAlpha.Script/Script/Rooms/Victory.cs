// Room_Victory - Type "override" followed by space to see list of C# methods to implement
using static SpacePoolAlpha.GlobalBase;
using static SpacePoolAlpha.Room_Victory;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MessagePack;
using Clarvalon.XAGE.Global;

namespace SpacePoolAlpha
{
    public partial class Room_Victory // 9
    {
        // Fields
        public int fadeTime = -1;
        public int roomTime;
        public DynamicSprite back;

        // Methods
        public override void room_Load()
        {
            back = DynamicSprite.CreateFromBackground();
            fadeTime = -1;
            roomTime = 0;
            aSPACPOL2.Play();
        }

        public override void room_RepExec()
        {
            DrawingSurface surf = Room.GetDrawingSurfaceForBackground();
            surf.DrawImage(0, 0, back.Graphic);
            if (roomTime < 100)
            {
                roomTime += 1;
            }
            else 
            {
                surf.DrawingColor = Game.GetColorFromRGB(255, 255, 255);
                surf.DrawAntialiasedVectorText(225.0f, 180.0f, 4.0f, "PRESS ESC TO RETURN TO THE MENU");
            }
            if (fadeTime >= 0)
            {
                fadeTime += 1;
                if (fadeTime <= 50)
                {
                    surf.Fade(fadeTime);
                    surf.Release();
                }
                else 
                {
                    cEgo.ChangeRoom(2);
                }
            }
        }

        public override void on_key_press(eKeyCode key)
        {
            if (key == eKeyEscape && roomTime >= 100 && fadeTime == -1)
            {
                fadeTime = 0;
            }
        }

    }

    #region Globally Exposed Items

    public partial class GlobalBase
    {

    }

    #endregion

}
