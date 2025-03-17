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
using Exiled.API.Features;
using Exiled.API.Extensions;
using MEC;
using SpireSCP.GUI.API.Features;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    public class FlipResult
    {
        public FlipResult(string name, string hint, Func<Exiled.API.Features.Player, bool> func)
        {
            Name = name;
            Hint = hint;
            Func = func;
        }

        public string Name { get; set; }
        public string Hint { get; set; }
        public Func<Exiled.API.Features.Player, bool> Func { get; set; }

        public void SendHint(Player player)
        {
            Manager.SendHint(player, Hint, 5f);
        }
    }

    public class CoinFlip : Module
    {
        public override string Name => "CoinFlip";

        public override bool IsInitializeOnStart => true;

        public List<FlipResult> _goodResults = new List<FlipResult>()
        {
            new("RandomItem", "You got a random item!", (Player) =>
            {
                var vals = Enum.GetValues(typeof(ItemType));
                Player.AddItem((ItemType)vals.GetValue(UnityEngine.Random.Range(0, vals.Length)));
                return true;
            }),
        };

        public List<FlipResult> _badResults = new List<FlipResult>()
        {
            new("Antigravity", "uʍop ǝpᴉsd∩", (Player) =>
            {
                Vector3 oldGrav = LabApi.Features.Wrappers.Player.Get(Player.NetworkIdentity).Gravity;
                LabApi.Features.Wrappers.Player.Get(Player.NetworkIdentity).Gravity *= -1;
                Timing.CallDelayed(10f, () => {LabApi.Features.Wrappers.Player.Get(Player.NetworkIdentity).Gravity = oldGrav; });
                return true;
            }),
            new("Explode", "BOOM", (Player) =>
            {
                Player.Explode();
                return true;
            })
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

            outcome.SendHint(ev.Player);
            outcome.Func.Invoke(ev.Player);

        }
    }
}
