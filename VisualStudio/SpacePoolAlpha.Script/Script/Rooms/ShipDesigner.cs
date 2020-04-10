// Room_ShipDesigner - Type "override" followed by space to see list of C# methods to implement
using static SpacePoolAlpha.GlobalBase;
using static SpacePoolAlpha.Room_ShipDesigner;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MessagePack;
using Clarvalon.XAGE.Global;

namespace SpacePoolAlpha
{
    public partial class Room_ShipDesigner // 6
    {
        // Fields
        public int ship = 6;
        public bool dragging;
        public bool isSphere;
        public int selVert = -1;
        public int selElem = -1;
        public float angle = 0.0f;

        // Methods
        public override void room_Load()
        {
            design.numVerts = 0;
            design.numElems = 0;
            ship_load(ship);
            Mouse.Visible = true;
        }

        public int mouse_snap(int x)
        {
            if (x < 14)
                return -1;
            if (x > 17 + 128)
                return -1;
            return (x - 14)/4;
        }

        public override void repeatedly_execute_always()
        {
            DrawingSurface surf = Room.GetDrawingSurfaceForBackground();
            surf.Clear(Game.GetColorFromRGB(16, 64, 16));
            surf.DrawAntialiasedVectorText(32.0f, 180.0f, 4.0f, StringFormatAGS("SHIP%d", ship));
            surf.DrawingColor = Game.GetColorFromRGB(32, 128, 128);
            surf.DrawAntialiasedCircle(80.0f, 80.0f, 64.0f);
            int x = 0;
            while (x < 33)
            {
                if (x%8 == 0)
                    surf.DrawingColor = Game.GetColorFromRGB(96, 96, 96);
                else 
                    surf.DrawingColor = Game.GetColorFromRGB(72, 72, 72);
                surf.DrawLine(16 + 4*x, 16, 16 + 4*x, 16 + 128);
                surf.DrawLine(16, 16 + 4*x, 16 + 128, 16 + 4*x);
                x += 1;
            }
            int i = 0;
            while (i < design.numVerts)
            {
                if (i == selVert)
                    surf.DrawingColor = Game.GetColorFromRGB(255, 0, 0);
                else 
                    surf.DrawingColor = Game.GetColorFromRGB(255, 255, 255);
                surf.DrawCircle(16 + 4*design.vx[i], 16 + 4*design.vy[i], 1);
                i += 1;
            }
            i = 0;
            surf.DrawingColor = Game.GetColorFromRGB(192, 192, 192);
            while (i < design.numElems)
            {
                int v1 = design.v1[i];
                if (!design.isSphere[i])
                {
                    int v2 = design.v2[i];
                    surf.DrawLine(16 + 4*design.vx[v1], 16 + 4*design.vy[v1], 16 + 4*design.vx[v2], 16 + 4*design.vy[v2]);
                }
                else 
                {
                    surf.DrawAntialiasedCircle(IntToFloat(16 + 4*design.vx[v1]), IntToFloat(16 + 4*design.vy[v1]), IntToFloat(4*design.radius[i]));
                }
                i += 1;
            }
            int mx = mouse_snap(mouse.x);
            int my = mouse_snap(mouse.y);
            if (mx != -1 && my != -1)
            {
                if (dragging && Mouse.IsButtonDown(eMouseLeft))
                {
                    if (selVert != -1)
                    {
                        design.vx[selVert] = mx;
                        design.vy[selVert] = my;
                    }
                }
            }
            if (!Mouse.IsButtonDown(eMouseLeft))
            {
                dragging = false;
            }
            angle += 0.02f;
            float ca = Maths.Cos(angle);
            float sa = Maths.Sin(angle);
            ship_draw(surf, 250.0f + ca, 20.0f + sa, ca, sa);
        }

        public override void on_key_press(eKeyCode key)
        {
            if (key == eKeyDelete)
                if (selVert != -1)
            {
                int i = 0;
                while (i < design.numElems)
                {
                    if (design.v1[i] == selVert || design.v2[i] == selVert)
                    {
                        int j = i + 1;
                        while (j < design.numElems)
                        {
                            design.isSphere[j-1] = design.isSphere[j];
                            design.v1[j-1] = design.v1[j];
                            design.v2[j-1] = design.v2[j];
                            design.radius[j-1] = design.radius[j];
                            j += 1;
                        }
                        design.numElems -= 1;
                    }
                    else 
                    {
                        if (design.v1[i] > selVert)
                            design.v1[i] -= 1;
                        if (design.v2[i] > selVert)
                            design.v2[i] -= 1;
                        i += 1;
                    }
                }
                i = selVert + 1;
                while (i < design.numVerts)
                {
                    design.vx[i-1] = design.vx[i];
                    design.vy[i-1] = design.vy[i];
                    i += 1;
                }
                design.numVerts -= 1;
                selVert = -1;
            }
            if (selElem != -1)
            {
                if (key == eKeyPageUp && design.radius[selElem] < 20)
                {
                    design.radius[selElem] += 1;
                }
                else if (key == eKeyPageDown && design.radius[selElem] > 1)
                {
                    design.radius[selElem] -= 1;
                }
            }
            if (key == eKeyCtrlS)
            {
                ship_save();
            }
            if (key == eKeyOpenBracket && ship > 0)
            {
                ship -= 1;
                ship_load(ship);
            }
            else if (key == eKeyCloseBracket && ship < 31)
            {
                ship += 1;
                ship_load(ship);
            }
        }

        public override void on_mouse_click(MouseButton button)
        {
            int mx = mouse_snap(mouse.x);
            int my = mouse_snap(mouse.y);
            if (mx != -1 && my != -1)
            {
                if (button == eMouseLeft)
                {
                    if (IsKeyPressed(405))
                    {
                        if (design.numVerts < MAXVERTS)
                        {
                            bool shouldPlace = true;
                            int v = 0;
                            while (v < design.numVerts   && shouldPlace)
                            {
                                if (design.vx[v] == mx && design.vy[v] == my)
                                {
                                    shouldPlace = false;
                                }
                                v += 1;
                            }
                            if (shouldPlace)
                            {
                                design.vx[design.numVerts] = mx;
                                design.vy[design.numVerts] = my;
                                design.numVerts += 1;
                            }
                        }
                    }
                    else 
                    {
                        int vert = -1;
                        int v = 0;
                        while (v < design.numVerts   && vert == -1)
                        {
                            if (design.vx[v] == mx && design.vy[v] == my)
                            {
                                vert = v;
                            }
                            v += 1;
                        }
                        if (vert != -1)
                        {
                            if (IsKeyPressed(403))
                            {
                                if (selVert != -1 && selVert != vert && design.numElems < MAXELEMS)
                                {
                                    design.isSphere[design.numElems] = false;
                                    design.v1[design.numElems] = vert;
                                    design.v2[design.numElems] = selVert;
                                    design.numElems += 1;
                                    selVert = vert;
                                }
                            }
                            else 
                            {
                                selVert = vert;
                                dragging = true;
                            }
                        }
                    }
                }
                else if (button == eMouseRight)
                {
                    if (selVert != -1 && design.numElems < MAXELEMS)
                    {
                        design.isSphere[design.numElems] = true;
                        design.v1[design.numElems] = selVert;
                        design.v2[design.numElems] = -1;
                        design.radius[design.numElems] = 8;
                        selElem = design.numElems;
                        design.numElems += 1;
                    }
                }
            }
        }

    }

    #region Globally Exposed Items

    public partial class GlobalBase
    {

    }

    #endregion

}
