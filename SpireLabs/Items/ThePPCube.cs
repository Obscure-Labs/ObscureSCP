///*
//        private static void pp(Player pl)
//        {


//            Primitive p = Primitive.Create(pl.Transform.position + (pl.Transform.forward * 1.25f) + (pl.Transform.up * 0.75f), Vector3.zero, Vector3.one, false);

//            p.Color = new Color(0, 255, 0);
//            p.Scale = Vector3.one * 0.25f;
//            p.MovementSmoothing = 125;
//            p.Base.gameObject.tag = "Cube";
//            p.Spawn();
//            p.Base.gameObject.transform.SetParent(pl.CameraTransform);

//        }
//*/

//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using CommandSystem;
//using Exiled.API.Features;
//using Exiled.API.Features.Toys;
//using Exiled.Permissions.Extensions;
//using UnityEngine;

//namespace SpireLabs.Items
//{
//    [CommandHandler(typeof(ClientCommandHandler))]
//    public class ThePPCube : ICommand
//    {
//        public ThePPCube() => LoadGeneratedCommands();
//        public string Command { get; set; } = "pp";
//        public string[] Aliases { get; set; } = new string[] { };
//        public string Description { get; set; } = "Cube camera";
//        public void LoadGeneratedCommands() { }

//        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
//        {
//            Player pl = Player.Get((CommandSender)sender);
//            Primitive p = Primitive.Create(pl.Transform.position + (pl.Transform.forward * 1.25f) + (pl.Transform.up * 0.75f), Vector3.zero, Vector3.one, false);
//            p.Color = new Color(0, 255, 0);
//            p.Scale = Vector3.one * 0.25f;
//            p.MovementSmoothing = 125;
//            p.Base.gameObject.tag = "Cube";
//            p.Spawn();
//            p.Base.gameObject.transform.SetParent(pl.CameraTransform);
//            response = "cube";
//            return true;
//        }
//    }
//}