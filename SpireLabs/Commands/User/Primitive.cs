using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Toys;
using ObscureLabs.API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ObscureLabs.Commands.User
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Primitive : ICommand
    {
        public string Command { get; set; } = "glow";

        public string[] Aliases { get; set; } = new string[] { };

        public string Description { get; set; } = "Cube camera";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var player = Player.Get((CommandSender)sender);
            var position = player.Transform.position;

            Exiled.API.Features.Toys.Light p = Exiled.API.Features.Toys.Light.Create(new Vector3(position.x, position.y + 1.35f, position.z), new Vector3(0, 0, 0), new Vector3(1, 1, 1), true, Color.magenta);

            p.Spawn();
            p.Range = 25;
            p.MovementSmoothing = 60;
            p.Intensity = 25f;
            p.Base.gameObject.transform.SetParent(player.GameObject.transform);

            var primitiveData = Plugin.Objects.FirstOrDefault(x => x.Player.NetworkIdentity == player.NetworkIdentity);

            if (primitiveData is null)
            {
                Plugin.Objects.Add(new PlayerPrimitiveData(player, new List<AdminToy>()));
                Plugin.Objects.FirstOrDefault(x => x.Player.NetworkIdentity == player.NetworkIdentity).Objects.Add(p);
            }
            else
            {
                primitiveData.Objects.Add(p);
            }

            response = "Made Object";
            return true;
        }
    }
}
