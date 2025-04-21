using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using GameCore;
using MEC;
using ObscureLabs.API.Features;
using static RoundSummary;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    internal class RoundEndPVP : Module
    {
        public override string Name => "RoundEndPvP";

        public override bool IsInitializeOnStart => true;

        public CoroutineHandle Routine;

        public override bool Enable()
        {
            Exiled.Events.Handlers.Server.RoundEnded += RoundEnd;
            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Server.RoundEnded -= RoundEnd;
            return base.Disable();
        }


        public void RoundEnd(RoundEndedEventArgs ev)
        {
            Server.FriendlyFire = true;
        }
    }
}
