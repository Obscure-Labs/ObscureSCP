
using Exiled.API.Interfaces;
using ObscureLabs.API.Data;
using ObscureLabs.Configs;
using PlayerRoles;
using System.Collections.Generic;
using System.ComponentModel;

namespace ObscureLabs
{
    public class Config : IConfig
    {
        [Description("Plugin toggle.")]
        public bool IsEnabled { get; set; }
        [Description("Plugin console output.")]
        public bool Debug { get; set; }

        [Description("Sets Various Items' DPS.")]
        public int HidDPS { get; set; } = 100;

        [Description("Dps.")]
        public int CokeDPS { get; set; } = 100;

        [Description("Health override")]
        public Dictionary<RoleTypeId, HealthData> HealthOverrides { get; set; } = new()
        {
            { RoleTypeId.Scp049, new HealthData() },
            { RoleTypeId.Scp0492, new HealthData() },
            { RoleTypeId.Scp079, new HealthData() },
            { RoleTypeId.Scp096, new HealthData() },
            { RoleTypeId.Scp106, new HealthData() },
            { RoleTypeId.Scp173, new HealthData() },
            { RoleTypeId.Scp939, new HealthData() },
            { RoleTypeId.Scp3114, new HealthData(true, 10, 0) },
            { RoleTypeId.NtfCaptain, new HealthData() }
        };

        [Description("Roles to consider:")]
        public Dictionary<RoleTypeId, bool> RolesToConsider { get; set; } = new()
        {
            { RoleTypeId.ClassD, false },
            { RoleTypeId.Scientist, false },
            { RoleTypeId.FacilityGuard, false }
        };

        [Description("Roles damage:")]
        public Dictionary<RoleTypeId, int> RolesDamage { get; set; } = new()
        {
            { RoleTypeId.Scp3114, 10 }
        };

        [Description("Hint config")]
        public HintConfig HintConfig { get; set; }

        [Description("Lobby config")]
        public LobbyConfig LobbyConfig { get; set; }
    }
}