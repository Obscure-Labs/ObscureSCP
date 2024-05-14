using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
            CustomItem.RegisterItems();
            PopulateModules();

            


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


        
        public class Module
        {
            public virtual string name { get; set; }
            public virtual bool initOnStart { get; set; } = false;
            public virtual bool Init()
            {
                Log.Warn($"Module {name} Initialized successfully");
                return true;
            }
            public virtual bool Disable()
            {
                Log.Warn($"Module {name} Disabled successfully");
                return true;
            }

        }


        public class ModuleINATOR
        {
            public List<Module> moduleList = new List<Module>();

            public virtual Module GetModule(string name)
            {
                return moduleList.FirstOrDefault(x => x.name.ToLower() == name.ToLower());
            }

            public virtual void AddModule(Module module)
            {
                moduleList.Add(module);
            }
        }
        
        public static ModuleINATOR modules = new ModuleINATOR();

        public void PopulateModules()
        {
            modules.AddModule(new MvpSystem());
            modules.AddModule(new corruptGuard());
            modules.AddModule(new DamageModifiers());
            modules.AddModule(new CoinFlip());
            modules.AddModule(new theNut());
            modules.AddModule(new profiles());
            modules.AddModule(new IDThief());
            modules.AddModule(new larry());
            modules.AddModule(new gamemodeHandler());
            modules.AddModule(new MapInteractions());
            modules.AddModule(new chaos());


            modules.AddModule(new CustomItemSpawner());


            RegisterEvents();
        }

        private void RegisterEvents()
        {
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStart;
            Exiled.Events.Handlers.Player.Joined += Player_Joined;
            Exiled.Events.Handlers.Server.RestartingRound += restarting;
            Exiled.Events.Handlers.Player.Left += left;
            Exiled.Events.Handlers.Player.Verified += joinMsg;
            Exiled.Events.Handlers.Player.Dying += died;



            foreach (Module m in modules.moduleList)
            {
                Log.Warn(m.name);
                if(m.initOnStart)
                {
                    m.Init();
                }
            }


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
            Timing.RunCoroutine(ChaosCounter.chaosUpdate(), tag: "chaosChecker");


            Log.Info("Round has started!");
            Timing.RunCoroutine(lockAnounce(), tag: "lockRoutine");

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
            foreach (Module m in modules.moduleList)
            {
                m.Disable();
            }
            Timing.KillCoroutines("juggerwave");
            PlayerList.Clear();
            playerCount = 0;
            Manager.killLoop(true);
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
