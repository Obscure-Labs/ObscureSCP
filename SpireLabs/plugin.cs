using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Exiled.API.Features;
using MEC;
using ObscureLabs.Items;
using Exiled.CustomItems.API.Features;
using Exiled.Loader;
using ObscureLabs.SpawnSystem;
using HarmonyLib;
using ObscureLabs.Gamemode_Handler;
using Exiled.API.Features.Doors;
using Exiled.API.Enums;
using PlayerRoles;
using Exiled.Events.EventArgs.Player;
using ObscureLabs.Modules.Gamemode_Handler.Core;
using SpireSCP.GUI.API.Features;
using UCRAPI = UncomplicatedCustomRoles.API.Features.Manager;
using UnityEngine;
using ObscureLabs.Modules.Gamemode_Handler.Minigames;

namespace ObscureLabs
{
    public class Plugin : Plugin<config>
    {
        public override string Name => "Obscure Labs";
        public override string Author => "ImIsaacTbh & ImKevin";
        public override System.Version Version => new System.Version(2, 0, 2);
        public override System.Version RequiredExiledVersion => new System.Version(8, 0, 1);

        #region When plugin is disabled
        public override void OnDisabled()
        {
            UnregisterEvents();
            Log.Info("Spire Labs has been disabled!");
            base.OnDisabled();
        }

        private void UnregisterEvents()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region OnPluginEnabled
        public override void OnEnabled()
        {
            LoadItems();
            Timing.RunCoroutine(checkPlayer());
            RegisterEvents();
            _harmony = new("DevDummies-Rotation-Patch");
            _harmony.PatchAll();

            Log.SendRaw("[ObscureLabs]\n\r\n .d8888b.           d8b                 .d8888b.   .d8888b.  8888888b.  \r\nd88P  Y88b          Y8P                d88P  Y88b d88P  Y88b 888   Y88b \r\nY88b.                                  Y88b.      888    888 888    888 \r\n \"Y888b.   88888b.  888 888d888 .d88b.  \"Y888b.   888        888   d88P \r\n    \"Y88b. 888 \"88b 888 888P\"  d8P  Y8b    \"Y88b. 888        8888888P\"  \r\n      \"888 888  888 888 888    88888888      \"888 888    888 888        \r\nY88b  d88P 888 d88P 888 888    Y8b.    Y88b  d88P Y88b  d88P 888        \r\n \"Y8888P\"  88888P\"  888 888     \"Y8888  \"Y8888P\"   \"Y8888P\"  888        \r\n           888                                                          \r\n           888                                                          \r\n           888                                                          \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n", color: ConsoleColor.DarkMagenta);
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

            hidDPS = Config.hidDPS;
            cokeDPS = Config.cokeDPS;
            OScp049 = Config.Scp049Override;
            OScp0492 = Config.Scp049Override;
            OScp079 = Config.Scp079Override;
            OScp096 = Config.Scp096Override;
            OScp106 = Config.Scp106Override;
            OScp173 = Config.Scp173Override;
            OScp939 = Config.Scp939Override;
            OScp3114 = Config.Scp3114Override;
            OCaptain = Config.CaptainOverride;

            Scp049 = Config.Scp049;
            Scp0492 = Config.Scp0492;
            Scp079 = Config.Scp079;
            Scp096 = Config.Scp096;
            Scp106 = Config.Scp106;
            Scp173 = Config.Scp173;
            Scp939 = Config.Scp939;
            Scp3114 = Config.Scp3114;

            classd = Config.classd;
            scientist = Config.scientist;
            guard = Config.guard;

            hintHeight = Config.hintHeight;
            hintTime = Config.timeBetweenHints;

            lobbyVector = Config.spawnRoomVector3;
            isLobbyEnabledConfig = Config.lobbyEnabled;

            Scp3114DMG = Config.Scp3114Damage;
        }

        private IEnumerator<float> checkPlayer()
        {
            while (true)
            {
                if (Player.List.Count() == 0)
                {
                    Round.Restart();
                }
                yield return Timing.WaitForSeconds(10);
            }
        }

        private void RegisterEvents()
        {
            Exiled.Events.Handlers.Scp106.Attacking += larry.onLarryAttack;
            Exiled.Events.Handlers.Player.ChangedItem += IDThief.item_change;
            Exiled.Events.Handlers.Player.Verified += profiles.OnPlayerJoined;
            Exiled.Events.Handlers.Player.Left += profiles.OnPlayerLeave;
            Exiled.Events.Handlers.Player.Spawned += IDThief.Player_Spawned;
            Exiled.Events.Handlers.Player.FlippingCoin += CoinFlip.Player_FlippingCoin;
            Exiled.Events.Handlers.Player.Hurting += theNut.scp173DMG;
            Exiled.Events.Handlers.Scp173.Blinking += theNut.scp173TP;
            Exiled.Events.Handlers.Scp173.UsingBreakneckSpeeds += theNut.scp173ZOOM;
            Exiled.Events.Handlers.Scp106.Attacking += larry.pdExits;
            Exiled.Events.Handlers.Scp049.ActivatingSense += doctor.doctorBoost;
            Exiled.Events.Handlers.Scp049.SendingCall += doctor.call;
            Exiled.Events.Handlers.Scp3114.Disguised += boner.OnDisguise;
            Exiled.Events.Handlers.Scp3114.Revealed += boner.OnReveal;
            Exiled.Events.Handlers.Player.Spawned += corruptGuard.spawned;
            Exiled.Events.Handlers.Player.Shot += corruptGuard.shot;
            Exiled.Events.Handlers.Scp914.UpgradingPickup += Modules.Gamemode_Handler.Core._914.OnItem914;
            Exiled.Events.Handlers.Player.Hurting += Modules.Gamemode_Handler.Core.DamageModifiers.SetDamageModifiers;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStart;
            Exiled.Events.Handlers.Player.Spawned += doctor.Player_Spawned;
            Exiled.Events.Handlers.Player.Joined += Player_Joined;
            Exiled.Events.Handlers.Server.RestartingRound += restarting;
            Exiled.Events.Handlers.Player.ChangedItem += Items.ItemUtils.item_change;
            Exiled.Events.Handlers.Warhead.Detonated += Modules.Gamemode_Handler.Core.MapInteractions.map_nuked;
            Exiled.Events.Handlers.Player.UsingItemCompleted += usingItem;
            Exiled.Events.Handlers.Player.Left += left;
            Exiled.Events.Handlers.Player.Verified += joinMsg;
            Exiled.Events.Handlers.Player.Dying += died;
            Exiled.Events.Handlers.Server.RespawningTeam += chaos.chaosroundRespawnWave;
            #region to split events
            //Exiled.Events.Handlers.Player.Hurting += theThing;
            //Exiled.Events.Handlers.Server.RoundStarted += OnRoundStart;
            //Exiled.Events.Handlers.Player.Spawned += Player_Spawned;
            //Exiled.Events.Handlers.Player.Joined += Player_Joined;
            //Exiled.Events.Handlers.Player.PreAuthenticating += Authing;
            //Exiled.Events.Handlers.Server.RestartingRound += restarting;
            //Exiled.Events.Handlers.Player.ChangedItem += item_change;
            //Exiled.Events.Handlers.Warhead.Detonated += map_nuked;
            //Exiled.Events.Handlers.Player.UsingItemCompleted += usingItem;
            //Exiled.Events.Handlers.Player.Left += left;
            //Exiled.Events.Handlers.Player.Verified += joinMsg;
            //Exiled.Events.Handlers.Player.Dying += died;
            #endregion


            CustomItem.RegisterItems();
        }

        public ItemConfigs.Items ItemConfigs { get; private set; } = null!;

        private void LoadItems()
        {
            if (!Directory.Exists(spireConfigLoc + "/CustomItems/"))
                Directory.CreateDirectory(spireConfigLoc + "/CustomItems/");
            string filePath = spireConfigLoc + "/CustomItems/global.yml";
            if (!File.Exists(filePath))
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
        #endregion

        #region Variables
        public static float hidDPS;
        public static float cokeDPS;

        public static OverrideData OScp049;
        public static OverrideData OScp0492;
        public static OverrideData OScp079;
        public static OverrideData OScp096;
        public static OverrideData OScp106;
        public static OverrideData OScp173;
        public static OverrideData OScp939;
        public static OverrideData OScp3114;
        public static int Scp3114DMG;
        public static OverrideData OCaptain;

        public static ScalingData Scp049;
        public static ScalingData Scp0492;
        public static ScalingData Scp079;
        public static ScalingData Scp096;
        public static ScalingData Scp106;
        public static ScalingData Scp173;
        public static ScalingData Scp939;
        public static ScalingData Scp3114;

        public static bool classd;
        public static bool scientist;
        public static bool guard;

        public static string[] file;

        public static int lpc = 0;

        public static int hintHeight = 0;
        public static int hintTime = 0;

        public static bool inLobby = false;

        public static List<Player> PlayerList = new List<Player>();

        public static UnityEngine.Vector3 lobbyVector;

        public static string lastId;

        public static bool limitMet = false;

        public static bool first = false;
        public static bool allowStart = false;
        public static int playerCount = 0;

        public static bool hasRestarted = false;
        public static bool ConMet = false;

        public static bool realRoundEnd = false;
        public static bool startingRound = false;
        public static bool initing = false;

        internal static bool[] corruptGuards = new bool[60];

        public static bool isLobbyEnabledConfig = false;

        public static string spireConfigLoc;

        private Harmony _harmony;

        public static List<PlayerPrimitive> OBJLST = new List<PlayerPrimitive>();

        public static bool IsActiveEventround = false;

        public static string EventRoundType = "";
        #endregion

        #region RoundStart
        void OnRoundStart()
        {
            Timing.KillCoroutines("flockerRoutine");
            Timing.KillCoroutines("lockRoutine");
            Timing.KillCoroutines("chaosChecker");
            SCPHandler.doSCPThings();
            Timing.RunCoroutine(Modules.Gamemode_Handler.Core.ChaosCounter.chaosUpdate(), tag: "chaosChecker");
            Timing.RunCoroutine(Modules.Gamemode_Handler.Core.MapInteractions.randomFlicker(), tag: "flickerRoutine");
            if (!IsActiveEventround)
            {
                gamemodeHandler.WriteAllGMInfo(false, -1, gamemodeHandler.ReadNext());
                gamemodeHandler.AttemptGMRound(false, -1);
            }
            Log.Info("Round has started!");
            Timing.RunCoroutine(lockAnounce(), tag: "lockRoutine");
            Timing.RunCoroutine(corruptGuard.initcantShoot());
            foreach (Door d in Door.List)
            {
                if (d.Zone == ZoneType.Surface)
                    d.Lock(9999, DoorLockType.Regular079);
                switch (d.Type)
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

            //foreach (Player p in PlayerList)
            //{
            //    Timing.RunCoroutine(antiFall.antiFallRoutine(p));
            //}
        }
        #endregion
#region coreFunctionality
        private IEnumerator<float> lockAnounce()
        {
            yield return Timing.WaitForSeconds(420);
            if (Manager.checkLoop()) { }
            else
            {
                Cassie.Message(@"jam_043_3 Surface armory has been opened for all jam_020_3 pitch_0.8 warhead pitch_1 authorized personnel . . . enter with pitch_0.9 jam_010_1 caution", false, false, true);
                foreach (Door d in Door.List)
                {
                    if (d.Zone == ZoneType.Surface)
                        d.Unlock();
                    switch (d.Type)
                    {
                        case DoorType.NukeSurface: d.Unlock(); break;
                        case DoorType.EscapePrimary: d.Unlock(); break;
                        case DoorType.EscapeSecondary: d.Unlock(); break;
                        case DoorType.ElevatorGateA: d.Unlock(); break;
                        case DoorType.ElevatorGateB: d.Unlock(); break;
                        case DoorType.SurfaceGate: d.Unlock(); break;
                    }

                }
            }
        }

        private void Player_Joined(JoinedEventArgs ev)
        {
            PlayerList.Add(ev.Player);
            lastId = string.Empty;
            ConMet = false;
            playerCount++;
            Log.Info($"Player count is now at: \"{playerCount}\"");

        }

        private void restarting()
        {
            PlayerList.Clear();
            playerCount = 0;
            Manager.killLoop(true);
            //if (realRoundEnd)
            //{
            //    Exiled.API.Features.Log.Info("Restarting the round (real restart)");
            //    hasRestarted = false;
            //    first = false;
            //    inLobby = false;
            //    allowStart = false;
            //    realRoundEnd = false;
            //    playerCount = 0;
            //    ConMet = false;
            //    startingRound = false;
            //    initing = false;
            //    Exiled.Events.Handlers.Player.Left -= Player_Leave;
            //}
        }

        private void Player_Leave(LeftEventArgs ev)
        {
            PlayerList.Remove(ev.Player);
            if (lastId != ev.Player.UserId)
            {
                playerCount--;
                Log.Info($"Player count is now: \"{playerCount}\"");
            }
            lastId = ev.Player.UserId;
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

        private void left(LeftEventArgs ev)
        {
            Manager.SendJoinLeave(ev.Player, 'l');
        }

        private void joinMsg(VerifiedEventArgs ev)
        {

            Manager.SendJoinLeave(ev.Player, 'j');
            foreach (Player p in PlayerList) { Log.Info($"Playername: {p.Nickname} joined with ID: {p.Id}"); }
        }

        private void died(DyingEventArgs ev)
        {
            ev.Player.Scale = new Vector3(1, 1, 1);
        }
        #endregion
    }
}
