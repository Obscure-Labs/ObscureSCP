using ObscureLabs.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    public class TestModule : Module
    {
        public override string Name => "TestModule";

        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {
            LabApi.Features.Console.Logger.Info("TestModule enabled");
            return base.Enable();
        }

        public override bool Disable()
        {
            LabApi.Features.Console.Logger.Info("TestModule disabled");
            return base.Disable();
        }
    }
}
