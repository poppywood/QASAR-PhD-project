using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Qasar.Controller;
using Qasar.Monitor;
using Qasar.DataLayer;

namespace Qasar.ESB.Core
{
    /// <summary>
    /// IntegrationHub
    /// </summary>
    public sealed class IntegrationHub : IMyHub
    {
        static int counter = 0;
        static int opto = 0;
        static DateTime start;
        static Metrics metrics = new Metrics();
        static int MetricsInterval = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MetricsInterval"]);
        static int OptoInterval = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["OptoInterval"]);
        private delegate PolicyResponse SubmitPolicyCallback(PolicyRequest request);
        SubmitPolicyCallback _submitPolicyCallback;

        /// <summary>
        /// This method accepts policy data
        /// </summary>
        /// <param name="request">A PolicyRequest message</param>
        /// <returns>A PolicyResponse message</returns>
        public PolicyResponse SubmitPolicy(PolicyRequest request)
        {
            PolicyResponse resp = new PolicyResponse();
            Security sec = new Security();
            Security.SecurityCheck result = sec.ConfirmCredentials(request.Credentials.LoginID, request.Credentials.Password, request.Credentials.Source);
            if (result == Security.SecurityCheck.Success)
            {
                counter++;
                opto++;
                if (counter == 1) start = DateTime.Now;
                Pipeline.Builder pipe = new Pipeline.Builder();
                resp = (PolicyResponse)pipe.Process(request);
                GatherMetrics();
                Opto();
                //if (counter == 100)
                //{
                //    counter = 0;
                //    Thread t = new Thread(new ThreadStart(GatherMetrics));
                //    t.Priority = ThreadPriority.Normal;
                //    t.Start();
                //}
                //if (opto == 500)
                //{
                //    opto = 0;
                //    Thread t = new Thread(new ThreadStart(Opto));
                //    t.Priority = ThreadPriority.Normal;
                //    t.Start();
                //}
            }
            else
            {
                //build a response to report the error.
                resp.PolicyRef = request.PolicyRef;
                resp.ProductCode = request.ProductCode;
                resp.OverallResult = "False";
                switch (result)
                {
                    case Security.SecurityCheck.InvalidLogin:
                        resp.Result = "The login was not recognized";
                        break;
                    case Security.SecurityCheck.InvalidPassword:
                        resp.Result = "The password is invalid";
                        break;
                    case Security.SecurityCheck.InvalidSource:
                        resp.Result = "The login is invalid for the source";
                        break;
                }
            }
            return resp;
        }

        private static void GatherMetrics()
        {
            MetricsCollector c = new MetricsCollector();
            DateTime now = DateTime.Now;
            DateTime last = metrics.GetLastMetricsDate();
            long interval = Math.Abs(DateAndTime.DateDiff(DateInterval.Second, last, now));
            int intv = Convert.ToInt32(interval);
            if (intv > MetricsInterval)
            {
                c.GatherMetrics(MetricsInterval, now.AddSeconds(-MetricsInterval));
                metrics.SetLastMetricsDate(now);
            }
        }

        private static void Opto()
        {
            Controller.Controller c = new Controller.Controller();
            DateTime now = DateTime.Now;
            DateTime last = metrics.GetLastOptoDate();
            long interval = Math.Abs(DateAndTime.DateDiff(DateInterval.Second, last, now));
            int intv = Convert.ToInt32(interval);
            if (intv > OptoInterval)
            {
                c.CheckCurrentCondition();
                metrics.SetLastOptoDate(now);
            }
        }


        /// <summary>
        /// Asyncronous version of SubmitPolicy, uses the standard dotnet AsnycResult pattern.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public System.IAsyncResult BeginSubmitPolicy(PolicyRequest request, System.AsyncCallback callback, object state)
        {
            if (request == null) throw new ArgumentNullException("request");

            _submitPolicyCallback = new SubmitPolicyCallback(SubmitPolicy);
            System.IAsyncResult result = _submitPolicyCallback.BeginInvoke(request, callback, state);
            return result;
        }

        /// <summary>
        /// standard End method for SubmitPolicy, uses IAsyncResult pattern.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public PolicyResponse EndSubmitPolicy(IAsyncResult result)
        {
            if (_submitPolicyCallback == null)
                throw new ApplicationException("Must call BeginSubmitPolicy before calling EndSubmitPolicy");
            return _submitPolicyCallback.EndInvoke(result);
        }
    }

    public enum DateInterval
    {
        Day,
        DayOfYear,
        Hour,
        Minute,
        Month,
        Quarter,
        Second,
        Weekday,
        WeekOfYear,
        Year
    }

    public class DateAndTime
    {
        public static long DateDiff(DateInterval interval, DateTime dt1, DateTime dt2)
        {
            return DateDiff(interval, dt1, dt2, System.Globalization.DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
        }

        private static int GetQuarter(int nMonth)
        {
            if (nMonth <= 3)
                return 1;
            if (nMonth <= 6)
                return 2;
            if (nMonth <= 9)
                return 3;
            return 4;
        }

        public static long DateDiff(DateInterval interval, DateTime dt1, DateTime dt2, DayOfWeek eFirstDayOfWeek)
        {
            if (interval == DateInterval.Year)
                return dt2.Year - dt1.Year;

            if (interval == DateInterval.Month)
                return (dt2.Month - dt1.Month) + (12 * (dt2.Year - dt1.Year));

            TimeSpan ts = dt2 - dt1;

            if (interval == DateInterval.Day || interval == DateInterval.DayOfYear)
                return Round(ts.TotalDays);

            if (interval == DateInterval.Hour)
                return Round(ts.TotalHours);

            if (interval == DateInterval.Minute)
                return Round(ts.TotalMinutes);

            if (interval == DateInterval.Second)
                return Round(ts.TotalSeconds);

            if (interval == DateInterval.Weekday)
            {
                return Round(ts.TotalDays / 7.0);
            }

            if (interval == DateInterval.WeekOfYear)
            {
                while (dt2.DayOfWeek != eFirstDayOfWeek)
                    dt2 = dt2.AddDays(-1);
                while (dt1.DayOfWeek != eFirstDayOfWeek)
                    dt1 = dt1.AddDays(-1);
                ts = dt2 - dt1;
                return Round(ts.TotalDays / 7.0);
            }

            if (interval == DateInterval.Quarter)
            {
                double d1Quarter = GetQuarter(dt1.Month);
                double d2Quarter = GetQuarter(dt2.Month);
                double d1 = d2Quarter - d1Quarter;
                double d2 = (4 * (dt2.Year - dt1.Year));
                return Round(d1 + d2);
            }

            return 0;

        }

        private static long Round(double dVal)
        {
            if (dVal >= 0)
                return (long)Math.Floor(dVal);
            return (long)Math.Ceiling(dVal);
        }
    } 
}
