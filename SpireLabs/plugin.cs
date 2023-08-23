namespace SpireLabs
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs.Item;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Server;
    using MEC;
    using PlayerRoles;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UnityEngine;
    using System.IO;

    public class Plugin : Plugin<config>
    {
        /// <summary>
        /// Add coin flip
        /// Make remote keycard
        /// </summary>
        public override string Name => "Spire Labs";
        public override string Author => "ImIsaacTbh";
        public override System.Version Version => new System.Version(1, 0, 0);
        public override System.Version RequiredExiledVersion => new System.Version(2, 1, 0);
        public static float hidDPS;

        public static OverrideData OScp049;
        public static OverrideData OScp0492;
        public static OverrideData OScp079;
        public static OverrideData OScp096;
        public static OverrideData OScp106;
        public static OverrideData OScp173;
        public static OverrideData OScp939;
        public static OverrideData OCaptain;

        public static ScalingData Scp049;
        public static ScalingData Scp0492;
        public static ScalingData Scp079;
        public static ScalingData Scp096;
        public static ScalingData Scp106;
        public static ScalingData Scp173;
        public static ScalingData Scp939;

        public static bool classd;
        public static bool scientist;
        public static bool guard;

        public static string[] file;

        public static int lpc = 0;

        public static int hintHeight = 0;
        public static int hintTime = 0;

        public static bool inLobby = false;

        public static UnityEngine.Vector3 lobbyVector;

        public static string lastId;

        public static bool limitMet = false;

        public static bool first = false;
        public static bool allowStart = false;
        public static int playerCount = 0;

        public static bool hasRestarted = false;
        public static bool ConMet = false;

        public static CoroutineHandle handle;
        public static bool realRoundEnd = false;
        public static bool startingRound = false;
        public static bool initing = false;

        public string[] good = { "You gained 20HP!", "You gained a 5 second speed boost!", "You found a keycard!", "You are invisible for 5 seconds!" };
        public string[] bad = { "You now have 1HP!", "You dropped all of your items, How clumsy...", "You have heavy feet for 5 seconds...", "You have dust in your eye!", "You got lost and found yourself in a random room!" };
        public override void OnDisabled()
        {
            UnregisterEvents();
            Exiled.API.Features.Log.Info("Spire Labs has been disabled!");
            base.OnDisabled();
        }
        public override void OnEnabled()
        {
            RegisterEvents();
            Exiled.API.Features.Log.Info("Spire Labs has been enabled!");
            base.OnEnabled();
            hidDPS = Config.hidDPS;
            OScp049 = Config.Scp049Override;
            OScp0492 = Config.Scp049Override;
            OScp079 = Config.Scp079Override;
            OScp096 = Config.Scp096Override;
            OScp106 = Config.Scp106Override;
            OScp173 = Config.Scp173Override;
            OScp939 = Config.Scp939Override;
            OCaptain = Config.CaptainOverride;

            Scp049 = Config.Scp049;
            Scp0492 = Config.Scp0492;
            Scp079 = Config.Scp079;
            Scp096 = Config.Scp096;
            Scp106 = Config.Scp106;
            Scp173 = Config.Scp173;
            Scp939 = Config.Scp939;

            classd = Config.classd;
            scientist = Config.scientist;
            guard = Config.guard;

            hintHeight = Config.hintHeight;
            hintTime = Config.timeBetweenHints;

            lobbyVector = Config.spawnRoomVector3;

            file = File.ReadAllLines(@"C:\Users\Kebin\AppData\Roaming\EXILED\Configs\Spire/lines.txt");
            Timing.RunCoroutine(ShowHint());
            inLobby = false;
        }

        private void RegisterEvents()
        {
            Exiled.Events.Handlers.Player.Hurting += theThing;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStart;
            Exiled.Events.Handlers.Player.Spawned += Player_Spawned;
            Exiled.Events.Handlers.Player.FlippingCoin += Player_FlippingCoin;
            Exiled.Events.Handlers.Player.Joined += Player_Joined;
            Exiled.Events.Handlers.Player.PreAuthenticating += Authing;
            Exiled.Events.Handlers.Server.RestartingRound += restarting;
            Exiled.Events.Handlers.Player.ChangedItem += item_change;
        }

        private void item_change(ChangedItemEventArgs ev)
        {
            if (ev.NewItem == null) return;
            if (ev.NewItem.Type != ItemType.Coin)
                return;
            string hint = string.Empty;
            if (hintHeight != 0 && hintHeight < 0)
            {
                for (int i = hintHeight; i < 0; i++)
                {
                    hint += "\n";
                }
            }
            hint += "Flipping this coin will cause a random event, use with caution!";
            if (hintHeight != 0 && hintHeight > 0)
            {
                for (int i = 0; i < hintHeight; i++)
                {
                    hint += "\n";
                }
            }
            ev.Player.ShowHint(hint, 5);
        }

        private void restarting()
        {
            if (realRoundEnd)
            {
                hasRestarted = false;
                first = false;
                inLobby = false;
                allowStart = false;
                realRoundEnd = false;
                playerCount = 0;
                ConMet = false;
                startingRound = false;
                initing = false;
                Exiled.Events.Handlers.Player.Left -= Player_Leave;
            }
        }
       private void Authing(PreAuthenticatingEventArgs ev)
       {
       }

       private IEnumerator<float> startCheck()
       {
           while (true)
           {
               while (!hasRestarted && !ConMet && !initing)
               {
                   if (playerCount > 1)
                   {
                       Exiled.API.Features.Log.Info($"Player count is : \"{playerCount}\" (startcheck)");
                       Timing.RunCoroutine(restart());
                       allowStart = true;
                       ConMet = true;
                       initing = true;
                   }
                   yield return Timing.WaitForSeconds(1);
               }
               yield return Timing.WaitForSeconds(1);
               if (startingRound == true)
               {
                   startingRound = false;
                   break;
               }
           }
       }

       private IEnumerator<float> EngageLobby(JoinedEventArgs ev)
       {
           Exiled.API.Features.Log.Debug("STARTING LEAVE EVENT");
           Exiled.Events.Handlers.Player.Left += Player_Leave;
           Exiled.API.Features.Log.Debug("ENABLED LEAVE EVENT");
           first = true;
           inLobby = true;
           yield return Timing.WaitForSeconds(2);
           Exiled.API.Features.Round.IsLocked = true;
           Exiled.API.Features.Round.Start();
           yield return Timing.WaitForSeconds(1);
           ev.Player.RoleManager.ServerSetRole(RoleTypeId.ClassD, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.UseSpawnpoint);
           yield return Timing.WaitForSeconds((float)0.25);
           ev.Player.Teleport(lobbyVector);
           handle = Timing.RunCoroutine(startCheck());
           File.WriteAllText(@"C:\Users\Kebin\AppData\Roaming\EXILED\Configs\Spire/stinky.txt", "pp");
       }

       private IEnumerator<float> restart()
       {
           Exiled.API.Features.Cassie.Message("Round Starting in 25 seconds", isSubtitles: true, isNoisy: false);
           yield return Timing.WaitForSeconds(25);
           if (playerCount < 2) {

               Exiled.API.Features.Cassie.Message($"ROUND START DISABLED (Too few players)", isSubtitles: true, isNoisy: false);
               initing = false;
           }
           else
           {
               Exiled.API.Features.Cassie.Message("STARTING MATCH", isSubtitles: true, isNoisy: false);
               foreach (Exiled.API.Features.Player p in Exiled.API.Features.Player.List)
               {
                   p.EnableEffect(EffectType.Blinded, 3);
               }
               yield return Timing.WaitForSeconds(2);
               hasRestarted = true;
               first = false;
               startingRound = true;
               Exiled.Events.Handlers.Player.Left -= Player_Leave;
               Exiled.API.Features.Round.RestartSilently();
           }

       }

       private IEnumerator<float> go()
       {
           yield return Timing.WaitForSeconds(2);
           Exiled.API.Features.Cassie.Message("STARTING ROUND", isSubtitles: true, isNoisy: false);
           yield return Timing.WaitForSeconds(7);
           Exiled.API.Features.Round.Start();
           File.WriteAllText(@"C:\Users\Kebin\AppData\Roaming\EXILED\Configs\Spire/stinky.txt", "ee");
           inLobby = false;
           realRoundEnd = true;
       }

       private void Player_Leave(LeftEventArgs ev)
       {
           if (lastId != ev.Player.UserId)
           {
               playerCount--;
               Exiled.API.Features.Log.Info($"Player count is now: \"{playerCount}\"");
           }
           lastId = ev.Player.UserId;
       }
       private IEnumerator<float> InsertPlayer(JoinedEventArgs ev)
       {
           yield return Timing.WaitForSeconds(2);
           ev.Player.RoleManager.ServerSetRole(RoleTypeId.ClassD, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.UseSpawnpoint);
           ev.Player.Teleport(lobbyVector);
       }

       private void Player_Joined(JoinedEventArgs ev)
       {
           lastId = string.Empty;
           ConMet = false;
           playerCount++;
           Exiled.API.Features.Log.Info($"Player count is now: \"{playerCount}\"");
           //ev.Player.Broadcast(new Broadcast { Content = "Player joined", Duration = 1, Show = true, Type = global::Broadcast.BroadcastFlags.Normal });
           if (!hasRestarted)
           {
               if (!inLobby && !first && !Exiled.API.Features.Round.IsStarted) //EngageLobby();
               {
                   Timing.RunCoroutine(EngageLobby(ev));
               }
               else if (first)
               {
                   Timing.RunCoroutine(InsertPlayer(ev));
                   return;
               }
           }
           else if (!first)
           {
               Timing.RunCoroutine(go());
               first = true;
           }
       }

        //private IEnumerator<float> scalePlayer(FlippingCoinEventArgs ev)
        //{
        //    for (int i = 0; i < 50; i++)
        //    {
        //        var rnd = new System.Random();
        //        float num1 = (float)(rnd.NextDouble() * rnd.Next(1, 5));
        //        float num2 = (float)(rnd.NextDouble() * rnd.Next(1, 5));
        //        ev.Player.Scale = new Vector3(num1, 1f, num2);
        //        yield return Timing.WaitForSeconds(0.1f);
        //    }
        //    ev.Player.Scale = new Vector3(1f, 1f, 1f);
        //    Throwable g = ev.Player.ThrowGrenade(ProjectileType.FragGrenade);
        //}

        private void Player_FlippingCoin(FlippingCoinEventArgs ev)
        {

            //Timing.RunCoroutine(scalePlayer(ev));

            var rnd = new System.Random();
            int num = rnd.Next(0, 1);
            bool result = false;
            if (num == 1) result = true;
            if (result)
            {
                switch (rnd.Next(0, good.Count()))
                {
                    case 0:
                        ev.Player.ShowHint(good[0], 3);
                        ev.Player.Heal(20, true);
                        break;
                    case 1:
                        ev.Player.ShowHint(good[1], 3);
                        ev.Player.EnableEffect(EffectType.MovementBoost, 5);
                        break;
                }
            }
            else
            {
                switch (rnd.Next(0, bad.Count()))
                {
                    case 0:
                        ev.Player.ShowHint(bad[0], 3);
                        ev.Player.Health = 1;
                        break;
                    case 1:
                        ev.Player.ShowHint(bad[1], 3);
                        break;
                }
            }
        }

        private void UnregisterEvents()
         {
             Exiled.Events.Handlers.Player.Hurting -= theThing;
             Exiled.Events.Handlers.Item.KeycardInteracting -= Item_KeycardInteracting;
             Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStart;
             Exiled.Events.Handlers.Player.Spawned -= Player_Spawned;
             Exiled.Events.Handlers.Player.FlippingCoin -= Player_FlippingCoin;
             Exiled.Events.Handlers.Player.Joined -= Player_Joined;
             Exiled.Events.Handlers.Player.PreAuthenticating -= Authing;
             Exiled.Events.Handlers.Server.RestartingRound -= restarting;
         }

         static void theThing(HurtingEventArgs ev)
         {
             if (ev.DamageHandler.Type == DamageType.MicroHid)
             {
                 ev.Amount *= (hidDPS / 100);
             }
         }
         public static void Item_KeycardInteracting(KeycardInteractingEventArgs ev)
         {
             Exiled.API.Features.Log.Info($"Door opened, requires: {ev.Door.RequiredPermissions.RequiredPermissions}");
         }

         private void Player_Spawned(SpawnedEventArgs ev)
         {
             if (OCaptain.enabled)
             {
                 if (ev.Player.RoleManager.CurrentRole.RoleTypeId == RoleTypeId.NtfCaptain)
                 {
                     ev.Player.MaxHealth = OCaptain.healthOverride;
                     ev.Player.Heal(OCaptain.healthOverride, false);
                 }
             }

         }

         private IEnumerator<float> ShowHint()
         {
             var rnd = new System.Random();
             yield return Timing.WaitForSeconds(30);
             while (!Exiled.API.Features.Round.IsEnded)
             {
                 file = File.ReadAllLines(@"C:\Users\Kebin\AppData\Roaming\EXILED\Configs\Spire/lines.txt");
                 string hintMessage = string.Empty;
                 if (hintHeight != 0 && hintHeight < 0)
                 {
                     for (int i = hintHeight; i < 0; i++)
                     {
                         hintMessage += "\n";
                     }
                 }
                 hintMessage += file[rnd.Next(0, file.Count() - 1)];
                 if (hintHeight != 0 && hintHeight > 0)
                 {
                     for (int i = 0; i < hintHeight; i++)
                     {
                         hintMessage += "\n";
                     }
                 }
                 foreach (Exiled.API.Features.Player p in Exiled.API.Features.Player.List)
                 {
                     if (!p.IsDead)
                     {
                         p.ShowHint($"{hintMessage}", 5);
                     }

                 }
                 yield return Timing.WaitForSeconds(hintTime);
             }
         }
         void OnRoundStart()
         {
             //Exiled.API.Features.Server.Broadcast.SendMessage("Lobby initialised. Awaiting round start.");
             //while(Exiled.API.Features.Player.List.Count() < 2)
             //{
             //    Thread.Sleep(2000);
             //}
             //while(Exiled.API.Features.Player.List.Count() > 1)
             //{

             //}
             //Exiled.API.Features.Round.RestartSilently();

             Exiled.API.Features.Log.Info("Round has started!");
             var players = Exiled.API.Features.Player.List;
             List<Exiled.API.Features.Player> SCPS = new List<Exiled.API.Features.Player>();
             int humanPlayers = 0;
             foreach (Exiled.API.Features.Player p in players)
             {
                 switch (p.RoleManager.CurrentRole.RoleTypeId)
                 {
                     case RoleTypeId.ClassD:
                         humanPlayers++;
                         break;
                     case RoleTypeId.Scientist:
                         humanPlayers++;
                         break;
                     case RoleTypeId.FacilityGuard:
                         humanPlayers++;
                         break;
                     case RoleTypeId.Scp049: SCPS.Add(p); break;
                     case RoleTypeId.Scp079: SCPS.Add(p); break;
                     case RoleTypeId.Scp096: SCPS.Add(p); break;
                     case RoleTypeId.Scp106: SCPS.Add(p); break;
                     case RoleTypeId.Scp173: SCPS.Add(p); break;
                     case RoleTypeId.Scp939: SCPS.Add(p); break;
                     case RoleTypeId.NtfCaptain:
                         p.MaxHealth = OCaptain.healthOverride;
                         p.Heal(150, false);
                         break;
                 }
             }
             foreach (Exiled.API.Features.Player p in SCPS)
             {
                 switch (p.RoleManager.CurrentRole.RoleTypeId)
                 {
                     case RoleTypeId.Scp049:
                         if (OScp049.enabled)
                         {
                             p.MaxHealth = OScp049.healthOverride;
                         }
                         Task.Delay(50);
                         if (Scp049.enabled)
                         {
                             p.MaxHealth += (Scp049.healthIncrease * humanPlayers);
                             p.Heal(p.MaxHealth);
                         }
                         break;
                     case RoleTypeId.Scp079:
                         if (OScp079.enabled)
                         {
                             p.MaxHealth = OScp079.healthOverride;
                         }
                         Task.Delay(50);
                         if (Scp079.enabled)
                         {
                             p.MaxHealth += (Scp079.healthIncrease * humanPlayers);
                             p.Heal(p.MaxHealth);
                         }
                         break;
                     case RoleTypeId.Scp096:
                         if (OScp096.enabled)
                         {
                             p.MaxHealth = OScp096.healthOverride;
                         }
                         Task.Delay(50);
                         if (Scp096.enabled)
                         {
                             p.MaxHealth += (Scp096.healthIncrease * humanPlayers);
                             p.Heal(p.MaxHealth);
                         }
                         break;
                     case RoleTypeId.Scp106:
                         if (OScp106.enabled)
                         {
                             p.MaxHealth = OScp106.healthOverride;
                         }
                         Task.Delay(50);
                         if (Scp106.enabled)
                         {
                             p.MaxHealth += (Scp106.healthIncrease * humanPlayers);
                             p.Heal(p.MaxHealth);
                         }
                         break;
                     case RoleTypeId.Scp173:
                         if (OScp173.enabled)
                         {
                             p.MaxHealth = OScp173.healthOverride;
                         }
                         Task.Delay(50);
                         if (Scp173.enabled)
                         {
                             p.MaxHealth += (Scp173.healthIncrease * humanPlayers);
                             p.Heal(p.MaxHealth);
                         }
                         break;
                     case RoleTypeId.Scp939:
                         if (OScp939.enabled)
                         {
                             p.MaxHealth = OScp939.healthOverride;
                         }
                         Task.Delay(50);
                         if (Scp939.enabled)
                         {
                             p.MaxHealth += (Scp939.healthIncrease * humanPlayers);
                             p.Heal(p.MaxHealth);
                         }
                         break;
                 }
             }
         }
    }
}
