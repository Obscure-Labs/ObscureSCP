using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using MEC;
using UncomplicatedCustomRoles.Commands.UCRSpawn;

namespace SpireLabs.Commands.Jukebox
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class JBPlay : ICommand
    {
        public JBPlay() => LoadGeneratedCommands();
        public string Command { get; set; } = "jb";
        public string[] Aliases { get; set; } = new string[] { };
        public string Description { get; set; } = "Mr Jukebox!";
        public void LoadGeneratedCommands() { }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Incorrect Syntax! Use: jb {play, stop, list}";
            if (arguments.Count == 0) return false;


            string folder = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\EXILED\\Configs\\Spire\\Audio/");
            IEnumerable<string> filder = Directory.EnumerateFiles(folder);


            if (!((CommandSender)sender).CheckPermission("spire.jb"))
            {
                response = "You do not have permission to use the jukebox.";
                return false;
            }


            if (arguments.FirstElement().ToLower() == "list")
            {
                string thing = "The audio files are as follows : ";
                for (int i = 0; i < filder.Count(); i++)
                {
                    thing += $"\n{i}. {filder.ElementAt(i).Replace(".ogg", "").Replace(@"C:\Users\Kevin\AppData\Roaming\EXILED\Configs\Spire\Audio\", "")}";
                }
                response = thing;
                return true;
            }

            if (arguments.FirstElement().ToLower() == "play")
            {
                if (arguments.Count() < 2)
                {
                    response = "You need to specify. Usage: jb {play, stop, list} {song id}";
                    return false;
                }
                if (int.TryParse(arguments.ElementAt(1), out int audioID))
                {
                    if (audioID > filder.Count())
                    {
                        response = "That file does not exist!";
                        return false;
                    }
                    List<int> l = new List<int>();
                    List<int> s = new List<int>();
                    foreach (profiles.personData pp in profiles.Profiles)
                    {
                        Log.Warn($"{pp.uid} is {pp.audioToggle}");
                        if (pp.audioToggle)
                        {
                            l.Add(pp.uid);

                        }
                        else
                        {
                            s.Add(pp.uid);
                        }
                    }
                    foreach (int i in l)
                    {
                        Log.Warn(i);
                    }
                    AudioPlayer.API.AudioController.PlayFromFilePlayer(l, filder.ElementAt(audioID), false, 4, VoiceChat.VoiceChatChannel.Proximity);
                    AudioPlayer.API.AudioController.StopPlayerFromPlaying(s);
                    response = $"Now playing: {filder.ElementAt(audioID).Replace(".ogg", "").Replace(@"C:\Users\Kevin\AppData\Roaming\EXILED\Configs\Spire\Audio\", "")}!";
                    return true;
                }

            }

            if (arguments.FirstElement().ToLower() == "stop")
            {
                AudioPlayer.API.AudioController.StopAudio(99);
                response = "Stopped playing successfully";
                return false;
            }
            if (arguments.FirstElement().ToLower() == "toggle")
            {
                if (!profiles.Profiles.FirstOrDefault(x => x.steam64 == Player.Get(sender).UserId).audioToggle) response = $"You toggled the jukebox on!"; else response = $"You toggled the jukebox off!";
                AudioPlayer.API.AudioController.StopPlayerFromPlaying(new List<int> { Player.Get(sender).Id });
                profiles.toggleAudio(Player.Get(sender));

                return false;
            }

            return false;
        }
    }
}
