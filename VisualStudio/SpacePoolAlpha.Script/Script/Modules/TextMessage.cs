// Module_TextMessage - Type "override" followed by space to see list of C# methods to implement
using static SpacePoolAlpha.GlobalBase;
using System.Diagnostics;
using static SpacePoolAlpha.Module_TextMessage;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MessagePack;
using Clarvalon.XAGE.Global;

namespace SpacePoolAlpha
{
    public partial class Module_TextMessage
    {
        // Fields

        // Methods
        public void ExtensionMethod_WriteTextMessage(DrawingSurface  thisItem, float x, float y, float size, String lines)
        {
            int sc = 0;
            int ec = sc;
            float yy = y;
            float width = size;
            while (ec < lines.Length)
            {
                if (lines.Chars()[ec] == ']')
                {
                    if (sc != ec)
                    {
                        String line = lines.Substring(sc, ec-sc);
                        float thisWidth = 1.5f *size*IntToFloat(line.Length + 2);
                        thisItem.DrawAntialiasedVectorText(IntToFloat(FloatToInt(x)), IntToFloat(FloatToInt(yy)), size, line);
                        if (thisWidth > width)
                        {
                            width = thisWidth;
                        }
                    }
                    yy += 2.0f *size;
                    sc = ec+1;
                    ec = sc;
                }
                else 
                {
                    ec += 1;
                }
            }
            yy -= 2.0f *size;
            thisItem.DrawAntialiasedLine(x - 0.5f *width, y - 2.0f *size, x + 0.5f *width, y - 2.0f *size);
            thisItem.DrawAntialiasedLine(x + 0.5f *width, y - 2.0f *size, x + 0.5f *width, yy + 2.0f *size);
            thisItem.DrawAntialiasedLine(x + 0.5f *width, yy + 2.0f *size, x - 0.5f *width, yy + 2.0f *size);
            thisItem.DrawAntialiasedLine(x - 0.5f *width, yy + 2.0f *size, x - 0.5f *width, y - 2.0f *size);
        }

        public void ExtensionMethod_WriteAliasedTextMessage(DrawingSurface  thisItem, float x, float y, float size, String lines, MessageStyle style, Justify justify)
        {
            int sc = 0;
            int ec = sc;
            float yy = y;
            float width = size;
            while (ec < lines.Length)
            {
                if (lines.Chars()[ec] == ']')
                {
                    if (sc != ec)
                    {
                        String line = lines.Substring(sc, ec-sc);
                        float thisWidth = 1.5f *size*IntToFloat(line.Length + 2);
                        thisItem.DrawVectorText(x, yy, size, line, justify);
                        if (thisWidth > width)
                        {
                            width = thisWidth;
                        }
                    }
                    yy += 2.0f *size;
                    sc = ec+1;
                    ec = sc;
                }
                else 
                {
                    ec += 1;
                }
            }
            if (style == eMessageStyle_Boxed)
            {
                yy -= 2.0f *size;
                thisItem.DrawLine(FloatToInt(x - 0.5f *width), FloatToInt(y - 2.0f *size), FloatToInt(x + 0.5f *width), FloatToInt(y - 2.0f *size));
                thisItem.DrawLine(FloatToInt(x + 0.5f *width), FloatToInt(y - 2.0f *size), FloatToInt(x + 0.5f *width), FloatToInt(yy + 2.0f *size));
                thisItem.DrawLine(FloatToInt(x + 0.5f *width), FloatToInt(yy + 2.0f *size), FloatToInt(x - 0.5f *width), FloatToInt(yy + 2.0f *size));
                thisItem.DrawLine(FloatToInt(x - 0.5f *width), FloatToInt(yy + 2.0f *size), FloatToInt(x - 0.5f *width), FloatToInt(y - 2.0f *size));
            }
        }

        public void ExtensionMethod_WriteTextMessageWrap(DrawingSurface  thisItem, float x, float y, float w, float size, String text, int amt)
        {
            int sc = 0;
            int ec = sc + 1;
            int ls = 0;
            float yy = y;
            int len = amt;
            if (len > text.Length)
                len = text.Length;
            while (ec < len)
            {
                if (text.Chars()[ec] == ' ')
                {
                    ls = ec;
                }
                else 
                {
                    String line = text.Substring(sc, ec - sc);
                    float thisWidth = size*IntToFloat(ec - sc) + 0.5f *size*IntToFloat((ec - sc) - 1);
                    if (thisWidth >= w)
                    {
                        String printLine = text.Substring(sc, ls - sc);
                        thisItem.DrawVectorText(x, yy, size, printLine, eJustify_Left);
                        yy += 2.0f *size;
                        sc = ls + 1;
                        ec = sc;
                    }
                }
                ec += 1;
            }
            if (ec > sc)
            {
                String printLine = text.Substring(sc, ec - sc);
                thisItem.DrawVectorText(x, yy, size, printLine, eJustify_Left);
            }
        }

    }

    #region Globally Exposed Items

    public partial class GlobalBase
    {
        // Expose Enums and instances of each
        public enum MessageStyle
        {
            eMessageStyle_Boxed = 0, 
            eMessageStyle_Unboxed = 1
        }
        public const MessageStyle eMessageStyle_Boxed = MessageStyle.eMessageStyle_Boxed;
        public const MessageStyle eMessageStyle_Unboxed = MessageStyle.eMessageStyle_Unboxed;


    }

    #endregion

    #region Extension Methods Wrapper (AGS workaround)

    public static partial class ExtensionMethods
    {
        public static void WriteTextMessage(this DrawingSurface  thisItem, float x, float y, float size, String text)
        {
            GlobalBase.TextMessage.ExtensionMethod_WriteTextMessage(thisItem, x, y, size, text);
        }

        public static void WriteAliasedTextMessage(this DrawingSurface  thisItem, float x, float y, float size, String text, MessageStyle style = eMessageStyle_Boxed, Justify justify = eJustify_Centre)
        {
            GlobalBase.TextMessage.ExtensionMethod_WriteAliasedTextMessage(thisItem, x, y, size, text, style, justify);
        }

        public static void WriteTextMessageWrap(this DrawingSurface  thisItem, float x, float y, float w, float size, String text, int amt)
        {
            GlobalBase.TextMessage.ExtensionMethod_WriteTextMessageWrap(thisItem, x, y, w, size, text, amt);
        }

    }

    #endregion

}
