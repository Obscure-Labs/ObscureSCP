using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ObscureLabs.API.Features;
using YamlDotNet.Serialization;
using ObscureLabs.API.Enums;
using PlayerRoles;
using ObscureLabs.API.Data;
using ObscureLabs.API.Powerups;

namespace ObscureLabs.Modules.PowerupsHandling
{
    internal class Handler : Module
    {
        public override string Name => "PowerupHandler";
        public override bool IsInitializeOnStart => false;

        public static List<SpawnedPowerupBase> SpawnedPowerups = new List<SpawnedPowerupBase>();

        public override bool Enable()
        {
            try
            {
                if(spawnThread.ThreadState != ThreadState.Running)
                {
                    spawnThread.Start();
                    SpawnedPowerups.Clear();
                }
                return base.Enable();
            }
            catch
            {
                if (spawnThread.ThreadState == ThreadState.Running)
                {
                    spawnThread.Abort();
                    SpawnedPowerups.Clear();
                }
                return false;
            }

        }

        public override bool Disable()
        {
            try
            {
                if (spawnThread.ThreadState != ThreadState.Running)
                {
                    spawnThread.Abort();
                    SpawnedPowerups.Clear();
                }
                return base.Disable();
            }
            catch
            {
                return false;
            }
        }

        Thread spawnThread = new Thread(() =>
        {
            var rnd = new Random();
            while (true)
            {
                Thread.Sleep(10000);
                Powerup powerup = Powerups.PowerupList.RandomItem();
                int id = SpawnedPowerups.Count + 1;
                
            }
        });
    }
}
