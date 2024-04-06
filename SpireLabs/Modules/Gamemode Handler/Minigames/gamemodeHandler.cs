using MEC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ObscureLabs.Gamemode_Handler
{
    public static class gamemodeHandler
    {
        public static IDeserializer Deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        public static ISerializer Serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

        public class gamemodeInfo
        {
            public bool gamemodeRound { get; set; }
            public int lastGamemode { get; set; }
            public bool nextRoundIsGamemode { get; set; }
        }

        public static bool ReadLast()
        {
            gamemodeInfo data = Deserializer.Deserialize<gamemodeInfo>(File.ReadAllText((Plugin.spireConfigLoc + "gamemodeInfo.yaml")));
            return data.gamemodeRound;
        }

        public static int ReadMode()
        {
            gamemodeInfo data = Deserializer.Deserialize<gamemodeInfo>(File.ReadAllText((Plugin.spireConfigLoc + "gamemodeInfo.yaml")));
            return data.lastGamemode;
        }

        public static bool ReadNext()
        {
            gamemodeInfo data = Deserializer.Deserialize<gamemodeInfo>(File.ReadAllText((Plugin.spireConfigLoc + "gamemodeInfo.yaml")));
            return data.nextRoundIsGamemode;
        }

        public static void WriteAllGMInfo(bool gmR, int Lgm, bool nGM)
        {
            File.WriteAllText((Plugin.spireConfigLoc + "gamemodeInfo.yaml"), Serializer.Serialize(new gamemodeInfo { gamemodeRound = gmR, lastGamemode = Lgm, nextRoundIsGamemode = nGM }));
        }

        public static void AttemptGMRound(bool force)
        {
            var ran = new Random();
            int chance = ran.Next(0, 100);
            if(chance > 30 && chance < 50 && !ReadLast())
            {
                WriteAllGMInfo(ReadLast(), ReadMode(), true);
            }
            if (ReadNext() || force)
            {
                Plugin.IsActiveEventround = true;
                int[] modes =
                {
                0, //JBTDM
                //1, //Othermode
                //2, //OtherOtherMode
                };
                int selectedGM = ran.Next(0, modes.Count());
                switch (selectedGM)
                {
                    case 0:
                        Timing.RunCoroutine(jailBirdTDM.runJbTDM()); break;
                }
                WriteAllGMInfo(true, selectedGM, false);
            }
            else
            {

                Plugin.IsActiveEventround = false;
                WriteAllGMInfo(false, -1, ReadNext());
            }
        }
    }
}

