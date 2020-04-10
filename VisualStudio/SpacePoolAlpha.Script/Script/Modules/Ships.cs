// Module_Ships - Type "override" followed by space to see list of C# methods to implement
using static SpacePoolAlpha.GlobalBase;
using System.Diagnostics;
using static SpacePoolAlpha.Module_Ships;
using static SpacePoolAlpha.ShipsStaticRef;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MessagePack;
using Clarvalon.XAGE.Global;

namespace SpacePoolAlpha
{
    public partial class Module_Ships
    {
        // Fields
        public SShipDesign design = new SShipDesign();
        public SShipImpl impl = new SShipImpl();
        public int ship;
        public String[] pilots = new String[32];
        public String[] bios = new String[32];
        public int[] sprites = new int[32];
        public int[] round_1_ships = new int[3];
        public int[] round_2_ships = new int[3];
        public int[] round_3_ships = new int[3];
        public int[] round_4_ships = new int[3];
        public int[] round_5_ships = new int[3];
        public int[] round_6_ships = new int[3];
        public int[] round_7_ships = new int[3];

        // Methods
        public void ships_setup()
        {
            pilots[0] = "KIM KIRK";
            pilots[1] = "GRACE RIMMER";
            pilots[2] = "RAJ BLAKE";
            pilots[3] = "ORFEA";
            pilots[4] = "DANNI DARE";
            pilots[5] = "FELISHA GORDON";
            pilots[6] = "ETTIE FELSON";
            pilots[7] = "HANNA SOLO";
            pilots[8] = "LUCY WALKER";
            pilots[9] = "MICHAELA COLLINS";
            pilots[10] = "VELMA DEERING";
            pilots[11] = "STACY LONG";
            pilots[12] = "KYRA THRACE";
            pilots[13] = "KALLY RIDE";
            pilots[14] = "SIOUX IVANOVA";
            pilots[15] = "ALANIS CARTER";
            pilots[16] = "DOCTOR WENDY";
            pilots[17] = "MALLORY REYNOLDS";
            pilots[18] = "MIKA MELVILL";
            pilots[19] = "ELENA RIPLEY";
            pilots[20] = "GEORGIE TAYLOR";
            pilots[21] = "LONA STARR";
            pilots[22] = "WILLY DAVIDGE";
            pilots[23] = "KLARA BARADA";
            pilots[24] = "GWEN ROSS";
            pilots[25] = "SPACE COWGIRL";
            pilots[26] = "BILLIE FROOG";
            pilots[27] = "JASMINE NESMITH";
            pilots[28] = "DIANA";
            pilots[29] = "ALEXIS ROGAN";
            pilots[30] = "CHRISTINA BLAIR";
            pilots[31] = "EMPRESS";
            sprites[0] = 1;
            sprites[1] = 2;
            sprites[2] = 11;
            sprites[3] = 0;
            sprites[4] = 4;
            sprites[5] = 6;
            sprites[6] = 0;
            sprites[7] = 7;
            sprites[8] = 0;
            sprites[9] = 3;
            sprites[10] = 5;
            sprites[11] = 0;
            sprites[12] = 10;
            sprites[13] = 20;
            sprites[14] = 0;
            sprites[15] = 19;
            sprites[16] = 17;
            sprites[17] = 15;
            sprites[18] = 21;
            sprites[19] = 9;
            sprites[20] = 0;
            sprites[21] = 13;
            sprites[22] = 16;
            sprites[23] = 18;
            sprites[24] = 22;
            sprites[25] = 0;
            sprites[26] = 8;
            sprites[27] = 0;
            sprites[28] = 0;
            sprites[29] = 0;
            sprites[30] = 0;
            sprites[31] = 12;
            bios[0]  = "Brash and impetuous, yet quick witted, Kim is the youngest captain in Space Fleet history. Her piloting skills are hampered by having to tell someone to press a button to move forward.";
            bios[1]  = "Grace has committed countless selfless acts of bravery and heroism. But that@s easy when you@re a hologram. She is easily distracted by pickled herring.";
            bios[2]  = "Raj learned all she knows about flying a ship when marooned on a planet of supersentient swans. Which explains why she@s not the greatest pilot around.";
            bios[3]  = "BALETED";
            bios[4]  = "Danni is chief pilot of the Interplanetary Air Force. She is a scrupulously honest person, and would rather die than break her word. She is afraid of large green globes.";
            bios[5]  = "Felisha, a championship polo player, entered this tournament after misreading the advertising posters. Fortunately her friend Zarkov has a home made rocket ship.";
            bios[6]  = "BALETED";
            bios[7]  = "A stems smuggler from far, far away. She has a fast ship but is crippled by having a shag carpet as a copilot.";
            bios[8]  = "BALETED";
            bios[9]  = "Michaela vowed revenge after crewmates Fuzz and Headstrong left her to guard the ship while they explored the spa planet of Orgasmus IV. She can@t stop thinking about it, actually.";
            bios[10] = "Once a straight-laced colonel in the Earth Defence Squad with admiralty in her future, Velma tossed it all away when she met a time-travelling loser. Her skills with a joystick have suffered.";
            bios[11] = "Stacy is a real person. Honest. She just likes to dress up in a silver suit and speak in a robotic voice. The red light waving back and forth across her vision doesn@t make life easy either.";
            bios[12] = "An eyelid sprain dashed Kyra@s dreams of being a pro Pentahedron player. Luckily most of Earth@s population was wiped out by hubris, leaving athletes like Kyra diasporadic man@s last defenders.";
            bios[13] = "Dr Kally has a PhD in astrophysics and a robot arm, but that doesn@t make her a good pool player. Her motto is gallant oherwydd eu bod yn credu y gallant.";
            bios[14] = "There aren@t many native Americans in space. And Sioux isn@t helping, since she@s a Russian Jew. When she@s not running a huge space station, she loves hanging out with fellow telepaths.";
            bios[15] = "Alanis is the best pilot on Moonbase Beta. She is also the only pilot on Moonbase Beta. She@s a shoot first, ask questions later sort of Sheila.";
            bios[16] = "Wendy@s origins are almost as mysterious as the ship she pilots, which is smaller on the inside than the outside. She often travels with a young male companion. It@s a sex thing.";
            bios[17] = "After the war, a lot of pilots found gainful employment with the corporations. Mallory chose smuggling instead.";
            bios[18] = "Mika could not be present for this tournament, and has instead left the ship on autopilot.";
            bios[19] = "Elena is a warrant officer for a minor mining corporation. She@s just trying to make enough money to get back home, and maybe kill a few aliens on the way.";
            bios[20] = "Following a period marooned on an alien planet, Georgie eschews uniforms and prefers to wear animal skins. She believes that the human race has a bright future.";
            bios[21] = "Lona often suspects she is nothing more than a parody of a real pilot. She says the lack of a denial in this biography isn@t very reassuring.";
            bios[22] = "Willy has a bad habit of falling in love with her opponents, and then letting them win. Also of crash landing on alien planets.";
            bios[23] = "Klara is an emissary for a more powerful alien creature. Her dream is to one day be able to afford a flying surfboard. In the meantime this saucer will have to do.";
            bios[24] = "Gwen is left handed, but doesn@t know it, so she sometimes confuses left and right. She became a professional pool player when her crew were all killed in a tragic accident involving a Klein bottle.";
            bios[25] = "BALETED";
            bios[26] = "Janitor Billie snuck on board when the mission@s navigator fell ill. She has a pathological fear of beach balls.";
            bios[27] = "BALETED";
            bios[28] = "BALETED";
            bios[29] = "BALETED";
            bios[30] = "BALETED";
            bios[31] = "Still a teenager, Empress is a member of an elite fighting force that protects Earth from attacks from beyond space. Looking good in a cape and bird mask won@t help. Being awesome will.";
            round_1_ships[0] = 0;
            round_1_ships[1] = 4;
            round_1_ships[2] = 1;
            round_2_ships[0] = 26;
            round_2_ships[1] = 2;
            round_2_ships[2] = 9;
            round_3_ships[0] = 5;
            round_3_ships[1] = 13;
            round_3_ships[2] = 10;
            round_4_ships[0] = 24;
            round_4_ships[1] = 18;
            round_4_ships[2] = 19;
            round_5_ships[0] = 16;
            round_5_ships[1] = 21;
            round_5_ships[2] = 23;
            round_6_ships[0] = 17;
            round_6_ships[1] = 31;
            round_6_ships[2] = 15;
            round_7_ships[0] = 22;
            round_7_ships[1] = 12;
            round_7_ships[2] = 7;
        }

        public int ship_get_for_round(int round)
        {
            if (round == 0)
            {
                return round_1_ships[Random(2)];
            }
            else if (round == 1)
            {
                return round_2_ships[Random(2)];
            }
            else if (round == 2)
            {
                return round_3_ships[Random(2)];
            }
            else if (round == 3)
            {
                return round_4_ships[Random(2)];
            }
            else if (round == 4)
            {
                return round_5_ships[Random(2)];
            }
            else if (round == 5)
            {
                return round_6_ships[Random(2)];
            }
            else if (round == 6)
            {
                return round_7_ships[Random(2)];
            }
            return 0;
        }

        public String ship_get_name(int s)
        {
            return pilots[s];
        }

        public String ship_get_current_name()
        {
            return pilots[ship];
        }

        public String ship_get_current_bio()
        {
            return bios[ship];
        }

        public int ship_get_current_index()
        {
            return ship;
        }

        public int ship_get_current_sprite()
        {
            return sprites[ship];
        }

        public void ship_load(int s)
        {
            ship = s;
            File f = File.Open(StringFormatAGS("SHIP%d.DAT", ship), eFileRead);
            if (f != null)
            {
                design.numVerts = f.ReadInt();
                int v = 0;
                while (v < design.numVerts)
                {
                    design.vx[v] = f.ReadInt();
                    design.vy[v] = f.ReadInt();
                    v += 1;
                }
                v = 0;
                design.numElems = f.ReadInt();
                while (v < design.numElems)
                {
                    design.isSphere[v] = f.ReadInt();
                    design.v1[v] = f.ReadInt();
                    design.v2[v] = f.ReadInt();
                    design.radius[v] = f.ReadInt();
                    v += 1;
                }
                f.Close();
            }
            else 
            {
                design.numVerts = 0;
                design.numElems = 0;
            }
        }

        public void ship_save()
        {
            File f = File.Open(StringFormatAGS("SHIP%d.DAT", ship), eFileWrite);
            if (f != null)
            {
                f.WriteInt(design.numVerts);
                int v = 0;
                while (v < design.numVerts)
                {
                    f.WriteInt(design.vx[v]);
                    f.WriteInt(design.vy[v]);
                    v += 1;
                }
                f.WriteInt(design.numElems);
                v = 0;
                while (v < design.numElems)
                {
                    f.WriteInt(design.isSphere[v]);
                    f.WriteInt(design.v1[v]);
                    f.WriteInt(design.v2[v]);
                    f.WriteInt(design.radius[v]);
                    v += 1;
                }
            }
        }

        public void ship_draw(DrawingSurface surf, float x, float y, float ca, float sa)
        {
            int v = 0;
            while (v < design.numVerts)
            {
                float vx = (6.0f /16.0f)*IntToFloat(design.vx[v]-16);
                float vy = (6.0f /16.0f)*IntToFloat(design.vy[v]-16);
                impl.vx[v] = x + ca*vx - sa*vy;
                impl.vy[v] = y + sa*vx + ca*vy;
                v += 1;
            }
            v = 0;
            while (v < design.numElems)
            {
                int v1 = design.v1[v];
                if (design.isSphere[v])
                {
                    surf.DrawAntialiasedCircle(impl.vx[v1], impl.vy[v1], (6.0f /16.0f)*IntToFloat(design.radius[v]));
                }
                else 
                {
                    int v2 = design.v2[v];
                    surf.DrawAntialiasedLine(impl.vx[v1], impl.vy[v1], impl.vx[v2], impl.vy[v2]);
                }
                v += 1;
            }
        }

    }

    #region Globally Exposed Items

    public partial class GlobalBase
    {
        // Expose AGS singular #defines as C# constants (or static getters)
        public const int MAXVERTS = 48;
        public const int MAXELEMS = 40;

        // Expose Ships methods so they can be used without instance prefix
        public static int ship_get_for_round(int round)
        {
            return Ships.ship_get_for_round(round);
        }

        public static void ships_setup()
        {
            Ships.ships_setup();
        }

        public static String ship_get_name(int s)
        {
            return Ships.ship_get_name(s);
        }

        public static String ship_get_current_name()
        {
            return Ships.ship_get_current_name();
        }

        public static String ship_get_current_bio()
        {
            return Ships.ship_get_current_bio();
        }

        public static int ship_get_current_sprite()
        {
            return Ships.ship_get_current_sprite();
        }

        public static int ship_get_current_index()
        {
            return Ships.ship_get_current_index();
        }

        public static void ship_load(int s)
        {
            Ships.ship_load(s);
        }

        public static void ship_save()
        {
            Ships.ship_save();
        }

        public static void ship_draw(DrawingSurface surf, float x, float y, float ca, float sa)
        {
            Ships.ship_draw(surf, x, y, ca, sa);
        }

        // Expose Ships variables so they can be used without instance prefix
        public static SShipDesign design { get { return Ships.design; } set { Ships.design = value; } }

    }

    #endregion

    #region SShipImpl (AGS struct from .asc converted to class)

    [MessagePackObject(keyAsPropertyName:true)]
    public class SShipImpl
    {
        // Fields
        public float[] vx = new float[MAXVERTS];
        public float[] vy = new float[MAXVERTS];

    }

    #endregion

    #region SShipDesign (AGS struct from .ash converted to class)

    [MessagePackObject(keyAsPropertyName:true)]
    public class SShipDesign
    {
        // Fields
        public int numVerts;
        public int[] vx = new int[MAXVERTS];
        public int[] vy = new int[MAXVERTS];
        public int numElems;
        public bool[] isSphere = new bool[MAXELEMS];
        public int[] v1 = new int[MAXELEMS];
        public int[] v2 = new int[MAXELEMS];
        public int[] radius = new int[MAXELEMS];

    }

    #endregion

    #region Static class for referencing parent class without prefixing with instance (AGS struct workaround)

    public static class ShipsStaticRef
    {
        // Static Fields
        public static SShipImpl impl { get { return GlobalBase.Ships.impl; } set { GlobalBase.Ships.impl = value; } }
        public static int ship { get { return GlobalBase.Ships.ship; } set { GlobalBase.Ships.ship = value; } }
        public static String[] pilots { get { return GlobalBase.Ships.pilots; } set { GlobalBase.Ships.pilots = value; } }
        public static String[] bios { get { return GlobalBase.Ships.bios; } set { GlobalBase.Ships.bios = value; } }
        public static int[] sprites { get { return GlobalBase.Ships.sprites; } set { GlobalBase.Ships.sprites = value; } }
        public static int[] round_1_ships { get { return GlobalBase.Ships.round_1_ships; } set { GlobalBase.Ships.round_1_ships = value; } }
        public static int[] round_2_ships { get { return GlobalBase.Ships.round_2_ships; } set { GlobalBase.Ships.round_2_ships = value; } }
        public static int[] round_3_ships { get { return GlobalBase.Ships.round_3_ships; } set { GlobalBase.Ships.round_3_ships = value; } }
        public static int[] round_4_ships { get { return GlobalBase.Ships.round_4_ships; } set { GlobalBase.Ships.round_4_ships = value; } }
        public static int[] round_5_ships { get { return GlobalBase.Ships.round_5_ships; } set { GlobalBase.Ships.round_5_ships = value; } }
        public static int[] round_6_ships { get { return GlobalBase.Ships.round_6_ships; } set { GlobalBase.Ships.round_6_ships = value; } }
        public static int[] round_7_ships { get { return GlobalBase.Ships.round_7_ships; } set { GlobalBase.Ships.round_7_ships = value; } }

        // Static Methods
        public static void ships_setup()
        {
            GlobalBase.Ships.ships_setup();
        }

        public static String ship_get_current_name()
        {
            return GlobalBase.Ships.ship_get_current_name();
        }

        public static String ship_get_current_bio()
        {
            return GlobalBase.Ships.ship_get_current_bio();
        }

        public static int ship_get_current_index()
        {
            return GlobalBase.Ships.ship_get_current_index();
        }

        public static int ship_get_current_sprite()
        {
            return GlobalBase.Ships.ship_get_current_sprite();
        }

        public static void ship_save()
        {
            GlobalBase.Ships.ship_save();
        }

    }

    #endregion
    
}
