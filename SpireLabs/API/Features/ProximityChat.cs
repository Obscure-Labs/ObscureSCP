using Exiled.Events.EventArgs.Player;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObscureLabs.Extensions;
using AdminToys;
using Mirror;
using SpireSCP.GUI.API.Features;
using UnityEngine;
using Exiled.API.Extensions;
using VoiceChat.Networking;
using Exiled.API.Features.Roles;

namespace ObscureLabs.API.Features
{
    internal class ProximityChat : Module
    {
        public override string Name => "ProximityChat";
        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {
            ProximityEnabled.Clear();
            Exiled.Events.Handlers.Player.VoiceChatting += OnVoiceChatting;
            Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;
            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Player.VoiceChatting -= OnVoiceChatting;
            Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
            return base.Disable();
        }

        public static Dictionary<Player, SpeakerToy> ProximityEnabled = new();

        private void OnVoiceChatting(VoiceChattingEventArgs ev)
        {
            Player p = ev.Player;

            if (ev.VoiceMessage.Channel != VoiceChat.VoiceChatChannel.ScpChat)
                return;

            if(ProximityEnabled.TryGetValue(p, out SpeakerToy s))
            {
                OpusStuff o = OpusStuff.Get(p);
                float[] decoded = new float[480];
                o.Decoder.Decode(ev.VoiceMessage.Data, ev.VoiceMessage.DataLength, decoded);

                for(int i = 0; i < decoded.Length; i++)
                {
                    decoded[i] *= 10f;
                }

                byte[] encoded = new byte[512];
                int dataLen = o.Encoder.Encode(decoded, encoded);

                AudioMessage a = new AudioMessage(s.ControllerId, encoded, dataLen);
                foreach(Player target in Player.List)
                {
                    if (target.Role is not IVoiceRole voicerole || voicerole.VoiceModule.ValidateReceive(p.ReferenceHub, VoiceChat.VoiceChatChannel.Proximity) == VoiceChat.VoiceChatChannel.None)
                        continue;

                    if (target.IsScp)
                        continue;

                    target.ReferenceHub.connectionToClient.Send(a);
                }

                ev.IsAllowed = true;
            }
        }

        private void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (ProximityEnabled.ContainsKey(ev.Player))
            {
                ToggleProximity(ev.Player);
            }

            if (ev.NewRole.GetTeam() == PlayerRoles.Team.SCPs)
            {
                Manager.SendHint(ev.Player, "You can toggle proximity chat by pressing the keybind set in the settings menu.", 5f);
            }
        }

        public static void ToggleProximity(Player player)
        {
            if(ProximityEnabled.ContainsKey(player))
            {
                NetworkServer.Destroy(ProximityEnabled[player].gameObject);
                ProximityEnabled.Remove(player);

                OpusStuff.Remove(player);

                Manager.SendHint(player, "<color=red>Proximity Chat Disabled</color>", 3);
            }
            else
            {
                SpeakerToy speaker = UnityEngine.Object.Instantiate(PrefabHelper.GetPrefab<SpeakerToy>(Exiled.API.Enums.PrefabType.SpeakerToy), player.Transform, true);
                NetworkServer.Spawn(speaker.gameObject);
                speaker.NetworkControllerId = (byte)player.Id;
                speaker.NetworkMinDistance = 2f;
                speaker.NetworkMaxDistance = 10f;
                speaker.transform.position = player.Position;

                ProximityEnabled.Add(player, speaker);

                Manager.SendHint(player, "<color=green>Proximity Chat Enabled</color>", 3);
            }
        }
    }
}
