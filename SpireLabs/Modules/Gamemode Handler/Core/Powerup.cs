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
using Exiled.API.Enums;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.API.Features;
using System.Diagnostics;
using LabApi.Features.Wrappers;
using Room = Exiled.API.Features.Room;


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
            Powerup.PowerupUsed(gameObject);
        }
    }

    public class Powerup : Module
    {
        public override string Name => "Powerup";

        public override bool IsInitializeOnStart => true;

        public int currentPickups = 0;
        public int maxPickups = 10;

        public List<GameObject> pickups = new List<GameObject>();

        public override bool Enable()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            
            Log.Info("Enabling Powerup Module");
            return base.Enable();

        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
            return base.Disable();
        }

        public static void PowerupUsed(GameObject g)
        {
            
        }

        private void SpawnPowerup(Room room)
        {
            Log.Info("bentation");
            Primitive cube = Primitive.Create(PrimitiveType.Cube, new Vector3(room.transform.position.x, room.transform.position.y + 0.5f, room.transform.position.z), Vector3.zero, Vector3.one, false);
            cube.Flags = PrimitiveFlags.Collidable | PrimitiveFlags.Visible;
            cube.Color = new Color(2550, 0, 2550, 0.1f);
            cube.Scale = Vector3.one / 2f;
            Rigidbody rb = cube.Base.gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            cube.Base.gameObject.GetComponent<Collider>().isTrigger = true;
            cube.Base.gameObject.AddComponent<PowerUpScript>();
            cube.Spawn();

        }
        private void OnWaitingForPlayers()
        {
            Log.Info("Map Generated");

            foreach (Room room in Room.List.ToList().OrderBy(x => Guid.NewGuid()))
            {
                Log.Warn($"Selected Room: {room.name}");
                if (UnityEngine.Random.Range(0, 100) <= 75 && currentPickups < maxPickups)
                {
                    Log.Warn($"Trying to spawn powerup in: {room.name}");
                    SpawnPowerup(room);
                }
            }
        }
    }
}
