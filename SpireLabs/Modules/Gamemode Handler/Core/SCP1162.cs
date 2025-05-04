using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using ObscureLabs.API.Features;
using PlayerRoles;
using SpireSCP.GUI.API.Features;
using UnityEngine;
using static InventorySystem.Items.Firearms.ShotEvents.ShotEventManager;
namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    internal class Scp1162 : Module
    {
        public override string Name => "Scp1162";
        public override bool IsInitializeOnStart => true;

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

        private CoroutineHandle _playerPositionCoroutine;

        private const string _hintText = "You are currently in a room occupied by SCP1162! \nDrop an item on the ground to have it transformed into another\nat the cost of 10HP...";
        private const string _hintTextUsedScp1162 = "You just used SCP1162 to transform one item into another\nThis has costed you 10HP";

        public override bool Enable()
        {
            Exiled.Events.Handlers.Player.DroppedItem += OnDroppedItem;
            _playerPositionCoroutine = Timing.RunCoroutine(CheckPlayerPosition());
            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Player.DroppedItem -= OnDroppedItem;
            //Timing.KillCoroutines(playerposroutine);

            return base.Disable();
        }

        public void OnDroppedItem(DroppedItemEventArgs ev)
        {
            var pickup = ev.Pickup;
            var player = ev.Player;
            var customItemChance = UnityEngine.Random.Range(0, 101);
            var isCustomItem = customItemChance >= 50 && customItemChance <= 55;

            var isIn1162 = Vector3.Distance(ev.Pickup.Position, RoleTypeId.Scp173.GetRandomSpawnLocation().Position) <= 8.2f;

            if (isCustomItem && isIn1162)
            {
                player.Hurt(10);
                pickup.Destroy();
                _customitemlist.RandomItem().Spawn(pickup.Position);
                Manager.SendHint(player, _hintTextUsedScp1162, 2f);
            }
            else if (!isCustomItem && isIn1162)
            {
                Manager.SendHint(player, _hintTextUsedScp1162, 2f);

                player.Hurt(10);
                pickup.Destroy();

                var randomItemType = Enum.GetValues(typeof(ItemType)).ToArray<ItemType>().GetRandomValue();

                Pickup.CreateAndSpawn(randomItemType, pickup.Position, pickup.Rotation);

            }
        }

        private IEnumerator<float> CheckPlayerPosition()
        {
            while (!Round.IsEnded)
            {
                yield return Timing.WaitForSeconds(1f);
                foreach (var player in Player.List)
                {
                    var player_in1162 = Vector3.Distance(player.Transform.position, RoleTypeId.Scp173.GetRandomSpawnLocation().Position) <= 8.2f;

                    if (player_in1162)
                    {
                        Manager.SendHint(player, _hintText, 2f);
                    }
                }
            }
        }
    }
}
