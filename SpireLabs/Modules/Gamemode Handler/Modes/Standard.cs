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
    internal class Standard : Gamemode
    {
        public override string Name => "Standard";

        public override List<Module> InitModules => new List<Module>
        {

        };

        public override List<Module> StartModules => new List<Module>()
        {
            new ItemGlow()
        };

        public override bool PreInitialise()
        {
            foreach(Module m in Plugin.Instance._modules.Modules)
            {
                if (m.IsInitializeOnStart)
                {
                    m.Enable();
                }
            }
            return base.PreInitialise();
        }

        public override bool Start()
        {
            return base.Start();
        }

        public override bool Stop()
        {
            foreach(Module m in Plugin.Instance._modules.Modules)
            {
                if (m.IsInitializeOnStart)
                {
                    m.Disable();
                }
            }
            return base.Stop();
        }
    }
}
