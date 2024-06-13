namespace ObscureLabs
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Scp049;
    using MEC;
    using ObscureLabs.API.Features;
    using ObscureLabs.SpawnSystem;
    using PlayerRoles;
    using SpireSCP.GUI.API.Features;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    internal class Doctor : Module
    {
        public override string Name => "Doctor";

        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {
            Exiled.Events.Handlers.Scp049.ActivatingSense += OnActivatingSense;
            Exiled.Events.Handlers.Scp049.SendingCall += OnSendingCall;
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;

            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Scp049.ActivatingSense -= OnActivatingSense;
            Exiled.Events.Handlers.Scp049.SendingCall -= OnSendingCall;
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;

            return base.Disable();
        }

        private void OnActivatingSense(ActivatingSenseEventArgs ev)
        {
            if (ev.Target is null)
            {
                return;
            }

            ev.Player.EnableEffect(EffectType.MovementBoost, 10);
            ev.Player.ChangeEffectIntensity(EffectType.MovementBoost, 10, 10);

            Manager.SendHint(ev.Player, "You provide a <color=lightblue><u>Speed Boost</u></color> to all nearby <color=red><u>SCP Entities</u></color>!", 7);
            Timing.RunCoroutine(FixedScpBoostCoroutine(ev.Player));
        }

        private void OnSendingCall(SendingCallEventArgs ev)
        {
            if (!ev.IsAllowed)
            {
                return;
            }

            Manager.SendHint(ev.Player, "You are giving <color=lightblue><u>HS points</u></color> to all nearby <color=red><u>SCP Entities</u></color>\n<color=yellow>(this can overflow past natural max)</color>", 7);
            Timing.RunCoroutine(FixedScpShieldCoroutine(ev.Player));
        }

        private IEnumerator<float> FixedScpBoostCoroutine(Player player)
        {
            player.EnableEffect(EffectType.MovementBoost, 15f);
            player.EnableEffect(EffectType.MovementBoost, 30, 15f);

            Log.Info("Running fixedScpBoost");
            for (int i = 0; i < 60; i++)
            {
                Manager.SendHint(player, "You provide a<color=lightblue><u>Speed Boost</u></color> to all nearby <color=red><u>SCP Entities</u></color>!", 0.75f);
                foreach (var player1 in Player.List)
                {
                    if (player1 == player)
                    {
                        continue;
                    }

                    var playerId = Player.Get(player.Id);
                    int loopCntr = 0;
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

                    if (Math.Sqrt(Math.Pow(playerId.Position.x - player2.Position.x, 2) + Math.Pow(playerId.Position.y - player2.Position.y, 2)) > 10)
                    {
                        continue;
                    }

                    if (!player2.IsHuman && player2 != player)
                    {
                        Manager.SendHint(player2, "You are recieving a <color=lightblue><u>Speed Boost</u></color> from a nearby <color=red><u>SCP 049</u></color>!", 0.75f);
                        player2.EnableEffect(EffectType.MovementBoost, 1.5f);
                        player2.ChangeEffectIntensity(EffectType.MovementBoost, 30, 1.5f);
                    }
                }

                yield return Timing.WaitForSeconds(0.5f);
            }

            Log.Info("Stopped fixedScpBoost");
        }

        private IEnumerator<float> FixedScpShieldCoroutine(Player player)
        {
            Log.Info("Running fixedScpShield");
            for (int j = 0; j < 120; j++)
            {
                Manager.SendHint(player, "You provide <color=lightblue><u>HS points</u></color> to all nearby <color=red><u>SCP Entities</u></color>!", 0.75f);
                foreach (var player1 in Player.List)
                {
                    if (player1 == player)
                    {
                        continue;
                    }

                    var playerId = Player.Get(player.Id);
                    int loopCntr = 0;
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

                    if (Math.Sqrt(Math.Pow(playerId.Position.x - player2.Position.x, 2) + Math.Pow(playerId.Position.y - player2.Position.y, 2)) > 10)
                    {
                        continue;
                    }

                    if (!player2.IsHuman && player2 != player)
                    {
                        Manager.SendHint(player2, "You are recieving <color=lightblue><u>HS points</u></color> from a nearby <color=red><u>SCP 049</u></color>!", 0.75f);
                        player2.HumeShield += 2.7f;
                    }
                }

                yield return Timing.WaitForSeconds(0.5f);
            }

            Log.Info("Stopped fixedScpShield");
        }

        public static void OnSpawned(SpawnedEventArgs ev)
        {
            Timing.RunCoroutine(Uppies(ev));
            if (ev.Player.Role == RoleTypeId.Scp0492)
            {
                var playerData = CustomRoles.RolesData.SingleOrDefault(x => x.Player.NetId == ev.Player.NetId) ?? null;
                if (playerData != null)
                {
                    switch (playerData.UcrId)
                    {
                        case 2: ev.Player.Scale = Vector3.one * 0.7f; break;
                    }
                }
            }

            Timing.RunCoroutine(HealthOverride.OverrideHealth(ev));
            Timing.RunCoroutine(CustomRoles.CheckRoles(ev.Player));
        }

        private static IEnumerator<float> Uppies(SpawnedEventArgs ev)
        {
            yield return Timing.WaitForSeconds(0.25f);
            ev.Player.Position = new Vector3(ev.Player.Position.x, ev.Player.Position.y + 0.3f, ev.Player.Position.z);
        }
    }
}
