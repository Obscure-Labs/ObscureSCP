using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Loader;
using MEC;
using ObscureLabs.API.Features;
using ObscureLabs.Items;
using ObscureLabs.Modules.Gamemode_Handler.Core;
using PlayerRoles;
using SpireLabs.GUI;
using SpireSCP.GUI.API.Features;
using System;
using System.Collections.Generic;
using System.IO;
using ObscureLabs.Configs;
using UnityEngine;
using ObscureLabs.Modules.Default;
using ObscureLabs.Modules;
using Exiled.API.Features.Core.UserSettings;
using TMPro;
using UserSettings.ServerSpecific;
using ObscureLabs.Modules.Gamemode_Handler.Core.SCP_Rebalances;
using Player = Exiled.API.Features.Player;
using Cassie = Exiled.API.Features.Cassie;
using UserSettings.ControlsSettings;
using ObscureLabs.SpawnSystem;
using HarmonyLib;
using CustomPlayerEffects;
using ObscureLabs.Modules.Gamemode_Handler;
using ObscureLabs.Hud;

namespace ObscureLabs
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance { get; private set; }

        public ModulesManager _modules { get; set; }

        public static string SpireConfigLocation { get; private set; }

        public static string[] file;

        public override string Name => "Obscure Labs";

        public override string Author => "ImIsaacTbh & ImKevin";

        public override Version Version { get; } = new Version(3, 0, 0);

        public override Version RequiredExiledVersion { get; } = new Version(9, 0, 0);

        public OverrideConfig overrideConfigs { get; set; }
        public List<KeyCode> KeybindList { get; set; } = new List<KeyCode>
        {
            KeyCode.H
        };

        //[HarmonyPatch(typeof(PlayerEffectsController), nameof(PlayerEffectsController.TryGetEffect))]
        //class Patch
        //{
        //    public bool TryGetEffect<T>(out T playerEffect) where T : StatusEffectBase
        //    {
        //        StatusEffectBase statusEffectBase;
        //        if (this._effectsByType.TryGetValue(typeof(T), out statusEffectBase))
        //        {
        //            T t = statusEffectBase as T;
        //            if (t != null)
        //            {
        //                playerEffect = t;
        //                return true;
        //            }
        //        }
        //        playerEffect = default(T);
        //        return false;
        //    }
        //}


        public override void OnEnabled()
        {
            Instance = this;
            _modules = new ModulesManager();
            CustomItem.RegisterItems();
            foreach(var key in KeybindList)
            {
                keybinds.Add(new KeybindSetting(keybinds.Count+1, key.ToString(), key));
            }
            //Log.SendRaw("[ObscureLabs]\n\r\n .d8888b.           d8b                 .d8888b.   .d8888b.  8888888b.  \r\nd88P  Y88b          Y8P                d88P  Y88b d88P  Y88b 888   Y88b \r\nY88b.                                  Y88b.      888    888 888    888 \r\n \"Y888b.   88888b.  888 888d888 .d88b.  \"Y888b.   888        888   d88P \r\n    \"Y88b. 888 \"88b 888 888P\"  d8P  Y8b    \"Y88b. 888        8888888P\"  \r\n      \"888 888  888 888 888    88888888      \"888 888    888 888        \r\nY88b  d88P 888 d88P 888 888    Y8b.    Y88b  d88P Y88b  d88P 888        \r\n \"Y8888P\"  88888P\"  888 888     \"Y8888  \"Y8888P\"   \"Y8888P\"  888        \r\n           888                                                          \r\n           888                                                          \r\n           888                                                          \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n                                                                        \r\n", color: ConsoleColor.DarkMagenta);
            Log.SendRaw(@"[ObscureLabs]

 ▄██████▄ ▀█████████▄    ▄████████  ▄████████ ███    █▄     ▄████████   ▄████████
███    ███  ███    ███  ███    ███ ███    ███ ███    ███   ███    ███  ███    ███
███    ███  ███    ███  ███    █▀  ███    █▀  ███    ███   ███    ███  ███    █▀ 
███    ███ ▄███▄▄▄██▀   ███        ███        ███    ███  ▄███▄▄▄▄██▀ ▄███▄▄▄    
███    ███▀▀███▀▀▀██▄ ▀███████████ ███        ███    ███ ▀▀███▀▀▀▀▀  ▀▀███▀▀▀    
███    ███  ███    ██▄         ███ ███    █▄  ███    ███ ▀███████████  ███    █▄ 
███    ███  ███    ███   ▄█    ███ ███    ███ ███    ███   ███    ███  ███    ███
 ▀██████▀ ▄█████████▀  ▄████████▀  ████████▀  ████████▀    ███    ███  ██████████
                                                           ███    ███            
", ConsoleColor.DarkMagenta);
            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/EXILED/Configs/Obscure/"))
            
            {
                SpireConfigLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/EXILED/Configs/Obscure/";
            }
            else
            {
                Log.Info("Making Spire Config Folder");
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/EXILED/Configs/Obscure/");
                SpireConfigLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/EXILED/Configs/Obscure/";
                File.WriteAllText(SpireConfigLocation + "lines.txt", "CHANGE ME IN :  [EXILEDCONIG]/Obscure/lines.txt");
            }
            PopulateModules();
            FetchOverrides();
            Log.Info($"Found Spire Config Folder : \"{SpireConfigLocation}\"");
            
        }

        public override void OnDisabled()
        {
            _modules.Clear();

            Log.Info("Spire Labs has been disabled!");
            base.OnDisabled();
        }

        public void PopulateModules()
        {

            //- Core Utils -//
            _modules.AddModule(new GamemodeManager());
            _modules.AddModule(new HudController());
            _modules.AddModule(new MvpSystem());
            _modules.AddModule(new CustomItemSpawner());
            _modules.AddModule(new RemoteKeycard());
            _modules.AddModule(new LightHandler());
            _modules.AddModule(new Lobby());
            _modules.AddModule(new ItemRarityModule());
            _modules.AddModule(new HealthOverride());
            _modules.AddModule(new EffectController());

            //- Gameplay Utils -//
            _modules.AddModule(new Powerup());
            _modules.AddModule(new ItemGlow());

            //- Mechanics and Features -//
            _modules.AddModule(new CoinFlip());
            _modules.AddModule(new AttachmentFix());
            _modules.AddModule(new SCPsDropItems());

            //- SCP Additions and rebalances -//
            _modules.AddModule(new Scp1162());
            _modules.AddModule(new SCP106());
            _modules.AddModule(new SCP173());
            _modules.AddModule(new Scp049());

            _modules.AddModule(new Scp914Handler());

            //- Fun modules -//
            _modules.AddModule(new RoundEndPVP());
            _modules.AddModule(new EmotionRandomiser());

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

            _modules.GetModule("ItemRarity").Enable();
            _modules.GetModule("GamemodeManager").Enable();

            //foreach (Module m in _modules.Modules)
            //{
            //    if (m.IsInitializeOnStart == true)
            //    {
            //        m.Enable();
            //    }
            //    else
            //    {
            //        continue;
            //    }

            //}
        }

        private void OnRoundStarted()
        {
            HudRenderer.fontAsset = TMP_FontAsset.CreateFontAsset(SpireConfigLocation + "scoopFont.otf", "OliversBarney-Regular", 16);
            Timing.KillCoroutines("flockerRoutine");
            Timing.KillCoroutines("lockRoutine");
            Timing.KillCoroutines("chaosChecker");

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
            Timing.RunCoroutine(HudRenderer.RenderUI(ev.Player.ReferenceHub), "guiRoutine");
        }

        private void OnRestarting()
        {
            foreach (Module m in _modules.Modules)
            {
                m.Disable();
            }

            foreach (Module m in _modules.Modules)
            {
                if (m.IsInitializeOnStart == true)
                {
                    m.Enable();
                }
            }
        }

        private void OnLeft(LeftEventArgs ev)
        {
            Manager.SendJoinLeave(ev.Player, true);
        }

        //public static UserTextInputSetting XresInput = new UserTextInputSetting(0, "Resolution X", "1920", 4, TMP_InputField.ContentType.IntegerNumber, "Used for UI Scaling");
        //public static UserTextInputSetting YresInput = new UserTextInputSetting(1, "Resolution Y", "1080", 4, TMP_InputField.ContentType.IntegerNumber, "Used for UI Scaling");
        public static List<KeybindSetting> keybinds = new List<KeybindSetting>();

        private void OnVerified(VerifiedEventArgs ev)
        {
            //ServerSpecificSettingsSync.SendToPlayer(ev.Player.ReferenceHub, settings.ToArray());
            //ServerSpecificSettingsSync.ServerOnStatusReceived += (p, s) =>
            //{
            //    Log.Warn($"{Player.Get(p).Nickname} has aspect ratio : {p.aspectRatioSync.AspectRatio} : and their status is now {s.Version}");
            //};
            //Manager.SendHint(ev.Player, $"{ev.Player.DisplayNickname}", 3);
            Manager.SendJoinLeave(ev.Player, false);
            foreach (Player p in Player.List) { Log.Info($"Playername: {p.Nickname} joined with ID: {p.Id}"); }
        }

        private void OnDying(DyingEventArgs ev)
        {
            ev.Player.Scale = new Vector3(1, 1, 1);
        }


    }
}
