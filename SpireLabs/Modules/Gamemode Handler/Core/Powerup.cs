using Exiled.API.Features;
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

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    public class PowerUpScript : MonoBehaviour
    {
        public void Start()
        {
            //idk if debug.log works tbh it should but just incase northwood moment
            LabApi.Features.Console.Logger.Info("Powerup script attached to object");

            gameObject.tag = "";
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

            Log.Info("Enabling Powerup Module");
            return base.Enable();

        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            return base.Disable();
        }

        public void PowerupTriggerEnter(Collider other, GameObject trigger)
        {
            var itemList = Enum.GetValues(typeof(ItemType)).ToArray<ItemType>();
            int index = pickups.IndexOf(trigger);

            LabApi.Features.Console.Logger.Info("Triggered Event");

            pickups.Remove(trigger.gameObject);

            Log.Debug($"Pickup Object: {trigger.gameObject.name} in index of: {index}");
            Player player = Player.Get(other);
            var randomitem = new System.Random();
            player.AddItem(itemList.ElementAt(randomitem.Next(0, itemList.Count() + 1)));

            foreach (Transform g in trigger.gameObject.GetComponentsInChildren<Transform>())
            {
                NetworkServer.Destroy(g.gameObject);
            }

            Log.Warn($"Destroyed: {trigger.gameObject.name}");
            GameObject.Destroy(trigger.gameObject);

            Log.Warn($"There are: {pickups.Count()} pickups left!");
        }

        private void SpawnPowerup(Room room)
        {
            GameObject container = new GameObject();
            Vector3 pos = new Vector3(room.transform.position.x, room.transform.position.y + 1f, room.transform.position.z);
            container.transform.position = pos;

            BoxCollider collider = container.gameObject.AddComponent<BoxCollider>();
            collider.size = Vector3.one / 2;
            collider.isTrigger = true;

            Rigidbody rb = container.gameObject.AddComponent<Rigidbody>();
            rb.mass = 0f;
            rb.drag = 0f;
            rb.angularDrag = 0f;
            rb.useGravity = false;

            Primitive cube = Primitive.Create(PrimitiveType.Cube, container.transform.position, Vector3.zero, Vector3.one / 2, false);
            cube.Flags = PrimitiveFlags.Collidable | PrimitiveFlags.Visible;
            cube.Color = Color.yellow;
            cube.Scale = Vector3.one / 2f;
            cube.Collidable = false;
            cube.Base.GetComponent<MeshRenderer>().receiveShadows = true;

            Light light = Light.Create(new Vector3(room.transform.position.x, room.transform.position.y + 2f, room.transform.position.z), Vector3.zero, Vector3.one, false, Color.red);
            light.Intensity = 10f;
            light.Range = 10f;
            light.LightType = LightType.Spot;
            light.ShadowType = LightShadows.Soft;
            light.Color = Color.yellow * 5;


            container.gameObject.AddComponent<PowerUpScript>();

            light.Base.gameObject.transform.SetParent(container.transform, false);
            light.Base.gameObject.transform.position = pos;
            cube.Base.gameObject.transform.SetParent(container.transform, false);
            cube.Base.gameObject.transform.position = pos + Vector3.up / 2;

            Quaternion rot = Quaternion.Euler(90, 0, 0);
            light.Base.NetworkRotation.Set(rot.x, rot.y, rot.z, rot.w);

            light.Base.transform.position += (Vector3.up * 2);
            light.Spawn();
            cube.Spawn();


            pickups.Add(container.gameObject);
            container.gameObject.name = $"{pickups.Count -1}_container";
            cube.Base.gameObject.name = $"{pickups.Count -1}_cube";
            light.Base.gameObject.name = $"{pickups.Count -1}_light";
            Log.Info($"Spawned powerup of index: {pickups.Count() -1}");
        }
        private void OnRoundStarted()
        {
            Log.Info("Map Generated");
            List<Room> rooms = Room.List.ToList();
            for (int i = 0; i < maxPickups; i++)
            {
                var selectedRoom = rooms.GetRandomValue();
                SpawnPowerup(selectedRoom);
                rooms.Remove(selectedRoom);
            }
            Log.Info($"Spawned: {pickups.Count()} powerups");
        }
    }
}