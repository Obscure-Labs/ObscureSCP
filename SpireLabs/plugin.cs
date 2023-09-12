namespace SpireLabs
{
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
    using Exiled.API.Features.Pickups.Projectiles;
    using InventorySystem.Items.ThrowableProjectiles;
    using Exiled.API.Features.Items;
    using Interactables.Interobjects.DoorUtils;
    using System;
    using LiteNetLib;

    public class Plugin : Plugin<config>
    {
        /// <summary>
        /// Add more coin flip shit
        /// Go away
        /// </summary>
        public override string Name => "Spire Labs";
        public override string Author => "ImIsaacTbh & ImKevin";
        public override System.Version Version => new System.Version(1, 0, 1);
        public override System.Version RequiredExiledVersion => new System.Version(8, 0, 1);
        public static float hidDPS;

        public static OverrideData OScp049;
        public static OverrideData OScp0492;
        public static OverrideData OScp079;
        public static OverrideData OScp096;
        public static OverrideData OScp106;
        public static OverrideData OScp173;
        public static OverrideData OScp939;
        public static OverrideData OCaptain;

        public static ScalingData Scp049;
        public static ScalingData Scp0492;
        public static ScalingData Scp079;
        public static ScalingData Scp096;
        public static ScalingData Scp106;
        public static ScalingData Scp173;
        public static ScalingData Scp939;

        public static bool classd;
        public static bool scientist;
        public static bool guard;

        public static string[] file;

        public static int lpc = 0;

        public static int hintHeight = 0;
        public static int hintTime = 0;

        public static bool inLobby = false;

        public static UnityEngine.Vector3 lobbyVector;

        public static string lastId;

        public static bool limitMet = false;

        public static bool first = false;
        public static bool allowStart = false;
        public static int playerCount = 0;

        public static bool hasRestarted = false;
        public static bool ConMet = false;

        public static CoroutineHandle handle;
        public static bool realRoundEnd = false;
        public static bool startingRound = false;
        public static bool initing = false;

        public string spireConfigLoc;

        public string[] good = { "You gained 20HP!", "You gained a 5 second speed boost!", "You found a keycard!", "You are invisible for 5 seconds!", "You are healed!", "GRENADE FOUNTAIN!" };
        public string[] bad = { "You now have 50HP!", "You dropped all of your items, How clumsy...", "You have heavy feet for 5 seconds...", "Pocket Sand!", "You got lost and found yourself in a random room!", "You flipped the coin so hard your hands fell off!", "BOOM!", "Sent To Brazil!!!"};
        public override void OnDisabled()
        {
            UnregisterEvents();
            Log.Info("Spire Labs has been disabled!");
            base.OnDisabled();
        }
        public override void OnEnabled()
        {
            RegisterEvents();
            Log.SendRaw("[KevinIsBent] [SpireLabs]\n\r\n .d8888b.           d8b                 .d8888b.   .d8888b.  8888888b.  \r\nd88P  Y88b          Y8P                d88P  Y88b d88P  Y88b 888   Y88b \r\nY88b.                                  Y88b.      888    888 888    888 \r\n \"Y888b.   88888b.  888 888d888 .d88b.  \"Y888b.   888        888   d88P \r\n    \"Y88b. 888 \"88b 888 888P\"  d8P  Y8b    \"Y88b. 888        8888888P\"  \r\n      \"888 888  888 888 888    88888888      \"888 888    888 888        \r\nY88b  d88P 888 d88P 888 888    Y8b.    Y88b  d88P Y88b  d88P 888        \r\n \"Y8888P\"  88888P\"  888 888     \"Y8888  \"Y8888P\"   \"Y8888P\"  888        \r\n           888                                                          \r\n           888                                                          \r\n           888                                                          \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n", color: ConsoleColor.DarkMagenta);
            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\EXILED\\Configs\\Spire/"))
            {
                spireConfigLoc = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\EXILED\\Configs\\Spire/";
            }
            else
            {
                Log.Info("Making Spire Config Folder");
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\EXILED\\Configs\\Spire/");
                spireConfigLoc = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\EXILED\\Configs\\Spire/";
                File.WriteAllText(spireConfigLoc + "lines.txt", "CHANGE ME IN :  [EXILEDCONIG]\\Spire/lines.txt");
            }
            Log.Info($"Found Spire Config Folder : \"{spireConfigLoc}\"");
            base.OnEnabled();
            hidDPS = Config.hidDPS;
            OScp049 = Config.Scp049Override;
            OScp0492 = Config.Scp049Override;
            OScp079 = Config.Scp079Override;
            OScp096 = Config.Scp096Override;
            OScp106 = Config.Scp106Override;
            OScp173 = Config.Scp173Override;
            OScp939 = Config.Scp939Override;
            OCaptain = Config.CaptainOverride;

            Scp049 = Config.Scp049;
            Scp0492 = Config.Scp0492;
            Scp079 = Config.Scp079;
            Scp096 = Config.Scp096;
            Scp106 = Config.Scp106;
            Scp173 = Config.Scp173;
            Scp939 = Config.Scp939;

            classd = Config.classd;
            scientist = Config.scientist;
            guard = Config.guard;

            hintHeight = Config.hintHeight;
            hintTime = Config.timeBetweenHints;

            lobbyVector = Config.spawnRoomVector3;

            //file = File.ReadAllLines(@"C:\Users\Kevin\AppData\Roaming\EXILED\Configs\Spire/lines.txt");
            Timing.RunCoroutine(ShowHint());
            inLobby = false;
        }

        private void RegisterEvents()
        {
            //Exiled.Events.Handlers.Player.EnteringPocketDimension += pocketEnter;
            Exiled.Events.Handlers.Player.Hurting += theThing;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStart;
            Exiled.Events.Handlers.Player.Spawned += Player_Spawned;
            Exiled.Events.Handlers.Player.FlippingCoin += Player_FlippingCoin;
            Exiled.Events.Handlers.Player.Joined += Player_Joined;
            Exiled.Events.Handlers.Player.PreAuthenticating += Authing;
            Exiled.Events.Handlers.Server.RestartingRound += restarting;
            Exiled.Events.Handlers.Player.ChangedItem += item_change;
            CustomItem.RegisterItems();
        }

        //made by isaac 
        //trust the process
        public static IEnumerator<float> enterPD(Player p, ZoneType zt)
        {
            Door door = Room.List.FirstOrDefault().Doors.FirstOrDefault();
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
                Exiled.Events.Handlers.Player.EscapingPocketDimension -= ExitVoid;
                Exiled.Events.Handlers.Player.FailingEscapePocketDimension -= FixThing;
            }
            void FixThing(FailingEscapePocketDimensionEventArgs e)
            {
                Exiled.Events.Handlers.Player.EscapingPocketDimension -= ExitVoid;
                Exiled.Events.Handlers.Player.FailingEscapePocketDimension -= FixThing;
            }
            yield return Timing.WaitForOneFrame;
        }

        //trust the process
        //private void pocketEnter(EnteringPocketDimensionEventArgs ev)
        //{
        //    RoomType rt = ev.Player.CurrentRoom.Type;
        //    enterPD(ev.Player, rt);
        //}

        private void item_change(ChangedItemEventArgs ev)
        {
            if (ev.Item == null) return;
            if (ev.Item.Type != ItemType.Coin)
                return;
            string hint = string.Empty;
            if (hintHeight != 0 && hintHeight < 0)
            {
                for (int i = hintHeight; i < 0; i++)
                {
                    hint += "\n";
                }
            }
            hint += "Flipping this coin will cause a random event, use with caution!";
            if (hintHeight != 0 && hintHeight > 0)
            {
                for (int i = 0; i < hintHeight; i++)
                {
                    hint += "\n";
                }
            }
            ev.Player.ShowHint(hint, 5);
        }

        private void restarting()
        {
            if (realRoundEnd)
            {
                Exiled.API.Features.Log.Info("Restarting the round (real restart)");
                hasRestarted = false;
                first = false;
                inLobby = false;
                allowStart = false;
                realRoundEnd = false;
                playerCount = 0;
                ConMet = false;
                startingRound = false;
                initing = false;
                Exiled.Events.Handlers.Player.Left -= Player_Leave;
            }
        }
       private void Authing(PreAuthenticatingEventArgs ev)
       {
       }

       private IEnumerator<float> startCheck()
       {
           while (true)
           {
               while (!hasRestarted && !ConMet && !initing)
               {
                   if (playerCount > 1)
                   {
                       Log.Info($"Player count is : \"{playerCount}\" (startcheck)");
                       Timing.RunCoroutine(restart());
                       allowStart = true;
                       ConMet = true;
                       initing = true;
                   }
                   yield return Timing.WaitForSeconds(1);
               }
               yield return Timing.WaitForSeconds(1);
               if (startingRound == true)
               {
                   startingRound = false;
                   break;
               }
           }
       }

       private IEnumerator<float> EngageLobby(JoinedEventArgs ev)
       {
           Log.Info("STARTING LEAVE EVENT");
           Exiled.Events.Handlers.Player.Left += Player_Leave;
           Log.Info("ENABLED LEAVE EVENT");
           first = true;
           inLobby = true;
           yield return Timing.WaitForSeconds(2);
           Round.IsLocked = true;
           Round.Start();
           yield return Timing.WaitForSeconds(1);
           ev.Player.RoleManager.ServerSetRole(RoleTypeId.ClassD, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.UseSpawnpoint);
           yield return Timing.WaitForSeconds((float)0.25);
           ev.Player.Teleport(lobbyVector);
           handle = Timing.RunCoroutine(startCheck());
            //File.WriteAllText(@"C:\Users\Kevin\AppData\Roaming\EXILED\Configs\Spire/stinky.txt", "pp");
            File.WriteAllText((spireConfigLoc + "stinky.txt"), "pp");

        }

        private IEnumerator<float> restart()
       {
           Cassie.Message("Round Starting in 25 seconds", isSubtitles: true, isNoisy: false);
           yield return Timing.WaitForSeconds(25);
           if (playerCount < 2) {

               Cassie.Message($"ROUND START DISABLED (Too few players)", isSubtitles: true, isNoisy: false);
               initing = false;
           }
           else
           {
               Cassie.Message("STARTING MATCH", isSubtitles: true, isNoisy: false);
               foreach (Player p in Player.List)
               {
                   p.EnableEffect(EffectType.Blinded, 3);
               }
               yield return Timing.WaitForSeconds(2);
               hasRestarted = true;
               first = false;
               startingRound = true;
               Exiled.Events.Handlers.Player.Left -= Player_Leave;
               Round.RestartSilently();
           }

       }

       private IEnumerator<float> go()
       {
           yield return Timing.WaitForSeconds(2);
           Cassie.Message("STARTING ROUND", isSubtitles: true, isNoisy: false);
           yield return Timing.WaitForSeconds(7);
           Round.Start();
            // File.WriteAllText(@"C:\Users\Kebin\AppData\Roaming\EXILED\Configs\Spire/stinky.txt", "ee");
            File.WriteAllText((spireConfigLoc + "stinky.txt"), "ee");

            inLobby = false;
           realRoundEnd = true;
       }

       private void Player_Leave(LeftEventArgs ev)
       {
           if (lastId != ev.Player.UserId)
           {
               playerCount--;
               Log.Info($"Player count is now: \"{playerCount}\"");
           }
           lastId = ev.Player.UserId;
       }
       private IEnumerator<float> InsertPlayer(JoinedEventArgs ev)
       {
           yield return Timing.WaitForSeconds(2);
           ev.Player.RoleManager.ServerSetRole(RoleTypeId.ClassD, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.UseSpawnpoint);
           ev.Player.Teleport(lobbyVector);
       }

       private void Player_Joined(JoinedEventArgs ev)
       {
           lastId = string.Empty;
           ConMet = false;
           playerCount++;
           Log.Info($"Player count is now: \"{playerCount}\"");
           //ev.Player.Broadcast(new Broadcast { Content = "Player joined", Duration = 1, Show = true, Type = global::Broadcast.BroadcastFlags.Normal });
           if (!hasRestarted)
           {
               if (!inLobby && !first && !Round.IsStarted) //EngageLobby();
               {
                   Timing.RunCoroutine(EngageLobby(ev));
               }
               else if (first)
               {
                   Timing.RunCoroutine(InsertPlayer(ev));
                   return;
               }
           }
           else if (!first)
           {
               Timing.RunCoroutine(go());
               first = true;
           }
       }

        //private IEnumerator<float> scalePlayer(FlippingCoinEventArgs ev)
        //{
        //    for (int i = 0; i < 50; i++)
        //    {
        //        var rnd = new System.Random();
        //        float num1 = (float)(rnd.NextDouble() * rnd.Next(1, 5));
        //        float num2 = (float)(rnd.NextDouble() * rnd.Next(1, 5));
        //        ev.Player.Scale = new Vector3(num1, 1f, num2);
        //        yield return Timing.WaitForSeconds(0.1f);
        //    }
        //    ev.Player.Scale = new Vector3(1f, 1f, 1f);
        //    Throwable g = ev.Player.ThrowGrenade(ProjectileType.FragGrenade);
        //}

        private IEnumerator<float> grenadeFountain(Player p)
        {
            int bombs = 0;
            while (bombs != 10)
            {
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                yield return Timing.WaitForSeconds(0.1f);
                bombs++;
            }

        }



        private void Player_FlippingCoin(FlippingCoinEventArgs ev)
        {
            // Projectile D = Projectile.CreateAndSpawn(ProjectileType.FragGrenade, new Vector3(ev.Player.Position.x, ev.Player.Position.y, ev.Player.Position.z), new Quaternion(ev.Player.Rotation.x, ev.Player.Rotation.y, ev.Player.Rotation.z, ev.Player.Transform.rotation.w), true);
            //Pickup d;
            //CustomItem.TrySpawn((uint)534588, new Vector3(ev.Player.Position.x, ev.Player.Position.y + 1, ev.Player.Position.z), out d);
            //Timing.RunCoroutine(scalePlayer(ev
            var rnd = new System.Random();
            int num = rnd.Next(0, 100);
            int result = 0;
            if (num > 20 && num < 45) result = 1;
            if (num > 45 && num < 100) result = 2;
            if (result == 1)
            {
                Exiled.API.Features.Log.Info($"{ev.Player.Nickname} flipped a coin and got a good result!");
                switch (rnd.Next(0, good.Count()))
                {
                    case 0:
                        ev.Player.ShowHint(good[0], 3);
                        ev.Player.Heal(20, true);
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
                        ev.Player.ShowHint(good[1], 3);
                        ev.Player.EnableEffect(EffectType.MovementBoost, 5);
                        ev.Player.ChangeEffectIntensity(EffectType.MovementBoost, 65, 5);
                        break;
                    case 2:
                        bool todrop = false;
                        ev.Player.ShowHint(good[2], 3);
                        if (ev.Player.IsInventoryFull)
                        {
                            todrop = true;

                        }
                        else
                        {
                            todrop = false;
                        }

                        var rnd2 = new System.Random();
                        int card = rnd2.Next(1, 3);
                        if (card == 1)
                        {
                            if (todrop)
                            {
                                Pickup.CreateAndSpawn(ItemType.KeycardZoneManager, new Vector3(ev.Player.Position.x, ev.Player.Position.y, ev.Player.Position.z), new Quaternion(ev.Player.Rotation.x, ev.Player.Rotation.y, ev.Player.Rotation.z, ev.Player.Transform.rotation.w));
                            }
                            else
                            {
                                ev.Player.AddItem(ItemType.KeycardZoneManager);
                            }
                        }
                        else if (card == 2)
                        {
                            if (todrop)
                            {
                                Pickup.CreateAndSpawn(ItemType.KeycardMTFOperative, new Vector3(ev.Player.Position.x, ev.Player.Position.y, ev.Player.Position.z), new Quaternion(ev.Player.Rotation.x, ev.Player.Rotation.y, ev.Player.Rotation.z, ev.Player.Transform.rotation.w));
                            }
                            else
                            {
                                ev.Player.AddItem(ItemType.KeycardMTFOperative);
                            }
                        }
                        else
                        {
                            if (todrop)
                            {
                                Pickup.CreateAndSpawn(ItemType.KeycardResearchCoordinator, new Vector3(ev.Player.Position.x, ev.Player.Position.y, ev.Player.Position.z), new Quaternion(ev.Player.Rotation.x, ev.Player.Rotation.y, ev.Player.Rotation.z, ev.Player.Transform.rotation.w));
                            }
                            else
                            {
                                ev.Player.AddItem(ItemType.KeycardResearchCoordinator);
                            }
                        }
                        break;
                    case 3:
                        ev.Player.ShowHint(good[3], 3);
                        ev.Player.EnableEffect(EffectType.Invisible, 5);
                        break;
                    case 4:
                        ev.Player.Heal(150, false);
                        ev.Player.ShowHint(good[4], 3);
                        break;
                    case 5:
                        ev.Player.ShowHint(good[5], 3);
                        Timing.RunCoroutine(grenadeFountain(ev.Player));
                        break;
                }

            }
            else if (result == 2)
            {
                Exiled.API.Features.Log.Info($"{ev.Player.Nickname} flipped a coin and got a bad result!");
                switch (rnd.Next(0, bad.Count()))
                {
                    case 0:
                        ev.Player.ShowHint(bad[0], 3);
                        ev.Player.Health = 50;
                        break;
                    case 1:
                        ev.Player.ShowHint(bad[1], 3);
                        ev.Player.DropItems();
                        break;
                    case 2:
                        ev.Player.ShowHint(bad[2], 3);
                        ev.Player.EnableEffect(EffectType.SinkHole, 5);
                        break;
                    case 3:
                        ev.Player.ShowHint(bad[3], 3);
                        ev.Player.EnableEffect(EffectType.Flashed, 5);
                        break;
                    case 4:
                        if (Warhead.IsDetonated)
                            break;
                        ev.Player.ShowHint(bad[4], 3);
                        var r = new System.Random();
                        var n = r.Next(0, 2);
                        bool goodRoom = false;
                        Room room = Room.List.ElementAt(4);
                        Door door = Room.List.ElementAt(4).Doors.FirstOrDefault();
                        while (goodRoom == false)
                        {
                            var roomNd = new System.Random();
                            int roomNum = roomNd.Next(0, Room.List.Count());
                            if (Map.IsLczDecontaminated)
                            {
                                if (Room.List.ElementAt(roomNum).Type != RoomType.HczTesla && Room.List.ElementAt(roomNum).Zone != ZoneType.LightContainment)
                                {
                                    goodRoom = true;
                                    door = Room.List.ElementAt(roomNum).Doors.FirstOrDefault();
                                }
                            }
                            else
                            {
                                if (Room.List.ElementAt(roomNum).Type != RoomType.HczTesla)
                                {
                                    goodRoom = true;
                                    door = Room.List.ElementAt(roomNum).Doors.FirstOrDefault();
                                }
                            }

                        }
                        ev.Player.Teleport(new Vector3(door.Position.x, door.Position.y + 1f, door.Position.z));
                        
                        break;
                    case 5:
                        ev.Player.ShowHint(bad[5], 3);
                        ev.Player.EnableEffect(EffectType.SeveredHands, 999);
                        ev.Player.EnableEffect(EffectType.CardiacArrest, 60);
                        ev.Player.ChangeEffectIntensity(EffectType.CardiacArrest, 5);
                        //Pickup p;
                        //SpireNade.TrySpawn((uint)534588, ev.Player.Position, out p);
                        break;
                    case 6:
                        ev.Player.Vaporize();
                        break;
                    case 7:
                        ev.Player.ShowHint(bad[7], 3);
                        ZoneType zt = ev.Player.CurrentRoom.Zone;
                        ev.Player.Teleport(Room.List.FirstOrDefault(x => x.Type == RoomType.Pocket));
                        ev.Player.EnableEffect(EffectType.PocketCorroding, 60);
                        Timing.RunCoroutine(enterPD(ev.Player, zt));
                        break;
                }
            }
            else
            {
                Exiled.API.Features.Log.Info($"{ev.Player.Nickname} flipped a coin and got nothing!");
                ev.Player.ShowHint("No consequences, this time...", 3);
            }
        }

        public class SpireNade : CustomGrenade
        {
            public override bool ExplodeOnCollision { get; set; } = true;
            public override float FuseTime { get; set; } = 0.1f;
            public override uint Id { get; set; } = 534588;
            public override string Name { get; set; } = "spireBomb";
            public override string Description { get; set; } = "n/a";
            public override float Weight { get; set; } = 40f;
            public override SpawnProperties SpawnProperties { get; set; } = null;
        }

        private void UnregisterEvents()
         {
             Exiled.Events.Handlers.Player.Hurting -= theThing;
             Exiled.Events.Handlers.Item.KeycardInteracting -= Item_KeycardInteracting;
             Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStart;
             Exiled.Events.Handlers.Player.Spawned -= Player_Spawned;
             Exiled.Events.Handlers.Player.FlippingCoin -= Player_FlippingCoin;
             Exiled.Events.Handlers.Player.Joined -= Player_Joined;
             Exiled.Events.Handlers.Player.PreAuthenticating -= Authing;
             Exiled.Events.Handlers.Server.RestartingRound -= restarting;
         }

         static void theThing(HurtingEventArgs ev)
         {
             if (ev.DamageHandler.Type == DamageType.MicroHid)
             {
                 ev.Amount *= (hidDPS / 100);
             }
         }
         public static void Item_KeycardInteracting(KeycardInteractingEventArgs ev)
         {
             Log.Info($"Door opened, requires: {ev.Door.RequiredPermissions.RequiredPermissions}");
         }

         private void Player_Spawned(SpawnedEventArgs ev)
         {
             if (OCaptain.enabled)
             {
                 if (ev.Player.RoleManager.CurrentRole.RoleTypeId == RoleTypeId.NtfCaptain)
                 {
                     ev.Player.MaxHealth = OCaptain.healthOverride;
                     ev.Player.Heal(OCaptain.healthOverride, false);
                 }
             }

         }

         private IEnumerator<float> ShowHint()
         {
             var rnd = new System.Random();
             yield return Timing.WaitForSeconds(30);
             while (!Exiled.API.Features.Round.IsEnded)
             {
                //file = File.ReadAllLines(@"C:\Users\Kebin\AppData\Roaming\EXILED\Configs\Spire/lines.txt");
                file = File.ReadAllLines((spireConfigLoc + "lines.txt"));
                string hintMessage = string.Empty;
                 if (hintHeight != 0 && hintHeight < 0)
                 {
                     for (int i = hintHeight; i < 0; i++)
                     {
                         hintMessage += "\n";
                     }
                 }
                 hintMessage += file[rnd.Next(0, file.Count() - 1)];
                 if (hintHeight != 0 && hintHeight > 0)
                 {
                     for (int i = 0; i < hintHeight; i++)
                     {
                         hintMessage += "\n";
                     }
                 }
                 foreach (Exiled.API.Features.Player p in Exiled.API.Features.Player.List)
                 {
                     if (!p.IsDead)
                     {
                         p.ShowHint($"{hintMessage}", 5);
                     }

                 }
                 yield return Timing.WaitForSeconds(hintTime);
             }
         }
         void OnRoundStart()
         {
             //Exiled.API.Features.Server.Broadcast.SendMessage("Lobby initialised. Awaiting round start.");
             //while(Exiled.API.Features.Player.List.Count() < 2)
             //{
             //    Thread.Sleep(2000);
             //}
             //while(Exiled.API.Features.Player.List.Count() > 1)
             //{

             //}
             //Exiled.API.Features.Round.RestartSilently();

             Log.Info("Round has started!");
             var players = Player.List;
             List<Player> SCPS = new List<Player>();
             int humanPlayers = 0;
             foreach (Player p in players)
             {
                 switch (p.RoleManager.CurrentRole.RoleTypeId)
                 {
                     case RoleTypeId.ClassD:
                         humanPlayers++;
                         break;
                     case RoleTypeId.Scientist:
                         humanPlayers++;
                         break;
                     case RoleTypeId.FacilityGuard:
                         humanPlayers++;
                         break;
                     case RoleTypeId.Scp049: SCPS.Add(p); break;
                     case RoleTypeId.Scp079: SCPS.Add(p); break;
                     case RoleTypeId.Scp096: SCPS.Add(p); break;
                     case RoleTypeId.Scp106: SCPS.Add(p); break;
                     case RoleTypeId.Scp173: SCPS.Add(p); break;
                     case RoleTypeId.Scp939: SCPS.Add(p); break;
                     case RoleTypeId.NtfCaptain:
                         p.MaxHealth = OCaptain.healthOverride;
                         p.Heal(150, false);
                         break;
                 }
             }
             foreach (Player p in SCPS)
             {
                 switch (p.RoleManager.CurrentRole.RoleTypeId)
                 {
                     case RoleTypeId.Scp049:
                         if (OScp049.enabled)
                         {
                             p.MaxHealth = OScp049.healthOverride;
                         }
                         Task.Delay(50);
                         if (Scp049.enabled)
                         {
                             p.MaxHealth += (Scp049.healthIncrease * humanPlayers);
                             p.Heal(p.MaxHealth);
                         }
                         break;
                     case RoleTypeId.Scp079:
                         if (OScp079.enabled)
                         {
                             p.MaxHealth = OScp079.healthOverride;
                         }
                         Task.Delay(50);
                         if (Scp079.enabled)
                         {
                             p.MaxHealth += (Scp079.healthIncrease * humanPlayers);
                             p.Heal(p.MaxHealth);
                         }
                         break;
                     case RoleTypeId.Scp096:
                         if (OScp096.enabled)
                         {
                             p.MaxHealth = OScp096.healthOverride;
                         }
                         Task.Delay(50);
                         if (Scp096.enabled)
                         {
                             p.MaxHealth += (Scp096.healthIncrease * humanPlayers);
                             p.Heal(p.MaxHealth);
                         }
                         break;
                     case RoleTypeId.Scp106:
                         if (OScp106.enabled)
                         {
                             p.MaxHealth = OScp106.healthOverride;
                         }
                         Task.Delay(50);
                         if (Scp106.enabled)
                         {
                             p.MaxHealth += (Scp106.healthIncrease * humanPlayers);
                             p.Heal(p.MaxHealth);
                         }
                         break;
                     case RoleTypeId.Scp173:
                         if (OScp173.enabled)
                         {
                             p.MaxHealth = OScp173.healthOverride;
                         }
                         Task.Delay(50);
                         if (Scp173.enabled)
                         {
                             p.MaxHealth += (Scp173.healthIncrease * humanPlayers);
                             p.Heal(p.MaxHealth);
                         }
                         break;
                     case RoleTypeId.Scp939:
                         if (OScp939.enabled)
                         {
                             p.MaxHealth = OScp939.healthOverride;
                         }
                         Task.Delay(50);
                         if (Scp939.enabled)
                         {
                             p.MaxHealth += (Scp939.healthIncrease * humanPlayers);
                             p.Heal(p.MaxHealth);
                         }
                         break;
                 }
             }
         }
    }
}
