using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Spawn;
using Exiled.API.Features.Toys;
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
using Map = Exiled.Events.Handlers.Map;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    internal class Powerup : Module
    {
        public override string Name => "Powerup";

        public override bool IsInitializeOnStart => true;

        public int currentPickups = 0;
        public int maxPickups = 10;

        public List<GameObject> pickups = new List<GameObject>();

        public void Enable()
        {
            Map.Generated += MapGenerated;
            base.Enable();
        }

        public void Disable()
        {
            Map.Generated -= MapGenerated;
            base.Disable();
        }

        public void SpawnPowerup(Room room)
        {
            Vector3 pos = new Vector3(room.Position.x, room.Position.y + 0.5f, room.Position.z);
            Primitive primitive = Primitive.Create(pos, Vector3.zero, Vector3.one, false);
            primitive.Collidable = true;
            primitive.Base.gameObject.GetComponent<Collider>().isTrigger = true;
            primitive.Base.gameObject.GetComponent<Rigidbody>().useGravity = false;
            primitive.Color = new Color(2500, 0, 2500, 0.15f);
            primitive.Base.gameObject.tag = "Powerup";
            primitive.Spawn();
            pickups.Add(primitive.Base.gameObject);

        }
        

        public void MapGenerated()
        {
            for (int i = 0; i < maxPickups; i++)
            {
                foreach (Room room in Room.List)
                {
                    if (UnityEngine.Random.Range(0, 100) <= 75 && currentPickups < maxPickups)
                    {
                        SpawnPowerup(room);
                    }
                }
            }
            

       
        }
    }
}
