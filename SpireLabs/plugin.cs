using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Loader;
using MEC;
using ObscureLabs.API.Features;
using ObscureLabs.Configs;
using ObscureLabs.Hud;
using SpireSCP.GUI.API.Features;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using Player = Exiled.API.Features.Player;
using System.CodeDom;
using HarmonyLib;
using ObscureLabs.Modules.Gamemode_Handler;

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

        public static Vector3 PlayerDefaultGravity { get; } = new Vector3(0, -19.60f, 0);

        public override void OnEnabled()
        {
            Instance = this;
            _modules = new ModulesManager();
            CustomItem.RegisterItems();
            //foreach (var key in KeybindList)
            //{
            //    keybinds.Add(new KeybindSetting(keybinds.Count + 1, key.ToString(), key));
            //}
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

        public unsafe void PopulateModules()
        {
            _modules.AddModule(new GamemodeManager());
            _modules.AddModules(*ReflctyScrip.GetStartupModules());
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

        private unsafe void RegisterEvents()
        {
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Player.Joined += OnPlayerJoined;
            Exiled.Events.Handlers.Server.RestartingRound += OnRestarting;
            Exiled.Events.Handlers.Player.Left += OnLeft;
            Exiled.Events.Handlers.Player.Verified += OnVerified;

            _modules.GetModule("GamemodeManager").Enable();

            foreach (Module m in _modules.Modules)
            {
                if(m.IsInitializeOnStart == true)
                    m.Enable();
            }
        }

        private void OnRoundStarted()
        {
            HudRenderer.fontAsset = TMP_FontAsset.CreateFontAsset(SpireConfigLocation + "scoopFont.otf", "OliversBarney-Regular", 16);

            Log.Info("Round has started!");
        }

        private void OnPlayerJoined(JoinedEventArgs ev)
        {
            Log.Info($"Player count is now at: \"{Player.List.Count}\"");
            Timing.RunCoroutine(HudRenderer.RenderUI(ev.Player.ReferenceHub), "guiRoutine");
        }

        private unsafe void OnRestarting()
        {
            string[] modNames = new string[_modules.Modules.Count];
            for (int i = 0; i < modNames.Length; i++)
            {
                modNames[i] = _modules.Modules[i].Name;
            }
            foreach (string s in modNames)
            {
                var m = _modules.GetModule(s);
                if(m.IsInitializeOnStart == true)
                    m.Disable();
            }
            _modules.GetModule("GamemodeManager").Disable();
            _modules.Clear();
            _modules.AddModule(new GamemodeManager());
            _modules.AddModules(*ReflctyScrip.GetStartupModules());
            _modules.GetModule("GamemodeManager").Enable();
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
    }
    public unsafe static class ReflctyScrip
    {
        public static List<Module>* GetStartupModules()
        {
            var interfaceType = typeof(Module);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(ass => ass.GetTypes())
                .Where(type => interfaceType.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);
            List<Module> modules = new List<Module>();
            foreach (Type t in types)
            {
                var lad = (Module)Activator.CreateInstance(t);
                if(lad.IsInitializeOnStart)
                {
                    modules.Add(lad);
                }
            }
            return &modules;
        }
    }
}
