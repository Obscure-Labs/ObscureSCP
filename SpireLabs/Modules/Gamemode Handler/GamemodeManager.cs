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
        public Gamemode[] _gamemodes = { new Vanilla() };

        public override bool Enable()
        {
            LabApi.Events.Handlers.ServerEvents.RoundStarted += OnRoundStarted;
            LabApi.Events.Handlers.ServerEvents.RoundEnded += OnRoundEnded;

            if(UnityEngine.Random.RandomRange(0, 101) < 35)
            {
                //Selected gamemode round
                selectedGamemode = _gamemodes[UnityEngine.Random.Range(0, _gamemodes.Count())];
                if (!selectedGamemode.PreInitialise())
                {
                    foreach (var module in Plugin.Instance._modules.Modules)
                    {
                        if (module.IsInitializeOnStart)
                        {
                            module.Enable();
                        }
                    }
                    this.Disable();
                }
            }
            else
            {
                foreach (var module in Plugin.Instance._modules.Modules)
                {
                    if (module.IsInitializeOnStart)
                    {
                        module.Enable();
                    }
                }
                this.Disable();
            }
            return base.Enable();
        }

        public override bool Disable()
        {
            LabApi.Events.Handlers.ServerEvents.RoundStarted -= OnRoundStarted;
            LabApi.Events.Handlers.ServerEvents.RoundEnded -= OnRoundEnded;

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
