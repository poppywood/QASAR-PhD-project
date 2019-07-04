using System;
using Qasar.ESB.Filter;
using Qasar.ESB.Adapter;

namespace Qasar.ESB.Pipeline
{
    /// <summary>
    /// Manages Notifications
    /// </summary>
    public static class Notification
    {
        /// <summary>
        /// Send mail to HelpDesk
        /// </summary>
        /// <param name="r"></param>
        /// <param name="id"></param>
        public static void Raise(int id, PipeData data)
        {
            SMTPAdapter s = new SMTPAdapter();
            s.Submit("hub@Qasar-ina.com", Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["NotificationRecipient"]),
                "PRIORITY 1 HUB EXCEPTION", "Hub transaction #" + id + " failed. " + data.Request);
            
        }
        /// <summary>
        /// Sanitise content to return to source system.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static void SanitiseError(int id, PipeData data)
        {
            string msg = string.Empty;
            msg = "<Exception><Message>A serious system error has occurred and the GDC Helpdesk have been notified. ";
            msg = msg + "The hub tracking reference number that led to this failure was #" + id + ".</Message></Exception>";
            data.Request = msg;
        }
    }

}