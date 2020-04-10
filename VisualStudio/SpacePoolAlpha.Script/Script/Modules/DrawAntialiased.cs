// Module_DrawAntialiased - Type "override" followed by space to see list of C# methods to implement
using static SpacePoolAlpha.GlobalBase;
using System.Diagnostics;
using static SpacePoolAlpha.Module_DrawAntialiased;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MessagePack;
using Clarvalon.XAGE.Global;

namespace SpacePoolAlpha
{
    public partial class Module_DrawAntialiased
    {
        // Fields
        public DrawingSurface gSurfaceToDrawOn;
        public DynamicSprite gPixel;
        public int gPixelCol;
        public float gOverallOpacity;

        // Methods
        public int ipart(float x)
        {
            return FloatToInt(x, eRoundDown);
        }

        public int round(float x)
        {
            return FloatToInt(x, eRoundNearest);
        }

        public float fpart(float x)
        {
            return x - IntToFloat(ipart(x));
        }

        public float rfpart(float x)
        {
            return 1.0f - fpart(x);
        }

        public void plot(bool steep, int x, int y, float c)
        {
            int t = 100 - FloatToInt(100.0f *c*gOverallOpacity);
            if (t >= 0 && t < 100)
            {
                if (steep)
                {
                    //gSurfaceToDrawOn.DrawingColor = gPixelCol;
                    //gSurfaceToDrawOn.DrawPixel(y, x);
                    gSurfaceToDrawOn.DrawImage(y, x, gPixel.Graphic, t);
                }
                else 
                {
                    //gSurfaceToDrawOn.DrawingColor = gPixelCol;
                    //gSurfaceToDrawOn.DrawPixel(x, y);
                    gSurfaceToDrawOn.DrawImage(x, y, gPixel.Graphic, t);
                }
            }
        }

        public void plotline(int xl, int xr, int y)
        {
            int t = 100 - FloatToInt(100.0f *gOverallOpacity);
            gSurfaceToDrawOn.DrawImage(xl, y, gPixel.Graphic, t, xr - xl + 1, 1);
        }

        public void ExtensionMethod_DrawAntialiasedLine(DrawingSurface  thisItem, float x1, float y1, float x2, float y2, int transparency)
        {
            gSurfaceToDrawOn = thisItem;
            gOverallOpacity = 1.0f - IntToFloat(transparency)/100.0f;

            // Optimisation 1:  Cache gPixel
            if (gPixel == null)
            {
                gPixel = DynamicSprite.Create(1, 1);
            }

            // Optimisation 2:  Only change pixel colour if needed
            if (gPixelCol != thisItem.DrawingColor)
            {
                DrawingSurface dss = gPixel.GetDrawingSurface();
                dss.Clear(thisItem.DrawingColor);
                dss.Release();
            }
            gPixelCol = thisItem.DrawingColor;

            float dx = x2 - x1;
            float dy = y2 - y1;
            bool steep = false;
            if (dx*dx < dy*dy)
            {
                float s = x1;
                x1 = y1;
                y1 = s;
                s = x2;
                x2 = y2;
                y2 = s;
                steep = true;
            }
            if (x2 < x1)
            {
                float s = x1;
                x1 = x2;
                x2 = s;
                s = y1;
                y1 = y2;
                y2 = s;
            }
            dx = x2 - x1;
            dy = y2 - y1;
            float gradient = dy / dx;
            int xend = round(x1);
            float yend = y1 + gradient * (IntToFloat(xend) - x1);
            float xgap = rfpart(x1 + 0.5f);
            int xpxl1 = xend;
            int ypxl1 = ipart(yend);
            plot(steep, xpxl1, ypxl1, rfpart(yend) * xgap);
            plot(steep, xpxl1, ypxl1 + 1, fpart(yend) * xgap);
            float intery = yend + gradient;
            xend = round(x2);
            yend = y2 + gradient * (IntToFloat(xend) - x2);
            xgap = fpart(x2 + 0.5f);
            int xpxl2 = xend;
            int ypxl2 = ipart(yend);
            plot(steep, xpxl2, ypxl2, rfpart(yend) * xgap);
            plot(steep, xpxl2, ypxl2 + 1, fpart(yend) * xgap);
            int x = xpxl1 + 1;
            while (x < xpxl2)
            {
                int ipart_intery = FloatToInt(intery, eRoundDown);
                float fpart_intery = intery - IntToFloat(ipart_intery);
                float rfpart_intery = 1.0f - fpart_intery;
                plot(steep, x, ipart_intery, rfpart_intery);
                plot(steep, x, ipart_intery + 1, fpart_intery);
                intery = intery + gradient;
                x += 1;
            }

            //gPixel.Delete();
        }

        public void drawAntialiasedCircle(DrawingSurface surf, float centre_x, float centre_y, float radius, int transparency, bool filled)
        {
            gSurfaceToDrawOn = surf;
            gOverallOpacity = 1.0f - IntToFloat(transparency)/100.0f;

            // Optimisation 1:  Cache gPixel
            if (gPixel == null)
            {
                gPixel = DynamicSprite.Create(1, 1);
            }
            
            // Optimisation 2:  Only change pixel colour if needed
            if (gPixelCol != surf.DrawingColor)
            {
                DrawingSurface dss = gPixel.GetDrawingSurface();
                dss.Clear(surf.DrawingColor);
                dss.Release();
            }
            gPixelCol = surf.DrawingColor;

            int ystage = 0;
            int y = FloatToInt(centre_y - radius) - 1;
            if (y < 0)
                y = 0;
            int ymax = FloatToInt(centre_y + radius) + 2;
            if (ymax > Room.Height)
                ymax = Room.Height;
            while (y < ymax)
            {
                float y_d = IntToFloat(y) - centre_y;
                float y_d_2 = y_d*y_d;
                float x_l = Maths.Sqrt((radius + 2.0f)*(radius + 2.0f) - y_d_2);
                int x = FloatToInt(centre_x - x_l);
                while (x < FloatToInt(centre_x + x_l) + 1)
                {
                    float x_d = IntToFloat(x) - centre_x;
                    float d = Maths.Sqrt(x_d*x_d + y_d_2);
                    if (filled)
                    {
                        if (d < radius + 1.0f)
                        {
                            if (d < radius - 1.0f)
                            {
                                int c = FloatToInt(centre_x);
                                if (x < c)
                                {
                                    int rx = 2*c - x + 1;
                                    plotline(x, rx, y);
                                    x = rx;
                                }
                                else 
                                {
                                    plot(false, x, y, 1.0f);
                                }
                            }
                            else 
                            {
                                float c = 0.5f *(radius + 1.0f - d);
                                plot(false, x, y, c);
                            }
                        }
                    }
                    else 
                    {
                        if (d < radius - 1.0f)
                        {
                            int c = FloatToInt(centre_x);
                            if (x < c)
                            {
                                x = 2*c - x;
                            }
                        }
                        else if (d > radius - 1.0 && d < radius + 1.0f)
                        {
                            float c = radius - d;
                            if (c < 0.0f)
                                c = -c;
                            c = 1.0f - c;
                            plot(false, x, y, c);
                        }
                    }
                    x += 1;
                }
                y += 1;
            }

            //gPixel.Delete();
        }

        public void ExtensionMethod_DrawAntialiasedCircle(DrawingSurface  thisItem, float centre_x, float centre_y, float radius, int transparency)
        {
            drawAntialiasedCircle(thisItem, centre_x, centre_y, radius, transparency, false);
        }

        public void ExtensionMethod_DrawAntialiasedFilledCircle(DrawingSurface  thisItem, float centre_x, float centre_y, float radius, int transparency)
        {
            drawAntialiasedCircle(thisItem, centre_x, centre_y, radius, transparency, true);
        }

        public void ExtensionMethod_DrawBox(DrawingSurface  thisItem, int x, int y, int w, int h)
        {
            thisItem.DrawLine(x, y, x+w, y);
            thisItem.DrawLine(x+w, y, x+w, y+h);
            thisItem.DrawLine(x+w, y+h, x, y+h);
            thisItem.DrawLine(x, y+h, x, y);
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
        public static void DrawAntialiasedLine(this DrawingSurface  thisItem, float x1, float y1, float x2, float y2, int transparency = 0)
        {
            GlobalBase.DrawAntialiased.ExtensionMethod_DrawAntialiasedLine(thisItem, x1, y1, x2, y2, transparency);
        }

        public static void DrawAntialiasedCircle(this DrawingSurface  thisItem, float centre_x, float centre_y, float radius, int transparency = 0)
        {
            GlobalBase.DrawAntialiased.ExtensionMethod_DrawAntialiasedCircle(thisItem, centre_x, centre_y, radius, transparency);
        }

        public static void DrawAntialiasedFilledCircle(this DrawingSurface  thisItem, float centre_x, float centre_y, float radius, int transparency = 0)
        {
            GlobalBase.DrawAntialiased.ExtensionMethod_DrawAntialiasedFilledCircle(thisItem, centre_x, centre_y, radius, transparency);
        }

        public static void DrawBox(this DrawingSurface  thisItem, int x, int y, int w, int h)
        {
            GlobalBase.DrawAntialiased.ExtensionMethod_DrawBox(thisItem, x, y, w, h);
        }

    }

    #endregion

}
