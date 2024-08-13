using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using ObscureLabs.API.Data;
using ObscureLabs.API.Features;
using PlayerRoles;
using SpireSCP.GUI.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading;
using System.Runtime.CompilerServices;

namespace ObscureLabs.Modules.Gamemode_Handler.Minigames
{
    public class TeamDeathmatch : Module
    {
        public override string Name => "TeamDeathmatch";

        public override bool IsInitializeOnStart => false;

        public List<Thread> Threads = new List<Thread>();

        public static List<Player> NTF = new List<Player>();
        public static List<Player> Chaos = new List<Player>();

        public override bool Enable()
        {
            Thread startupThread = new Thread(StartUp);
            startupThread.Start();
            Threads.Add(startupThread);
            return base.Enable();
        }

        public override bool Disable()
        {
            CleanUp();
            return base.Disable();
        }

        public void CleanUp()
        {

           foreach (Thread thread in Threads)
            {
                thread.Abort();
            }
        }

        public static void StartUp()
        {
            
        }

        public static void AssignTeams()
        {
            List<Player> PlayerList = Player.List.ToList();
            PlayerList.OrderBy(x => Guid.NewGuid());

            for(int i = 0; i < PlayerList.Count; i++)
            {
                if(i % 2 == 0)
                {
                    NTF.Add(PlayerList[i]);
                }
                else
                {
                    Chaos.Add(PlayerList[i]);
                }
            }
        }

        public static void SpawnTeams()
        {

        }
    }
}
