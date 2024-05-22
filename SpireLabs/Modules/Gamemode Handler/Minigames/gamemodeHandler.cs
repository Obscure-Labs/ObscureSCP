using MEC;
using ObscureLabs.API.Data;
using ObscureLabs.API.Enums;
using ObscureLabs.API.Features;
using ObscureLabs.Modules.Gamemode_Handler.Minigames;
using ObscureLabs.SpawnSystem;
using System;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ObscureLabs.Gamemode_Handler
{
    public class GamemodeHandler : Module
    {
        public override string Name => "GamemodeHandler";

        public override bool IsInitializeOnStart => true;

        private static readonly IDeserializer _deserializer = new DeserializerBuilder()
    .WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

        private static readonly ISerializer _serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

        private static SerializableGameModeData _serializableGameMode;

        private static readonly int[] _gameModes = new[]
        {
            0, //JBTDM
            1, // Chaos
            2, //OtherOtherMode
        };

        public override bool Enable()
        {
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;

            Timing.CallDelayed(Timing.WaitForOneFrame, () =>
            {
                _serializableGameMode = ReadGameMode();
            });

            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;

            return base.Disable();
        }

        public static SerializableGameModeData ReadGameMode()
        {
            return _deserializer.Deserialize<SerializableGameModeData>(File.ReadAllText(Plugin.SpireConfigLocation + "gamemodeInfo.yaml"));
        }

        public static void WriteAllGameModeData(bool isGameModeRound, int lastGameMode, bool isNextRoundGameMode)
        {
            File.WriteAllText(
                Plugin.SpireConfigLocation + "gamemodeInfo.yaml",
                _serializer.Serialize(new SerializableGameModeData(isGameModeRound, lastGameMode, isNextRoundGameMode)));
        }


        public void OnRoundStarted()
        {
            if (!Plugin.IsActiveEventround)
            {
                WriteAllGameModeData(false, -1, _serializableGameMode.IsNextRoundGameMode ?? false);
                AttemptGameModeRound(false, -1);
            }
        }

        public static void AttemptGameModeRound(bool force, int args)
        {
            var ran = new Random();
            int chance = ran.Next(0, 100);

            if (_serializableGameMode.IsNextRoundGameMode ?? false || force || chance > 30 && chance < 50 && _serializableGameMode.IsGameModeRound)
            {
                Plugin.IsActiveEventround = true;
                int selectedGM;
                if (args > _gameModes.Count() || args == -1)
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
                        Timing.RunCoroutine(TeamDeathMatch.runJbTDM());
                        Plugin.IsActiveEventround = true;
                        Plugin.EventRoundType = EventRoundType.JailbirdsDeatchMatch;
                        break;
                    case 1:
                        Timing.RunCoroutine(Chaos.RunChaosCoroutine());
                        Plugin.IsActiveEventround = true;
                        Plugin.EventRoundType = EventRoundType.Chaos;
                        break;
                    case 2:
                        Timing.RunCoroutine(Juggernaut.RunJuggernautCoroutine());
                        Plugin.IsActiveEventround = true;
                        Plugin.EventRoundType = EventRoundType.Juggernaut;
                        break;
                }

                WriteAllGameModeData(true, selectedGM, false);
            }
            else
            {
                ModulesManager.GetModule("SCP3114").Enable();
                ModulesManager.GetModule("ChaosRound").Enable();
                ModulesManager.GetModule("GamemodeHandler").Disable();
                Plugin.IsActiveEventround = false;
                SCPHandler.doSCPThings();
                WriteAllGameModeData(false, -1, _serializableGameMode.IsNextRoundGameMode ?? false);
            }
        }
    }
}

