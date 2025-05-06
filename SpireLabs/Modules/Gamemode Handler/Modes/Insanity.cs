using Exiled.API.Features;
using InventorySystem;
using ObscureLabs.API.Features;
using ObscureLabs.Modules.Default;
using ObscureLabs.Modules.Gamemode_Handler.Core;
using ObscureLabs.Modules.Gamemode_Handler.Core.SCP_Rebalances;
using ObscureLabs.Modules.Gamemode_Handler.Mode_Specific_Modules;
using ObscureLabs.SpawnSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs.Modules.Gamemode_Handler.Modes
{
    internal class Insanity : Gamemode
    {
        /// <summary>
        /// teams are scientists and dclass. all items are custom. randomise ammo brick type but dont replace with items.
        /// </summary>

        public override string Name => "Insanity";
        public override List<Module> InitModules => new List<Module>
        {
            new LightHandler(),
            new RemoteKeycard(),
            new EmotionRandomiser(),
            new Scp049(),
            new SCP106(),
            new SCP173(),
            new HealthOverride(),
            //new ItemGlow(),
            new CoinFlip(),
            new MvpSystem(),
            new InsanityRandomiser()
        };
        public override List<Module> StartModules => new List<Module>
        {
            // Add modules that should be started when the round starts
        };
        public override bool PreInitialise()
        { 
            return base.PreInitialise();
        }
        public override bool Start()
        {
            List<Player> pList = new List<Player>();
            foreach (Player p in Player.List)
            {
                if (p.AuthenticationType != Exiled.API.Enums.AuthenticationType.DedicatedServer)
                {
                    pList.Add(p);
                }
            }
            pList.ShuffleList();

            for(int i = 0; i < pList.Count; i++)
            {
                Player p = pList[i];
                if(i % 2 == 0)
                {
                    p.RoleManager.ServerSetRole(PlayerRoles.RoleTypeId.ClassD, PlayerRoles.RoleChangeReason.RoundStart, PlayerRoles.RoleSpawnFlags.UseSpawnpoint);
                }
                else
                {
                    p.RoleManager.ServerSetRole(PlayerRoles.RoleTypeId.Scientist, PlayerRoles.RoleChangeReason.RoundStart, PlayerRoles.RoleSpawnFlags.UseSpawnpoint);
                }
                p.Inventory.ServerAddItem(ItemType.Coin, InventorySystem.Items.ItemAddReason.StartingItem);
                p.Inventory.ServerAddItem(ItemType.KeycardZoneManager, InventorySystem.Items.ItemAddReason.StartingItem);
            }
            return base.Start();
        }
        public override bool Stop()
        {
            return base.Stop();
        }
    }
}
