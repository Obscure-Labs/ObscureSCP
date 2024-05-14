using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.Events.EventArgs.Player;
using UnityEngine.WSA;
using System.Runtime.InteropServices;
using MEC;
using Utf8Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ObscureLabs
{
    internal class profiles : Plugin.Module
    {
        public override string name { get; set; } = "Profiles";
        public override bool initOnStart { get; set; } = true;

        public override bool Init()
        {
            try
            {
                Exiled.Events.Handlers.Player.Verified += profiles.OnPlayerJoined;
                Exiled.Events.Handlers.Player.Left += profiles.OnPlayerLeave;
                base.Init();
                return true;
            }
            catch { return false; }
        }

        public override bool Disable()
        {
            try
            {
                Profiles.Clear();
                Exiled.Events.Handlers.Player.Verified -= profiles.OnPlayerJoined;
                Exiled.Events.Handlers.Player.Left -= profiles.OnPlayerLeave;
                base.Disable();
                return true;
            }
            catch { return false; }
        }

        public static List<personData> Profiles = new List<personData>();
        public static string folder = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\EXILED\\Configs\\Spire\\Profiles/");

        public class profile
        {
            public string steam64;
            public bool audioToggle;
        }
        public class personData
        {
            public string steam64;
            public bool audioToggle;
            public int uid;
        }

        internal static void toggleAudio(Player p)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            var nD = serializer.Serialize(new profile { steam64 = p.UserId, audioToggle = false} );
            if (Profiles.FirstOrDefault(x => x.steam64 == p.UserId).audioToggle == true)
            {
                Profiles.FirstOrDefault(x => x.steam64 == p.UserId).audioToggle = false;
            }
            else
            {
                Profiles.FirstOrDefault(x => x.steam64 == p.UserId).audioToggle = true;
                nD = serializer.Serialize(new profile { steam64 = p.UserId, audioToggle = true });
            }
            File.WriteAllText($"{folder}{p.UserId}.yaml", nD);
        }

        internal static void OnPlayerJoined(VerifiedEventArgs ev)
        {
            Timing.RunCoroutine(otherthreadfuckyou(ev));
        }

        private static IEnumerator<float> otherthreadfuckyou(VerifiedEventArgs ev)
        {
            yield return Timing.WaitForSeconds(0.25f);
            IEnumerable<string> profiles = Directory.GetFiles(folder);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

            Player p = ev.Player;
            if (profiles.Contains($"{folder}{p.UserId}.yaml"))
            {
                var raw = File.ReadAllText(profiles.FirstOrDefault(x => x == $"{folder}{p.UserId}.yaml"));
                Log.Warn(raw);
                var person = deserializer.Deserialize<profile>(raw);
                Log.Warn(person.steam64 + person.audioToggle);
                Profiles.Add(new personData { steam64 = p.UserId, audioToggle = person.audioToggle, uid = p.Id });
            }
            else 
            { 
                Profiles.Add(new personData { steam64 = p.UserId, audioToggle = false, uid = p.Id });
                Log.Warn($"steam64 = {p.UserId}, audioToggle = false");
                //var raw = JsonSerializer.Serialize(new profile { steam64 = p.UserId, audioToggle = false });
                profile baseProfile = new profile { steam64 = p.UserId, audioToggle = false };
                var raw = serializer.Serialize(baseProfile);
                Log.Warn(raw);
                File.WriteAllText($"{folder}{p.UserId}.yaml", raw);
            }
        }

        internal static void OnPlayerLeave(LeftEventArgs ev)
        {
            Profiles.Remove(Profiles.FirstOrDefault(x => x.steam64 == ev.Player.UserId));
        }
    }
}
