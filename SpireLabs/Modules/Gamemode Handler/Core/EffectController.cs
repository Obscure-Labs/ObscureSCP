using LabApi.Features.Wrappers;
using ObscureLabs.API.Features;
using ObscureLabs.Modules.Gamemode_Handler.StatusEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    internal class EffectController : Module
    {
        public override string Name => "EffectController";
        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {
            LabApi.Features.Console.Logger.Info("EffectController Loaded");
            return base.Enable();
        }

        public override bool Disable()
        {
            LabApi.Features.Console.Logger.Info("EffectController Unloaded");
            return base.Disable();
        }
    }
}