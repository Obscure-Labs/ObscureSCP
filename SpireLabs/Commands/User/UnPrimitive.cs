using CommandSystem;
using Exiled.API.Features;
using System;
using System.Linq;

namespace ObscureLabs.Commands.User
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class UnPrimitive : ICommand
    {
        public bool SanitizeResponse { get; }

        public string Command { get; set; } = "unglow";

        public string[] Aliases { get; set; } = new string[] { };

        public string Description { get; set; } = "Cube";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Uh oh!";

            try
            {
                var objects = Plugin.Objects.FirstOrDefault(x => x.Player.NetworkIdentity == Player.Get((CommandSender)sender).NetworkIdentity).Objects;

                for (var i = 0; i < objects.Count; i++)
                {
                    var o = objects.First();
                    o.Destroy();

                    objects.Remove(o);
                }

                response = "Deleted objects";
            }
            catch
            {
                response = "You dont exist lmao";
            }

            return true;
        }
    }
}
