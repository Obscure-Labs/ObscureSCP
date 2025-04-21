using Exiled.CustomItems;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Item;
using ObscureLabs.API.Features;
using SpireSCP.GUI.API.Features;
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
            ev.IsAllowed = !CustomItem.TryGet(ev.Player.CurrentItem, out _);
            Manager.SendHint(ev.Player, "<color=red>You are not allowed to change attachments on this weapon</color>", 5f);
        }
    }
}