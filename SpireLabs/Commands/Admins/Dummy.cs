using CommandSystem;
using CommandSystem.Commands.RemoteAdmin.Dummies;
using Exiled.API.Features;
using GameCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs.Commands.Admins
{
    // [CommandHandler(typeof(RemoteAdminCommandHandler))]
    // public class DummyCom : ICommand
    // {
    //     public string Command => "dumby";
    //
    //     public string[] Aliases => new string[] { "dumb" };
    //
    //     public string Description => "aaaa";
    //
    //     public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    //     {
    //         if (arguments.Count == 0)
    //         {
    //             response =
    //                 "Usage: suck yer gran";
    //             return true;
    //         }
    //
    //         switch(arguments.At(0).ToLower())
    //         {
    //             case "spawn":
    //             {
    //                 response = "Spawned";
    //                 var dumbbb = DummyUtils.SpawnDummy(arguments.At(1));
    //                     var p = Player.Get(dumbbb);
    //                     p.Id = 111;
    //                         p.RoleManager.ServerSetRole(PlayerRoles.RoleTypeId.ClassD, PlayerRoles.RoleChangeReason.ItemUsage);
    //                     p.Emotion = PlayerRoles.FirstPersonControl.Thirdperson.Subcontrollers.EmotionPresetType.Chad;
    //                     dumbbb.gameObject.AddComponent<PlayerFollower>().Init(Player.Get(sender).ReferenceHub, 40, 2, 80);
    //                 return true;
    //             }
    //         }
    //         response = "error";
    //         return false;
    //     }
    // }
}
