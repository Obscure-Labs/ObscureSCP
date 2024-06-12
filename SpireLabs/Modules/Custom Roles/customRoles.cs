using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using ObscureLabs.API.Data;
using ObscureLabs.API.Features;
using PlayerRoles;
using SpireSCP.GUI.API.Features;
using System.Collections.Generic;
using UncomplicatedCustomRoles.Extensions;
using UCRAPI = UncomplicatedCustomRoles.API.Features.CustomRole;

namespace ObscureLabs
{
    public class CustomRoles : Module
    {
        public static List<RoleData> RolesData { get; } = new();

        public static IEnumerator<float> CheckRoles(Player player)
        {
            var scps = string.Empty;
            var counter = 0;

            foreach (var player1 in Player.List)
            {
                if (player1.Role.Team is Team.SCPs)
                {
                    if (counter != 0)
                    {
                        scps += $"<color=white>, <color=red>{player1.Role.Name}";
                    }
                    else
                    {
                        scps += $"<color=red>{player1.Role.Name}";
                    }

                    counter++;
                }
            }

            int? UCRID = null;

            yield return Timing.WaitForSeconds(0.5f);

            if (player.HasCustomRole())
            {
                UCRID = player.GetCustomRole().Id;

                if (UCRID == 4)
                {
                    yield return Timing.WaitForSeconds(6);
                    Manager.SendHint(player, $"<b>The currently active SCP subjects are: {scps}", 15);
                }

                if (UCRID == 3)
                {
                    player.ChangeAppearance(RoleTypeId.FacilityGuard);
                }
            }
        }

        public override string Name => "CustomRoles";

        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {
            Exiled.Events.Handlers.Player.UsingItemCompleted += OnUsingItem;
            base.Enable();
            return true;
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Player.UsingItemCompleted -= OnUsingItem;
            base.Disable();
            return true;
        }

        private void OnUsingItem(UsingItemCompletedEventArgs ev)
        {
            if (ev.Item.Type is not ItemType.SCP330)
            {
                return;
            }

            if (!ev.Player.HasCustomRole())
            {
                return;
            }

            if (ev.Player.GetCustomRole().Id != 2)
            {
                return;
            }

            ev.Player.EnableEffect(EffectType.MovementBoost);
            ev.Player.ChangeEffectIntensity(EffectType.MovementBoost, 80, 10);
        }
    }
}
