using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using ObscureLabs.API.Features;
using PlayerRoles;
using SpireSCP.GUI.API.Features;
using System.Collections.Generic;
using UCRAPI = UncomplicatedCustomRoles.API.Features.Manager;

namespace ObscureLabs
{
    public class IDThief : Module
    {
        public static bool Disguised = false;

        public override string Name => "IDThief";

        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {
            Exiled.Events.Handlers.Player.ChangedItem += OnItemChanged;
            Exiled.Events.Handlers.Player.Spawned += PlayerSpawned;
            base.Enable();
            return true;
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Player.ChangedItem -= OnItemChanged;
            Exiled.Events.Handlers.Player.Spawned -= PlayerSpawned;
            base.Disable();
            return true;
        }

        private static void PlayerSpawned(SpawnedEventArgs ev)
        {
            //:skull:
            CoroutineHandle pp = Timing.RunCoroutine(WaitCoroutine());
        }

        private static void OnItemChanged(ChangedItemEventArgs ev)
        {
            var customRoleID = -1;

            if (ev.Item is null)
            {
                return;
            }

            if (UncomplicatedCustomRoles.API.Features.Manager.HasCustomRole(ev.Player))
            {
                customRoleID = UCRAPI.Get(ev.Player).Id;
            }
            else
            {
                return;
            }

            Log.Debug($"{ev.Player.DisplayNickname} is custom role ID: {customRoleID}");

            if (customRoleID == -1)
            {
                return;
            }
            else if (customRoleID == 1)
            {
                var ToRole = ev.Player.Role.Type;
                var IsKeyCard = ev.Item.Type.IsKeycard();
                var IsWeapon = ev.Item.Type.IsWeapon(true);

                if (ev.Item.Type == ItemType.KeycardScientist || ev.Item.Type == ItemType.KeycardContainmentEngineer || ev.Item.Type == ItemType.KeycardResearchCoordinator) // scientist
                {
                    ToRole = RoleTypeId.Scientist;
                }
                else if (ev.Item.Type == ItemType.KeycardGuard) // guard
                {
                    ToRole = RoleTypeId.FacilityGuard;
                }
                else if (ev.Item.Type == ItemType.KeycardMTFCaptain || ev.Item.Type == ItemType.KeycardMTFOperative || ev.Item.Type == ItemType.KeycardMTFPrivate) // MTF 
                {
                    ToRole = RoleTypeId.NtfSergeant;
                }
                else if (ev.Item.Type == ItemType.KeycardChaosInsurgency) // CHAOS
                {
                    ToRole = RoleTypeId.ChaosRifleman;

                }

                var team = RoleExtensions.GetTeam(ToRole);
                var teamColor = "white";

                if (team is Team.FoundationForces)
                {
                    teamColor = "blue";
                }
                else if (team is Team.ChaosInsurgency)
                {
                    teamColor = "green";
                }
                else if (team is Team.Scientists)
                {
                    teamColor = "yellow";
                }

                if (Disguised)
                {
                    if (IsWeapon)
                    {
                        ev.Player.ChangeAppearance(ToRole, true);
                        //ev.Player.ShowHint($"<Color=Red>You are no longer Disguised!</color>");
                        Manager.SendHint(ev.Player, $"<Color=Red>You are no longer Disguised!</color>", 3);
                        Disguised = false;
                    }
                    else if (IsKeyCard)
                    {
                        ev.Player.ChangeAppearance(ToRole, true);
                        //ev.Player.ShowHint($"You are now disguised as: <Color={TeamColor}>{ToRole.GetFullName()}</color>! \n<color=red>If you take out a weapon your cover will be blown!</color>");
                        Manager.SendHint(ev.Player, $"You are now disguised as: <Color={teamColor}>{ToRole.GetFullName()}</color>! \n<color=red>If you take out a weapon your cover will be blown!</color>", 3);
                        Disguised = true;
                    }
                    else if (!IsWeapon | !IsKeyCard)
                    {
                        Disguised = true;
                    }
                }
                else if (!Disguised)
                {
                    if (!IsKeyCard)
                    {
                        Disguised = false;
                    }
                    else if (IsKeyCard)
                    {
                        ev.Player.ChangeAppearance(ToRole, true);
                        //ev.Player.ShowHint($"You are now disguised as: <Color={TeamColor}>{ToRole.GetFullName()}</color>! \n<color=red>If you take out a weapon your cover will be blown!</color>");
                        Manager.SendHint(ev.Player, $"You are now disguised as: <Color={teamColor}>{ToRole.GetFullName()}</color>! \n<color=red>If you take out a weapon your cover will be blown!</color>", 3);
                        Disguised = true;
                    }
                }
            }
        }

        private static IEnumerator<float> WaitCoroutine()
        {
            yield return Timing.WaitForSeconds(1);
        }
    }
}