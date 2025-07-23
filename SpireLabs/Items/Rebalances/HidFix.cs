using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.Events.EventArgs.Player;
using MEC;
using ObscureLabs.API.Features;
using UnityEngine;

namespace ObscureLabs.Items.Rebalances
{
    internal class HidFix : Module
    {
        public override string Name => "HidFix";
        public override bool IsInitializeOnStart => true;


        public override bool Enable()
        {
            Exiled.Events.Handlers.Player.Hurting += Attacking;

            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Player.Hurting -= Attacking;
            return base.Disable();
        }


        private void Attacking(HurtingEventArgs ev)
        {
            if (ev.DamageHandler.Type == Exiled.API.Enums.DamageType.MicroHid)
            {
                ev.Amount = ev.Amount * 0.65f;
            }
        }
    }
}
