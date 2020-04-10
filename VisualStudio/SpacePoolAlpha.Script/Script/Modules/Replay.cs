// Module_Replay - Type "override" followed by space to see list of C# methods to implement
using static SpacePoolAlpha.GlobalBase;
using System.Diagnostics;
using static SpacePoolAlpha.Module_Replay;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MessagePack;
using Clarvalon.XAGE.Global;

namespace SpacePoolAlpha
{
    public partial class Module_Replay
    {
        // Fields
        public bool replay_Record;
        public bool replay_Playback;
        public int[] inputBuffer = new int[MAX_REPLAY];
        public int bufPos;
        public int bufSize;
        public int seed;

        // Methods
        public void replay_start()
        {
            DateTime dt = DateTime.Now;
            seed = dt.RawTime() & 65535;
            Random2_Seed(seed);
            bufPos = 0;
            replay_Record = true;
        }

        public int replay_get_best()
        {
            File f = File.Open("REPLAY.DAT", eFileRead);
            if (f != null)
            {
                int best = f.ReadInt();
                f.Close();
                return best;
            }
            return MAX_REPLAY;
        }

        public int replay_get_time()
        {
            return bufPos;
        }

        public void replay_start_replay()
        {
            Random2_Seed(0);
            bufSize = 0;
            bufPos = 0;
            File f = File.Open("REPLAY.DAT", eFileRead);
            if (f != null)
            {
                bufSize = f.ReadInt();
                seed = f.ReadInt();
                Random2_Seed(seed);
                if (bufSize > 0 && bufSize <= MAX_REPLAY)
                {
                    int i = 0;
                    while (i < bufSize)
                    {
                        inputBuffer[i] = f.ReadInt();
                        i += 1;
                    }
                    replay_Playback = true;
                }
                else 
                {
                    bufSize = 0;
                }
                f.Close();
            }
        }

        public void replay_save_input(bool left, bool right, bool thrust, bool fire)
        {
            int input = 0;
            if (left)
            {
                input = 1;
            }
            if (right)
            {
                input += 2;
            }
            if (thrust)
            {
                input += 4;
            }
            if (fire)
            {
                input += 8;
            }
            if (bufPos < MAX_REPLAY)
            {
                inputBuffer[bufPos] = input;
                bufPos += 1;
            }
        }

        public void replay_end()
        {
            if (bufPos > 0)
            {
                File f = File.Open("REPLAY.DAT", eFileWrite);
                if (f != null)
                {
                    f.WriteInt(bufPos);
                    f.WriteInt(seed);
                    int i = 0;
                    while (i < bufPos)
                    {
                        f.WriteInt(inputBuffer[i]);
                        i += 1;
                    }
                    f.Close();
                }
            }
            replay_Record = false;
        }

        public bool replay_exists()
        {
            return File.Exists("REPLAY.DAT");
        }

        public bool replay_get_left()
        {
            return (inputBuffer[bufPos] & 1) > 0;
        }

        public bool replay_get_right()
        {
            return (inputBuffer[bufPos] & 2) > 0;
        }

        public bool replay_get_thrust()
        {
            return (inputBuffer[bufPos] & 4) > 0;
        }

        public bool replay_get_fire()
        {
            return (inputBuffer[bufPos] & 8) > 0;
        }

        public void replay_next_frame()
        {
            if (bufPos < bufSize)
            {
                bufPos += 1;
            }
            else 
            {
                replay_Playback = false;
            }
        }

    }

    #region Globally Exposed Items

    public partial class GlobalBase
    {
        // Expose AGS singular #defines as C# constants (or static getters)
        public const int MAX_REPLAY = 12000;

        // Expose Replay methods so they can be used without instance prefix
        public static void replay_start()
        {
            Replay.replay_start();
        }

        public static void replay_save_input(bool left, bool right, bool thrust, bool fire)
        {
            Replay.replay_save_input(left, right, thrust, fire);
        }

        public static bool replay_get_left()
        {
            return Replay.replay_get_left();
        }

        public static bool replay_get_right()
        {
            return Replay.replay_get_right();
        }

        public static bool replay_get_fire()
        {
            return Replay.replay_get_fire();
        }

        public static bool replay_get_thrust()
        {
            return Replay.replay_get_thrust();
        }

        public static void replay_next_frame()
        {
            Replay.replay_next_frame();
        }

        public static void replay_end()
        {
            Replay.replay_end();
        }

        public static bool replay_exists()
        {
            return Replay.replay_exists();
        }

        public static int replay_get_time()
        {
            return Replay.replay_get_time();
        }

        public static int replay_get_best()
        {
            return Replay.replay_get_best();
        }

        public static void replay_start_replay()
        {
            Replay.replay_start_replay();
        }

        // Expose Replay variables so they can be used without instance prefix
        public static bool replay_Record { get { return Replay.replay_Record; } set { Replay.replay_Record = value; } }
        public static bool replay_Playback { get { return Replay.replay_Playback; } set { Replay.replay_Playback = value; } }

    }

    #endregion

}
