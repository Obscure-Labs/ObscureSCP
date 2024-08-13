using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ObscureLabs.Modules.Gamemode_Handler.Minigames.Data
{
    public class TDMData
    {
        public TDMData(Vector3 point1, Vector3 point2, IDictionary<Vector3, ItemType> pickups)
        {
            Point1 = point1;
            Point2 = point2;

        }

        Vector3 Point1 { get; set; }
        Vector3 Point2 { get; set; }

        IDictionary<RoleTypeId, SerializableTeamVolume> TeamSpawnVolumes { get; set; }

        IDictionary<Vector3, ItemType> Pickups { get; set; }
    }

    public class SerializableTeamVolume
    {
        public SerializableTeamVolume(Vector3 spawnVolumePoint1, Vector3 spawnVolumePoint2)
        {
            SpawnVolumePoint1 = spawnVolumePoint1;
            SpawnVolumePoint2 = spawnVolumePoint2;
        }

        Vector3 SpawnVolumePoint1 { get; set; }
        Vector3 SpawnVolumePoint2 { get; set; }
    }
}
