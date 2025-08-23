using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Features.DamageHandlers;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Roles;
using MEC;
using ObscureLabs.API.Features;
using ObscureLabs.Modules.Gamemode_Handler.Core;
using PlayerRoles.FirstPersonControl;
using SpireSCP.GUI.API.Features;
using UnityEngine;

namespace ObscureLabs.Modules.Gamemode_Handler.Modes
{
    internal class RedLightGreenLight_Standard : Gamemode
    {
        public override string Name => "Red Light, Green Light (Standard)";


        public bool RedLight = false;

        public override List<Module> InitModules => new List<Module>
        {
            new SSSStuff(),
            new ProximityChat(),
        };

        public override List<Module> StartModules => new List<Module>()
        {
            new ItemGlow()
        };

        public override bool PreInitialise()
        {
            foreach (Module m in Plugin.Instance._modules.Modules)
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
            Timing.RunCoroutine(PlayerKiller(), "RGLightStandard");
            Timing.RunCoroutine(LightChanger(), "RGLightStandard");

            foreach (Room room in Room.List)
            {
                room.Color = Color.green;
            }

            Manager.setModifier(0, "<color=green>Green Light</color>");
            return base.Start();
        }

        public override bool Stop()
        {
            foreach (Module m in Plugin.Instance._modules.Modules)
            {
                m.Disable();
            }
            Timing.KillCoroutines("RGLightStandard");
            return base.Stop();
        }


        public IEnumerator<float> ChangeLights()
        {
            yield return Timing.WaitForOneFrame;
            Manager.setModifier(0, "<color=red>Red Light</color>");
            foreach (Room room in Room.List)
            {
                room.Color = Color.red;

                foreach (Door d in room.Doors)
                {
                    d.PlaySound(Exiled.API.Enums.DoorBeepType.InteractionDenied);
                }
            }

            Cassie.Message("pitch_1.10 jam_45_2 yield_10 Red Light", false, false, false);
            Timing.CallDelayed(0.7f, () => { RedLight = true; });

            Timing.CallDelayed(10f, () =>
            {

                foreach (Room room in Room.List)
                {
                    room.Color = Color.green;
                    foreach (Door d in room.Doors)
                    {
                        d.PlaySound(Exiled.API.Enums.DoorBeepType.LockBypassDenied);
                    }
                }

                RedLight = false;
                Cassie.Message("pitch_1.10 jam_45_2 yield_10 Green Light", false, false, false);
                Manager.setModifier(0, "<color=green>Green Light</color>");
            });

        }

        private IEnumerator<float> LightChanger()
        {
            
            while (true)
            {
                if (RedLight == false)
                {
                    int waittime = UnityEngine.Random.Range(10, 50);
                    yield return Timing.WaitForSeconds(waittime);
                    Timing.RunCoroutine(ChangeLights());
                }

            }
        }


        private IEnumerator<float> PlayerKiller()
        {
            while (true)
            {
                yield return Timing.WaitForOneFrame;
                if (RedLight)
                {
                    foreach (Player p in Player.List)
                    {

                        if (p.Velocity != new Vector3(0, p.Velocity.y, 0))
                        {
                            p.Explode();
                            p.Kill(Exiled.API.Enums.DamageType.Recontainment);
                        }
                    }
                }

            }
        }
    }
}
