using System.Collections.Generic;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Player = Exiled.Events.Handlers.Player;
using SpireSCP.GUI.API.Features;
using MEC;
using System.Threading;
using PlayerRoles;
using MapEditorReborn.Commands.UtilityCommands;
using ObscureLabs.API.Data;
using System.Linq;
using UnityEngine.Rendering;
using Exiled.API.Features;
using System;
using Exiled.API.Features.Items;

namespace ObscureLabs.Items
{

        [CustomItem(ItemType.Radio)]
        public class S_NAV : CustomItem
        {
            //function of item: give the distance from you and any SCPs on the map, as long as they are within a certain radius.
            public override string Name { get; set; } = "S-NAV";
            public override string Description { get; set; } = "S-NAV";
            public override uint Id { get; set; } = 12;
            public override float Weight { get; set; } = 2.0f;

            private bool equipped = false;

            public override SpawnProperties SpawnProperties { get; set; } = new()
            {
                Limit = 2,
                DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 0,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideGr18,
                },
            },
            };

            protected override void SubscribeEvents()
            {
                Player.ChangedItem += Equipped;
                base.SubscribeEvents();
            }

            protected override void UnsubscribeEvents()
            {
                Player.ChangedItem -= Equipped;
                base.UnsubscribeEvents();
            }

        protected override void OnAcquired(Exiled.API.Features.Player player, Item item, bool displayMessage)
        {
            base.OnAcquired(player, item, displayMessage);
            Manager.SendHint(player, "S-NAV ver1.0.1:", 5.0f);
            Timing.CallDelayed(5f, () => { Timing.RunCoroutine(Snav(player, item)); });

        }

        private void Equipped(ChangedItemEventArgs ev)
            {
            equipped = Check(ev.Item);

            if (equipped)
            {

                }
                else
                {
                    return;
                }
            }

            public IEnumerator<float> Snav(Exiled.API.Features.Player player, Item item)
            {
                Dictionary<string, float> _nearbySCPs = new();

                string yep = _nearbySCPs.ToString();
                while (player.Items.Contains(item))
                {
                    yield return Timing.WaitForSeconds(0f);
                    if (_nearbySCPs.Count < 1)
                    {
                        Manager.SendHint(player, "No SCP Subjects Nearby! ", 2f);
                    }



                    foreach (Exiled.API.Features.Player pl in Exiled.API.Features.Player.List)
                    {
                        float relative = UnityEngine.Vector3.Distance(pl.Position, player.Position);
                        yield return Timing.WaitForOneFrame;


                        _nearbySCPs = _nearbySCPs.OrderBy(x => x.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

                        if (_nearbySCPs.FirstOrDefault(x => x.Key == pl.Role.Name).Value != null && relative > 50)
                        {
                            _nearbySCPs.Remove(pl.Role.Name);
                        }

                        if (pl.IsScp && relative <= 50f)
                        {

                            if (_nearbySCPs.FirstOrDefault(x => x.Key == pl.Role.Name).Value != null)
                            {
                                _nearbySCPs.Remove(pl.Role.Name);
                                _nearbySCPs.Add(pl.Role.Name, relative);
                            }
                            else
                            {
                                _nearbySCPs.Add(pl.Role.Name, relative);
                            }

                            string hint = string.Empty;
                            for (int i = 0; i < _nearbySCPs.Count; i++)
                            {

                                if (i > 3)
                                {
                                    break;
                                }
 
                                hint += $"{_nearbySCPs.ElementAt(i).Key.ToString()}: {(int)(_nearbySCPs.ElementAt(i).Value)}m\t";
                                
                            }
                            Manager.SendHint(player, hint, 1f);



                        }

                    }
                }
            }
        }
}
