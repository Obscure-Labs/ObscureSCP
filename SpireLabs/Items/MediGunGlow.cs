using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.CustomItems;
using Exiled.CustomItems.API.Features;
using MEC;
using Mirror;
using ObscureLabs.API.Features;
using Respawning.Objectives;
using UnityEngine;
using Light = Exiled.API.Features.Toys.Light;

namespace ObscureLabs.Items
{
    internal class MediGunGlow : Module
    {
        public override string Name => "MediGunGlow";
        public override bool IsInitializeOnStart => true;

        public Dictionary<string, GameObject> MedGunGlowingPlayers = new Dictionary<string, GameObject>();
        public CoroutineHandle Coroutine;

        public override bool Enable()
        {
            Coroutine = Timing.RunCoroutine(MediGunGlowManager());

            return base.Enable();
        }

        public override bool Disable()
        {
            Timing.KillCoroutines(Coroutine);
            return base.Disable();
        }


        public void MakeLight(Player p)
        {
            Light light = Light.Create(p.GameObject.transform.position, new Vector3(90, 0, 0), Vector3.one, false, Color.green);
            light.Intensity = 15f;
            light.Range = 2f;
            light.LightType = LightType.Point;
            light.ShadowType = LightShadows.Soft;

            light.Spawn();
            light.Base.gameObject.transform.parent = p.GameObject.transform;
            MedGunGlowingPlayers[p.DisplayNickname] = light.GameObject;
        }

        public IEnumerator<float> MediGunGlowManager()
        {

            while (true)
            {
                yield return Timing.WaitForSeconds(0.1f);
                foreach (Player p in Player.List)
                {
                    var i = CustomItem.TryGet(p.CurrentItem, out CustomItem a);
                    if (a == null || a.Id != 14)
                    {
                        if (MedGunGlowingPlayers.ContainsKey(p.DisplayNickname)) // If they are in the list of players with a glow already
                        {

                            if (MedGunGlowingPlayers[p.DisplayNickname] == null)
                            {
                                MedGunGlowingPlayers.Remove(p.DisplayNickname);
                            }
                            else
                            {
                                NetworkServer.Destroy(MedGunGlowingPlayers[p.DisplayNickname].gameObject);
                                MedGunGlowingPlayers.Remove(p.DisplayNickname);
                            }
                        }
                        continue;
                    }

                    if (a.Id == 14 && p.IsAlive) // If player is holding the medgun
                    {

                        if (MedGunGlowingPlayers.ContainsKey(p.DisplayNickname)) // If they are in the list of players with a glow already
                        {

                            if (MedGunGlowingPlayers[p.DisplayNickname] == null)
                            {
                                MakeLight(p);
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else // If they are NOT in the list of players with a glow already
                        {
                            MedGunGlowingPlayers.Add(p.DisplayNickname, null);
                            MakeLight(p);
                        }
                    }
                }
            }
        }
    }
}
