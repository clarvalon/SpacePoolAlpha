// Room_TimeAttack - Type "override" followed by space to see list of C# methods to implement
using static SpacePoolAlpha.GlobalBase;
using static SpacePoolAlpha.Room_TimeAttack;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MessagePack;
using Clarvalon.XAGE.Global;

namespace SpacePoolAlpha
{
    public partial class Room_TimeAttack // 3
    {
        // Fields
        public int fadeTime = -1;
        public int roomTime;
        public bool done;
        public bool enteringName;
        public int cursorFlash;
        public String topName;
        public int topIndex;
        public int[] topTimes = new int[5];
        public String[] topNames = new String[5];

        // Methods
        public override void room_Load()
        {
            pool_set_num_players(1);
            pool_set_num_ships(1);
            pool_setup();
            pool_rack_em(0);
            ships[0].disableControls = 0;
            fadeTime = -1;
            roomTime = 0;
            done = false;
            enteringName = false;
            cursorFlash = 0;
            topTimes[0] = 3200;
            topTimes[1] = 3600;
            topTimes[2] = 4400;
            topTimes[3] = 4800;
            topTimes[4] = 5200;
            topNames[0] = "STEVE";
            topNames[1] = "BICI";
            topNames[2] = "SHELDON";
            topNames[3] = "MODS";
            topNames[4] = "CHRIS";
            if (File.Exists("HI.DAT"))
            {
                File f = File.Open("HI.DAT", eFileRead);
                if (f)
                {
                    int i = 0;
                    while (i < 5)
                    {
                        topTimes[i] = f.ReadInt();
                        topNames[i] = f.ReadStringBack();
                        i += 1;
                    }
                    f.Close();
                }
            }
            replay_start();
            aSPACTUT.Play();
        }

        public void save_scores()
        {
            File f = File.Open("HI.DAT", eFileWrite);
            if (f)
            {
                int i = 0;
                while (i < 5)
                {
                    f.WriteInt(topTimes[i]);
                    f.WriteString(topNames[i]);
                    i += 1;
                }
                f.Close();
            }
        }

        public override void repeatedly_execute_always()
        {
            pool_update();
            if (!done)
            {
                roomTime += 1;
            }
            DrawingSurface surf = Room.GetDrawingSurfaceForBackground();
            surf.DrawingColor = Game.GetColorFromRGB(255, 255, 255);
            if (enteringName)
            {
                surf.DrawingColor = Game.GetColorFromRGB(255, 255, 0);
                cursorFlash += 1;
                if ((cursorFlash % 40) < 30)
                {
                    int x = 200 + 6*topName.Length;
                    int y = 100 + 12*topIndex;
                    surf.DrawLine(x, y+3, x+5, y+3);
                }
                surf.DrawVectorText(200.0f, IntToFloat(100 + 12*topIndex), 4.0f, topName, eJustify_Left);
                surf.DrawingColor = Game.GetColorFromRGB(255, 255, 255);
                surf.DrawVectorText(160.0f, 10.0f, 4.0f, "ENTER YOUR NAME");
            }
            else 
            {
                String message = StringFormatAGS("TIME %s", get_time_string(roomTime));
                surf.DrawVectorText(20.0f, 10.0f, 4.0f, message, eJustify_Left);
                String message2 = StringFormatAGS("HIGH %s %s", get_time_string(topTimes[0]), topNames[0]);
                surf.DrawVectorText(300.0f, 10.0f, 4.0f, message2, eJustify_Right);
            }
            if (ships[0].numPocketed == 9)
            {
                if (!done)
                {
                    done = true;
                    if (replay_get_time() < replay_get_best())
                    {
                        replay_end();
                    }
                    int i2 = 0;
                    while (i2 < 5)
                    {
                        if (roomTime < topTimes[i2])
                        {
                            int j = 4;
                            while (j > i2)
                            {
                                topTimes[j] = topTimes[j - 1];
                                topNames[j] = topNames[j - 1];
                                j -= 1;
                            }
                            topTimes[i2] = roomTime;
                            topNames[i2] = "";
                            topName = "";
                            topIndex = i2;
                            enteringName = true;
                            i2 = 5;
                        }
                        i2 += 1;
                    }
                }
                String time = get_time_string(roomTime);
                String message = StringFormatAGS("TIME %s]", time);
                surf.WriteAliasedTextMessage(160.0f, 60.0f, 7.0f, message);
                int i = 0;
                while (i < 5)
                {
                    surf.DrawVectorText(100.0f, 100.0f + IntToFloat(12*i), 4.0f, get_time_string(topTimes[i]), eJustify_Centre);
                    surf.DrawVectorText(200.0f, 100.0f + IntToFloat(12*i), 4.0f, topNames[i], eJustify_Left);
                    i += 1;
                }
            }
            else if (ships[1].numPocketed > 0)
            {
                done = true;
                surf.WriteAliasedTextMessage(160.0f, 90.0f, 7.0f, "FAILED!]KNOCKED IN A BALL]");
            }
            if (fadeTime >= 0)
            {
                fadeTime += 1;
                if (fadeTime <= 50)
                {
                    surf.Fade(fadeTime);
                }
            }
            else if (roomTime < 400 || done)
            {
                surf.DrawingColor = Game.GetColorFromRGB(255, 255, 255);
                surf.DrawVectorText(160.0f, 190.0f, 3.0f, "PRESS ESC TO RETURN TO THE MENU");
            }
            surf.Release();
        }

        public override void on_key_press(eKeyCode key)
        {
            if (enteringName)
            {
                if (key == eKeyEscape)
                {
                    topNames[topIndex] = "PLAYER1";
                    enteringName = false;
                    save_scores();
                }
                else if ((key >= 'A' && key <= 'Z') || key == '!' || key == '.' || key == ',' || key == '?' || key == ' ')
                {
                    if (topName.Length < 16)
                    {
                        topName = topName.AppendChar(key);
                    }
                }
                else if (key == eKeyDelete || key == eKeyBackspace)
                {
                    if (topName.Length > 0)
                    {
                        topName = topName.Substring(0, topName.Length-1);
                    }
                }
                else if (key == eKeyReturn)
                {
                    topNames[topIndex] = topName;
                    enteringName = false;
                    save_scores();
                }
            }
            else 
            {
                if (key == eKeyEscape && fadeTime == -1)
                {
                    fadeTime = 0;
                }
            }
        }

        public override void room_RepExec()
        {
            if (fadeTime >= 50)
            {
                replay_Record = false;
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
