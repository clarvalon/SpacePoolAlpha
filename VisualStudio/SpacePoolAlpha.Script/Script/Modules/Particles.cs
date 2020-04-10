// Module_Particles - Type "override" followed by space to see list of C# methods to implement
using static SpacePoolAlpha.GlobalBase;
using System.Diagnostics;
using static SpacePoolAlpha.Module_Particles;
using static SpacePoolAlpha.ParticlesStaticRef;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MessagePack;
using Clarvalon.XAGE.Global;

namespace SpacePoolAlpha
{
    public partial class Module_Particles
    {
        // Fields
        public Particle[] particles = CreateAndInstantiateArray<Particle>(MAX_PARTICLES);
        public int freeListHead;
        public int usedListHead = -1;

        // Methods
        public void remove_from_list(int i)
        {
            int p = particles[i].prev;
            int n = particles[i].next;
            if (p != -1)
            {
                particles[p].next = n;
            }
            if (n != -1)
            {
                particles[n].prev = p;
            }
        }

        public void add_to_freelist(int i)
        {
            remove_from_list(i);
            if (usedListHead == i)
            {
                usedListHead = particles[i].next;
            }
            if (freeListHead != -1)
            {
                particles[freeListHead].prev = i;
            }
            particles[i].prev = -1;
            particles[i].next = freeListHead;
            freeListHead = i;
        }

        public void add_to_usedlist(int i)
        {
            remove_from_list(i);
            if (freeListHead == i)
            {
                freeListHead = particles[i].next;
            }
            if (usedListHead != -1)
            {
                particles[usedListHead].prev = i;
            }
            particles[i].prev = -1;
            particles[i].next = usedListHead;
            usedListHead = i;
        }

        public void setup_particles()
        {
            int i = 0;
            while (i < MAX_PARTICLES)
            {
                particles[i].prev = i-1;
                particles[i].next = i+1;
                i += 1;
            }
            particles[MAX_PARTICLES-1].next = -1;
            freeListHead = 0;
            usedListHead = -1;
        }

        public void add_particle(float x, float y, float vx, float vy, float len, float angle, int colour)
        {
            if (freeListHead != -1)
            {
                int i = freeListHead;
                add_to_usedlist(i);
                float dx = len*Maths.Cos(Maths.DegreesToRadians(angle));
                float dy = len*Maths.Sin(Maths.DegreesToRadians(angle));
                particles[i].x = x - 0.5f *dx;
                particles[i].y = y - 0.5f *dy;
                particles[i].vx = vx;
                particles[i].vy = vy;
                particles[i].dx = dx;
                particles[i].dy = dy;
                particles[i].colour = colour;
            }
        }

        public void update_particles(DrawingSurface surf)
        {
            int i = usedListHead;
            while (i != -1)
            {
                particles[i].x += particles[i].vx;
                particles[i].y += particles[i].vy;
                particles[i].vx = 0.95f *particles[i].vx;
                particles[i].vy = 0.95f *particles[i].vy;
                int next = particles[i].next;
                if (particles[i].vx*particles[i].vx + particles[i].vy*particles[i].vy < 0.01f)
                {
                    add_to_freelist(i);
                }
                else 
                {
                    surf.DrawingColor = particles[i].colour;
                    surf.DrawAntialiasedLine(particles[i].x, particles[i].y, particles[i].x + particles[i].dx, particles[i].y + particles[i].dy);
                }
                i = next;
            }
        }

    }

    #region Globally Exposed Items

    public partial class GlobalBase
    {
        // Expose AGS singular #defines as C# constants (or static getters)
        public const int MAX_PARTICLES = 200;

        // Expose Particles methods so they can be used without instance prefix
        public static void setup_particles()
        {
            Particles.setup_particles();
        }

        public static void add_particle(float x, float y, float vx, float vy, float len, float angle, int colour)
        {
            Particles.add_particle(x, y, vx, vy, len, angle, colour);
        }

        public static void update_particles(DrawingSurface surf)
        {
            Particles.update_particles(surf);
        }


    }

    #endregion

    #region Particle (AGS struct from .asc converted to class)

    [MessagePackObject(keyAsPropertyName:true)]
    public class Particle
    {
        // Fields
        public float x;
        public float y;
        public float vx;
        public float vy;
        public float dx;
        public float dy;
        public int colour;
        public int prev;
        public int next;

    }

    #endregion

    #region Static class for referencing parent class without prefixing with instance (AGS struct workaround)

    public static class ParticlesStaticRef
    {
        // Static Fields
        public static Particle[] particles { get { return GlobalBase.Particles.particles; } set { GlobalBase.Particles.particles = value; } }
        public static int freeListHead { get { return GlobalBase.Particles.freeListHead; } set { GlobalBase.Particles.freeListHead = value; } }
        public static int usedListHead { get { return GlobalBase.Particles.usedListHead; } set { GlobalBase.Particles.usedListHead = value; } }

        // Static Methods
        public static void remove_from_list(int i)
        {
            GlobalBase.Particles.remove_from_list(i);
        }

        public static void add_to_freelist(int i)
        {
            GlobalBase.Particles.add_to_freelist(i);
        }

        public static void add_to_usedlist(int i)
        {
            GlobalBase.Particles.add_to_usedlist(i);
        }

        public static void setup_particles()
        {
            GlobalBase.Particles.setup_particles();
        }

    }

    #endregion
    
}
