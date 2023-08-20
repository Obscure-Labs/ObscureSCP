using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Interfaces;
using UnityEngine;
namespace SpireLabs
{
    public class OverrideData
    {
        public bool enabled { get; set; }
        public int healthOverride { get; set; }
    }
    public class ScalingData
    {
        public bool enabled { get; set; }
        public int healthIncrease { get; set; }
    }
    public class config : IConfig
    {
        [Description("Plugin toggle.")]
        public bool IsEnabled { get; set; }
        [Description("Plugin console output.")]
        public bool Debug { get; set; }
        [Description("Sets the 'Micro H.I.D' DPS.")]
        public int hidDPS { get; set; } = 100;
        [Description("Health Override")]
        public OverrideData Scp049Override { get; set; } = new OverrideData { enabled = false, healthOverride = 0 };
        public OverrideData Scp0492Override { get; set; } = new OverrideData { enabled = false, healthOverride = 0 };
        public OverrideData Scp079Override { get; set; } = new OverrideData { enabled = false, healthOverride = 0 };
        public OverrideData Scp096Override { get; set; } = new OverrideData { enabled = false, healthOverride = 0 };
        public OverrideData Scp106Override { get; set; } = new OverrideData { enabled = false, healthOverride = 0 };
        public OverrideData Scp173Override { get; set; } = new OverrideData { enabled = false, healthOverride = 0 };
        public OverrideData Scp939Override { get; set; } = new OverrideData { enabled = false, healthOverride = 0 };
        public OverrideData CaptainOverride { get; set; } = new OverrideData { enabled = false, healthOverride = 0 };
        [Description("Amount of health per player to add:")]
        public ScalingData Scp049 { get; set; } = new ScalingData { enabled = true, healthIncrease = 0 };
        public ScalingData Scp0492 { get; set; } = new ScalingData { enabled = true, healthIncrease = 0 };
        public ScalingData Scp079 { get; set; } = new ScalingData { enabled = true, healthIncrease = 0 };
        public ScalingData Scp096 { get; set; } = new ScalingData { enabled = true, healthIncrease = 0 };
        public ScalingData Scp106 { get; set; } = new ScalingData { enabled = true, healthIncrease = 0 };
        public ScalingData Scp173 { get; set; } = new ScalingData { enabled = true, healthIncrease = 0 };
        public ScalingData Scp939 { get; set; } = new ScalingData { enabled = true, healthIncrease = 0 };
        [Description("Roles to consider:")]
        public bool classd { get; set; }
        public bool scientist { get; set; }
        public bool guard { get; set; }
        [Description("Hint Config:")]
        public int hintHeight { get; set; } = -4;
        public int timeBetweenHints { get; set; } = 120;
        [Description("Lobby Data")]
        public UnityEngine.Vector3 spawnRoomVector3 { get; set; } = new UnityEngine.Vector3((float)-9.94, 1005, (float)82.37);
    }
}