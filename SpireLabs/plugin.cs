using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Loader;
using HarmonyLib;
using MEC;
using ObscureLabs.API.Data;
using ObscureLabs.API.Enums;
using ObscureLabs.API.Features;
using ObscureLabs.Gamemode_Handler;
using ObscureLabs.Items;
using ObscureLabs.Modules.Gamemode_Handler.Core;
using ObscureLabs.Modules.Gamemode_Handler.Minigames;
using ObscureLabs.SpawnSystem;
using PlayerRoles;
using SpireLabs.GUI;
using SpireSCP.GUI.API.Features;
using System;
using System.Collections.Generic;
using System.IO;
using ObscureLabs.Configs;
using UnityEngine;
using YamlDotNet.Serialization;
using ObscureLabs.Modules.Default;

namespace ObscureLabs
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance { get; private set; }

        public static string SpireConfigLocation { get; private set; }

        public static EventRoundType EventRoundType { get; internal set; }

        public static string[] file;

        public static string LastPlayerId;

        public static List<PlayerPrimitiveData> Objects = new();

        public static bool IsActiveEventround = false;

        public override string Name => "Obscure Labs";

        public override string Author => "ImIsaacTbh & ImKevin";

        public override Version Version { get; } = new Version(2, 0, 2);

        public override Version RequiredExiledVersion { get; } = new Version(8, 0, 1);

        public ItemConfigs.ItemConfig ItemConfigs { get; private set; } = null!;

        public OverrideConfig overrideConfigs { get; set; }

        private Harmony _harmony;

        public override void OnEnabled()
        {
            Instance = this;

            LoadItems();
            CustomItem.RegisterItems();

            Log.SendRaw("[ObscureLabs]\n\r\n .d8888b.           d8b                 .d8888b.   .d8888b.  8888888b.  \r\nd88P  Y88b          Y8P                d88P  Y88b d88P  Y88b 888   Y88b \r\nY88b.                                  Y88b.      888    888 888    888 \r\n \"Y888b.   88888b.  888 888d888 .d88b.  \"Y888b.   888        888   d88P \r\n    \"Y88b. 888 \"88b 888 888P\"  d8P  Y8b    \"Y88b. 888        8888888P\"  \r\n      \"888 888  888 888 888    88888888      \"888 888    888 888        \r\nY88b  d88P 888 d88P 888 888    Y8b.    Y88b  d88P Y88b  d88P 888        \r\n \"Y8888P\"  88888P\"  888 888     \"Y8888  \"Y8888P\"   \"Y8888P\"  888        \r\n           888                                                          \r\n           888                                                          \r\n           888                                                          \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n", color: ConsoleColor.DarkMagenta);
            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\EXILED\\Configs\\Obscure/"))
            {
                SpireConfigLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\EXILED\\Configs\\Obscure/";
            }
            else
            {
                Log.Info("Making Spire Config Folder");
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\EXILED\\Configs\\Obscure/");
                SpireConfigLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\EXILED\\Configs\\Obscure/";
                File.WriteAllText(SpireConfigLocation + "lines.txt", "CHANGE ME IN :  [EXILEDCONIG]\\Obscure/lines.txt");
            }
            PopulateModules();
            FetchOverrides();
            Log.Info($"Found Spire Config Folder : \"{SpireConfigLocation}\"");
        }

        public override void OnDisabled()
        {
            ModulesManager.Clear();

            Log.Info("Spire Labs has been disabled!");
            base.OnDisabled();
        }

        public void PopulateModules()
        {
            ModulesManager.AddModule(new HudController());
            ModulesManager.AddModule(new MvpSystem());
            ModulesManager.AddModule(new CorruptGuard());
            ModulesManager.AddModule(new DamageModifiers());
            ModulesManager.AddModule(new CoinFlip());
            ModulesManager.AddModule(new TheNut());
            ModulesManager.AddModule(new Profiles());
            ModulesManager.AddModule(new IDThief());
            ModulesManager.AddModule(new Larry());
            ModulesManager.AddModule(new GamemodeHandler());
            ModulesManager.AddModule(new MapInteractions());
            ModulesManager.AddModule(new Chaos());
            ModulesManager.AddModule(new Scp3114());
            ModulesManager.AddModule(new Doctor());
            ModulesManager.AddModule(new CustomItemSpawner());
            ModulesManager.AddModule(new HealthOverride());
            ModulesManager.AddModule(new Scp1162());
            ModulesManager.AddModule(new RemoteKeycard());
            ModulesManager.AddModule(new Scp914Handler());
            ModulesManager.AddModule(new ReconnectRecovery());
            ModulesManager.AddModule(new TeamHandler());
            ModulesManager.AddModule(new TDM());
            ModulesManager.AddModule(new LightHandler());
            ModulesManager.AddModule(new AttachmentFix());
            ModulesManager.AddModule(new Teamswap());
            ModulesManager.AddModule(new CustomRoles());

            RegisterEvents();
        }

        public void FetchOverrides()
        {
            if (!File.Exists(SpireConfigLocation + "PlayerOverrides.yaml"))
            {
                Log.Error("No override files exists");
                File.WriteAllText(SpireConfigLocation + "PlayerOverrides.yaml", Loader.Serializer.Serialize(new OverrideConfig()));
                overrideConfigs = new OverrideConfig();
            }
            else
            {
                overrideConfigs = Loader.Deserializer.Deserialize<OverrideConfig>(File.ReadAllText(SpireConfigLocation + "PlayerOverrides.yaml"));
                File.WriteAllText(SpireConfigLocation + "PlayerOverrides.yaml", Loader.Serializer.Serialize(overrideConfigs));
            }
        }

        private void RegisterEvents()
        {
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Player.Joined += OnPlayerJoined;
            Exiled.Events.Handlers.Server.RestartingRound += OnRestarting;
            Exiled.Events.Handlers.Player.Left += OnLeft;
            Exiled.Events.Handlers.Player.Verified += OnVerified;
            Exiled.Events.Handlers.Player.Dying += OnDying;

            foreach (Module m in ModulesManager.Modules)
            {
                if (m.IsInitializeOnStart == true)
                {
                    m.Enable();
                }
                else
                {
                    continue;
                }

            }
        }

        private void LoadItems()
        {
            if (!Directory.Exists(SpireConfigLocation + "/CustomItems/"))
                Directory.CreateDirectory(SpireConfigLocation + "/CustomItems/");
            string filePath = SpireConfigLocation + "/CustomItems/global.yml";
            if (!File.Exists(filePath))
            {
                ItemConfigs = new ItemConfigs.ItemConfig();
                File.WriteAllText(filePath, Loader.Serializer.Serialize(ItemConfigs));
            }
            else
            {
                ItemConfigs = Loader.Deserializer.Deserialize<ItemConfigs.ItemConfig>(File.ReadAllText(filePath));
                File.WriteAllText(filePath, Loader.Serializer.Serialize(ItemConfigs));
            }

        }

        private void OnRoundStarted()
        {
            Timing.KillCoroutines("flockerRoutine");
            Timing.KillCoroutines("lockRoutine");
            Timing.KillCoroutines("chaosChecker");
            Timing.RunCoroutine(ChaosCounter.ChaosUpdateCoroutine(), "chaosChecker");

            Log.Info("Round has started!");
            Timing.RunCoroutine(OnLockAnnouncement(), "lockRoutine");

            foreach (var door in Door.List)
            {
                if (door.Zone == ZoneType.Surface)
                {
                    door.Lock(Mathf.Infinity, DoorLockType.Regular079);
                }

                switch (door.Type)
                {
                    case DoorType.NukeSurface: door.Unlock(); break;
                    case DoorType.EscapePrimary: door.Unlock(); break;
                    case DoorType.EscapeSecondary: door.Unlock(); break;
                    case DoorType.ElevatorGateA: door.Unlock(); break;
                    case DoorType.ElevatorGateB: door.Unlock(); break;
                    case DoorType.SurfaceGate: door.Unlock(); break;
                }

            }

            List<Player> SCPS = new();
            var humanPlayers = 0;
            foreach (var p in Player.List)
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
                        p.MaxHealth = overrideConfigs.HealthOverrides[RoleTypeId.NtfCaptain].Health;
                        p.Heal(150, false);
                        break;
                }
            }
        }

        private IEnumerator<float> OnLockAnnouncement()
        {
            yield return Timing.WaitForSeconds(420);
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

        private void OnPlayerJoined(JoinedEventArgs ev)
        {
            Log.Info($"Player count is now at: \"{Player.List.Count}\"");
        }

        private void OnRestarting()
        {
            foreach (Module m in ModulesManager.Modules)
            {
                m.Disable();
            }

            Timing.KillCoroutines("juggerwave");

            foreach (Module m in ModulesManager.Modules)
            {
                if (m.IsInitializeOnStart == true)
                {
                    m.Enable();
                }
            }
        }

        private void OnPlayerLeave(LeftEventArgs ev)
        {
            Log.Info($"Player count is now: \"{Player.List.Count}\"");
        }

        private void OnLeft(LeftEventArgs ev)
        {
            Manager.SendJoinLeave(ev.Player, true);
        }

        private void OnVerified(VerifiedEventArgs ev)
        {
            Manager.SendHint(ev.Player, $"{ev.Player.DisplayNickname}", 3);
            Manager.SendJoinLeave(ev.Player, false);
            foreach (Player p in Player.List) { Log.Info($"Playername: {p.Nickname} joined with ID: {p.Id}"); }
        }

        private void OnDying(DyingEventArgs ev)
        {
            ev.Player.Scale = new Vector3(1, 1, 1);
        }
    }
}
