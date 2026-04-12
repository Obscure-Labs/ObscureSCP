using Exiled.API.Features;
using ObscureLabs.API.Features;
using ObscureLabs.Items;
using ObscureLabs.Modules.Gamemode_Handler.Core;
using ObscureLabs.Modules.Gamemode_Handler.Core.SCP_Rebalances;
using ObscureLabs.SpawnSystem;
using SpireLabs.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs.Modules.Gamemode_Handler.Modes
{
    internal class Standard : Gamemode
    {
        public override string Name => "Standard Mode";

        public override List<Module> InitModules => new List<Module>
        {
            //- Core Utils -//
            new MvpSystem(),
            new LightHandler(),
            new Lobby(),
            new SSSStuff(),
            new ProximityChat(),
            new CustomItemSpawner(),

            //- Gameplay Utils -//
            new Powerup(),
            new MediGunGlow(),

            //- Mechanics and Features -//
            new CoinFlip(),
            new AttachmentFix(),
            new SCPsDropItems(),

            //- Fun modules -//
            new Scp914Handler(),
            new RoundEndPVP(),
            new EmotionRandomiser(),
        };

        public override List<Module> StartModules => new List<Module>()
        {
            new ItemGlow()
        };
    }
}
