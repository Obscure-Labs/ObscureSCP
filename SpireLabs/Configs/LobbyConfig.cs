using System.ComponentModel;
using System.Numerics;

namespace ObscureLabs.Configs
{
    public class LobbyConfig
    {
        [Description("Lobby Data")]
        public Vector3 SpawnRoomVector3 { get; set; } = new Vector3(-9.94f, 1005, 82.37f);

        [Description("Use Lobby Function?")]
        public bool LobbyEnabled { get; set; }
    }
}
