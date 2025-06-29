﻿using Exiled.API.Features;
using Exiled.API.Features.Toys;
using ObscureLabs.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AdminToys;
using Mirror;
using Room = Exiled.API.Features.Room;
using Player = Exiled.API.Features.Player;
using Light = Exiled.API.Features.Toys.Light;
using Exiled.API.Extensions;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;
using PlayerRoles.FirstPersonControl.NetworkMessages;
using PlayerRoles.Visibility;
using LiteNetLib4Mirror.Open.Nat;
using Exiled.API.Enums;
using CommandSystem.Commands.RemoteAdmin;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    public class PowerUpScript : MonoBehaviour
    {
        public void Start()
        {

        }
        public void Update()
        {
        }
        private void OnTriggerEnter(Collider other)
        {
            if (Player.Get(other) != null)
            {
                LabApi.Features.Console.Logger.Info("Collider Hit");
                ((Powerup)Plugin.Instance._modules.GetModule("Powerup")).PowerupTriggerEnter(other, this.gameObject);
            }
        }
    }

    public class PowerUpAnimate : MonoBehaviour
    {
        public Vector3 origin;
        public void Start()
        {
            origin = transform.position;
        }

        public void Update()
        {
            var sin = Mathf.Sin(Time.time);
            if (sin < 0) sin *= -1;
            gameObject.transform.position = new Vector3(transform.position.x, Mathf.Lerp(origin.y - 0.35f, origin.y, sin), transform.position.z);
        }
    }

    public class Powerup : Module
    {
        public override string Name => "Powerup";

        public override bool IsInitializeOnStart => true;

        public int currentPickups = 0;
        public int maxPickups = 20;

        public List<GameObject> pickups = new List<GameObject>();

        public override bool Enable()
        {
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;

            return base.Enable();

        }

        public override bool Disable()
        {
            foreach (GameObject gameObject in pickups)
            {
                foreach (Transform g in gameObject.GetComponentsInChildren<Transform>())
                {
                    NetworkServer.Destroy(g.gameObject);
                }
                GameObject.Destroy(gameObject);

            }
            pickups.Clear();

            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            return base.Disable();
        }

        public void PowerupTriggerEnter(Collider other, GameObject trigger)
        {
            var itemList = Enum.GetValues(typeof(ItemType)).ToArray<ItemType>();
            int index = pickups.IndexOf(trigger);

            pickups.Remove(trigger.gameObject);

            Log.Debug($"Pickup Object: {trigger.gameObject.name} in index of: {index}");
            Player player = Player.Get(other);
            var randomitem = new System.Random();
            player.AddItem(itemList.ElementAt(randomitem.Next(0, itemList.Count() + 1)));

            foreach (Transform g in trigger.gameObject.GetComponentsInChildren<Transform>())
            {
                NetworkServer.Destroy(g.gameObject);
            }

            Log.Debug($"Destroyed: {trigger.gameObject.name}");
            GameObject.Destroy(trigger.gameObject);

            Log.Debug($"There are: {pickups.Count()} pickups left!");
        }

        private void SpawnPowerup(LabApi.Features.Wrappers.Room room)
        {
            GameObject container = new GameObject();
            // Vector3 pos = new Vector3(room.Transform.position.x, room.Transform.position.y + 0.5f,
            //     room.Transform.position.z);
            Vector3 pos = new Vector3(room.Position.x,
                room.Position.y + 1f, room.Position.z);
            container.transform.position = pos;
            BoxCollider collider = container.gameObject.AddComponent<BoxCollider>();
            collider.size = Vector3.one / 2;
            collider.isTrigger = true;
            
            Rigidbody rb = container.gameObject.AddComponent<Rigidbody>();
            rb.mass = 0f;
            rb.drag = 0f;
            rb.angularDrag = 0f;
            rb.useGravity = false;
            
            Primitive cube = Primitive.Create(PrimitiveType.Cube, container.gameObject.transform.position, Vector3.zero, Vector3.one / 2, false);
            cube.Flags = PrimitiveFlags.Collidable | PrimitiveFlags.Visible;
            
            cube.Color = new Color(1f, 47f / 51f, 0.0156862754f, 0.75f);
            cube.Scale = Vector3.one / 2f;
            cube.Collidable = false;
            cube.Base.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.On;
            cube.Base.GetComponent<MeshRenderer>().receiveShadows = true;
            
            Light light = Light.Create(new Vector3(0, 2f, 0), new Vector3(90, 0, 0), Vector3.one, false, Color.red);
            light.Intensity = 10f;
            light.Range = 10f;
            light.LightType = LightType.Spot;
            light.ShadowStrength = 100;
            light.ShadowType = LightShadows.Hard;
            light.Color = Color.yellow * 5;
            
            container.gameObject.AddComponent<PowerUpScript>();
            cube.Base.gameObject.AddComponent<PowerUpAnimate>();
            light.Base.gameObject.AddComponent<PowerUpAnimate>();

            
            light.Spawn();
            cube.Spawn();

            light.Base.gameObject.transform.parent = container.transform;
            cube.Base.gameObject.transform.parent = container.transform;
            cube.Base.gameObject.transform.localPosition = container.transform.position;
            light.Base.gameObject.transform.transform.localPosition = container.transform.position + (Vector3.up * 2);

            pickups.Add(container.gameObject);
            container.gameObject.name = $"{pickups.Count - 1}_container";
            cube.Base.gameObject.name = $"{pickups.Count - 1}_cube";
            light.Base.gameObject.name = $"{pickups.Count - 1}_light";
            
            Log.Debug($"Spawned powerup of index: {pickups.Count() - 1}");
        }
        private void OnRoundStarted()
        {
            Log.Info("Map Generated");
            List<LabApi.Features.Wrappers.Room> rooms = LabApi.Features.Wrappers.Room.List.ToList();
            for (int i = 0; i < maxPickups; i++)
            {
            roomGo:
                if(!rooms.TryGetRandomItem(out var selectedRoom))
                {
                    Log.Warn("powerup did an oopsy");
                    goto roomGo;
                }
                var SelectedRoomType = Room.Get(selectedRoom.Transform.position).Type;

                SpawnPowerup(selectedRoom);
                rooms.Remove(selectedRoom);
            }
            Log.Info($"Spawned: {pickups.Count()} powerups");
        }
    }
}