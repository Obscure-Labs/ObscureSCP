using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceChat.Codec;
using VoiceChat.Codec.Enums;

namespace ObscureLabs.API.Features
{
    public class OpusStuff
    {
        private static readonly Dictionary<Player, OpusStuff> Instances = new();

        public OpusDecoder Decoder { get; } = new();
        public OpusEncoder Encoder { get; } = new(OpusApplicationType.Voip);
        
        public static OpusStuff Get(Player player)  
        {
            if (!Instances.TryGetValue(player, out var instance))
            {
                instance = new OpusStuff();
                Instances[player] = instance;
            }
            return instance;
        }

        public static void Remove(Player player)
        {
            if (Instances.ContainsKey(player))
            {
                Instances[player].Decoder.Dispose();
                Instances[player].Encoder.Dispose();

                Instances.Remove(player);
            }
        }
    }
}
