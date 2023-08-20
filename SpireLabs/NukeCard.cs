using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Player;
using PluginAPI.Core.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpireLabs
{
    public class NukeCard : CustomItem
    {
        public override string Name { get; set; }
        public override string Description { get; set; }
        public override uint Id { get; set; }
        public override float Weight { get; set; }
        public override ItemType Type { get; set; }
        public override SpawnProperties SpawnProperties { get; set; }
        public override void Init()
        {
            Name = "SCP-018B";
            Description = "MAXball able to MAX with MAXIMUM efficiency.";
            Id = 69;
            Weight = 1;
            Type = ItemType.SCP018;
        }

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Item.KeycardInteracting += OnKeycardUsed;
            base.SubscribeEvents();
        }

        private void OnKeycardUsed(KeycardInteractingEventArgs ev)
        {

        }

    }
}
