using Achievements.Handlers;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.CustomItems.API.Features;
using InventorySystem;
using InventorySystem.Items;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;
using MEC;
using Mirror;
using ObscureLabs.API.Features;
using ObscureLabs.Items;
using ObscureLabs.Modules.Default;
using ObscureLabs.Modules.Gamemode_Handler.Core;
using ObscureLabs.Modules.Gamemode_Handler.Core.SCP_Rebalances;
using ObscureLabs.Modules.Gamemode_Handler.Mode_Specific_Modules;
using ObscureLabs.SpawnSystem;
using PlayerRoles;
using SpireLabs.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Pickup = Exiled.API.Features.Pickups.Pickup;
using Player = Exiled.API.Features.Player;
using Room = Exiled.API.Features.Room;
using Server = Exiled.API.Features.Server;

namespace ObscureLabs.Modules.Gamemode_Handler.Modes
{
    internal class Insanity : Gamemode
    {
        /// <summary>
        /// teams are scientists and dclass. all items are custom. randomise ammo brick type but dont replace with items.
        /// </summary>


        private List<CustomItem> _customitemlist = new()
        {
            CustomItem.Get((uint)1), // Sniper
            CustomItem.Get((uint)5), // Lasergun
            CustomItem.Get((uint)6), // Particle Collapser
            CustomItem.Get((uint)13), // Super Capybara
            CustomItem.Get((uint)14),  // MediGun
            CustomItem.Get((uint)2), // ClusterHE
            CustomItem.Get((uint)4), // NovaGrenade
            CustomItem.Get((uint)12), // S-NAV
            CustomItem.Get((uint)0), // Essential Oils


        };


 

        public override string Name => "Insanity Mode";
        public override List<Module> InitModules => new List<Module>
        {
            //- Core Utils -//
            new HudController(),
            new MvpSystem(),
            new RemoteKeycard(),
            new LightHandler(),
            new Lobby(),
            new HealthOverride(),

            //- Gameplay Utils -//
            new Powerup(),


            //- Mechanics and Features -//
            new CoinFlip(),
            new AttachmentFix(),
            new SCPsDropItems(),

            //- SCP Additions and rebalances -//
            new Scp1162(),
            new SCP106(),
            new SCP173(),
            new Scp049(),


            //- Fun modules -//
            new Scp914Handler(),
            new RoundEndPVP(),
            new EmotionRandomiser(),
        };
        public override List<Module> StartModules => new List<Module>
        {
            new ItemGlow(),
            // Add modules that should be started when the round starts
        };
        public override bool PreInitialise()
        {


            return base.PreInitialise();
        }
        public override bool Start() // this runs on round start
        {

            Timing.RunCoroutine(ItemPlacer());
            Timing.RunCoroutine(ItemReplacer());
            Timing.RunCoroutine(TeamAssignment());
            Server.FriendlyFire = true;
            return base.Start();
        }
        public override bool Stop()
        {
            Server.FriendlyFire = false;
            return base.Stop();
        }


        public IEnumerator<float> ItemPlacer()
        {
            foreach (Room r in Room.List)
            {
                yield return Timing.WaitForOneFrame;
                if (UnityEngine.Random.Range(0, 100) <= 10)
                {

                    _customitemlist.RandomItem().Spawn(r.transform.position);

                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Pickup.CreateAndSpawn(InventoryItemLoader.AvailableItems.Values.GetRandomValue().ItemTypeId, r.Transform.position + (Vector3.one * UnityEngine.Random.Range(-2f, 2f)), Quaternion.identity);
                    }
                }
            }
        }
        public IEnumerator<float> ItemReplacer()
        {
            List<Pickup> plist = new List<Pickup>();
            plist.AddRange(Pickup.List);
            foreach (Pickup p in plist)
            {
                yield return Timing.WaitForOneFrame;
                if (p == null) { continue; }
                if (p.Type == ItemType.Coin || p.Category == ItemCategory.Ammo) { continue; }


                if (UnityEngine.Random.Range(0, 100) <= 10)
                {

                    _customitemlist.RandomItem().Spawn(p.Position);

                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        yield return Timing.WaitForOneFrame;
                        Pickup.CreateAndSpawn(InventoryItemLoader.AvailableItems.Values.GetRandomValue().ItemTypeId, p.Transform.position, p.Transform.rotation);
                    }
                }
                p.Destroy();

            }
        }
        public IEnumerator<float> TeamAssignment()
        {
            yield return Timing.WaitForOneFrame;
            List<Player> pList = new List<Player>();
            foreach (Player p in Player.List)
            {

                if (p.AuthenticationType != Exiled.API.Enums.AuthenticationType.DedicatedServer)
                {
                    pList.Add(p);
                }
            }
            pList.ShuffleList();

            for (int i = 0; i < pList.Count; i++)
            {

                Player p = pList[i];
                if (i % 2 == 0)
                {
                    p.RoleManager.ServerSetRole(PlayerRoles.RoleTypeId.ClassD, PlayerRoles.RoleChangeReason.RoundStart, PlayerRoles.RoleSpawnFlags.UseSpawnpoint);
                }
                else
                {
                    p.RoleManager.ServerSetRole(PlayerRoles.RoleTypeId.Scientist, PlayerRoles.RoleChangeReason.RoundStart, PlayerRoles.RoleSpawnFlags.UseSpawnpoint);
                }
                p.Inventory.ServerAddItem(ItemType.Coin, InventorySystem.Items.ItemAddReason.StartingItem);
                p.Inventory.ServerAddItem(ItemType.KeycardZoneManager, InventorySystem.Items.ItemAddReason.StartingItem);
                p.Inventory.ServerAddItem(ItemType.ArmorCombat, InventorySystem.Items.ItemAddReason.StartingItem);
                p.Inventory.ServerAddAmmo(ItemType.Ammo12gauge, 999);
                p.Inventory.ServerAddAmmo(ItemType.Ammo44cal, 999);
                p.Inventory.ServerAddAmmo(ItemType.Ammo556x45, 999);
                p.Inventory.ServerAddAmmo(ItemType.Ammo762x39, 999);
                p.Inventory.ServerAddAmmo(ItemType.Ammo9x19, 999);
                p.EnableEffect(EffectType.DamageReduction, 10f);
                p.ChangeEffectIntensity(EffectType.DamageReduction, 255, 10f);
                p.Teleport(RoleTypeId.ClassD.GetRandomSpawnLocation().Position);
            }
        }
    }
}
