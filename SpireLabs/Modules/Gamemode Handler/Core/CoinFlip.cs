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
    public class FlipResult
    {
        public string Name { get; set; }
        public Func<Exiled.API.Features.Player, bool> Func { get; set; }
    }

    public class CoinFlip : Module
    {
        public override string Name => "CoinFlip";

        public override bool IsInitializeOnStart => true;

        public List<FlipResult> _goodResults = new List<FlipResult>()
        {
            new FlipResult()
            {
                Name = "RandomItem",
                Func = (Player) =>
                {

                    return true;
                }
            },
        };

        public List<FlipResult> _badResults = new List<FlipResult>()
        {
            new FlipResult()
            {
                Name = "Antigravity",
                Func = (Player) =>
                {
                    return true;
                }
            },
            new FlipResult()
            {
                Name = "Explode",
                Func = (Player) =>
                {
                    return true;
                }
            }
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
            FlipResult outcome;

            if (result)
            {
                outcome = _goodResults.ElementAt(UnityEngine.Random.Range(0, _goodResults.Count()));
            }
            else
            {
                outcome = _badResults.ElementAt(UnityEngine.Random.Range(0, _badResults.Count()));
            }

            outcome.Func.Invoke(ev.Player);

        }
    }
}
