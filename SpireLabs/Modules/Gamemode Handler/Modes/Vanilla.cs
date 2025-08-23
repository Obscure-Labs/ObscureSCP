using ObscureLabs.API.Features;
using ObscureLabs.Modules.Gamemode_Handler.Core;
using ObscureLabs.Modules.Gamemode_Handler.Core.SCP_Rebalances;
using ObscureLabs.SpawnSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs.Modules.Gamemode_Handler.Modes
{
    internal class Vanilla : Gamemode
    {
        public override string Name => "Vanilla";

        public override List<Module> InitModules => new List<Module>
        {
            new SSSStuff(),
            new ProximityChat(),
            new RemoteKeycard(),
            new EmotionRandomiser(),
            new Scp049(),
            new Scp106(),
            new Scp173(),
            new HealthOverride()
        };

        public override List<Module> StartModules => new List<Module>
        {

        };

        public override bool PreInitialise()
        {
            return base.PreInitialise();
        }

        public override bool Start()
        {
            return base.Start();
        }

        public override bool Stop()
        {
            return base.Stop();
        }
    }
}
