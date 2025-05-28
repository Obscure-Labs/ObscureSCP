using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;
using LabApi.Events.Arguments.PlayerEvents;
using ObscureLabs.API.Features;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    internal class SCPsDropItems : Module
    {
        public override string Name => "SCPsDropItems";
        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {
            Exiled.Events.Handlers.Player.Hurt += OnPlayerHurt;
            return base.Enable();
        }
        public override bool Disable()
        {
            Exiled.Events.Handlers.Player.Hurt -= OnPlayerHurt;
            return base.Disable();
        }


        public void OnPlayerHurt(HurtEventArgs ev)
        {
            
            if (ev.Player.IsScp && ev.Attacker != null && ev.Player.Items.Count != 0)
            {
                int chance = UnityEngine.Random.Range(0, 100);
                if (chance >= 60 && chance <= 70)
                {
                    ev.Player.DropItem(ev.Player.Items.GetRandomValue());
                }
            }
        }
    }
}
