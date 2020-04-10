// Module_VectorLetters - Type "override" followed by space to see list of C# methods to implement
using static SpacePoolAlpha.GlobalBase;
using System.Diagnostics;
using static SpacePoolAlpha.Module_VectorLetters;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MessagePack;
using Clarvalon.XAGE.Global;

namespace SpacePoolAlpha
{
    public partial class Module_VectorLetters
    {
        // Fields
        public String[] chars = new String[128];
        public bool setup;

        // Methods
        public void Setup()
        {
            setup = true;
            chars['0'] = "1110111";
            chars['1'] = "0010010";
            chars['2'] = "1011101";
            chars['3'] = "1011011";
            chars['4'] = "0111010";
            chars['5'] = "1101011";
            chars['6'] = "1101111";
            chars['7'] = "1011010";
            chars['8'] = "1111111";
            chars['9'] = "1111010";
            chars['A'] = "1111110";
            chars['B'] = "1111111";
            chars['C'] = "1100101";
            chars['D'] = "0100111";
            chars['E'] = "1101101";
            chars['F'] = "1101100";
            chars['G'] = "1100111";
            chars['H'] = "0111110";
            chars['I'] = "1000001";
            chars['J'] = "0010011";
            chars['K'] = "0100100";
            chars['L'] = "0100101";
            chars['M'] = "0110110";
            chars['N'] = "0110110";
            chars['O'] = "1110111";
            chars['P'] = "1111100";
            chars['Q'] = "1110111";
            chars['R'] = "1111100";
            chars['S'] = "1101011";
            chars['T'] = "1000000";
            chars['U'] = "0110111";
            chars['V'] = "0110010";
            chars['W'] = "0110110";
            chars['X'] = "0000000";
            chars['Y'] = "0000000";
            chars['Z'] = "1000001";
            chars[':'] = "0000000";
            chars['!'] = "0000000";
            chars[','] = "0000000";
            chars['.'] = "0000000";
            chars['@'] = "0000000";
            chars['?'] = "1011000";
        }

        public void ExtensionMethod_DrawVectorText(DrawingSurface  thisItem, float fx, float fy, float size, String text, Justify justify)
        {
            if (!setup)
                Setup();
            int i = 0;
            int xsize = FloatToInt(size);
            int ysize = 2;
            int ssize = 2;
            if (xsize == 3)
            {
                xsize = 3;
                ssize = 2;
                ysize = 2;
            }
            if (xsize == 4 || xsize == 5)
            {
                ysize = 3;
                ssize = 2;
            }
            if (xsize == 6)
            {
                ysize = 4;
                ssize = 3;
            }
            if (xsize == 7)
            {
                ysize = 5;
                ssize = 3;
            }
            float fxsize = IntToFloat(xsize);
            float fysize = IntToFloat(ysize);
            float fssize = IntToFloat(ssize);
            int textLen = text.Length;
            int x = FloatToInt(fx);
            int w = (xsize*textLen + ssize*(textLen - 1));
            if (justify == eJustify_Right)
            {
                x = x - w;
            }
            else if (justify == eJustify_Centre)
            {
                x = x - w/2;
            }
            fx = IntToFloat(x);
            int startX = x;
            int y = FloatToInt(fy);
            fy = IntToFloat(y);
            while (i < text.Length)
            {
                int c = text.Chars()[i];
                if (c >= 'a' && c <= 'z')
                    c = c - ('a' - 'A');
                String map = chars[c];
                if (!String.IsNullOrEmpty(map))
                {
                    if (map.Chars()[0] == '1')
                        thisItem.DrawLine(x, y - ysize, x + xsize, y - ysize);
                    if (map.Chars()[1] == '1')
                        thisItem.DrawLine(x, y - ysize, x, y);
                    if (map.Chars()[2] == '1')
                        thisItem.DrawLine(x + xsize, y - ysize, x + xsize, y);
                    if (map.Chars()[3] == '1')
                        thisItem.DrawLine(x, y, x + xsize, y);
                    if (map.Chars()[4] == '1')
                        thisItem.DrawLine(x, y, x, y + ysize);
                    if (map.Chars()[5] == '1')
                        thisItem.DrawLine(x + xsize, y, x + xsize, y + ysize);
                    if (map.Chars()[6] == '1')
                        thisItem.DrawLine(x, y + ysize, x + xsize, y + ysize);
                    if (c == 'D')
                    {
                        thisItem.DrawAntialiasedLine(fx, fy - fysize, fx + fxsize, fy);
                    }
                    else if (c == 'I')
                    {
                        thisItem.DrawLine(x + xsize/2, y - ysize, x + xsize/2, y + ysize);
                    }
                    else if (c == 'K')
                    {
                        thisItem.DrawAntialiasedLine(fx, fy, fx + fxsize, fy - fysize);
                        thisItem.DrawAntialiasedLine(fx, fy, fx + fxsize, fy + fysize);
                    }
                    else if (c == 'M')
                    {
                        thisItem.DrawAntialiasedLine(fx, fy - fysize, fx + 0.5f *fxsize, fy);
                        thisItem.DrawAntialiasedLine(fx + 0.5f *fxsize, fy, fx + fxsize, fy - fysize);
                    }
                    else if (c == 'N')
                    {
                        thisItem.DrawAntialiasedLine(fx, fy - fysize, fx + fxsize, fy + fysize);
                    }
                    else if (c == 'Q' || c == 'R' || c == 'V')
                    {
                        thisItem.DrawAntialiasedLine(fx, fy, fx + fxsize, fy + fysize);
                    }
                    else if (c == 'T')
                    {
                        thisItem.DrawLine(x + xsize/2, y - ysize, x + xsize/2, y + ysize);
                    }
                    else if (c == 'W')
                    {
                        thisItem.DrawAntialiasedLine(fx, fy + fysize, fx + 0.5f *fxsize, fy);
                        thisItem.DrawAntialiasedLine(fx + 0.5f *fxsize, fy, fx + fxsize, fy + fysize);
                    }
                    else if (c == 'X')
                    {
                        thisItem.DrawAntialiasedLine(fx, fy - fysize, fx + fxsize, fy + fysize);
                        thisItem.DrawAntialiasedLine(fx, fy + fysize, fx + fxsize, fy - fysize);
                    }
                    else if (c == 'Y')
                    {
                        thisItem.DrawAntialiasedLine(fx, fy - fysize, fx + 0.5f *fxsize, fy);
                        thisItem.DrawAntialiasedLine(fx + 0.5f *fxsize, fy, fx + fxsize, fy - fysize);
                        thisItem.DrawLine(x + xsize/2, y, x + xsize/2, y + ysize);
                    }
                    else if (c == 'Z')
                    {
                        thisItem.DrawAntialiasedLine(fx, fy + fysize, fx + fxsize, fy - fysize);
                    }
                    else if (c == ':')
                    {
                        thisItem.DrawLine(x + xsize/2, y - ysize/2, x + xsize/2, y - ysize/2);
                        thisItem.DrawLine(x + xsize/2, y + ysize/2, x + xsize/2, y + ysize/2);
                    }
                    else if (c == '.' || c == '?')
                    {
                        thisItem.DrawLine(x + xsize/2, y + ysize, x + xsize/2, y + ysize);
                    }
                    else if (c == '!')
                    {
                        thisItem.DrawLine(x + xsize/2, y - ysize, x + xsize/2, y);
                        thisItem.DrawLine(x + xsize/2, y + ysize, x + xsize/2, y + ysize);
                    }
                    else if (c == ',')
                    {
                        thisItem.DrawAntialiasedLine(fx + 0.5f *fxsize, fy + 0.5f *fysize, fx, fy + fysize);
                    }
                    else if (c == '@')
                    {
                        thisItem.DrawAntialiasedLine(fx + 0.5f *fxsize, fy - 0.5f *fysize, fx, fy);
                    }
                }
                i += 1;
                x += xsize + ssize;
                fx += fxsize + fssize;
            }
        }

        public void ExtensionMethod_DrawAntialiasedVectorText(DrawingSurface  thisItem, float x, float y, float size, String text)
        {
            if (!setup)
                Setup();
            int i = 0;
            float xsize = size;
            float ysize = 0.7f *size;
            x -= 0.75f *xsize*IntToFloat(text.Length);
            var textChars = text.Chars(); // DAN
            while (i < text.Length)
            {
                int c = textChars[i];
                String map = chars[c];
                if (!String.IsNullOrEmpty(map))
                {
                    float lap = 0.5f;
                    if (map.Chars()[0] == '1')
                        thisItem.DrawAntialiasedLine(x - lap, y - ysize, x + xsize + lap, y - ysize);
                    if (map.Chars()[1] == '1')
                        thisItem.DrawAntialiasedLine(x, y - ysize - lap, x, y + lap);
                    if (map.Chars()[2] == '1')
                        thisItem.DrawAntialiasedLine(x + xsize, y - ysize - lap, x + xsize, y + lap);
                    if (map.Chars()[3] == '1')
                        thisItem.DrawAntialiasedLine(x - lap, y, x + xsize + lap, y);
                    if (map.Chars()[4] == '1')
                        thisItem.DrawAntialiasedLine(x, y - lap, x, y + ysize + lap);
                    if (map.Chars()[5] == '1')
                        thisItem.DrawAntialiasedLine(x + xsize, y - lap, x + xsize, y + ysize + lap);
                    if (map.Chars()[6] == '1')
                        thisItem.DrawAntialiasedLine(x - lap, y + ysize, x + xsize + lap, y + ysize);
                    if (c == 'D')
                    {
                        thisItem.DrawAntialiasedLine(x, y - ysize, x + xsize, y);
                    }
                    else if (c == 'I')
                    {
                        thisItem.DrawAntialiasedLine(x + 0.5f *xsize, y - ysize, x + 0.5f *xsize, y + ysize);
                    }
                    else if (c == 'K')
                    {
                        thisItem.DrawAntialiasedLine(x, y, x + xsize, y - ysize);
                        thisItem.DrawAntialiasedLine(x, y, x + xsize, y + ysize);
                    }
                    else if (c == 'M')
                    {
                        thisItem.DrawAntialiasedLine(x, y - ysize, x + 0.5f *xsize, y);
                        thisItem.DrawAntialiasedLine(x + 0.5f *xsize, y, x + xsize, y - ysize);
                    }
                    else if (c == 'N')
                    {
                        thisItem.DrawAntialiasedLine(x, y - ysize, x + xsize, y + ysize);
                    }
                    else if (c == 'Q' || c == 'R' || c == 'V')
                    {
                        thisItem.DrawAntialiasedLine(x, y, x + xsize, y + ysize);
                    }
                    else if (c == 'T')
                    {
                        thisItem.DrawAntialiasedLine(x + 0.5f *xsize, y - ysize, x + 0.5f *xsize, y + ysize);
                    }
                    else if (c == 'W')
                    {
                        thisItem.DrawAntialiasedLine(x, y + ysize, x + 0.5f *xsize, y);
                        thisItem.DrawAntialiasedLine(x + 0.5f *xsize, y, x + xsize, y + ysize);
                    }
                    else if (c == 'X')
                    {
                        thisItem.DrawAntialiasedLine(x, y - ysize, x + xsize, y + ysize);
                        thisItem.DrawAntialiasedLine(x, y + ysize, x + xsize, y - ysize);
                    }
                    else if (c == 'Y')
                    {
                        thisItem.DrawAntialiasedLine(x, y - ysize, x + 0.5f *xsize, y);
                        thisItem.DrawAntialiasedLine(x + 0.5f *xsize, y, x + xsize, y - ysize);
                        thisItem.DrawAntialiasedLine(x + 0.5f *xsize, y, x + 0.5f *xsize, y + ysize);
                    }
                    else if (c == 'Z')
                    {
                        thisItem.DrawAntialiasedLine(x, y + ysize, x + xsize, y - ysize);
                    }
                }
                i += 1;
                x += 1.5f *xsize;
            }
        }

    }

    #region Globally Exposed Items

    public partial class GlobalBase
    {
        // Expose Enums and instances of each
        public enum Justify
        {
            eJustify_Left = 0, 
            eJustify_Centre = 1, 
            eJustify_Right = 2
        }
        public const Justify eJustify_Left = Justify.eJustify_Left;
        public const Justify eJustify_Centre = Justify.eJustify_Centre;
        public const Justify eJustify_Right = Justify.eJustify_Right;


    }

    #endregion

    #region Extension Methods Wrapper (AGS workaround)

    public static partial class ExtensionMethods
    {
        public static void DrawVectorText(this DrawingSurface  thisItem, float x, float y, float size, String text, Justify justify = eJustify_Centre)
        {
            GlobalBase.VectorLetters.ExtensionMethod_DrawVectorText(thisItem, x, y, size, text, justify);
        }

        public static void DrawAntialiasedVectorText(this DrawingSurface  thisItem, float x, float y, float size, String text)
        {
            GlobalBase.VectorLetters.ExtensionMethod_DrawAntialiasedVectorText(thisItem, x, y, size, text);
        }

    }

    #endregion

}
