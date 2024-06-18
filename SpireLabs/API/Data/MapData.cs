using UnityEngine;

namespace ObscureLabs.API.Data
{
    public class MapData
    {
        public MapData(string name, string displayName, Vector3 spawnCi, Vector3 spawnNtf)
        {
            Name = name;
            DisplayName = displayName;
            SpawnCi = new Vector3(spawnCi.x, spawnCi.y + 1.0f, spawnCi.z);
            SpawnNtf = new Vector3(spawnNtf.x, spawnNtf.y + 1.0f, spawnNtf.z);
        }

        public string Name { get; }

        public string DisplayName { get; }

        public Vector3 SpawnCi { get; }

        public Vector3 SpawnNtf { get; }
    }
}
