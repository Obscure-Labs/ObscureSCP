﻿using Exiled.API.Features;
using LabApi.Events.Arguments.ServerEvents;
using ObscureLabs.API.Features;
using ObscureLabs.Modules.Gamemode_Handler.Modes;
using System.Linq;

namespace ObscureLabs.Modules.Gamemode_Handler
{
    internal class GamemodeManager : Module
    {
        public override string Name => "GamemodeManager";

        public override bool IsInitializeOnStart => false;

        public Gamemode selectedGamemode;
        public Gamemode[] _gamemodes = { new Insanity(), new Standard(), new RedLightGreenLight_Standard(), new Standard() };

        public override bool Enable()
        {
            selectedGamemode = null;

            LabApi.Events.Handlers.ServerEvents.RoundStarted += OnRoundStarted;
            LabApi.Events.Handlers.ServerEvents.RoundEnded += OnRoundEnded;

            //Selected gamemode round
            selectedGamemode = _gamemodes[UnityEngine.Random.Range(0, _gamemodes.Count())];
            Log.Warn($"[GamemodeManager] Gamemode {selectedGamemode.Name} was selected.");
            if (!selectedGamemode.PreInitialise())
            {
                Log.Info("Failed to pre-initialise gamemode.");
                foreach (var module in Plugin.Instance._modules.Modules)
                {
                    if (module.IsInitializeOnStart)
                    {
                        module.Enable();
                    }
                }
                this.Disable();
            }
           
            //else
            //{
            //    Log.Warn($"[GamemodeManager] No gamemode was selected.");
            //    foreach (var module in Plugin.Instance._modules.Modules)
            //    {
            //        if (module.IsInitializeOnStart)
            //        {
            //            module.Enable();
            //        }
            //    }
            //    this.Disable();
            //}
            return base.Enable();
        }

        public override bool Disable()
        {
            LabApi.Events.Handlers.ServerEvents.RoundStarted -= OnRoundStarted;
            LabApi.Events.Handlers.ServerEvents.RoundEnded -= OnRoundEnded;
            Log.Info($"[GamemodeManager] Disabling gamemode maanger.");
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
    }
}
