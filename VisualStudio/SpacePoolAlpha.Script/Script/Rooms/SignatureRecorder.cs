// Room_SignatureRecorder - Type "override" followed by space to see list of C# methods to implement
using static SpacePoolAlpha.GlobalBase;
using static SpacePoolAlpha.Room_SignatureRecorder;
using static SpacePoolAlpha.SignatureRecorderStaticRef;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MessagePack;
using Clarvalon.XAGE.Global;

namespace SpacePoolAlpha
{
    public partial class Room_SignatureRecorder // 7
    {
        // Fields
        public SLine[] lines = CreateAndInstantiateArray<SLine>(512);
        public int numLines;
        public bool penWasDown;
        public int frame;
        public bool replay;
        public int lastDrawnFrame;

        // Methods
        public override void room_Load()
        {
            Mouse.Visible = true;
            numLines = 0;
            replay = false;
        }

        public override void repeatedly_execute_always()
        {
            if (numLines > 0)
                frame += 1;
            if (replay)
            {
                int i = 0;
                while (i < numLines)
                {
                    if (lines[i].frame == frame)
                    {
                        if (lines[i].solid)
                        {
                            int n = i;
                            DrawingSurface surf = Room.GetDrawingSurfaceForBackground();
                            surf.DrawingColor = Game.GetColorFromRGB(255, 255, 255);
                            surf.DrawAntialiasedLine(0.5f *IntToFloat(320 + lines[n-1].x), 0.5f *IntToFloat(200 + lines[n-1].y), 0.5f *IntToFloat(320 + lines[n].x), 0.5f *IntToFloat(200 + lines[n].y));
                            surf.Release();
                        }
                        i = numLines;
                    }
                    i += 1;
                }
            }
            else 
            {
                if (Mouse.IsButtonDown(eMouseLeft))
                {
                    int n = numLines;
                    if (lines[n].x != mouse.x || lines[n].y != mouse.y)
                    {
                        numLines += 1;
                        n = numLines;
                        lines[n].x = mouse.x;
                        lines[n].y = mouse.y;
                        lines[n].frame = frame;
                        if (penWasDown)
                        {
                            lines[n].solid = true;
                            DrawingSurface surf = Room.GetDrawingSurfaceForBackground();
                            surf.DrawingColor = Game.GetColorFromRGB(255, 255, 255);
                            surf.DrawAntialiasedLine(IntToFloat(lines[n-1].x), IntToFloat(lines[n-1].y), IntToFloat(lines[n].x), IntToFloat(lines[n].y));
                            surf.DrawAntialiasedLine(0.5f *IntToFloat(lines[n-1].x), 0.5f *IntToFloat(lines[n-1].y), 0.5f *IntToFloat(lines[n].x), 0.5f *IntToFloat(lines[n].y));
                            surf.Release();
                        }
                    }
                    penWasDown = true;
                }
                else 
                {
                    if (penWasDown)
                    {
                        numLines += 1;
                    }
                    lines[numLines].x = mouse.x;
                    lines[numLines].y = mouse.y;
                    lines[numLines].solid = false;
                    penWasDown = false;
                }
            }
        }

        public override void on_key_press(eKeyCode key)
        {
            if (key == eKeyCtrlS)
            {
                int lx = 320;
                int ly = 200;
                int hx = 0;
                int hy = 0;
                int l = 0;
                while (l < numLines)
                {
                    if (lines[l].solid)
                    {
                        if (lx > lines[l].x)
                            lx = lines[l].x;
                        else if (hx < lines[l].x)
                            hx = lines[l].x;
                        if (ly > lines[l].y)
                            ly = lines[l].y;
                        else if (hy < lines[l].y)
                            hy = lines[l].y;
                    }
                    l += 1;
                }
                int cx = (lx + hx)/2;
                int cy = 0;
                cy = (ly + cy)/2;
                Display("%d %d", cx, cy);
                File f = File.Open("SIG.DAT", eFileWrite);
                if (f != null)
                {
                    f.WriteInt(numLines);
                    l = 0;
                    while (l < numLines)
                    {
                        f.WriteInt(lines[l].x - cx);
                        f.WriteInt(lines[l].y - cy);
                        f.WriteInt(lines[l].solid);
                        f.WriteInt(lines[l].frame);
                        l += 1;
                    }
                    f.Close();
                }
            }
            else if (key == eKeyCtrlO)
            {
                File f = File.Open("SIG.DAT", eFileRead);
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
                    replay = true;
                    frame = -40;
                }
            }
        }

    }

    #region Globally Exposed Items

    public partial class GlobalBase
    {

    }

    #endregion

    #region SLine (AGS struct from .asc converted to class)

    [MessagePackObject(keyAsPropertyName:true)]
    public class SLine
    {
        // Fields
        public int x;
        public int y;
        public bool solid;
        public int frame;

    }

    #endregion

    #region Static class for referencing parent class without prefixing with instance (AGS struct workaround)

    public static class SignatureRecorderStaticRef
    {
        // Static Fields
        public static SLine[] lines { get { return GlobalBase.SignatureRecorder.lines; } set { GlobalBase.SignatureRecorder.lines = value; } }
        public static int numLines { get { return GlobalBase.SignatureRecorder.numLines; } set { GlobalBase.SignatureRecorder.numLines = value; } }
        public static bool penWasDown { get { return GlobalBase.SignatureRecorder.penWasDown; } set { GlobalBase.SignatureRecorder.penWasDown = value; } }
        public static int frame { get { return GlobalBase.SignatureRecorder.frame; } set { GlobalBase.SignatureRecorder.frame = value; } }
        public static bool replay { get { return GlobalBase.SignatureRecorder.replay; } set { GlobalBase.SignatureRecorder.replay = value; } }
        public static int lastDrawnFrame { get { return GlobalBase.SignatureRecorder.lastDrawnFrame; } set { GlobalBase.SignatureRecorder.lastDrawnFrame = value; } }

        // Static Methods
        public static void room_Load()
        {
            GlobalBase.SignatureRecorder.room_Load();
        }

    }

    #endregion
    
}
