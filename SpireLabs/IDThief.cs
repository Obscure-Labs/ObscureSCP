using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using Exiled.CustomItems.API.Features;
using Exiled.API.Features.Spawn;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Doors;
using System;
using PlayerRoles.FirstPersonControl;
using HarmonyLib;
using System.Reflection.Emit;
using NorthwoodLib.Pools;
using Mirror;
using Exiled.API.Features.Roles;
using static UnityEngine.GraphicsBuffer;
using InventorySystem.Items.ThrowableProjectiles;
using Exiled.API.Features.Items;
using Hazards;
using Exiled.API.Features.Hazards;
using Utf8Json.Resolvers.Internal;
using Exiled.API.Features.Toys;
using InventorySystem.Items.Usables.Scp330;
using Exiled.CustomRoles.API;
using Exiled.CustomRoles.API.Features;
using System.Diagnostics.Eventing.Reader;
using Exiled.API.Extensions;
using JetBrains.Annotations;
using UCRAPI = UncomplicatedCustomRoles.API.Features.Manager;
using SpireSCP.GUI.API.Features;

namespace SpireLabs
{
    internal static class IDThief
    {
        private static IEnumerator<float> wait()
        {
            yield return Timing.WaitForSeconds(1);
        }
        internal static void Player_Spawned(SpawnedEventArgs ev)
        {
            CoroutineHandle pp = Timing.RunCoroutine(wait());
          
        }


        public static bool Disguised = false;
        internal static void item_change(ChangedItemEventArgs ev)
        {
            int? customRoleID = -1;


            if (ev.Item == null)
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
;           Log.Info(customRoleID);
            if (customRoleID == -1)
            {
                return;
            }
            else if (customRoleID == 1)
            {
                var ToRole = ev.Player.Role.Type;
                bool IsCard = false;
                bool IsGun = false;




                if (ev.Item.Type == ItemType.KeycardScientist || ev.Item.Type == ItemType.KeycardContainmentEngineer || ev.Item.Type == ItemType.KeycardResearchCoordinator) // scientist
                {
                    ToRole = RoleTypeId.Scientist;
                    IsCard = true;
                }
                else if (ev.Item.Type == ItemType.KeycardGuard) // guard
                {
                    ToRole = RoleTypeId.FacilityGuard;
                    IsCard = true;

                }
                else if (ev.Item.Type == ItemType.KeycardMTFCaptain || ev.Item.Type == ItemType.KeycardMTFOperative || ev.Item.Type == ItemType.KeycardMTFPrivate) // MTF 
                {
                    ToRole = RoleTypeId.NtfSergeant;
                    IsCard = true;

                }
                else if (ev.Item.Type == ItemType.KeycardChaosInsurgency) // CHAOS
                {
                    ToRole = RoleTypeId.ChaosRifleman;
                    IsCard = true;

                }
                else if (ev.Item.Type == ItemType.GunShotgun || ev.Item.Type == ItemType.GunRevolver || ev.Item.Type == ItemType.GunAK || ev.Item.Type == ItemType.GunA7 || ev.Item.Type == ItemType.GunCOM15 || ev.Item.Type == ItemType.GunCOM18 || ev.Item.Type == ItemType.GunCom45 || ev.Item.Type == ItemType.GunCrossvec || ev.Item.Type == ItemType.GunE11SR || ev.Item.Type == ItemType.GunFRMG0 || ev.Item.Type == ItemType.GunFSP9 || ev.Item.Type == ItemType.GunLogicer || ev.Item.Type == ItemType.Jailbird || ev.Item.Type == ItemType.MicroHID)
                {
                    Log.Info("Gun");
                    IsCard = false;
                    IsGun = true;

                }
                else
                {

                    IsCard = false;
                    IsGun = false;
                }

                var Team = RoleExtensions.GetTeam(ToRole);
                var TeamColor = "white";
                if (Team == Team.FoundationForces)
                {
                    TeamColor = "blue";
                }
                else if (Team == Team.ChaosInsurgency)
                {
                    TeamColor = "green";
                }
                else if (Team == Team.Scientists)
                {
                    TeamColor = "yellow";
                }

                if (Disguised)
                {
                    if (IsGun)
                    {
                        ev.Player.ChangeAppearance(ToRole, true);
                        //ev.Player.ShowHint($"<Color=Red>You are no longer Disguised!</color>");
                        Manager.SendHint(ev.Player, $"<Color=Red>You are no longer Disguised!</color>", 3);
                        Disguised = false;
                    }
                    else if (IsCard)
                    {
                        ev.Player.ChangeAppearance(ToRole, true);
                        //ev.Player.ShowHint($"You are now disguised as: <Color={TeamColor}>{ToRole.GetFullName()}</color>! \n<color=red>If you take out a weapon your cover will be blown!</color>");
                        Manager.SendHint(ev.Player, $"You are now disguised as: <Color={TeamColor}>{ToRole.GetFullName()}</color>! \n<color=red>If you take out a weapon your cover will be blown!</color>", 3);
                        Disguised = true;
                    }
                    else if (!IsGun | !IsCard)
                    {
                        Disguised = true;
                    }
                }
                else if (!Disguised)
                {
                    if (!IsCard)
                    {
                        Disguised = false;
                    }
                    else if (IsCard)
                    {
                        ev.Player.ChangeAppearance(ToRole, true);
                        //ev.Player.ShowHint($"You are now disguised as: <Color={TeamColor}>{ToRole.GetFullName()}</color>! \n<color=red>If you take out a weapon your cover will be blown!</color>");
                        Manager.SendHint(ev.Player, $"You are now disguised as: <Color={TeamColor}>{ToRole.GetFullName()}</color>! \n<color=red>If you take out a weapon your cover will be blown!</color>", 3);
                        Disguised = true;
                    }
                }

            }


        }

    }
}