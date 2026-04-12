using Exiled.API.Features;
using LabApi.Events.Arguments.ServerEvents;
using ObscureLabs.API.Features;
using ObscureLabs.Modules.Gamemode_Handler.Modes;
using RoundRestarting;
using System.Linq;

namespace ObscureLabs.Modules.Gamemode_Handler
{
    internal class GamemodeManager : Module
    {
        public override string Name => "GamemodeManager";

        public override bool IsInitializeOnStart => false;

        public Gamemode selectedGamemode;
        public Gamemode[] _gamemodes = { 
            new Insanity(),
            new Standard(),
            new Standard(),
            new RedLightGreenLight_Standard(),
        };

        public override bool Enable()
        {
            selectedGamemode = null;

            LabApi.Events.Handlers.ServerEvents.RoundStarted += OnRoundStarted;
            LabApi.Events.Handlers.ServerEvents.RoundRestarted += OnRoundRestarted;
            LabApi.Events.Handlers.ServerEvents.RoundEnded += OnRoundEnded;

            //Selected gamemode round
            selectedGamemode = _gamemodes[UnityEngine.Random.Range(0, _gamemodes.Count())];
            Log.Warn($"[GamemodeManager] Gamemode {selectedGamemode.Name} was selected.");
            selectedGamemode.PreInitialise();

            return base.Enable();
        }

        public override bool Disable()
        {
            //selectedGamemode.Stop();
            LabApi.Events.Handlers.ServerEvents.RoundStarted -= OnRoundStarted;
            LabApi.Events.Handlers.ServerEvents.RoundRestarted -= OnRoundRestarted;
            LabApi.Events.Handlers.ServerEvents.RoundEnded -= OnRoundEnded;
            Log.Info($"[GamemodeManager] Disabling gamemode manager.");
            return base.Disable();
        }

        private void OnRoundStarted()
        {
            selectedGamemode.Start();
        }

        private void OnRoundEnded(RoundEndedEventArgs ev)
        {
            selectedGamemode.Stop();
        }

        private void OnRoundRestarted()
        {
            selectedGamemode.Stop();
        }
    }
}
