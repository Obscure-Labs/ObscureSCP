using Exiled.API.Features;
using Exiled.API.Features.Core.UserSettings;
using ObscureLabs.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using UserSettings.ServerSpecific;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    internal class SSSStuff : Module
    {
        public override string Name => "SSSStuff";
        public override bool IsInitializeOnStart => true;
        public override bool Enable()
        {
            HeaderSetting header = new HeaderSetting("ObscureLabs");
            IEnumerable<SettingBase> settingBases = new SettingBase[]
            {
                    header,
                    new KeybindSetting(200, "SCP Voicechat Toggle", UnityEngine.KeyCode.LeftAlt, hintDescription: "Toggles the scp voice chat"),
            };

            SettingBase.Register(settingBases);
            SettingBase.SendToAll();

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


        private void OnSettingValueReceived(ReferenceHub hub, ServerSpecificSettingBase settingBase)
        {
            if (!Player.TryGet(hub, out Player player) || player.Role.Team == PlayerRoles.Team.SCPs)
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
