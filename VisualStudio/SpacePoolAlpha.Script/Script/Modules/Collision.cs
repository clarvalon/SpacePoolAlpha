// Module_Collision - Type "override" followed by space to see list of C# methods to implement
using static SpacePoolAlpha.GlobalBase;
using System.Diagnostics;
using static SpacePoolAlpha.Module_Collision;
using static SpacePoolAlpha.CollisionStaticRef;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MessagePack;
using Clarvalon.XAGE.Global;

namespace SpacePoolAlpha
{
    public partial class Module_Collision
    {
        // Fields
        public result res = new result();

        // Methods
        public bool test_ray_circle_2d(float px, float py, float vx, float vy, float radius)
        {
            float rayLength = Maths.Sqrt(vx*vx + vy*vy);
            if (rayLength > 0.0000001f)
            {
                float rx = vx/rayLength;
                float ry = vy/rayLength;
                float radiusSquared = radius*radius;
                float t = -(rx*px + ry*py);
                float tx = px + t*rx;
                float ty = py + t*ry;
                float offSquared = tx*tx + ty*ty;
                if (offSquared < radiusSquared)
                {
                    t = t - Maths.Sqrt(radiusSquared - offSquared);
                    if (t > -0.01 && t < rayLength)
                    {
                        res.px = px + t*rx;
                        res.py = py + t*ry;
                        res.nx = res.px/radius;
                        res.ny = res.py/radius;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool test_ray_capsule_2d(float px, float py, float vx, float vy, float length, float radius)
        {
            result tempres = new result();
            if (vx != 0.0f)
            {
                float t = 1.0f;
                tempres.px = radius;
                tempres.nx = 1.0f;
                tempres.ny = 0.0f;
                if ((vx < 0.0 && px > 0.0f) || (vx > 0.0 && px < 0.0f))
                {
                    if (px < 0.0f)
                    {
                        tempres.px = -radius;
                        tempres.nx = -1.0f;
                    }
                    float rayLength = Maths.Sqrt(vx*vx + vy*vy);
                    t = rayLength*(tempres.px - px)/vx;
                    if (t < rayLength)
                    {
                        if (t < 0.0f)
                        {
                            t = 0.0f;
                        }
                        tempres.py = py + (t/rayLength)*vy;
                        if (tempres.py*tempres.py < length*length)
                        {
                            res.px = tempres.px;
                            res.py = tempres.py;
                            res.nx = tempres.nx;
                            res.ny = tempres.ny;
                            return true;
                        }
                    }
                    else 
                    {
                        return false;
                    }
                }
            }
            if (test_ray_circle_2d(px, py - length, vx, vy, radius))
            {
                res.py = res.py + length;
                return true;
            }
            if (test_ray_circle_2d(px, py + length, vx, vy, radius))
            {
                res.py = res.py - length;
                return true;
            }
            return false;
        }

        public bool test_against_horizontal_bank(float px, float py, float vx, float vy, float cx, float cy, float length, float radius)
        {
            if (test_ray_capsule_2d(-(py-cy), px-cx, -vy, vx, length, radius))
            {
                float x = res.px;
                float y = res.py;
                res.px = cx + y;
                res.py = cy - x;
                x = res.nx;
                y = res.ny;
                res.nx = y;
                res.ny = -x;
                return true;
            }
            return false;
        }

        public bool test_against_vertical_bank(float px, float py, float vx, float vy, float cx, float cy, float length, float radius)
        {
            if (test_ray_capsule_2d(px-cx, py-cy, vx, vy, length, radius))
            {
                res.px += cx;
                res.py += cy;
                return true;
            }
            return false;
        }

    }

    #region Globally Exposed Items

    public partial class GlobalBase
    {
        // Expose Collision methods so they can be used without instance prefix
        public static bool test_ray_circle_2d(float px, float py, float vx, float vy, float radius)
        {
            return Collision.test_ray_circle_2d(px, py, vx, vy, radius);
        }

        public static bool test_ray_capsule_2d(float px, float py, float vx, float vy, float length, float radius)
        {
            return Collision.test_ray_capsule_2d(px, py, vx, vy, length, radius);
        }

        public static bool test_against_horizontal_bank(float px, float py, float vx, float vy, float cx, float cy, float length, float radius)
        {
            return Collision.test_against_horizontal_bank(px, py, vx, vy, cx, cy, length, radius);
        }

        public static bool test_against_vertical_bank(float px, float py, float vx, float vy, float cx, float cy, float length, float radius)
        {
            return Collision.test_against_vertical_bank(px, py, vx, vy, cx, cy, length, radius);
        }

        // Expose Collision variables so they can be used without instance prefix
        public static result res { get { return Collision.res; } set { Collision.res = value; } }

    }

    #endregion

    #region result (AGS struct from .ash converted to class)

    [MessagePackObject(keyAsPropertyName:true)]
    public class result
    {
        // Fields
        public float px;
        public float py;
        public float nx;
        public float ny;

    }

    #endregion

    #region Static class for referencing parent class without prefixing with instance (AGS struct workaround)

    public static class CollisionStaticRef
    {
    }

    #endregion
    
}
