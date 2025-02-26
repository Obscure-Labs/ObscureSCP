using ObscureLabs.API.Data;
using ObscureLabs.API.Features;
using RelativePositioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API;
using Utf8Json.Resolvers.Internal;
using Exiled.Events.EventArgs.Player;
using Discord;
using ServerOutput;
using UnityEngine;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    public class CoinFlip : Module
    {
        public override string Name => "CoinFlip";

        public override bool IsInitializeOnStart => true;

        public List<string> _bad = new List<string>()
        {
            "_antiGravity",
            "_explode"
        };
        public List<string> _good = new List<string>() 
        {
            "_randomItem",
            "_heal"
        };

        private int _goodThreshold = 60;


        public override bool Enable()
        {
            Exiled.Events.Handlers.Player.FlippingCoin += CoinFlipEvent;
            return base.Enable();
        }

        public override bool Disable()
        {
            return base.Disable();
        }


        private void CoinFlipEvent(FlippingCoinEventArgs ev)
        {
            int chance = UnityEngine.Random.Range(0, 100);
            bool result = chance >= _goodThreshold ? true : false;
            string outcome;

            if (result)
            {
                outcome = _good.ElementAt(UnityEngine.Random.Range(0, _good.Count()));
            }
            else
            {
                outcome = _bad.ElementAt(UnityEngine.Random.Range(0, _good.Count()));
            }

            switch (outcome)
            {
                case "_antigravity":
                    {
                        break;
                    }
                case "_explode":
                    {
                        ev.Player.Explode();
                        break;
                    }

            }

        }
    }
}
