
using System;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Player;
using MEC;
using SpireSCP.GUI.API.Features;
using System.Collections.Generic;
using System.Linq;
using AdminToys;
using Exiled.API.Enums;
using Exiled.API.Features.Toys;
using InventorySystem;
using Mirror;
using NetworkManagerUtils.Dummies;
using ObscureLabs.Items;
using PlayerRoles.FirstPersonControl.NetworkMessages;
using UnityEngine;
using UnityEngine.SearchService;
using Object = UnityEngine.Object;


namespace ObscureLabs.Items
{
    [CustomItem(ItemType.GrenadeHE)]
    public class SuperCapybara : Exiled.CustomItems.API.Features.CustomGrenade
    {
        public override string Name { get; set; } = "Super Capybara";

        public override uint Id { get; set; } = 420;

        public override string Description { get; set; } = "\t";

        public override float Weight { get; set; } = 0.01f;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 2,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 0,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideHczArmory,
                }
            },
        };

        public override bool ExplodeOnCollision { get; set; } = false;

        public override float FuseTime { get; set; } = 999f;

        public List<ReferenceHub> HiddenPlayers = new List<ReferenceHub>();
        public List<ReferenceHub> Dummies = new List<ReferenceHub>();
        public List<GameObject> Capybaras = new List<GameObject>();
        
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
            Exiled.Events.Handlers.Player.Died += OnDied;
            Exiled.Events.Handlers.Player.Dying += OnDying;
            base.SubscribeEvents();
        }
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangedItem -= OnChangedItem;
            Exiled.Events.Handlers.Player.Died -= OnDied;
            Exiled.Events.Handlers.Player.Dying -= OnDying;
            base.UnsubscribeEvents();
        }

        private void OnDying(DyingEventArgs ev)
        {
            if (HiddenPlayers.Contains(ev.Player.ReferenceHub))
            {
                ev.Player.Scale = Vector3.one;
                //     foreach(Player p in Player.List)
                //     {
                //         if (p != ev.Player) p.ReferenceHub.connectionToClient.Send(new ObjectHideMessage{ netId = ev.Player.NetId });
                //     }
            }
            
        }
        
        protected override void OnThrownProjectile(ThrownProjectileEventArgs ev)
        {
            ExplosionGrenadeProjectile g = ev.Projectile as ExplosionGrenadeProjectile;
            NetworkServer.Destroy(g.GameObject);

            var npc = Npc.Spawn(ev.Player.DisplayNickname + "CAPYBARACLONE", ev.Player.Role.Type,
                ev.Player.Transform.position);
            npc.Transform.rotation = ev.Player.Transform.rotation;
            // var dummy = DummyUtils.SpawnDummy(ev.Player.DisplayNickname + "CAPYBARA");
            // dummy.transform.position = ev.Player.Transform.position;
            // dummy.transform.rotation = Quaternion.Euler(0, 0, 0);

            // foreach (Player p in Player.List)
            // {
            //     p.ReferenceHub.connectionToClient.Send(new ObjectHideMessage{netId = g.Base.netId});
            //     if (p != ev.Player) p.ReferenceHub.connectionToClient.Send(new ObjectHideMessage{ netId = ev.Player.NetId });
            // }
            ev.Player.Scale = new Vector3(0.01f, 0.01f, 0.01f);
            HiddenPlayers.Add(ev.Player.ReferenceHub);

            var capybara = PrefabHelper.Spawn(PrefabType.CapybaraToy, ev.Player.Position, ev.Player.Rotation);
            try
            {
                ev.Player.ReferenceHub.connectionToClient.Send(new ObjectHideMessage
                    { netId = capybara.GetComponent<NetworkIdentity>().netId });
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            
            Timing.RunCoroutine(RunCapybara(ev.Player, npc, capybara, ev.Player.Transform.position));
            base.OnThrownProjectile(ev);
        }
        
        private IEnumerator<float> RunCapybara(Player p, Npc dummy, GameObject capybara, Vector3 oldPos)
        {
            var oldGrav = LabApi.Features.Wrappers.Player.Get(p.ReferenceHub).Gravity;
            LabApi.Features.Wrappers.Player.Get(p.ReferenceHub).Gravity = Vector3.zero;
            for(int i = 0; i < 1000; i++)
            {
                yield return Timing.WaitForOneFrame;
                capybara.transform.position += p.CameraTransform.forward.normalized*0.1f;
                capybara.transform.rotation = p.CameraTransform.rotation;
                capybara.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                p.Teleport(capybara.transform.position + -p.CameraTransform.forward*0.1f);
            }
            LabApi.Features.Wrappers.Player.Get(p.ReferenceHub).Gravity = oldGrav;
            p.Scale = Vector3.one;
            p.Teleport(oldPos);
            NetworkServer.Destroy(capybara); 
            dummy.Destroy();
        }
        
        
        
        private void OnDied(DiedEventArgs ev)
        {
            if (Dummies.Contains(ev.Player.ReferenceHub))
            {
                if (ev.Player.ReferenceHub.IsDummy)
                {
                    ev.Player.Disconnect();
                    Dummies.Remove(ev.Player.ReferenceHub);
                }
            }
        }
        
        private void OnChangedItem(ChangedItemEventArgs ev)
        {
            if (!Check(ev.Item)) return;
            Manager.SendHint(ev.Player, "You equipped the <b>Super Capybara</b>.", 3.0f);
        }
        
        public static void DestroyCapybara(GameObject capybara)
        {
            foreach (Transform g in capybara.GetComponentsInChildren<Transform>())
            {
                NetworkServer.Destroy(g.gameObject);
            }
            GameObject.Destroy(capybara);
        }
    }
}

public class CapybaraScript : MonoBehaviour
{
    public void Start()
    {
    }

    // public void OnTriggerEnter(Collider other)
    // {
    //     if (other.gameObject.layer == 8) return;
    //     SuperCapybara.DestroyCapybara(gameObject);
    // }
}