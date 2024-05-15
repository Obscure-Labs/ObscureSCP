using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Item;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomItems;
namespace ObscureLabs.Items
{
    internal class Attachment_Fix : Plugin.Module
    {
        public override string name { get; set; } = "AttachmentFix";
        public override bool initOnStart { get; set; } = true;


        public override bool Init()
        {
            try
            {
                Exiled.Events.Handlers.Item.ChangingAttachments += changedAttachment;
                base.Init();
                return true;
            }
            catch { return false; }
        }

        public override bool Disable()
        {
            try
            {
                Exiled.Events.Handlers.Item.ChangingAttachments -= changedAttachment;
                base.Disable();
                return true;
            }
            catch { return false; }
        }


        private static void changedAttachment(ChangingAttachmentsEventArgs ev)
        {
            if (CustomItems.API.API.HasCustomItemInHand(ev.Player, out CustomItems.API.CustomItem i))
            {
                ev.IsAllowed = false;
            }

        }
    }
}
