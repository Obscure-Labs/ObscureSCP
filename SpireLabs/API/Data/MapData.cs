using UnityEngine;

namespace ObscureLabs.API.Data
{
    public class MapData
    {
        public MapData(string name, string displayName, Vector3 spawnCi, Vector3 spawnNtf)
        {
            Name = name;
            DisplayName = displayName;
            SpawnCi = spawnCi;
            SpawnNtf = spawnNtf;
        }

        public string Name { get; }

        public string DisplayName { get; }

        public Vector3 SpawnCi { get; }

        public Vector3 SpawnNtf { get; }
    }
}
