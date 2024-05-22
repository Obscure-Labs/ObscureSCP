using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp173;
using MEC;
using ObscureLabs.API.Features;
using PlayerRoles;
using SpireSCP.GUI.API.Features;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ObscureLabs
{
    public class TheNut : Module
    {
        public override string Name => "TheNut";

        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Scp173.Blinking += OnBlinking;

            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            Exiled.Events.Handlers.Scp173.Blinking -= OnBlinking;

            return base.Disable();
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Player.Role.Type is not RoleTypeId.Scp173)
            {
                return;
            }

            int num = UnityEngine.Random.Range(1, 100);

            if (num < 20 && num > 13)
            {
                ev.Amount = 0;
                Manager.SendHint(ev.Player, "You just ignored some damage!", 5);
            }
        }

        private void OnBlinking(BlinkingEventArgs ev)
        {
            Timing.RunCoroutine(KillCoroutine(ev.Player, ev.Scp173.BreakneckActive));
        }

        private static IEnumerator<float> KillCoroutine(Player player, bool isBreakneck)
        {
            yield return Timing.WaitForOneFrame;

            if (isBreakneck && player.Health < 1000)
            {
                foreach (Player player1 in Player.List)
                {
                    if (player1 == player)
                    {
                        continue;
                    }

                    var playerId = Player.Get(player.Id);
                    var loopCntr = 0;
                    var raycastHit = new RaycastHit();
                    Player player2 = null;

                    do
                    {
                        var direction = player1.Position - new Vector3(playerId.Position.x, playerId.Position.y + 0.1f, playerId.Position.z);
                        Physics.Raycast(playerId.Position, direction, out raycastHit);
                        loopCntr++;
                    } while (!Player.TryGet(raycastHit.collider, out player2) && loopCntr != 5);

                    if (player2 is null)
                    {
                        continue;
                    }

                    if (Math.Sqrt(Math.Pow(playerId.Position.x - player2.Position.x, 2) + Math.Pow(playerId.Position.y - player2.Position.y, 2)) > 1.5)
                    {
                        continue;
                    }

                    if (player2.IsHuman)
                    {
                        player2.Hurt(200, DamageType.Crushed);
                    }
                }
            }
        }
    }
}
