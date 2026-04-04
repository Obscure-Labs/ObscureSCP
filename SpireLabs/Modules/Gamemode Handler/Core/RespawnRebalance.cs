using LabApi.Events.Arguments.WarheadEvents;
using MEC;
using ObscureLabs.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabApi.Features.Wrappers;
using static RoundSummary;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    internal class RespawnRebalance : Module
    {
        public override string Name => "RespawnRebalance";

        public override bool IsInitializeOnStart => true;

        public CoroutineHandle Routine;


        public override bool Enable()
        {
            LabApi.Events.Handlers.WarheadEvents.Detonated += WarheadDetonated;
 
            return base.Enable();
        }

        public override bool Disable()
        {
            LabApi.Events.Handlers.WarheadEvents.Detonated -= WarheadDetonated;
            return base.Disable();
        }



        private void WarheadDetonated(WarheadDetonatedEventArgs ev)
        {

            RespawnWaves.PrimaryMtfWave.RespawnTokens++;
            RespawnWaves.PrimaryMtfWave.TimeLeft = 30;
            RespawnWaves.PrimaryChaosWave.RespawnTokens++;
            RespawnWaves.PrimaryChaosWave.TimeLeft = 30;
        }
    }
}
