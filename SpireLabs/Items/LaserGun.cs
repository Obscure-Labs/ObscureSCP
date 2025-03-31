using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using Light = Exiled.API.Features.Toys.Light;
using UnityEngine;
using Player = Exiled.API.Features.Player;
using Server = Exiled.API.Features.Server;
using Item = Exiled.API.Features.Items.Item;
using Door = Exiled.API.Features.Doors.Door;
using Exiled.API.Features.Toys;

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
                new Color(250f, 250f, 25f, 0.015f),
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
                }
            },
        };

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }



        protected override void OnAcquired(Player player, Item item, bool displayMessage)
        {
            base.OnAcquired(player, item, false);

        }

        protected override void OnShooting(ShootingEventArgs ev)
        {
            ev.Firearm.Damage = 0f;
            ev.Firearm.AmmoDrain = 0;
            CreateLaser(ev);

        }

        public void CreateLaser(ShootingEventArgs ev)
        {
            Color color = colors[UnityEngine.Random.Range(0, colors.Count())];
            ev.Firearm.MagazineAmmo = ev.Firearm.MaxMagazineAmmo;

            Exiled.API.Features.Toys.Primitive projectile = Exiled.API.Features.Toys.Primitive.Create(ev.Player.Position + new Vector3(0f, -2, 0f), (ev.Player.CameraTransform.rotation * Quaternion.Euler(90f, 0f, 0f)).eulerAngles, new Vector3(1f, 0.01f, 1f), false); ;
            projectile.Type = PrimitiveType.Capsule;
            projectile.Color = color;


            projectile.Scale = new Vector3(0.035f, 0.07f, 0.035f);
            projectile.Collidable = false;

            Light light = Light.Create(projectile.Position, new Vector3(90, 0, 0), Vector3.one, false, Color.red);
            light.Intensity = 5f;
            light.Range = 0.5f;
            light.LightType = LightType.Point;
            light.Color = Color.red * 5;
            light.Base.transform.SetParent(projectile.Base.transform);
            light.Spawn();
            projectile.Spawn();
            Timing.RunCoroutine(ShootCoroutine(projectile, light, ev));
        }

        private IEnumerator<float> ShootCoroutine(Primitive primitive, Light light, ShootingEventArgs ev)
        {
            yield return Timing.WaitForOneFrame;
            //projectile.Position = ev.Player.Transform.position + new Vector3(0.45f, 0.7f, 0f);
            primitive.Position = ev.Player.Transform.position + ev.Player.Transform.forward.normalized + (ev.Player.Transform.up.normalized * 0.5f);

            Timing.RunCoroutine(ProjectiveMovementCoroutine(primitive, light, ev.Player.CameraTransform.forward, ev.Player, primitive.Position));

            yield return Timing.WaitForSeconds(0.1f);

            primitive.MovementSmoothing = 60;
            
        }


        private IEnumerator<float> ProjectiveMovementCoroutine(Primitive primitive, Light light, Vector3 direction, Player owner, Vector3 startPosition)
        {
            while (primitive.Base.gameObject.activeSelf)
            {
                if (Vector3.Distance(startPosition, primitive.Position) > 37.5f)
                {
                    primitive.Base.gameObject.SetActive(false);
                    primitive.UnSpawn();
                    light.UnSpawn();
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
                                light.UnSpawn();

                            }

                            primitive.Base.gameObject.SetActive(false);
                            primitive.UnSpawn();
                            light.UnSpawn();
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
                        if (player2 == owner)
                        {
                            continue;
                        }
                        else
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
                            light.UnSpawn();
                            Log.Info(Vector3.Distance(startPosition, player2.Position));
                        }



                    }

                    if (player2 == owner || player2.Role.Team == owner.Role.Team)
                    {
                        primitive.UnSpawn();
                        light.UnSpawn();
                    }
                }

                yield return Timing.WaitForOneFrame;
                primitive.Position += direction * 0.20f;


            }
        }
    }
}
