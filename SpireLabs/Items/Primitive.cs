/*
        private static void pp(Player pl)
        {


            Primitive p = Primitive.Create(pl.Transform.position + (pl.Transform.forward * 1.25f) + (pl.Transform.up * 0.75f), Vector3.zero, Vector3.one, false);

            p.Color = new Color(0, 255, 0);
            p.Scale = Vector3.one * 0.25f;
            p.MovementSmoothing = 125;
            p.Base.gameObject.tag = "Cube";
            p.Spawn();
            p.Base.gameObject.transform.SetParent(pl.CameraTransform);

        }
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandSystem;
using Exiled.API.Features.Toys;
using Exiled.Permissions.Extensions;
using Mirror;
using UnityEngine;
using Exiled.API.Features;

namespace ObscureLabs.Items
{
    public class PlayerPrimitive
    {
        public Player Player { get; set; }
        public List<AdminToy> Obj { get; set; }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class Primitive : ICommand
    {
        public Primitive() => LoadGeneratedCommands();
        public string Command { get; set; } = "glow";
        public string[] Aliases { get; set; } = new string[] { };
        public string Description { get; set; } = "Cube camera";
        public void LoadGeneratedCommands() { }

        
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player pl = Player.Get((CommandSender)sender);
            Vector3 loc = pl.Transform.position;
            //Exiled.API.Features.Toys.Primitive p = Exiled.API.Features.Toys.Primitive.Create(new Vector3(loc.x, loc.y + 1.35f, loc.z), new Vector3(0, 0, 0), new Vector3(1f, 0.01f, 1f), false);
            //Exiled.API.Features.Toys.Primitive p1 = Exiled.API.Features.Toys.Primitive.Create(new Vector3(loc.x, loc.y + 1.35f, loc.z), new Vector3(45, 45, 45), new Vector3(1f, 0.01f, 1f), false);


            //p1.Collidable = false;
            //p1.Type = PrimitiveType.Cube;
            //p1.Color = new Color(50f, 0f, 50f, 0.45f);
            //p1.MovementSmoothing = 55;
            //p1.Scale = new Vector3(0.2f, 0.2f, 0.2f);


            //p.Collidable = false;
            //p.Type = PrimitiveType.Cube;
            //p.Color = new Color(50f, 25f, 50f, 0.005f);
            //p.MovementSmoothing = 55;
            //p.Scale = new Vector3(0.2f, 0.2f, 0.2f);

            // p1.Spawn();

            Exiled.API.Features.Toys.Light p = Exiled.API.Features.Toys.Light.Create(new Vector3(loc.x, loc.y + 1.35f, loc.z), new Vector3(0, 0, 0), new Vector3(1, 1, 1), true, Color.magenta);

            p.Spawn();
            p.Range = 25;
            p.Intensity = 15f;
            p.Base.gameObject.transform.SetParent(pl.GameObject.transform);
            //p1.Base.gameObject.transform.SetParent(pl.GameObject.transform);

            if (Plugin.OBJLST.FirstOrDefault(x => x.Player.NetworkIdentity == pl.NetworkIdentity) == null)
                {
                    Plugin.OBJLST.Add(new PlayerPrimitive { Player = pl, Obj = new List<AdminToy>() });
                    Plugin.OBJLST.FirstOrDefault(x => x.Player.NetworkIdentity == pl.NetworkIdentity).Obj.Add(p);
                }
                else
                {
                    Plugin.OBJLST.FirstOrDefault(x => x.Player.NetworkIdentity == pl.NetworkIdentity).Obj.Add(p);
                }

            response = "Made Object";
            return true;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class UnPrimitive : ICommand
    {
        public UnPrimitive() => LoadGeneratedCommands();
        public string Command { get; set; } = "unglow";
        public string[] Aliases { get; set; } = new string[] { };
        public string Description { get; set; } = "Cube";
        public void LoadGeneratedCommands() { }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Uh oh!";
            try
            {
                for (int i = 0; i < Plugin.OBJLST.FirstOrDefault(x => x.Player.NetworkIdentity == Player.Get((CommandSender)sender).NetworkIdentity).Obj.Count(); i++)
                {
                    var o = Plugin.OBJLST.FirstOrDefault(x => x.Player.NetworkIdentity == Player.Get((CommandSender)sender).NetworkIdentity).Obj.First();
                    o.Destroy();
                    Plugin.OBJLST.FirstOrDefault(x => x.Player.NetworkIdentity == Player.Get((CommandSender)sender).NetworkIdentity).Obj.Remove(o);
                }
                response = "Deleted objects";
            }
            catch(Exception ex)
            {
                response = "You dont exist lmao";
            }

            return true;
        }
    }
   
}