using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using PlayerRoles;
using SpireSCP.GUI.API.Features;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CustomItem = Exiled.CustomItems.API.Features.CustomItem;


namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    internal class Scp1162 : Plugin.Module
    {
        public override string name { get; set; } = "Scp1162";
        public override bool initOnStart { get; set; } = true;


        static string hintText = $"You are currently in a room occupied by SCP1162! \nDrop an item on the ground to have it transformed into another\nat the cost of 10HP...";
        static string hintTextUsedScp1162 = $"You just used Scp1162 to transform an item into another at the cost of 10HP...";

        public override bool Init()
        {
            try
            {
                
                Exiled.Events.Handlers.Player.DroppedItem += droppedItem;
                playerposroutine = Timing.RunCoroutine(checkPlayerPos());
                base.Init();
                return true;
            }
            catch { return false; }
        }

        public override bool Disable()
        {
            try
            {
                Exiled.Events.Handlers.Player.DroppedItem -= droppedItem;
                Timing.KillCoroutines(playerposroutine);
                base.Disable();
                return true;
            }
            catch { return false; }
        }

        public CoroutineHandle playerposroutine { get; set; }


        private static IEnumerator<float> checkPlayerPos()
        {

            bool player_in1162 = false;
            while (true)
            {
                yield return Timing.WaitForSeconds(1f);
                foreach (Player p in Plugin.PlayerList)
                {
                    player_in1162 = false;
                    if (Vector3.Distance(p.Transform.position, RoleTypeId.Scp173.GetRandomSpawnLocation().Position) <= 8.2f) { player_in1162 = true; }
                    if (player_in1162) { Manager.SendHint(p, hintText, 2f); }
                    else { continue; }
                }
            }
        }


        List<ItemType> itemList = new List<ItemType> {
                ItemType.GunA7,
                ItemType.GunFSP9,
                ItemType.GunCom45,
                ItemType.GunE11SR,
                ItemType.GunRevolver,
                ItemType.GunLogicer,
                ItemType.GunFRMG0,
                ItemType.GunAK,
                ItemType.Adrenaline,
                ItemType.Painkillers,
                ItemType.ArmorLight,
                ItemType.ArmorHeavy,
                ItemType.ArmorCombat,
                ItemType.Medkit,
                ItemType.MicroHID,
                ItemType.ParticleDisruptor,
                ItemType.SCP500,
                ItemType.Radio,
                ItemType.Jailbird,
                ItemType.GunShotgun,
                ItemType.KeycardZoneManager,
                ItemType.KeycardScientist,
                ItemType.KeycardResearchCoordinator,
                ItemType.KeycardGuard,
                ItemType.KeycardJanitor,
                ItemType.KeycardContainmentEngineer,
                ItemType.KeycardFacilityManager,
                ItemType.GunCrossvec,
                ItemType.KeycardMTFCaptain,
                ItemType.KeycardMTFOperative,
                ItemType.KeycardMTFPrivate,
                ItemType.KeycardChaosInsurgency,
                ItemType.KeycardO5,
                ItemType.Flashlight,
                ItemType.SCP1576,
                ItemType.SCP244b,
                ItemType.SCP244a,
                ItemType.SCP207,
                ItemType.Coin,
                ItemType.Ammo12gauge,
                ItemType.Ammo44cal,
                ItemType.Ammo556x45,
                ItemType.Ammo762x39,
                ItemType.Ammo9x19,
                ItemType.None,
            };



        List<CustomItem> customitemlist = new List<CustomItem>
        {
            CustomItem.Get((uint)2), // ClusterHE
            CustomItem.Get((uint)7), // ClusterFlash
            CustomItem.Get((uint)0), // EssentialOils
            CustomItem.Get((uint)4), // NovaGrenade
            CustomItem.Get((uint)1), // sniper
            CustomItem.Get((uint)3), // grenade launcher
            CustomItem.Get((uint)5), // ER16
            CustomItem.Get((uint)6), // Particle Collapser
        };


        public void droppedItem(DroppedItemEventArgs ev)
        {
            var rnd = new System.Random();
            var drop = ev.Pickup;
            var pl = ev.Player;
            bool isIn1162 = false;
            int customItemChance = rnd.Next(0, 101);
            bool isCustomItem = false;


            CustomItem customItemPickup;
            ItemType itemPickup;

            if (Vector3.Distance(ev.Pickup.Position, RoleTypeId.Scp173.GetRandomSpawnLocation().Position) <= 8.2f) { isIn1162 = true; } else { isIn1162 = false; }
            if (customItemChance >= 50 && customItemChance <= 55) { isCustomItem = true; }


            if (isCustomItem && isIn1162)
            {
                pl.Hurt(10);
                drop.Destroy();
                customitemlist.RandomItem().Spawn(drop.Position);
                Manager.SendHint(pl, hintTextUsedScp1162, 5f);

            }
            else if (!isCustomItem && isIn1162)
            {
                pl.Hurt(10);
                drop.Destroy();
                Pickup.CreateAndSpawn(itemList.RandomItem(), drop.Position, drop.Rotation);
            }


        }


    }
}
