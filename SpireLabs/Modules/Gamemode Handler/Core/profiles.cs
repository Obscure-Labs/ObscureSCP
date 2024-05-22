using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using ObscureLabs.API.Data;
using ObscureLabs.API.Features;
using ObscureLabs.API.Serializable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ObscureLabs
{
    public class Profiles : Module
    {
        public override string Name => "Profiles";

        public override bool IsInitializeOnStart => true;

        private static readonly string folder = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\EXILED\\Configs\\Spire\\Profiles/");

        public static List<ProfileData> _profilesData = new();

        private ISerializer _serializer;

        private IDeserializer _deserializer;

        public override bool Enable()
        {
            Exiled.Events.Handlers.Player.Verified += OnPlayerJoined;
            Exiled.Events.Handlers.Player.Left += OnPlayerLeave;

            _deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

            _serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

            return base.Enable();
        }

        public override bool Disable()
        {
            _profilesData.Clear();
            Exiled.Events.Handlers.Player.Verified -= OnPlayerJoined;
            Exiled.Events.Handlers.Player.Left -= OnPlayerLeave;

            return base.Disable();
        }

        private void OnPlayerLeave(LeftEventArgs ev)
        {
            _profilesData.Remove(_profilesData.FirstOrDefault(x => x.Steam64 == ev.Player.UserId));
        }

        private void OnPlayerJoined(VerifiedEventArgs ev)
        {
            Timing.RunCoroutine(DeserializeProfilesCoroutine(ev));
        }

        public void ToggleAudio(Player player)
        {
            var nD = _serializer.Serialize(new SerializableProfileData(player));

            var profile = _profilesData.FirstOrDefault(x => x.Steam64 == player.UserId);

            if (profile.AudioToggle == true)
            {
                profile.AudioToggle = false;
            }
            else
            {
                profile.AudioToggle = true;
                nD = _serializer.Serialize(new SerializableProfileData(player) { AudioToggle = true });
            }

            File.WriteAllText($"{folder}{player.UserId}.yaml", nD);
        }

        private IEnumerator<float> DeserializeProfilesCoroutine(VerifiedEventArgs ev)
        {
            yield return Timing.WaitForSeconds(0.25f);
            var profiles = Directory.GetFiles(folder);
            var player = ev.Player;

            if (profiles.Contains($"{folder}{player.UserId}.yaml"))
            {
                var raw = File.ReadAllText(profiles.FirstOrDefault(x => x == $"{folder}{player.UserId}.yaml"));
                Log.Warn(raw);
                var profile = _deserializer.Deserialize<SerializableProfileData>(raw);
                Log.Warn(profile.Steam64 + profile.AudioToggle);

                _profilesData.Add(profile.ToNonSerializable());
            }
            else
            {
                _profilesData.Add(new ProfileData(player));
                Log.Warn($"steam64 = {player.UserId}, audioToggle = false");
                var serializableProfile = new SerializableProfileData(player);
                var raw = _serializer.Serialize(serializableProfile);
                Log.Warn(raw);
                File.WriteAllText($"{folder}{player.UserId}.yaml", raw);
            }
        }
    }
}
