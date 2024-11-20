using Exiled.Events.EventArgs.Player;
using ObscureLabs.API.Features;
using PlayerRoles;
using SpireSCP.GUI.API.Features;


namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    internal class ScpProximityChat : Module
    {
        public override string Name => "ScpProximityChat";

        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {

            Exiled.Events.Handlers.Player.TogglingNoClip += OnToggleChannel;
            return base.Enable();
        }

        public override bool Disable()
        {

            Exiled.Events.Handlers.Player.TogglingNoClip -= OnToggleChannel;
            return base.Disable();
        }


        public static void OnToggleChannel(TogglingNoClipEventArgs ev)
        {
            if (ev != null && ev.Player.Role.Team == PlayerRoles.Team.SCPs)
            {
                var p = ev.Player;
                var role = ev.Player.Role;
                if (role == RoleTypeId.Scp3114 || role == RoleTypeId.Scp106 || role == RoleTypeId.Scp049 || role == RoleTypeId.Scp939 || role == RoleTypeId.Scp096)
                {
                    if (p.VoiceChannel == VoiceChat.VoiceChatChannel.ScpChat)
                    {
                        p.VoiceChannel = VoiceChat.VoiceChatChannel.Proximity;
                        Manager.SendHint(p, "<color=lime>Proximity Chat<color=white>", 2f);
                    }
                    else
                    {
                        p.VoiceChannel = VoiceChat.VoiceChatChannel.ScpChat;
                        Manager.SendHint(p, "<color=red>SCP Chat<color=white>", 2f);
                    }
                }
                else
                {
                    Manager.SendHint(p, "This SCP Cannot Communicate with Humans", 3f);
                }

            }
            else
            {
                return;
            }
        }
    }
}
