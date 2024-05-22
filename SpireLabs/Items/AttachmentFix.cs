using Exiled.Events.EventArgs.Item;
using ObscureLabs.API.Features;

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
            ev.IsAllowed = !CustomItems.API.API.HasCustomItemInHand(ev.Player, out _);
        }
    }
}
