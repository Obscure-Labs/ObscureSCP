using Exiled.API.Features;
using ObscureLabs.API.Serializable;

namespace ObscureLabs.API.Data
{
    public class ProfileData
    {
        public ProfileData(string steam64, bool audioToggle, int uid)
        {
            Steam64 = steam64;
            AudioToggle = audioToggle;
            Id = uid;
        }

        public ProfileData(Player player)
        {
            Steam64 = player.UserId;
            Id = player.Id;
        }

        public string Steam64 { get; }

        public bool AudioToggle { get; set; }

        public int Id { get; }

        public SerializableProfileData ToSerializable()
        {
            return new SerializableProfileData(Steam64, AudioToggle, Id);
        }
    }
}
