// Room_Intro - Type "override" followed by space to see list of C# methods to implement
using static SpacePoolAlpha.GlobalBase;
using static SpacePoolAlpha.Room_Intro;
using static SpacePoolAlpha.IntroStaticRef;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MessagePack;
using Clarvalon.XAGE.Global;
using Microsoft.Xna.Framework.Input;

namespace SpacePoolAlpha
{
    public partial class Room_Intro // 8
    {
        // Fields
        public int fadeTime = -1;
        public SLine[] lines = CreateAndInstantiateArray<SLine>(512);
        public int numLines;
        public int frame;
        public int stage = 1;

        // Methods
        public void load_sig(String signame)
        {
            numLines = 0;
            File f = File.Open(signame, eFileRead);
            if (f != null)
            {
                numLines = f.ReadInt();
                int l = 0;
                while (l < numLines)
                {
                    lines[l].x = f.ReadInt();
                    lines[l].y = f.ReadInt();
                    lines[l].solid = f.ReadInt();
                    lines[l].frame = f.ReadInt();
                    l += 1;
                }
                f.Close();
                if (numLines > 1)
                {
                    int offset = lines[1].frame - lines[0].frame;
                    if (offset > 1)
                    {
                        l = 1;
                        while (l < numLines)
                        {
                            lines[l].frame -= offset;
                            l += 1;
                        }
                    }
                }
                frame = 0;
            }
        }

        public void draw_sig(DrawingSurface surf, int x, int y)
        {
            int i = 0;
            while (i < numLines)
            {
                if (frame == lines[i].frame/2)
                {
                    if (lines[i].solid)
                    {
                        int n = i;
                        surf.DrawAntialiasedLine(0.5f *IntToFloat(2*x + lines[n-1].x), 0.5f *IntToFloat(2*y + lines[n-1].y), 0.5f *IntToFloat(2*x + lines[n].x), 0.5f *IntToFloat(2*y + lines[n].y));
                    }
                    if (i == numLines-1)
                    {
                        stage += 1;
                        if (stage == 2)
                        {
                            surf.DrawVectorText(160.0f, 67.0f, 3.0f, "STEVE MCCREA");
                            surf.DrawVectorText(160.0f, 67.0f, 3.0f, "STEVE MCCREA");
                            load_sig("SIGSHEL.DAT");
                        }
                        else if (stage == 3)
                        {
                            surf.DrawVectorText(70.0f, 95.0f, 3.0f, "SHELDON MOSKOWITZ");
                            surf.DrawVectorText(70.0f, 95.0f, 3.0f, "SHELDON MOSKOWITZ");
                            load_sig("SIGMODS.DAT");
                        }
                        else if (stage == 4)
                        {
                            surf.DrawVectorText(265.0f, 95.0f, 3.0f, "MODS");
                            surf.DrawVectorText(265.0f, 95.0f, 3.0f, "MODS");
                        }
                        frame = 0;
                    }
                }
                i += 1;
            }
        }

        public override void room_Load()
        {
            mouse.Visible = false;
            stage = 1;
            load_sig("SIGSTEVE.DAT");
            ships_setup();
            aSPACPOOL.Play();
            FadeOutObj.Transparency = 100;
        }

        public override void repeatedly_execute_always()
        {
            DrawingSurface surf = Room.GetDrawingSurfaceForBackground();
            surf.DrawingColor = Game.GetColorFromRGB(255, 255, 255);
            if (numLines > 0)
                frame += 1;
            if (stage == 1)
            {
                if (frame == 1)
                {
                    surf.DrawVectorText(160.0f, 10.0f, 4.0f, "KWEEPA PRODUCTIONS 2010");
                    surf.DrawVectorText(160.0f, 10.0f, 4.0f, "KWEEPA PRODUCTIONS 2010");
                    surf.DrawVectorText(10.0f, 194.0f, 3.0f, "V16");
                    surf.DrawVectorText(10.0f, 194.0f, 3.0f, "V16");
                }
                draw_sig(surf, 160, 55);
            }
            else if (stage == 2)
            {
                draw_sig(surf, 120, 55);
            }
            else if (stage == 3)
            {
                draw_sig(surf, 240, 70);
            }
            else 
            {
                if (frame == 40)
                {
                    surf.DrawVectorText(160.0f, 110.0f, 4.0f, "P R E S E N T");
                    surf.DrawVectorText(160.0f, 110.0f, 4.0f, "P R E S E N T");
                }
                else if (frame == 100)
                {
                    surf.DrawVectorText(160.0f, 130.0f, 7.0f, "SPACE POOL ALPHA");
                    surf.DrawVectorText(160.0f, 130.0f, 7.0f, "SPACE POOL ALPHA");
                }
                else if (frame == 120)
                {
                    surf.DrawVectorText(246.0f, 124.0f, 3.0f, "TM");
                    surf.DrawVectorText(246.0f, 124.0f, 3.0f, "TM");
                    aLIDPING.Play();
                }
                else if (frame == 200)
                {
                    surf.DrawVectorText(160.0f, 152.0f, 3.0f, "ADDIIONAL GAME DESIGN AND TESTING BY BICILOTTI");
                    surf.DrawVectorText(160.0f, 160.0f, 3.0f, "ORIGINALLY MADE WITH ADVENTURE GAME STUDIO BY CHRIS JONES");
                    surf.DrawVectorText(160.0f, 152.0f, 3.0f, "ADDIIONAL GAME DESIGN AND TESTING BY BICILOTTI");
                    surf.DrawVectorText(160.0f, 160.0f, 3.0f, "ORIGINALLY MADE WITH ADVENTURE GAME STUDIO BY CHRIS JONES");
                    surf.DrawVectorText(160.0f, 168.0f, 3.0f, "PORTED TO XAGE BY DAN ALEXANDER");
                    surf.DrawVectorText(160.0f, 168.0f, 3.0f, "PORTED TO XAGE BY DAN ALEXANDER");
                }
                else if (frame == 250)
                {
                    surf.DrawVectorText(160.0f, 190.0f, 3.0f, "PRESS ANY KEY TO CONTINUE");
                    surf.DrawVectorText(160.0f, 190.0f, 3.0f, "PRESS ANY KEY TO CONTINUE");
                }
            }
            surf.Release();
        }

        public override void on_key_press(eKeyCode key)
        {
            if (key != eKeyCtrlX && fadeTime == -1)
            {
                fadeTime = 0;
            }
        }

        public override void ButtonPress(Buttons button)
        {
            if ((button == Buttons.A || button == Buttons.Start) && fadeTime == -1)
            {
                fadeTime = 0;
            }
        }

        public override void room_RepExec()
        {
            if (fadeTime >= 0)
            {
                fadeTime += 1;
                if (fadeTime <= 50)
                {
                    FadeOutObj.Transparency = 100 - 2*fadeTime;
                }
                if (fadeTime >= 50)
                {
                    cEgo.ChangeRoom(2);
                }
            }
        }

    }

    #region Globally Exposed Items

    public partial class GlobalBase
    {

    }

    #endregion

    #region Static class for referencing parent class without prefixing with instance (AGS struct workaround)

    public static class IntroStaticRef
    {
        // Static Fields
        public static int fadeTime { get { return GlobalBase.Intro.fadeTime; } set { GlobalBase.Intro.fadeTime = value; } }
        public static SLine[] lines { get { return GlobalBase.Intro.lines; } set { GlobalBase.Intro.lines = value; } }
        public static int numLines { get { return GlobalBase.Intro.numLines; } set { GlobalBase.Intro.numLines = value; } }
        public static int frame { get { return GlobalBase.Intro.frame; } set { GlobalBase.Intro.frame = value; } }
        public static int stage { get { return GlobalBase.Intro.stage; } set { GlobalBase.Intro.stage = value; } }

        // Static Methods
        public static void load_sig(String signame)
        {
            GlobalBase.Intro.load_sig(signame);
        }

        public static void draw_sig(DrawingSurface surf, int x, int y)
        {
            GlobalBase.Intro.draw_sig(surf, x, y);
        }

        public static void room_Load()
        {
            GlobalBase.Intro.room_Load();
        }

        public static void room_RepExec()
        {
            GlobalBase.Intro.room_RepExec();
        }

    }

    #endregion
    
}
