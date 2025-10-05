using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using HarmonyLib;
using ObscureLabs.API.Features;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using UserSettings.ServerSpecific;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    internal class SSSStuff : Module
    {
        public override string Name => "SSSStuff";
        public override bool IsInitializeOnStart => true;
        public static HeaderSetting header = new HeaderSetting(0, "ObscureLabs", "Hints for the ObscureLabs server");
        public static List<SettingBase> settingBases = new List<SettingBase>
        {
                    header,
                    new KeybindSetting(200, "SCP Voicechat Toggle", UnityEngine.KeyCode.LeftAlt, hintDescription: "Toggles the scp voice chat"),
        };
        public override bool Enable()
        {



            SettingBase.Register(settingBases);

            ServerSpecificSettingsSync.ServerOnSettingValueReceived += OnSettingValueReceived;
            Exiled.Events.Handlers.Player.Verified += (ev) =>
            {
                ServerSpecificSettingsSync.SendToPlayer(ev.Player.ReferenceHub);
            };

            return base.Enable();
        }

        public override bool Disable()
        {

                ServerSpecificSettingsSync.ServerOnSettingValueReceived -= OnSettingValueReceived;
                return base.Disable();
        }

        private void AddSettingToAll(SettingBase b)
        {
            settingBases.AddItem(b);
            foreach (Player p in Player.List)
            {
                SettingBase.Unregister(p);
                SettingBase.Register(settingBases);
                ServerSpecificSettingsSync.SendToPlayer(p.ReferenceHub);
            }
        }

        private void RemoveSettingForAll(int id)
        {
            settingBases.Remove(settingBases.FirstOrDefault(x => x.Id == id));
            foreach(Player p in Player.List)
            {
                SettingBase.Unregister(p);
                SettingBase.Register(settingBases);
                ServerSpecificSettingsSync.SendToPlayer(p.ReferenceHub);
            }
        }

        private void OnSettingValueReceived(ReferenceHub hub, ServerSpecificSettingBase settingBase)
        {
            if (!Player.TryGet(hub, out Player player) || player.Role.Team != PlayerRoles.Team.SCPs)
                return;

            if (settingBase is SSKeybindSetting keyindSetting && keyindSetting.SyncIsPressed)
            {
                SSKeybindSetting keybindSetting = settingBase as SSKeybindSetting;
                switch (keybindSetting.SettingId)
                {
                    case 200: ProximityChat.ToggleProximity(player); break;
                }
            }
        }
    }
}
