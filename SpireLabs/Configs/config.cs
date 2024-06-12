
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


        [Description("Roles damage:")]
        public Dictionary<RoleTypeId, int> RolesDamage { get; set; } = new()
        {
            { RoleTypeId.Scp3114, 10 }
        };

        //[Description("Roles to consider:")]
        //public Dictionary<RoleTypeId, bool> RolesToConsider { get; set; } = new()
        //{
        //    { RoleTypeId.ClassD, false },
        //    { RoleTypeId.Scientist, false },
        //    { RoleTypeId.FacilityGuard, false }
        //};


        //[Description("Hint config")]
        //public HintConfig HintConfig { get; set; }

        //[Description("Lobby config")]
        //public LobbyConfig LobbyConfig { get; set; }
    }
}