using Exiled.API.Features;
using ObscureLabs.API.Data;

namespace ObscureLabs.API.Serializable
{
    public class SerializableProfileData
    {
        public SerializableProfileData() { }

        public SerializableProfileData(string steam64, bool audioToggle, int uid)
        {
            Steam64 = steam64;
            AudioToggle = audioToggle;
            Id = uid;
        }

        public SerializableProfileData(Player player)
        {
            Steam64 = player.UserId;
            Id = player.Id;
        }

        public string Steam64 { get; set; }

        public bool AudioToggle { get; set; }

        public int Id { get; set; }

        public ProfileData ToNonSerializable()
        {
            return new ProfileData(Steam64, AudioToggle, Id);
        }
    }
}
