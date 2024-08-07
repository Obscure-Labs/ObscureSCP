using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;
using MEC;
using SpireSCP.GUI.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Firearm = Exiled.API.Features.Items.Firearm;
using Player = Exiled.API.Features.Player;

namespace ObscureLabs.Items
{
    [CustomItem(ItemType.GunE11SR)]
    public class ER16 : Exiled.CustomItems.API.Features.CustomWeapon
    {
        public static int trueAmmo = 30;

        public override float Damage { get; set; } = 1f;

        public override string Name { get; set; } = "MTF-ER16-SR";

        public override uint Id { get; set; } = 5;

        public override float Weight { get; set; } = 1.25f;

        public override string Description { get; set; } = "\t";

        public override byte ClipSize { get; set; } = 30;

        public Color[] colors = {
                //new Color(25f, 500f, 5000f, 0.015f),
                //new Color(500f, 25f, 5000f, 0.015f),
                new Color(5000f, 500f, 25f, 0.015f),
                //new Color(25f, 5000f, 5000f, 0.015f)
            };

        public int shotsFired = 0;

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
                    Chance = 0,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideNukeArmory,
                },
                new()
                {
                    Chance = 0,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideHczArmory,
                },
                new()
                {
                    Chance = 0,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideLocker,
                },
            },
        };

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }

        protected override void OnShooting(ShootingEventArgs ev)
        {
            if (ev.Player.CurrentItem is Firearm firearm)
            {
                Color color = colors[UnityEngine.Random.Range(0, colors.Count())];

                firearm.Ammo = firearm.MaxAmmo;

                Exiled.API.Features.Toys.Primitive projectile = Exiled.API.Features.Toys.Primitive.Create(ev.Player.Position + new Vector3(0f, -5, 0f), (ev.Player.CameraTransform.rotation * Quaternion.Euler(90f, 0f, 0f)).eulerAngles, new Vector3(1f, 0.01f, 1f), false); ;
                projectile.Type = PrimitiveType.Capsule;
                projectile.Color = color;

                projectile.Scale = new Vector3(0.035f, 0.07f, 0.035f);
                projectile.Collidable = false;

                projectile.Spawn();
                Timing.RunCoroutine(ShootCoroutine(projectile, ev));
            }
        }

        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            if (!Check(ev.Item)) return;
            ev.IsAllowed = false;
        }

        protected override void OnShot(ShotEventArgs ev)
        {
            ev.CanHurt = false;
        }

        private void OnChangedItem(ChangedItemEventArgs ev)
        {
            if (!Check(ev.Item)) return;
            if (ev.Player.CurrentItem is Firearm firearm)
            {
                Manager.SendHint(ev.Player, "You equipped the <b>MTF-ER16-SR</b> \n <b>This is an energy rifle that shoots very hot laser rounds</b>.", 3.0f);
            }
        }

        private IEnumerator<float> ShootCoroutine(Exiled.API.Features.Toys.Primitive primitive, ShootingEventArgs ev)
        {
            yield return Timing.WaitForOneFrame;
            //projectile.Position = ev.Player.Transform.position + new Vector3(0.45f, 0.7f, 0f);
            primitive.Position = ev.Player.Transform.position + ev.Player.Transform.forward.normalized + (ev.Player.Transform.up.normalized * 0.5f);

            Timing.RunCoroutine(ProjectiveMovementCoroutine(primitive, ev.Player.CameraTransform.forward, ev.Player, primitive.Position));

            yield return Timing.WaitForSeconds(0.1f);

            primitive.MovementSmoothing = 60;
        }

        private IEnumerator<float> ProjectiveMovementCoroutine(Exiled.API.Features.Toys.Primitive primitive, Vector3 direction, Player owner, Vector3 startPosition)
        {
            while (primitive.Base.gameObject.activeSelf)
            {
                if (Vector3.Distance(startPosition, primitive.Position) > 37.5f)
                {
                    primitive.Base.gameObject.SetActive(false);
                    primitive.UnSpawn();
                }

                Exiled.API.Features.Toys.Primitive g = primitive;
                foreach (Player player1 in Player.List)
                {
                    Player player2 = null;
                    var direction1 = player1.Position - new Vector3(g.Position.x, g.Position.y, g.Position.z);

                    Physics.Raycast(g.Position, direction1, out var h, maxDistance: 0.76f);

                    if (h.collider is not null)
                    {
                        if (!Player.TryGet(h.collider, out player2))
                        {
                            if (Door.Get(h.transform.root.gameObject) is not null)
                            {
                                primitive.Base.gameObject.SetActive(false);
                                primitive.UnSpawn();

                            }

                            primitive.Base.gameObject.SetActive(false);
                            primitive.UnSpawn();
                        }
                    }

                    if (player2 is null)
                    {
                        continue;
                    }

                    if (Math.Sqrt(Math.Pow(g.Position.x - player2.Position.x, 2) + Math.Pow(g.Position.y - player2.Position.y, 2)) > 0.75f)
                    {
                        continue;
                    }

                    if (player2.Role.Side != owner.Role.Side || Server.FriendlyFire == true)
                    {
                        if (player2.Health < 20.7f)
                        {
                            player2.Kill($"The victim was incinerated by some sort of energy weapon");
                        }

                        player2.Hurt(7.7f);
                        owner.ShowHitMarker(1);
                        player2.EnableEffect(EffectType.Burned, 1, false);

                        primitive.Base.gameObject.SetActive(false);
                        primitive.UnSpawn();
                        Log.Info(Vector3.Distance(startPosition, player2.Position));
                    }

                    if (player2 == owner || player2.Role.Team == owner.Role.Team)
                    {
                        primitive.UnSpawn();
                    }
                }

                yield return Timing.WaitForOneFrame;
                primitive.Position += direction * 0.20f;
            }
        }
    }
}

