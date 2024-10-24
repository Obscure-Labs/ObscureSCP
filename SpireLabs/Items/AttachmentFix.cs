using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
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
            ev.IsAllowed = !CustomItem.TryGet(ev.Player.CurrentItem, out _);
        }
    }
}
