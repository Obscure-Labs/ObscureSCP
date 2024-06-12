using ObscureLabs.API.Data;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs.Configs
{
    public class OverrideConfig
    {
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
    }
}
