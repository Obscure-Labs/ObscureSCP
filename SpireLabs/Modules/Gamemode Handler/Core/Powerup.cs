using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Spawn;
using Exiled.API.Features.Toys;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Handlers;
using LabApi.Events.Arguments.ServerEvents;
using ObscureLabs.API.Features;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Exiled.API;
using Exiled.Events;
using AdminToys;
using MapEditorReborn.Commands.ModifyingCommands.Position;
using MapEditorReborn.Commands.ModifyingCommands.Rotation;
using Mirror;
using static HarmonyLib.Code;

using LabApi.Features.Wrappers;
using Room = Exiled.API.Features.Room;
using Player = Exiled.API.Features.Player;
using Light = Exiled.API.Features.Toys.Light;
using UnityEngine;
using MapEditorReborn.Commands.ModifyingCommands.Scale;
using Exiled.API.Extensions;
using Exiled.CustomRoles.Commands;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    public class PowerUpScript : MonoBehaviour
    {
        public void Start()
        {
            //idk if debug.log works tbh it should but just incase northwood moment
            LabApi.Features.Console.Logger.Info("Powerup script attached to object");
        }
        public void Update()
        {
            gameObject.GetComponent<Rigidbody>().AddTorque(new Vector3(0.1f, 0, 0.1f));
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
            LabApi.Features.Console.Logger.Info("Triggered Event");
            pickups.Remove(trigger.gameObject);
            Log.Info($"Object: {trigger.gameObject.name}");
            Player player = Player.Get(other);
            var randomitem = new System.Random();
            player.AddItem(itemList.ElementAt(randomitem.Next(0, itemList.Count() + 1)));
            pickups.Remove(trigger);
            NetworkServer.Destroy(trigger.gameObject);
            Log.Warn($"There are: {pickups.Count()} pickups left!");

        }

        private void SpawnPowerup(Room room)
        {
            Log.Info("bentation");
            Primitive cube = Primitive.Create(PrimitiveType.Cube, new Vector3(room.transform.position.x, room.transform.position.y + 1f, room.transform.position.z), Vector3.zero, Vector3.one, false);
            Light light = Light.Create(new Vector3(room.transform.position.x, room.transform.position.y + 1f, room.transform.position.z), Vector3.zero, Vector3.one, false, Color.red);
            light.Color = cube.Color = new Color(10f, 0, 10f, 0.01f);
            light.Intensity = 50f;
            light.Range = 15f;
            light.LightType = LightType.Spot;
            light.ShadowType = LightShadows.Soft;
            light.Base.gameObject.transform.SetParent(cube.Base.transform, true);
            light.Base.gameObject.transform.localPosition = Vector3.zero;
            cube.Flags = PrimitiveFlags.Collidable | PrimitiveFlags.Visible;
            cube.Color = new Color(100f, 0, 100f, 0.25f);
            cube.Scale = Vector3.one / 2f;
            Rigidbody rb = cube.Base.gameObject.AddComponent<Rigidbody>();
            rb.mass = 0f;
            rb.drag = 0f;
            rb.angularDrag = 0f;

            rb.useGravity = false;
            cube.Collidable = false;
            BoxCollider collider = cube.Base.gameObject.AddComponent<BoxCollider>();
            collider.size = cube.Scale;
            collider.isTrigger = true;
            cube.Spawn();
            cube.Base.gameObject.AddComponent<PowerUpScript>();
            pickups.Add(cube.Base.gameObject);

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