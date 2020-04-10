// Module_Sounds - Type "override" followed by space to see list of C# methods to implement
using static SpacePoolAlpha.GlobalBase;
using System.Diagnostics;
using static SpacePoolAlpha.Module_Sounds;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using MessagePack;
using Clarvalon.XAGE.Global;

namespace SpacePoolAlpha
{
    public partial class Module_Sounds
    {
        // Fields

        // Methods
        public void PlaySoundSafe(AudioClip clip, int volume)
        {
            AudioChannel chan = clip.Play();
            if (chan != null)
            {
                int clampedVolume = volume;
                if (volume > 100)
                    clampedVolume = 100;
                else if (volume < 1)
                    clampedVolume = 1;
                chan.Volume = clampedVolume;
            }
        }

        public void PlaySoundSafePrio(AudioClip clip, int volume, int prio)
        {
            AudioChannel chan = clip.Play((AudioPriority)prio);
            if (chan != null)
            {
                int clampedVolume = volume;
                if (volume > 100)
                    clampedVolume = 100;
                else if (volume < 1)
                    clampedVolume = 1;
                chan.Volume = clampedVolume;
            }
        }

    }

    #region Globally Exposed Items

    public partial class GlobalBase
    {
        // Expose Sounds methods so they can be used without instance prefix
        public static void PlaySoundSafe(AudioClip clip, int volume)
        {
            Sounds.PlaySoundSafe(clip, volume);
        }

        public static void PlaySoundSafePrio(AudioClip clip, int volume, int prio)
        {
            Sounds.PlaySoundSafePrio(clip, volume, prio);
        }


    }

    #endregion

}
