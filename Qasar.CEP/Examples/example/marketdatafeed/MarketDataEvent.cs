using System;

namespace net.esper.example.marketdatafeed
{
    public class MarketDataEvent
    {
        private string symbol;
        private FeedEnum feed;

        public MarketDataEvent(String symbol, FeedEnum feed)
        {
            this.symbol = symbol;
            this.feed = feed;
        }

        public string Symbol
        {
            get { return symbol; }
        }

        public FeedEnum Feed
        {
            get { return feed; }
        }
    }
}