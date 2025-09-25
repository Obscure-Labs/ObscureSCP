using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using ObscureLabs.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    internal class LightHandler : Module
    {
        public override string Name => "LightHandler";

        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {
            Exiled.Events.Handlers.Map.Generated += OnMapGenerated;

            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Map.Generated -= OnMapGenerated;

            return base.Disable();
        }

        private IEnumerator<float> OnMapGenerated()
        {
            Lift.List.Where(x => x.Name.Contains("Gate")).ToList().Where(x => x.CurrentLevel == 1).ToList().ForEach(x => x.TryStart(0, true));
            yield return Timing.WaitForSeconds(7f);
            //Room.Get(ZoneType.Surface).First().Color = new UnityEngine.Color(0f, 0f, 0f);
        }
    }
}
