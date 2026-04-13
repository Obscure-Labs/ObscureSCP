using Cassie;
using Exiled.API.Features;
using Exiled.API.Features.DamageHandlers;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Roles;
using Exiled.Events.Handlers;
using LabApi.Features.Wrappers;
using MEC;
using ObscureLabs.API.Features;
using ObscureLabs.Items;
using ObscureLabs.Modules.Gamemode_Handler.Core;
using ObscureLabs.Modules.Gamemode_Handler.Core.SCP_Rebalances;
using PlayerRoles.FirstPersonControl;
using SpireSCP.GUI.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Scp106 = ObscureLabs.Modules.Gamemode_Handler.Core.SCP_Rebalances.Scp106;
using Scp173 = ObscureLabs.Modules.Gamemode_Handler.Core.SCP_Rebalances.Scp173;
using Scp939 = ObscureLabs.Modules.Gamemode_Handler.Core.SCP_Rebalances.Scp939;

namespace ObscureLabs.Modules.Gamemode_Handler.Modes
{
    internal class RedLightGreenLight_Standard : Gamemode
    {
        public override string Name => "Red Light, Green Light (Standard)";


        public bool RedLight = false;

        public override List<Module> InitModules => new List<Module>
        {
                        new MvpSystem(),
            new LightHandler(),
            new Lobby(),
            new SSSStuff(),
            new ProximityChat(),

            //- Gameplay Utils -//
            new Powerup(),
            new MediGunGlow(),

            //- Mechanics and Features -//
            new CoinFlip(),
            new AttachmentFix(),
            new SCPsDropItems(),
            new Scp1162(),

            //- Fun modules -//
            new Scp914Handler(),
            new RoundEndPVP(),
            new EmotionRandomiser(),
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

            foreach (Exiled.API.Features.Room room in Exiled.API.Features.Room.List)
            {
                room.Color = Color.green;
            }

            Manager.setModifier(0, "<color=green>Green Light</color>");
            return base.Start();
        }

        public override bool Stop()
        {
            Timing.KillCoroutines("RGLightStandard");
            return base.Stop();
        }


        public IEnumerator<float> ChangeLights()
        {
            yield return Timing.WaitForOneFrame;
            Manager.setModifier(0, "<color=red>Red Light</color>");
            foreach (Exiled.API.Features.Room room in Exiled.API.Features.Room.List)
            {
                room.Color = Color.red;

                foreach (Exiled.API.Features.Doors.Door d in room.Doors)
                {
                    d.PlaySound(Exiled.API.Enums.DoorBeepType.InteractionDenied);
                }
            }
            Exiled.API.Features.Cassie.Message("Red Light");

            Timing.CallDelayed(0.7f, () => { RedLight = true; });

            Timing.CallDelayed(10f, () =>
            {

                foreach (Exiled.API.Features.Room room in Exiled.API.Features.Room.List)
                {
                    room.Color = Color.green;
                    foreach (Exiled.API.Features.Doors.Door d in room.Doors)
                    {
                        d.PlaySound(Exiled.API.Enums.DoorBeepType.LockBypassDenied);
                    }
                }

                RedLight = false;
                Exiled.API.Features.Cassie.Message("Green Light");

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
                    foreach (Exiled.API.Features.Player p in Exiled.API.Features.Player.List)
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
