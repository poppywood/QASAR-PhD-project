using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

using net.esper.client;
using net.esper.compat;
using net.esper.support.util;

using org.apache.commons.logging;

namespace net.esper.example.marketdatafeed
{
    public class FeedSimMain
    {
        private static readonly Log log = LogFactory.GetLog(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void Main(String[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine(
                    "Arguments are: <number of threads> <drop probability percent> <number of seconds to run>");
                Console.WriteLine("  number of threads: the number of threads sending feed events into the engine");
                Console.WriteLine("  drop probability percent: a number between zero and 100 that dictates the ");
                Console.WriteLine("                            probability that per second one of the feeds drops off");
                Console.WriteLine("  number of seconds: the number of seconds the simulation runs");
                Environment.Exit(-1);
            }

            int numberOfThreads;
            try
            {
                numberOfThreads = Int32.Parse(args[0]);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid number of threads: " + args[0]);
                Environment.Exit(-2);
                return;
            }

            double dropProbability;
            try
            {
                dropProbability = Double.Parse(args[1]);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Invalid drop probability:" + args[1]);
                Environment.Exit(-2);
                return;
            }

            int numberOfSeconds;
            try
            {
                numberOfSeconds = Int32.Parse(args[2]);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Invalid number of seconds to run:" + args[2]);
                Environment.Exit(-2);
                return;
            }

            // Run the sample
            Console.WriteLine("Using " + numberOfThreads + " threads with a drop probability of " + dropProbability +
                              "%, for " + numberOfSeconds + " seconds");
            FeedSimMain feedSimMain = new FeedSimMain(numberOfThreads, dropProbability, numberOfSeconds, true);
            feedSimMain.Run();
        }

        private readonly int numberOfThreads;
        private readonly double dropProbability;
        private readonly int numSeconds;
        private readonly bool isWaitKeypress;

        public FeedSimMain(int numberOfThreads, double dropProbability, int numSeconds, bool isWaitKeypress)
        {
            this.numberOfThreads = numberOfThreads;
            this.dropProbability = dropProbability;
            this.numSeconds = numSeconds;
            this.isWaitKeypress = isWaitKeypress;
        }

        public void Run()
        {
            if (isWaitKeypress)
            {
                Console.WriteLine("...press enter to start simulation...");
                Console.ReadKey();
            }

            // Configure engine with event names to make the statements more readable.
            // This could also be done in a configuration file.
            Configuration configuration = new Configuration();
            configuration.AddEventTypeAlias("MarketDataEvent", typeof(MarketDataEvent).FullName);

            // Get engine instance
            EPServiceProvider epService = EPServiceProviderManager.GetProvider("FeedSimMain", configuration);

            // Set up statements
            TicksPerSecondStatement tickPerSecStmt = new TicksPerSecondStatement(epService.EPAdministrator);
            tickPerSecStmt.AddListener(new RateReportingListener());

            TicksFalloffStatement falloffStmt = new TicksFalloffStatement(epService.EPAdministrator);
            falloffStmt.AddListener(new RateFalloffAlertListener());

            // Send events
            ExecutorService threadPool = Executors.NewFixedThreadPool(numberOfThreads);
            MarketDataSendRunnable[] runnables = new MarketDataSendRunnable[numberOfThreads];
            for (int i = 0; i < numberOfThreads; i++)
            {
                runnables[i] = new MarketDataSendRunnable(epService);
                threadPool.Submit(runnables[i]);
            }

            int seconds = 0;
            Random random = new Random();
            while (seconds < numSeconds)
            {
                seconds++;
                Thread.Sleep(1000);

                FeedEnum? feedToDropOff;
                if (random.NextDouble() * 100 < dropProbability)
                {
                    feedToDropOff = FeedEnum.FEED_A;
                    if (random.Next(0, 2) == 1)
                    {
                        feedToDropOff = FeedEnum.FEED_B;
                    }
                    log.Info("Setting drop-off for feed " + feedToDropOff);
                }
                else
                {
                    feedToDropOff = null;
                }
                for (int i = 0; i < runnables.Length; i++)
                {
                    runnables[i].SetRateDropOffFeed(feedToDropOff);
                }
            }

            log.Info("Shutting down threadpool");
            for (int i = 0; i < runnables.Length; i++)
            {
                runnables[i].SetShutdown();
            }
            threadPool.Shutdown();
            threadPool.AwaitTermination(new TimeSpan(0, 0, 0, 10));
        }
    }
}