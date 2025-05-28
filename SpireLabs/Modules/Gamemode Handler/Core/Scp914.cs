﻿using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Scp914;
using MEC;
using ObscureLabs.API.Data;
using ObscureLabs.API.Features;
using SpireSCP.GUI.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    public class Scp914Handler : Module
    {

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

        public override string Name => "Scp914Handler";

        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {
            Exiled.Events.Handlers.Scp914.UpgradingInventoryItem += InvItemThrough914;
            Exiled.Events.Handlers.Scp914.UpgradingPickup += ItemThrough914;
            Exiled.Events.Handlers.Scp914.UpgradingPlayer += PlayerThrough914;
            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Scp914.UpgradingInventoryItem -= InvItemThrough914;
            Exiled.Events.Handlers.Scp914.UpgradingPickup -= ItemThrough914;
            Exiled.Events.Handlers.Scp914.UpgradingPlayer -= PlayerThrough914;
            return base.Disable();
        }


        public void InvItemThrough914(UpgradingInventoryItemEventArgs ev)
        {

            var rng = UnityEngine.Random.Range(0, 101);
            if (ev.KnobSetting == Scp914.Scp914KnobSetting.VeryFine && ev.Item.Type == ItemType.KeycardO5)
            {
                ev.IsAllowed = false;
                if (rng <= 20f)
                {
                    ev.Item.Destroy();
                    CustomItem.TryGive(ev.Player, _customitemlist.RandomItem().Name);

                }

            }
        }
        public void ItemThrough914(UpgradingPickupEventArgs ev)
        {


            if (ev.KnobSetting == Scp914.Scp914KnobSetting.VeryFine && ev.Pickup.Type == ItemType.KeycardO5)
            {
                ev.IsAllowed = false;
                var rng = UnityEngine.Random.Range(0, 101);
                if (rng <= 20f)
                {
                    ev.Pickup.Destroy();
                    CustomItem.TrySpawn(_customitemlist.RandomItem().Id, ev.OutputPosition, out Pickup p);

                }
                else if (rng <= 30 && rng >= 20)
                {
                    ev.Pickup.Transform.position = ev.OutputPosition;
                    ev.Pickup.Clone().Transform.position = ev.OutputPosition;
                }
            }
        }

        public void PlayerThrough914(UpgradingPlayerEventArgs ev)
        {
            Timing.RunCoroutine(randomTP(ev));
        }

        private IEnumerator<float> randomTP(UpgradingPlayerEventArgs ev)
        {
            var p = ev.Player;
            var setting = ev.KnobSetting;
            var rng = UnityEngine.Random.Range(0, 101);

            if (setting == Scp914.Scp914KnobSetting.Rough)
            {
                p.EnableEffect(EffectType.Blinded, 2, false);
                if (rng > 20) { Manager.SendHint(p, $"You were {rng - 20} points away from the result you were looking for...", 5f); yield break; }
                var goodRoom = false;
                //what the magic number
                var room = Room.List.ElementAt(4);
                var door = Room.List.ElementAt(4).Doors.FirstOrDefault();
                while (!goodRoom)
                {
                    var roomNd = new System.Random();
                    var roomNum = roomNd.Next(0, Room.List.Count());
                    var room2 = Room.List.ElementAt(roomNum);
                    if (Map.IsLczDecontaminated)
                    {
                        if (room2.Type is not RoomType.HczTesla or RoomType.HczElevatorA or RoomType.HczElevatorB && Room.List.ElementAt(roomNum).Zone is not ZoneType.LightContainment)
                        {
                            goodRoom = true;
                            door = Room.List.ElementAt(roomNum).Doors.FirstOrDefault();
                        }
                    }
                    else
                    {
                        if (room2.Type != RoomType.HczTesla)
                        {
                            goodRoom = true;
                            door = Room.List.ElementAt(roomNum).Doors.FirstOrDefault();
                        }
                    }
                }
                Manager.SendHint(p, $"You were placed in a random room by Scp914", 5);
                yield return Timing.WaitForSeconds(0.05f);
                ev.Player.Teleport(new Vector3(door.Position.x, door.Position.y + 1.5f, door.Position.z));
            }
        }
    }
}