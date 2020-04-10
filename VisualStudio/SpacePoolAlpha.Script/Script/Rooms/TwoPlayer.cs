// Room_TwoPlayer - Type "override" followed by space to see list of C# methods to implement
using static SpacePoolAlpha.GlobalBase;
using static SpacePoolAlpha.Room_TwoPlayer;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MessagePack;
using Clarvalon.XAGE.Global;

namespace SpacePoolAlpha
{
    public partial class Room_TwoPlayer // 4
    {
        // Fields
        public int fadeTime = -1;
        public int roomTime;

        // Methods
        public override void room_Load()
        {
            pool_set_num_players(2);
            pool_set_num_ships(2);
            pool_setup();
            ship_load(0);
            pool_rack_em(0);
            fadeTime = -1;
            roomTime = 0;
            aSPACCER.Play();
        }

        public override void repeatedly_execute_always()
        {
            pool_update();
            DrawingSurface surf = Room.GetDrawingSurfaceForBackground();
            int numPocketedBy0 = ships[0].numPocketed;
            int numPocketedBy1 = ships[1].numPocketed;
            if (numPocketedBy0 >= 5 || numPocketedBy1 >= 5)
            {
                surf.DrawingColor = Game.GetColorFromRGB(255, 255, 255);
                if (numPocketedBy0 > numPocketedBy1)
                {
                    surf.WriteAliasedTextMessage(160.0f, 100.0f, 7.0f, "PLAYER 1 WINS]");
                }
                else 
                {
                    surf.WriteAliasedTextMessage(160.0f, 100.0f, 7.0f, "PLAYER 2 WINS]");
                }
                if (fadeTime == -1)
                {
                    surf.DrawVectorText(160.0f, 190.0f, 3.0f, "PRESS ESC TO RETURN TO THE MENU");
                }
            }
            if (fadeTime >= 0)
            {
                fadeTime += 1;
                if (fadeTime <= 50)
                {
                    surf.Fade(fadeTime);
                }
            }
            else if (roomTime < 400)
            {
                roomTime += 1;
                surf.DrawingColor = Game.GetColorFromRGB(255, 255, 255);
                surf.DrawVectorText(160.0f, 190.0f, 3.0f, "PRESS ESC TO RETURN TO THE MENU");
                surf.DrawVectorText(160.0f, 10.0f, 4.0f, "PLAYER 2 KEYS ARE THE CURSOR KEYS");
            }
            surf.Release();
        }

        public override void on_key_press(eKeyCode key)
        {
            if (key == eKeyEscape && fadeTime == -1)
            {
                fadeTime = 0;
            }
        }

        public override void room_RepExec()
        {
            if (fadeTime >= 50)
            {
                cEgo.ChangeRoom(2);
            }
        }

    }

    #region Globally Exposed Items

    public partial class GlobalBase
    {

    }

    #endregion

}
