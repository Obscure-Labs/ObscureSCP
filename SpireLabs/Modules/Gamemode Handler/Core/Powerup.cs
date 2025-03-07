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
using UnityEngine;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    public class PowerUpScript : MonoBehaviour
    {
        public void Start()
        {
            Debug.Log("Powerup script attached to object");
        }
        public void Update()
        {

        }
        private void CollisionEnter(Collision other)
        {
            ((Powerup)Plugin.Instance._modules.GetModule("Powerup")).PowerupTriggerEnter(other.collider, gameObject);
            Debug.Log("Collider hit");
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
            UnityEngine.Debug.Log("Triggering Enter");
            pickups.Remove(trigger);
            trigger.GetComponent<Primitive>().UnSpawn();
            Player player = Player.Get(trigger.GetComponent<Player>());
            player.ApplyRandomEffect();
        }

        private void SpawnPowerup(Room room)
        {
            Log.Info("bentation");
            Primitive cube = Primitive.Create(PrimitiveType.Cube, new Vector3(room.transform.position.x, room.transform.position.y + 0.5f, room.transform.position.z), Vector3.zero, Vector3.one, false);
            cube.Flags = PrimitiveFlags.Collidable | PrimitiveFlags.Visible;
            cube.Color = new Color(255, 0, 255, 0.7f);
            cube.Scale = Vector3.one / 2f;
            Rigidbody rb = cube.Base.gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            cube.Collidable = false;
            cube.Base.gameObject.AddComponent<MeshCollider>().isTrigger = true; ;
            cube.Base.gameObject.AddComponent<PowerUpScript>();
            cube.Spawn();

        }
        private void OnRoundStarted()
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
