using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using ObscureLabs.API.Features;
using PlayerRoles;
using SpireSCP.GUI.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ObscureLabs
{
    internal class CoinFlip : Module
    {
        public override string Name => "Coinflip";

        public override bool IsInitializeOnStart => true;

        private readonly string[] _good = { "You gained 50HP!", "You gained a 5 second speed boost!", "You found a keycard!", "You are invisible for 5 seconds!", "You are healed!", "GRENADE FOUNTAIN!", "Ammo pile!!", "FREE CANDY!", "You can't die for the next 25s!", "You bring health to those around you!", "Nice hat..", "You have such radiant skin!", "You got an item!", "Brought a random player to you!" };
        private readonly string[] _bad = { "You now have 50HP!", "You dropped all of your items, How clumsy...", "You have heavy feet for 5 seconds...", "Pocket Sand!", "You got lost and found yourself in a random room!", "Don't gamble kids!", "You hear the sound of an alarm!", "Portal to hell!!!", "Others percieve you as upside down!", "You caused a blackout in your zone!", "Door stuck! DOOR STUCK!", "Your coin melted :(", "You have been detained!", "You have been brought to a random player!", "The facility is having some technical difficulties" };

        private Dictionary<int, CandyKindID> _candies = new(6)
        {
            { 0, CandyKindID.Red },
            { 1, CandyKindID.Blue },
            { 2, CandyKindID.Yellow },
            { 2, CandyKindID.Purple },
            { 2, CandyKindID.Rainbow },
            { 2, CandyKindID.Pink },

        };

        public override bool Enable()
        {
            Exiled.Events.Handlers.Player.FlippingCoin += Player_FlippingCoin;
            Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;

            return base.Enable();
        }
        public override bool Disable()
        {
            Exiled.Events.Handlers.Player.FlippingCoin -= Player_FlippingCoin;
            Exiled.Events.Handlers.Player.ChangedItem -= OnChangedItem;

            return base.Disable();
        }

        private void OnChangedItem(ChangedItemEventArgs ev)
        {
            if (ev.Item is null)
            {
                return;
            }

            if (ev.Item.Type != ItemType.Coin)
            {
                return;
            }

            Manager.SendHint(ev.Player, "Flipping this coin will cause a random event, use with caution!", 5);
        }

        private void Player_FlippingCoin(FlippingCoinEventArgs ev)
        {
            var itemList = Enum.GetValues(typeof(ItemType)).ToArray<ItemType>();
        reroll:
            var num = Random.Range(0, 100);
            var result = 0;

            if (num > 20 && num < 45)
            {
                result = 1;
            }

            if (num > 45 && num < 100)
            {
                result = 2;
            }

            if (result == 1)
            {
                Log.Debug($"{ev.Player.Nickname} flipped a coin and got a good result!");
                switch (Random.Range(0, _good.Count()))
                {
                    case 0:
                        //ev.Player.ShowHint(good[0], 3);
                        Manager.SendHint(ev.Player, _good[0], 3);
                        ev.Player.Heal(50, true);
                        if (ev.Player.Role == RoleTypeId.NtfCaptain)
                        {
                            ev.Player.MaxHealth = 150;
                        }
                        else
                        {
                            ev.Player.MaxHealth = 100;
                        }
                        break;
                    case 1:
                        Manager.SendHint(ev.Player, _good[1], 3);
                        ev.Player.EnableEffect(EffectType.MovementBoost, 5);
                        ev.Player.ChangeEffectIntensity(EffectType.MovementBoost, 205, 5);
                        break;
                    case 2:
                        bool isDrop = false;
                        Manager.SendHint(ev.Player, _good[2], 3);
                        if (ev.Player.IsInventoryFull)
                        {
                            isDrop = true;

                        }
                        else
                        {
                            isDrop = false;
                        }

                        int card = Random.Range(1, 3);
                        if (card == 1)
                        {
                            if (isDrop)
                            {
                                Pickup.CreateAndSpawn(ItemType.KeycardZoneManager, ev.Player.Position, ev.Player.Rotation);
                            }
                            else
                            {
                                ev.Player.AddItem(ItemType.KeycardZoneManager);
                            }
                        }
                        else if (card == 2)
                        {
                            if (isDrop)
                            {
                                Pickup.CreateAndSpawn(ItemType.KeycardMTFOperative, ev.Player.Position, ev.Player.Rotation);
                            }
                            else
                            {
                                ev.Player.AddItem(ItemType.KeycardMTFOperative);
                            }
                        }
                        else
                        {
                            if (isDrop)
                            {
                                Pickup.CreateAndSpawn(ItemType.KeycardResearchCoordinator, ev.Player.Position, ev.Player.Rotation);
                            }
                            else
                            {
                                ev.Player.AddItem(ItemType.KeycardResearchCoordinator);
                            }
                        }
                        break;
                    case 3:
                        Manager.SendHint(ev.Player, _good[3], 3);
                        ev.Player.EnableEffect(EffectType.Invisible, 5);
                        ev.Player.ChangeEffectIntensity(EffectType.Invisible, 1, 5);
                        break;
                    case 4:
                        ev.Player.Heal(150, false);
                        Manager.SendHint(ev.Player, _good[4], 3);
                        break;
                    case 5:
                        Manager.SendHint(ev.Player, _good[5], 3);
                        Timing.RunCoroutine(GrenadeFountainCoroutine(ev.Player));
                        break;
                    case 6:
                        Manager.SendHint(ev.Player, _good[6], 3);
                        Timing.RunCoroutine(AmmoFountainCoroutine(ev.Player));
                        break;
                    case 7:
                        Manager.SendHint(ev.Player, _good[7], 3);
                        Timing.RunCoroutine(CandyFountainCoroutine(ev.Player));
                        break;
                    case 8:
                        Manager.SendHint(ev.Player, _good[8], 3);
                        ev.Player.EnableEffect(EffectType.DamageReduction, 25);
                        ev.Player.ChangeEffectIntensity(EffectType.DamageReduction, 255, 25);
                        ev.Player.GetEffect(EffectType.DamageReduction).Duration.ToString();
                        break;
                    case 9:
                        Manager.SendHint(ev.Player, _good[9], 1.5f);
                        Timing.RunCoroutine(RaycastHealCoroutine(ev.Player));
                        break;
                    case 10:
                        Manager.SendHint(ev.Player, _good[10], 3);
                        var tp = false;

                        if (ev.Player.IsInventoryFull)
                        {
                            tp = true;
                        }
                        else
                        {
                            tp = false;
                        }

                        if (tp)
                        {
                            Pickup.CreateAndSpawn(ItemType.SCP268, ev.Player.Position, ev.Player.Rotation);
                        }
                        else
                        {
                            ev.Player.AddItem(ItemType.SCP268);
                        }
                        break;
                    case 11:
                        Manager.SendHint(ev.Player, _good[11], 3);
                        Timing.RunCoroutine(GlowCoroutine(ev.Player));
                        break;
                    case 12:
                        Manager.SendHint(ev.Player, _good[12], 3);
                        var randomitem = new System.Random();
                        Pickup.CreateAndSpawn(itemList.ElementAt(randomitem.Next(0, itemList.Count() + 1)), ev.Player.Position, ev.Player.Rotation);
                        break;
                    case 13:
                        var target = Player.List.ElementAt(Random.Range(0, Player.List.Count));
                        Log.Info(target.Nickname);
                        if (Player.List.Count == 1)
                        {
                            ev.Player.Vaporize(ev.Player);
                            Manager.SendHint(ev.Player, "LOL", 5);
                            break;
                        }
                        else
                        {
                            if (!target.IsAlive || target.Role.Type is RoleTypeId.Overwatch or RoleTypeId.Spectator || target == ev.Player)
                            {
                                goto reroll;
                            }
                            else
                            {
                                target.Position = ev.Player.Position;
                                Manager.SendHint(ev.Player, _good[13], 3);
                            }
                        }

                        break;
                }
            }
            else if (result == 2)
            {
                Log.Debug($"{ev.Player.Nickname} flipped a coin and got a bad result!");
                switch (Random.Range(0, _bad.Count()))
                {
                    case 0:
                        Manager.SendHint(ev.Player, _bad[0], 3);
                        ev.Player.Health = 50;
                        break;
                    case 1:
                        Manager.SendHint(ev.Player, _bad[1], 3);
                        ev.Player.DropItems();
                        break;
                    case 2:
                        Manager.SendHint(ev.Player, _bad[2], 3);
                        ev.Player.EnableEffect(EffectType.SinkHole, 5, true);
                        ev.Player.ChangeEffectIntensity(EffectType.SinkHole, 1, 5);
                        break;
                    case 3:
                        Manager.SendHint(ev.Player, _bad[3], 3);
                        ev.Player.EnableEffect(EffectType.Flashed, 5, true);
                        ev.Player.ChangeEffectIntensity(EffectType.Flashed, 1, 5);
                        break;
                    case 4:
                        if (Warhead.IsDetonated)
                        {
                            break;
                        }

                        Manager.SendHint(ev.Player, _bad[4], 3);
                        var number = Random.Range(0, 2);
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

                        ev.Player.Teleport(new Vector3(door.Position.x, door.Position.y + 1.5f, door.Position.z));
                        break;
                    case 5:
                        var randomdeathpick = Random.Range(0, 4);

                        if (randomdeathpick == 0)
                        {
                            ev.Player.EnableEffect(EffectType.CardiacArrest, 999);
                            ev.Player.ChangeEffectIntensity(EffectType.CardiacArrest, 25);
                            ev.Player.EnableEffect(EffectType.SeveredHands, 999);
                            Manager.SendHint(ev.Player, "Look ma' no hands!", 3);
                        }

                        if (randomdeathpick == 1)
                        {
                            ev.Player.Vaporize(ev.Player);
                            Manager.SendHint(ev.Player, "Voop", 3);
                        }
                        else if (randomdeathpick == 2)
                        {
                            ev.Player.Explode();
                            Manager.SendHint(ev.Player, "Bang!", 3);
                        }
                        else if (randomdeathpick == 3)
                        {
                            Timing.RunCoroutine(ShitOnTheFloorCoroutine(ev.Player));
                            Manager.SendHint(ev.Player, "Shidded", 3);
                        }
                        break;
                    case 6:
                        Timing.RunCoroutine(BeepCoroutine(ev.Player));
                        Manager.SendHint(ev.Player, _bad[6], 3);
                        break;
                    case 7:
                        Manager.SendHint(ev.Player, _bad[7], 3);
                        var zt = ev.Player.CurrentRoom.Zone;
                        ev.Player.Teleport(Room.List.FirstOrDefault(x => x.Type == RoomType.Pocket));
                        ev.Player.EnableEffect(EffectType.PocketCorroding, 999);
                        ev.Player.EnableEffect(EffectType.Corroding, 999);
                        Timing.RunCoroutine(EnterPDCoroutine(ev.Player, zt));
                        break;
                    case 8:
                        Manager.SendHint(ev.Player, _bad[8], 3);

                        ev.Player.Scale = Vector3.one * -1;
                        Timing.RunCoroutine(SetScaleCoroutine(ev.Player));
                        break;
                    case 9:
                        Manager.SendHint(ev.Player, _bad[9], 3);
                        var zone = ev.Player.CurrentRoom.Zone;
                        Map.TurnOffAllLights(15f, zone);
                        break;
                    case 10:
                        Manager.SendHint(ev.Player, _bad[10], 3);
                        foreach (var roomSel in Room.List)
                        {
                            if (roomSel.Zone == ev.Player.Zone)
                            {
                                Timing.RunCoroutine(RoomRGBCoroutine(roomSel));
                            }
                        }
                        break;
                    case 11:
                        Manager.SendHint(ev.Player, _bad[11], 3);
                        ev.Player.RemoveHeldItem(true);
                        break;
                    case 12:
                        Manager.SendHint(ev.Player, _bad[12], 3);
                        Timing.RunCoroutine(DisarmCoroutine(ev.Player));
                        break;
                    case 13:

                        var random = new System.Random();
                        var target = Player.List.ElementAt(random.Next(Player.List.Count));
                        Log.Info(target.Nickname);
                        if (Player.List.Count == 1)
                        {
                            ev.Player.Vaporize(ev.Player);
                            Manager.SendHint(ev.Player, "LOL", 5);
                            break;
                        }
                        else
                        {
                            if (!target.IsAlive || target.Role.Type is RoleTypeId.Overwatch or RoleTypeId.Spectator || target == ev.Player)
                            {
                                goto reroll;
                            }
                            else
                            {
                                ev.Player.Position = target.Position;
                            }
                            Manager.SendHint(ev.Player, _bad[13], 3);
                        }
                        break;
                    case 14:
                        Manager.SendHint(ev.Player, _bad[14], 3);
                        foreach (Room _r in Room.List)
                        {
                            _r.TurnOffLights(15);
                            foreach (Door _d in _r.Doors)
                            {
                                _d.IsOpen = false;
                            }
                        }
                        break;
                }
            }
            else
            {
                Log.Debug($"{ev.Player.Nickname} flipped a coin and got nothing!");
                //ev.Player.ShowHint("No consequences, this time...", 3);
                Manager.SendHint(ev.Player, "No consequences, this time...", 5);
            }
        }

        private IEnumerator<float> GrenadeFountainCoroutine(Player p)
        {
            var bombs = 0;
            while (bombs != 2)
            {
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                yield return Timing.WaitForSeconds(0.1f);
                bombs++;
            }
        }

        private IEnumerator<float> AmmoFountainCoroutine(Player p)
        {
            int num = Random.Range(0, 100);

            int itemTotal;
            if (num >= 95)
            {
                itemTotal = 350;
            }
            else
            {
                itemTotal = 50;
            }

            Log.Info("Running ammo fountain on " + p);
            var items = 0;
            while (items != itemTotal)
            {
                Pickup.CreateAndSpawn(ItemType.Ammo9x19, p.Position, p.Rotation);
                Pickup.CreateAndSpawn(ItemType.Ammo762x39, p.Position, p.Rotation);
                Pickup.CreateAndSpawn(ItemType.Ammo12gauge, p.Position, p.Rotation);
                Pickup.CreateAndSpawn(ItemType.Ammo556x45, p.Position, p.Rotation);
                Pickup.CreateAndSpawn(ItemType.Ammo44cal, p.Position, p.Rotation);

                yield return Timing.WaitForOneFrame;
                items++;
            }
        }
        private IEnumerator<float> CandyFountainCoroutine(Player p)
        {
            Log.Info("Running candy fountain on " + p);
            var items = 0;
            while (items != 50)
            {
                var scp330 = Pickup.Create(ItemType.SCP330) as Exiled.API.Features.Pickups.Scp330Pickup;
                var number = Random.Range(0, 6);

                scp330.Candies.Add(_candies[number]);

                scp330.Spawn(new(p.Position.x, p.Position.y, p.Position.z), p.Rotation);

                yield return Timing.WaitForOneFrame;

                items++;
            }
        }

        private IEnumerator<float> SetScaleCoroutine(Player player)
        {
            player.Scale = -Vector3.one;
            yield return Timing.WaitForSeconds(30);
            player.Scale = Vector3.one;

        }

        private IEnumerator<float> RaycastHealCoroutine(Player player)
        {
            yield return Timing.WaitForSeconds(1.5f);
            for (int j = 0; j < 20; j++)
            {
                Manager.SendHint(player, "You are providing health to those around you!", 0.75f);
                foreach (Player player1 in Player.List)
                {
                    if (player1 == player) continue;
                    Player nP = Player.Get(player.Id);
                    int loopCntr = 0;
                    RaycastHit h = new();
                    Player target = null;
                    do
                    {
                        Vector3 dir = target.Position - new Vector3(nP.Position.x, nP.Position.y + 0.1f, nP.Position.z);
                        Physics.Raycast(nP.Position, dir, out h);
                        loopCntr++;
                    } while (!Player.TryGet(h.collider, out target) && loopCntr != 5);

                    if (target is null)
                    {
                        continue;
                    }

                    if (target == nP)
                    {
                        continue;
                    }

                    if (Math.Sqrt(Math.Pow(nP.Position.x - target.Position.x, 2) + Math.Pow(nP.Position.y - target.Position.y, 2)) > 10)
                    {
                        continue;
                    }

                    if (target.IsHuman)
                    {
                        //Log.Info($"{ppp.DisplayNickname} is {ppp.Role.Name} this role is {ppp.IsHuman}");
                        Manager.SendHint(target, "Someone's coinflip is giving you health!", 0.75f);
                        target.Health += 4.75f;
                    }
                }
            }

            yield return Timing.WaitForSeconds(0.5f);
        }

        private IEnumerator<float> DisarmCoroutine(Player player)
        {
            player.Handcuff();
            yield return Timing.WaitForSeconds(10);
            player.RemoveHandcuffs();
        }

        private IEnumerator<float> ShitOnTheFloorCoroutine(Player player)
        {
            for (int i = 0; i < 30; i++)
            {
                yield return Timing.WaitForSeconds(0.5f);
                player.PlaceTantrum(true);

                player.PlaceBlood(player.Transform.up * -1f);
            }
            player.Explode();
        }

        private IEnumerator<float> RoomRGBCoroutine(Room roomSel)
        {

            Color[] colors = {
            Color.red, //this is red you fucking twat
            Color.green,
            Color.blue,
            Color.cyan,
            Color.magenta,
            Color.yellow,
            };
            var rnd = new System.Random();
            Color color = colors[rnd.Next(0, colors.Count())];
            roomSel.Color = color * 9;
            roomSel.LockDown(10, DoorLockType.Warhead);
            yield return Timing.WaitForSeconds(30);
            roomSel.ResetColor();
        }

        private IEnumerator<float> GlowCoroutine(Player p)
        {
            Exiled.API.Features.Toys.Light li = Exiled.API.Features.Toys.Light.Create(new Vector3(p.Transform.position.x, p.Transform.position.y + 1.2f, p.Transform.position.z), Vector3.zero, Vector3.one, false);
            li.Range = 45f;
            li.Intensity = 9999f;
            li.Color = Color.yellow;
            li.Spawn();
            li.Base.gameObject.transform.SetParent(p.GameObject.transform);
            yield return Timing.WaitForSeconds(30f);
            li.Destroy();
        }
        private IEnumerator<float> BeepCoroutine(Player p)
        {
            p.CameraTransform.rotation = new Quaternion(0, 0, 180, 0);
            yield break;
        }
        private IEnumerator<float> EnterPDCoroutine(Player p, ZoneType zt)
        {
            var door = Room.List.FirstOrDefault().Doors.FirstOrDefault();

            yield return Timing.WaitForOneFrame;

            Exiled.Events.Handlers.Player.EscapingPocketDimension += ExitVoid;
            Exiled.Events.Handlers.Player.FailingEscapePocketDimension += FixThing;

            void ExitVoid(EscapingPocketDimensionEventArgs ev)
            {
                bool goodRoom = false;
                while (goodRoom == false)
                {
                    var roomNd = new System.Random();
                    int roomNum = roomNd.Next(0, Room.List.Count());
                    if (Room.List.ElementAt(roomNum).Type != RoomType.HczTesla && Room.List.ElementAt(roomNum).Zone == zt)
                    {
                        goodRoom = true;
                        door = Room.List.ElementAt(roomNum).Doors.FirstOrDefault();
                    }
                }
                ev.TeleportPosition = new Vector3(door.Position.x, door.Position.y + 1.5f, door.Position.z);
                p.DisableEffect(EffectType.PocketCorroding);
                p.DisableEffect(EffectType.Corroding);
                Exiled.Events.Handlers.Player.EscapingPocketDimension -= ExitVoid;
                Exiled.Events.Handlers.Player.FailingEscapePocketDimension -= FixThing;
            }

            //:skull:
            void FixThing(FailingEscapePocketDimensionEventArgs e)
            {
                Exiled.Events.Handlers.Player.EscapingPocketDimension -= ExitVoid;
                Exiled.Events.Handlers.Player.FailingEscapePocketDimension -= FixThing;
            }

            yield return Timing.WaitForOneFrame;
        }
    }
}