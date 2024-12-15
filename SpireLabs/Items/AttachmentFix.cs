using Exiled.CustomItems;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Item;
using ObscureLabs.API.Features;
using System.Linq;

namespace ObscureLabs.Items
{
    internal class AttachmentFix : Module
    {
        public override string Name => "AttachmentFix";

        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {
            Exiled.Events.Handlers.Item.ChangingAttachments += OnChangingAttachments;

            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Item.ChangingAttachments -= OnChangingAttachments;

            return base.Disable();
        }

        private void OnChangingAttachments(ChangingAttachmentsEventArgs ev)
        {
            if (CustomItem.Registered.FirstOrDefault(x => x.TrackedSerials.Contains(ev.Firearm.Serial)) != null)
            {
                ev.IsAllowed = false;
            }
            //ev.IsAllowed = !CustomItems.API.HasCustomItemInHand(ev.Player, out _);
        }
    }
}
