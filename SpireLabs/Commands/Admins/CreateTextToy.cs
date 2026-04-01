//using AdminToys;
//using CommandSystem;
//using CommandSystem.Commands.RemoteAdmin.Dummies;
//using Exiled.API.Features;
//using GameCore;
//using LabApi.Features.Wrappers;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Remoting.Messaging;
//using System.Text;
//using System.Threading.Tasks;

//namespace ObscureLabs.Commands.Admins
//{
//    [CommandHandler(typeof(RemoteAdminCommandHandler))]
//    public class CreateTextToy : ICommand
//    {
//        public string Command => "createtexttoy";
//        public string[] Aliases => new string[] { "ctt" };
//        public string Description => "Creates a floating text toy at your position.";
//        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
//        {
//            if (arguments.Count < 1)
//            {
//                response = "Usage: createtexttoy <text>";
//                return false;
//            }
//            var player = Player.Get(sender);
//            if (player == null)
//            {
//                response = "Error: Could not find player.";
//                return false;
//            }
//            string text = string.Join(" ", arguments.Array.Skip(arguments.Offset).Take(arguments.Count));
//            var toy = ;
//            response = $"Created text toy at current position";
//            return true;
//    }
//}
