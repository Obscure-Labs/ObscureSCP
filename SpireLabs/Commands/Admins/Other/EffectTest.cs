using CommandSystem;
using Exiled.API.Features;
using System;
using ObscureLabs.API.Enums;
using ObscureLabs.API.Features;
using ObscureLabs.Extensions;
using ObscureLabs.Modules.Gamemode_Handler.Gamemodes;

namespace ObscureLabs.Commands.Admin.Other
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class EffectTest : ICommand
    {
        public bool SanitizeResponse { get; }
        public string Command { get; set; } = "effect";

        public string[] Aliases { get; set; } = new string[] { };

        public string Description { get; set; } = "Testing only!";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player.Get(sender).GiveEffect(Effects.StolenUniformGuard);
            response = $"granting effect";

            return true;
        }
    }
}
