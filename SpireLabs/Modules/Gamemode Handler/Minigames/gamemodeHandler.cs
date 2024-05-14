using GameCore;
using MEC;
using ObscureLabs.Gamemode_Handler;
using ObscureLabs.Modules.Gamemode_Handler.Minigames;
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
    public class gamemodeHandler : Plugin.Module
    {

        public override string name { get; set; } = "GamemodeHandler";
        public override bool initOnStart { get; set; } = true;

        public override bool Init()
        {
            try
            {
                Exiled.Events.Handlers.Server.RoundStarted += roundStarted;
                base.Init();
                return true;
            }
            catch { return false; }
        }

        public override bool Disable()
        {
            try
            {
                Exiled.Events.Handlers.Server.RoundStarted -= roundStarted;
                base.Disable();
                return true;
            }
            catch { return false; };
        }

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


        public static void roundStarted()
        {
            if (!Plugin.IsActiveEventround)
            {
                gamemodeHandler.WriteAllGMInfo(false, -1, gamemodeHandler.ReadNext());
                gamemodeHandler.AttemptGMRound(false, -1);
            }
        }

        public static void AttemptGMRound(bool force, int args)
        {



            //await Task.Delay(1);
            var ran = new Random();
            int chance = ran.Next(0, 100);
                //if (chance > 30 && chance < 50 && !ReadLast())
                //{
                //    WriteAllGMInfo(ReadLast(), ReadMode(), true);
                //}
            if (ReadNext() || force || chance > 30 && chance < 50 && !ReadLast())
            {
                Plugin.IsActiveEventround = true;
                int[] modes =
                {
                0, //JBTDM
                1, // Chaos
                2, //OtherOtherMode
                };
                int selectedGM = 0;
                if (args == null || args > modes.Count() || args == -1)
                {
                    selectedGM = ran.Next(0, modes.Count());
                }
                else
                {
                    selectedGM = args;
                }



                switch (selectedGM)
                {
                    case 0:
                        Timing.RunCoroutine(tdm.runJbTDM()); Plugin.IsActiveEventround = true; Plugin.EventRoundType = "jbtdm"; break;
                    case 1:
                        Timing.RunCoroutine(chaos.runChaos()); Plugin.IsActiveEventround = true; Plugin.EventRoundType = "chaos"; break;
                    case 2:
                        Timing.RunCoroutine(juggernaut.runJuggernaut()); Plugin.IsActiveEventround = true; Plugin.EventRoundType = "juggernaut";  break;
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

