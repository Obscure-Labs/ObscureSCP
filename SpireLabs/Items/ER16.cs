using AdminToys;
using AudioPlayer.Commands.SubCommands;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Components;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.API.Features.Spawn;
using Exiled.API.Features.Toys;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Handlers;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.BasicMessages;
using InventorySystem.Items.Firearms.Modules;
using JetBrains.Annotations;
using MapEditorReborn.API.Features.Objects;
using MEC;
using SpireSCP.GUI.API.Features;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.Core.Tokens;
using Firearm = Exiled.API.Features.Items.Firearm;
using Player = Exiled.API.Features.Player;

namespace ObscureLabs.Items
{
    [CustomItem(ItemType.GunE11SR)]
    public class ER16 : Exiled.CustomItems.API.Features.CustomWeapon
    {

        public override float Damage { get; set; } = 1f;
        public override string Name { get; set; } = "MTF-ER16-SR";
        public override uint Id { get; set; } = 5;
        public override float Weight { get; set; } = 1.25f;
        public override string Description { get; set; } = "\t";
        public override byte ClipSize { get; set; } = 30;

        public override AttachmentName[] Attachments { get; set; } = new[]
{
            AttachmentName.ExtendedBarrel,
            AttachmentName.Laser,
            AttachmentName.LowcapMagJHP,
            AttachmentName.HoloSight,
            AttachmentName.StandardStock,
        };

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 3,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 50,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideNukeArmory,
                },
                new()
                {
                    Chance = 50,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideHczArmory,
                },
                new()
                {
                    Chance = 50,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideLocker,
                },
            },
        };

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangedItem += Equipped;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }
       
        private void Equipped(ChangedItemEventArgs ev)
        {
            if (!Check(ev.Item)) return;
            if (ev.Player.CurrentItem is Firearm firearm)
            {
                Manager.SendHint(ev.Player, "You equipped the <b>MTF-ER16-SR</b> \n <b>This is an energy rifle that shoots very hot laser rounds</b>.", 3.0f);
            }
        }

        public static int trueAmmo = 30;
        public int shotsFired = 0;
        protected override void OnShooting(ShootingEventArgs ev)
        {
            if (ev.Player.CurrentItem is Firearm firearm)
            {
                ev.IsAllowed = false;
                firearm.Ammo = firearm.MaxAmmo;
                var pl = ev.Player;
                Vector3 loc = pl.Transform.position;

                Exiled.API.Features.Toys.Primitive projectile = Exiled.API.Features.Toys.Primitive.Create(ev.Player.Position + new Vector3(0f, -5, 0f), (ev.Player.CameraTransform.rotation * Quaternion.Euler(90f, 0f, 0f)).eulerAngles, new Vector3(1f, 0.01f, 1f), false); ;
                projectile.Type = PrimitiveType.Capsule;
                projectile.Color = new Color(25f, 2500f, 5000f, 0.015f);
                projectile.MovementSmoothing = 0;
                projectile.Scale = new Vector3(0.035f, 0.15f, 0.035f);
                projectile.Collidable = false;
                projectile.Spawn();
                Timing.RunCoroutine(er16shoot(projectile, ev));
            }
        }


        private IEnumerator<float> er16shoot(Exiled.API.Features.Toys.Primitive projectile, ShootingEventArgs ev)
        {
            yield return Timing.WaitForSeconds(0.1f);
            //projectile.Position = ev.Player.Transform.position + new Vector3(0.45f, 0.7f, 0f);
            projectile.Position = ev.Player.Transform.position + ev.Player.Transform.forward.normalized + (ev.Player.Transform.up.normalized * 0.5f);
            Timing.RunCoroutine(projectivemovement(projectile, ev.Player.CameraTransform.forward, ev.Player, projectile.Position));
        }

        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            if (!Check(ev.Item)) return;
            ev.IsAllowed = false;
        }

        private IEnumerator<float> projectivemovement(Exiled.API.Features.Toys.Primitive primitive, Vector3 dir, Exiled.API.Features.Player owner, Vector3 startpos)
        {
            while (primitive.Base.gameObject.activeSelf == true)
            {
                if (Vector3.Distance(startpos, primitive.Position) > 37.5f)
                {
                    primitive.Base.gameObject.SetActive(false);
                    primitive.UnSpawn();
                }
                Exiled.API.Features.Toys.Primitive g = primitive;
                foreach (Player pp in Player.List)
                {
                    int loopCntr = 0;
                    RaycastHit h;
                    Player ppp = null;
                    Vector3 dire = pp.Position - new Vector3(g.Position.x, g.Position.y, g.Position.z);
                    Physics.Raycast(g.Position, dire, out h, maxDistance: 0.76f);
                    if (h.collider != null)
                    {
                        if (Player.TryGet(h.collider, out ppp) == false)
                        {
                            primitive.UnSpawn();
                        }
                    }
                    if (ppp == null) continue;
                    if (Math.Sqrt((Math.Pow((g.Position.x - ppp.Position.x), 2)) + (Math.Pow((g.Position.y - ppp.Position.y), 2))) > 0.75f) continue;
                    if (ppp.Role.Team != owner.Role.Team)
                    {
                        if (ppp.Health < 20.7f) ppp.Kill($"The victim was incinerated by some sort of energy weapon");
                        ppp.Hurt(20.7f);
                        owner.ShowHitMarker(1);
                        ppp.EnableEffect(EffectType.Burned, 1, false);
                        primitive.Base.gameObject.SetActive(false);
                        primitive.UnSpawn();
                        Log.Info(Vector3.Distance(startpos, ppp.Position));
                    }
                    if(ppp == owner || ppp.Role.Team == owner.Role.Team)
                    {
                        primitive.UnSpawn();
                    }
                }
                yield return Timing.WaitForOneFrame;
                primitive.Position += dir * 0.45f;
            }
        }   

        protected override void OnShot(ShotEventArgs ev)
        {
            ev.CanHurt = false;
        }
    }

    //public class LaserCollisionHandler : MonoBehaviour
    //{
    //    public void OnCollisionEnter(Collider other)
    //    {
    //        Exiled.API.Features.Player nP = null;
    //        Exiled.API.Features.Player.TryGet((Collider)other, out nP);
    //        nP.Explode();
    //        Destroy(gameObject);
    //    }
    //}
}

