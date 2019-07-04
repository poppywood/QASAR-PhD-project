using System;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Security;
using System.Security.Principal;


namespace Qasar.ESB.Helpers
{
    /// <summary>
    /// A handler for exceptions
    /// </summary>
    public static class ExceptionHandler
    {

        public static string CreateMessage(Exception exception)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Source: " + exception.Source + "\r\n");
            stringBuilder.Append("Message: " + exception.Message + "\r\n");
            if (exception.InnerException != null)
                stringBuilder.Append("Exception: " + exception.InnerException.ToString() + "\r\n");
            stringBuilder.Append("MachineName: " + GetMachineName() + "\r\n");
            stringBuilder.Append("TimeStamp: " + DateTime.UtcNow.ToString(CultureInfo.CurrentCulture) + "\r\n");
            stringBuilder.Append("FullName: " + GetExecutingAssembly() + "\r\n");
            stringBuilder.Append("AppDomainName: " + AppDomain.CurrentDomain.FriendlyName + "\r\n");
            stringBuilder.Append("ThreadIdentity: " + GetThreadIdentity() + "\r\n");
            stringBuilder.Append("WindowsIdentity: " + GetWindowsIdentity() + "\r\n");
            return stringBuilder.ToString();
        }


        private static string GetMachineName()
        {
            string machineName = String.Empty;
            try
            {
                machineName = Environment.MachineName;
            }
            catch (InvalidOperationException)
            {
                machineName = "Permission Denied";
            }
            catch
            {
                machineName = "Unknown Read Error";
            }

            return machineName;
        }

        private static string GetThreadIdentity()
        {
            string threadIdentity = String.Empty;
            try
            {
                threadIdentity = Thread.CurrentPrincipal.Identity.Name;
            }
            catch (SecurityException)
            {
                threadIdentity = "Permission Denied";
            }
            catch
            {
                threadIdentity ="Unknown Read Error";
            }

            return threadIdentity;
        }

        private static string GetWindowsIdentity()
        {
            string windowsIdentity = String.Empty;
            try
            {
                windowsIdentity = WindowsIdentity.GetCurrent().Name;
            }
            catch (SecurityException)
            {
                windowsIdentity = "Permission Denied";
            }
            catch
            {
                windowsIdentity = "Unknown Read Error";
            }

            return windowsIdentity;
        }

        private static string GetExecutingAssembly()
        {
            string executingAssembly = String.Empty;

            try
            {
                executingAssembly = Assembly.GetExecutingAssembly().FullName;
            }
            catch (SecurityException)
            {
                executingAssembly = "Permission Denied";
            }
            catch
            {
                executingAssembly = "Unknown Read Error";
            }

            return executingAssembly;
        }
    }
}