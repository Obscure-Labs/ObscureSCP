using MEC;
using ObscureLabs.API.Data;
using ObscureLabs.API.Enums;
using ObscureLabs.API.Features;
using ObscureLabs.Modules.Gamemode_Handler.Minigames;
using ObscureLabs.SpawnSystem;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Exiled.API.Features;
using Exiled.Loader;
using Unity.Collections;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ObscureLabs.Gamemode_Handler
{
    public class GamemodeHandler : Module
    {
        public override string Name => "GamemodeHandler";

        public override bool IsInitializeOnStart => true;

        private static gamemodeInfo _serializableGameMode;

        private static readonly int[] _gameModes = new[]
        {
            0, //JBTDM
            1, // Chaos
            2, //OtherOtherMode
        };

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

        public static bool runningChecks = false;

        public override bool Enable()
        {
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;

            _serializableGameMode = Deserializer.Deserialize<gamemodeInfo>(File.ReadAllText(Plugin.SpireConfigLocation + "gamemodeInfo.Yaml"));
            //File.WriteAllText(Plugin.SpireConfigLocation + "gamemodeInfo.yaml",
            //    Loader.Serializer.Serialize(new SerializableGameModeData(false, 0, false)));
            //_serializableGameMode = new SerializableGameModeData(false, 0, false);
            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            return base.Disable();
        }

        //public static SerializableGameModeData ReadGameMode()
        //{
        //    return Loader.Deserializer.Deserialize<SerializableGameModeData>(
        //        File.ReadAllText(Plugin.SpireConfigLocation + "gamemodeInfo.yaml"));
        //}

        public static void WriteAllGameModeData(bool isGameModeRound, int lastGameMode, bool isNextRoundGameMode)
        {
            File.WriteAllText(
                Plugin.SpireConfigLocation + "gamemodeInfo.yaml",
                Serializer.Serialize(new gamemodeInfo{gamemodeRound = isGameModeRound, lastGamemode = lastGameMode, nextRoundIsGamemode = isNextRoundGameMode}));
        }
        
        public void OnRoundStarted()
        {
            if (!Plugin.IsActiveEventround || runningChecks)
            {
                WriteAllGameModeData(false, 0, _serializableGameMode.nextRoundIsGamemode);
                AttemptGameModeRound(false, 0);
            }
        }

        public static void AttemptGameModeRound(bool force, int args)
        {
            runningChecks = true;
            var ran = new Random();
            int chance = ran.Next(0, 100);

            //if (_serializableGameMode.nextRoundIsGamemode || force || chance > 30 && chance < 50 && Plugin.IsActiveEventround == false)
            if (force)
            {
                Plugin.IsActiveEventround = true;
                int selectedGM;
                if (args == -1)
                {
                    selectedGM = ran.Next(0, _gameModes.Count());
                }
                else
                {
                    selectedGM = args;
                }

                switch (selectedGM)
                {
                    case 0:
                        ModulesManager.GetModule("TeamHandler").Enable();
                        ModulesManager.GetModule("TDM").Enable();
                        Plugin.IsActiveEventround = true;
                        Plugin.EventRoundType = EventRoundType.TeamDeathMatch;
                        break;
                    case 1:
                        ModulesManager.GetModule("TeamHandler").Enable();
                        Timing.RunCoroutine(Chaos.RunChaosCoroutine());
                        Plugin.IsActiveEventround = true;
                        Plugin.EventRoundType = EventRoundType.Chaos;
                        break;
                    case 2:
                        ModulesManager.GetModule("TeamHandler").Enable();
                        Timing.RunCoroutine(Juggernaut.RunJuggernautCoroutine());
                        Plugin.IsActiveEventround = true;
                        Plugin.EventRoundType = EventRoundType.Juggernaut;
                        break;
                }

                WriteAllGameModeData(true, selectedGM, false);
            }
            else
            {
                runningChecks = false;
                //WriteAllGameModeData(false, -1, _serializableGameMode.nextRoundIsGamemode);
                Plugin.IsActiveEventround = false;
                SCPHandler.doSCPThings();
                ModulesManager.GetModule("SCP3114").Enable();
                ModulesManager.GetModule("ChaosRound").Disable();
                ModulesManager.GetModule("GamemodeHandler").Disable();
            }
        }
    }
}

