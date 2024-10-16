using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace ObscureLabs.API.Data
{
    public class SpawnedPowerupBase
    {
        public int ID { get; }
        public Powerup Powerup { get; }
        public float TimeLeft { get; private set; }
        public Vector3 Position { get; }

        public SpawnedPowerupBase(int id, Powerup powerup, float timeLeft, Vector3 position)
        {
            ID = id;
            Powerup = powerup;
            TimeLeft = timeLeft;
            Position = position;
        }

        public virtual void Give(Player p)
        {
            Powerup.Use(p);
        }

        public void Time()
        {
            Thread t = new Thread(() =>
            {
                while (TimeLeft > 0)
                {
                    TimeLeft -= 1;
                    Thread.Sleep(1000);
                }
            });
            t.Start();
        }
    }
}
