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
    using Exiled.API.Features.Doors;
    using System;
    using HarmonyLib;
    using UCRAPI = UncomplicatedCustomRoles.API.Features.Manager;
    using Exiled.Loader;
    using Exiled.API.Features.Toys;

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
        public static float cokeDPS;

        public static CoroutineHandle LobbyTimer;

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

        public static bool isLobbyEnabledConfig = false;

        public string spireConfigLoc;

        private Harmony _harmony;

        public static CoroutineHandle flickerHandle;
        public static CoroutineHandle lockHandle;

        public override void OnDisabled()
        {
            UnregisterEvents();
            Log.Info("Spire Labs has been disabled!");
            base.OnDisabled();
        }

        //[HarmonyPatch(typeof(FpcMouseLook), nameof(FpcMouseLook.UpdateRotation))]
        //public class RotationPatch
        //{
        //    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        //    {
        //        List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

        //        Label skip = generator.DefineLabel();

        //        newInstructions[newInstructions.Count - 1].labels.Add(skip);

        //        newInstructions.InsertRange(0, new List<CodeInstruction>()
        //    {
        //        new(OpCodes.Ldarg_0),
        //        new(OpCodes.Ldfld, AccessTools.Field(typeof(FpcMouseLook), nameof(FpcMouseLook._hub))),
        //        new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Player), "Get", new[] { typeof(ReferenceHub) })),
        //        new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Player), nameof(Player.IsNPC))),
        //        new CodeInstruction(OpCodes.Brtrue_S, skip),
        //    });

        //        foreach (CodeInstruction instruction in newInstructions)
        //            yield return instruction;

        //        ListPool<CodeInstruction>.Shared.Return(newInstructions);
        //    }
        //}

        private IEnumerator<float> checkPlayer()
        {
            while(true)
            {
                if(Player.List.Count() == 0)
                {
                    Round.Restart();
                }
                yield return Timing.WaitForSeconds(10);
            }
        }

        public override void OnEnabled()
        {
            CustomItem.RegisterItems(overrideClass: ItemConfigs);
            LoadItems();
            Timing.RunCoroutine(checkPlayer());
            RegisterEvents();
            _harmony = new("DevDummies-Rotation-Patch");
            _harmony.PatchAll();

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
            cokeDPS = Config.cokeDPS;
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
            isLobbyEnabledConfig = Config.lobbyEnabled;


            //file = File.ReadAllLines(@"C:\Users\Kevin\AppData\Roaming\EXILED\Configs\Spire/lines.txt");
            Timing.RunCoroutine(ShowHint());
            inLobby = false;
        }

        private void RegisterEvents()
        {
            //Exiled.Events.Handlers.Player.EnteringPocketDimension += pocketEnter;
            Exiled.Events.Handlers.Player.Hurting += theThing;
            Exiled.Events.Handlers.Player.Hurting += theNut.scp173DMG;
            //Exiled.Events.Handlers.Scp173.Blinking += theNut.scp173TP;
            Exiled.Events.Handlers.Scp173.UsingBreakneckSpeeds += theNut.scp173ZOOM;
            Exiled.Events.Handlers.Scp106.Attacking += larry.pdExits;
            Exiled.Events.Handlers.Scp049.ActivatingSense += doctor.doctorBoost;
            Exiled.Events.Handlers.Scp049.SendingCall += doctor.call;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStart;
            Exiled.Events.Handlers.Player.Spawned += Player_Spawned;
            Exiled.Events.Handlers.Player.Spawned += IDThief.Player_Spawned;
            Exiled.Events.Handlers.Player.FlippingCoin += coin.Player_FlippingCoin;
            Exiled.Events.Handlers.Player.Joined += Player_Joined;
            Exiled.Events.Handlers.Player.PreAuthenticating += Authing;
            Exiled.Events.Handlers.Server.RestartingRound += restarting;
            Exiled.Events.Handlers.Player.ChangedItem += item_change;
            Exiled.Events.Handlers.Player.ChangedItem += IDThief.item_change;
            Exiled.Events.Handlers.Warhead.Detonated += map_nuked;
            Exiled.Events.Handlers.Scp106.Attacking += larry.onLarryAttack;
            Exiled.Events.Handlers.Player.UsingItemCompleted += usingItem;
            Exiled.Events.Handlers.Player.Left += left;
            Exiled.Events.Handlers.Player.Verified += joinMsg;
            //Exiled.Events.Handlers.Server.RespawningTeam += customRoles.spawnWave;
            CustomItem.RegisterItems();
        }
        
        private void joinMsg(VerifiedEventArgs ev)
        {
            
            Timing.RunCoroutine(guiHandler.sendJoinLeave(ev.Player, 'j'));
        }

        private void left(LeftEventArgs ev)
        {
            Timing.RunCoroutine(guiHandler.sendJoinLeave(ev.Player, 'l'));
        }


        private void item_change(ChangedItemEventArgs ev)
        {
            if (ev.Item == null) return;

            if (ev.Item.Type != ItemType.Coin)
                return;
            string hint = string.Empty;
            //if (hintHeight != 0 && hintHeight < 0)
            //{
            //    for (int i = hintHeight; i < 0; i++)
            //    {
            //        hint += "\n";
            //    }
            //}
            hint += "Flipping this coin will cause a random event, use with caution!";
            //if (hintHeight != 0 && hintHeight > 0)
            //{
            //    for (int i = 0; i < hintHeight; i++)
            //    {
            //        hint += "\n";
            //    }
            //}
            Timing.RunCoroutine(guiHandler.sendHint(ev.Player, hint, 5));
            
        }

        private void restarting()
        {
            guiHandler.killLoop = true;
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
        public ItemConfigs.Items ItemConfigs { get; private set; } = null!;
        private void Authing(PreAuthenticatingEventArgs ev)
        {
        }
        private void LoadItems()
        {
            if (!Directory.Exists(spireConfigLoc + "/CustomItems/"))
                Directory.CreateDirectory(spireConfigLoc + "/CustomItems/");
            string filePath = spireConfigLoc + "/CustomItems/global.yml";
            if(!File.Exists(filePath))
            {
                ItemConfigs = new ItemConfigs.Items();
                File.WriteAllText(filePath, Loader.Serializer.Serialize(ItemConfigs));
            }
            else
            {
                ItemConfigs = Loader.Deserializer.Deserialize<ItemConfigs.Items>(File.ReadAllText(filePath));
                File.WriteAllText(filePath, Loader.Serializer.Serialize(ItemConfigs));
            }
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
            
            guiHandler.killLoop = false;
            Timing.RunCoroutine(guiHandler.displayGUI(ev.Player));
            lastId = string.Empty;
           ConMet = false;
           playerCount++;
           Log.Info($"Player count is now: \"{playerCount}\"");
            //ev.Player.Broadcast(new Broadcast { Content = "Player joined", Duration = 1, Show = true, Type = global::Broadcast.BroadcastFlags.Normal });
            if (isLobbyEnabledConfig)
            {
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
             Exiled.Events.Handlers.Player.FlippingCoin -= coin.Player_FlippingCoin;
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
             if(ev.DamageHandler.Type == DamageType.Scp207)
            {
                //Log.Info($"Conk hit {ev.Player.DisplayNickname}");
                ev.Amount *= (cokeDPS / 100);
            }
         } 
         public static void Item_KeycardInteracting(KeycardInteractingEventArgs ev)
         {
             Log.Info($"Door opened, requires: {ev.Door.RequiredPermissions.RequiredPermissions}");
        }

        private void Player_Spawned(SpawnedEventArgs ev)
        {
            if(ev.Player.Role == RoleTypeId.Scp0492)
            {
                var plData = customRoles.rd.SingleOrDefault(x => x.player.NetId == ev.Player.NetId) ?? null; 
                if(plData != null)
                {
                    switch(plData.UCRID)
                    {
                        case 2: ev.Player.Scale = Vector3.one * 0.7f; break;
                    }
                }
            }

            Timing.RunCoroutine(customRoles.CheckRoles(ev.Player));

            if (OCaptain.enabled)
            {
                if (ev.Player.RoleManager.CurrentRole.RoleTypeId == RoleTypeId.NtfCaptain)
                {
                    ev.Player.MaxHealth = OCaptain.healthOverride;
                    ev.Player.Heal(OCaptain.healthOverride, false);
                }
            }

        }

        private void usingItem(UsingItemCompletedEventArgs ev)
        {
            if (ev.Item.Type == ItemType.SCP330)
            {
                if (UCRAPI.HasCustomRole(ev.Player))
                {
                    if (UCRAPI.Get(ev.Player).Id == 2)
                    {
                        ev.Player.EnableEffect(EffectType.MovementBoost);
                        ev.Player.ChangeEffectIntensity(EffectType.MovementBoost, 80, 10);
                    }
                }
            }
        }

        internal IEnumerator<float> ShowHint()
        {
            var rnd = new System.Random();
            yield return Timing.WaitForSeconds(30);
            while (!Exiled.API.Features.Round.IsEnded)
            {
                //file = File.ReadAllLines(@"C:\Users\Kebin\AppData\Roaming\EXILED\Configs\Spire/lines.txt");
                file = File.ReadAllLines((spireConfigLoc + "lines.txt"));
                string hintMessage = string.Empty;
                //if (hintHeight != 0 && hintHeight < 0)
                //{
                //    for (int i = hintHeight; i < 0; i++)
                //    {
                //        hintMessage += "\n";
                //    }
                //}
                hintMessage += file[rnd.Next(0, file.Count() - 1)];
                //if (hintHeight != 0 && hintHeight > 0)
                //{
                //    for (int i = 0; i < hintHeight; i++)
                //    {
                //        hintMessage += "\n";
                //    }
                //}
                foreach (Exiled.API.Features.Player p in Exiled.API.Features.Player.List)
                {
                    if (!p.IsDead)
                    {
                        Timing.RunCoroutine(guiHandler.sendHint(p, hintMessage, 5));
                    }

                }
                yield return Timing.WaitForSeconds(hintTime);
            }
        }









        private void map_nuked()
        {
            foreach (Door d in Door.List)
            {
                if (d.Zone == ZoneType.Surface)
                {
                    d.IsOpen = true;
                    d.Lock(9999, DoorLockType.Warhead);
                }
            }

        }

        private IEnumerator<float> randomFlicker()
        {
            while (true)
            {
                if (guiHandler.killLoop) break;
                var roomFlicker = Room.Random(ZoneType.LightContainment);
                roomFlicker.TurnOffLights(0.15f);
                yield return Timing.WaitForSeconds(1);
                roomFlicker = Room.Random(ZoneType.HeavyContainment);
                roomFlicker.TurnOffLights(0.15f);
                roomFlicker = Room.Random(ZoneType.Entrance);
                roomFlicker.TurnOffLights(0.15f);
                Log.Debug($"Flickering lights in: {roomFlicker.RoomName}");
                var rnd = new System.Random();
                int num = rnd.Next(5, 30);
                Log.Debug($"Waiting for {num} seconds till next flicker");
                yield return Timing.WaitForSeconds(num);
            }


        }

        private IEnumerator<float> lockAnounce()
        {
            yield return Timing.WaitForSeconds(600);
            if (guiHandler.killLoop) { }
            else
            {
                Cassie.Message(@"jam_043_3 Surface armory has been opened for all jam_020_3 pitch_0.8 warhead pitch_1 authorized personnel . . . enter with pitch_0.7 jam_010_1 caution", false, false, true);
            }
        }

        void OnRoundStart()
         {
            guiHandler.startHints();
            //Exiled.API.Features.Server.Broadcast.SendMessage("Lobby initialised. Awaiting round start.");
            //while(Exiled.API.Features.Player.List.Count() < 2)
            //{
            //    Thread.Sleep(2000);
            //}
            //while(Exiled.API.Features.Player.List.Count() > 1)
            //{

            //}
            //Exiled.API.Features.Round.RestartSilently();
            Timing.KillCoroutines("flockerRoutine");
            Timing.KillCoroutines("lockRoutine");

            Timing.RunCoroutine(randomFlicker(), "flickerRoutine");
            Log.Info("Round has started!");
            Timing.RunCoroutine(lockAnounce(), "lockRoutine");
            foreach (Door d in Door.List)
            {
                if (d.Zone == ZoneType.Surface)
                    d.Lock(600, DoorLockType.Regular079);
                switch(d.Type)
                {
                    case DoorType.NukeSurface: d.Unlock(); break;
                    case DoorType.EscapePrimary: d.Unlock(); break;
                    case DoorType.EscapeSecondary: d.Unlock(); break;
                    case DoorType.ElevatorGateA: d.Unlock(); break;
                    case DoorType.ElevatorGateB: d.Unlock(); break;
                    case DoorType.SurfaceGate: d.Unlock(); break;
                }
                    
            }
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
