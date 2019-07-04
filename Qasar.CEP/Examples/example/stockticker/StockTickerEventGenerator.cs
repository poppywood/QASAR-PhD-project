using System;
using System.Collections.Generic;

using net.esper.compat;
using net.esper.example.stockticker.eventbean;

namespace net.esper.example.stockticker
{
	public class StockTickerEventGenerator : StockTickerRegressionConstants
	{
	    private readonly Random random = new Random();

	    public List<Object> MakeEventStream(int numberOfTicks, int ratioOutOfLimit, int numberOfStocks)
	    {
	        List<Object> stream = new List<Object>();

	        PriceLimit[] limitBeans = MakeLimits("example_user",
	                numberOfStocks, PRICE_LIMIT_PCT_LOWER_LIMIT, PRICE_LIMIT_PCT_UPPER_LIMIT);

	        for (int i = 0; i < limitBeans.Length; i++)
	        {
	            stream.Add(limitBeans[i]);
	        }

	        // The first stock ticker sets up an initial price
	        StockTick[] initialPrices = MakeInitialPriceStockTicks(limitBeans,
	                PRICE_LOWER_LIMIT, PRICE_UPPER_LIMIT);

	        for (int i = 0; i < initialPrices.Length; i++)
	        {
	            stream.Add(initialPrices[i]);
	        }

	        for (int i = 0; i < numberOfTicks; i++)
	        {
	            int index = i % limitBeans.Length;
	            StockTick tick = MakeStockTick(limitBeans[index], initialPrices[index]);

	            // Generate an out-of-limit price
	            if ((i % ratioOutOfLimit) == 0)
	            {
	                tick = new StockTick(tick.StockSymbol, -1);
	            }

	            // Last tick is out-of-limit as well
	            if (i == (numberOfTicks - 1))
	            {
	                tick = new StockTick(tick.StockSymbol, 9999);
	            }

	            stream.Add(tick);
	        }

	        return stream;
	    }

	    public StockTick MakeStockTick(PriceLimit limitBean, StockTick initialPrice)
	    {
	        String stockSymbol = limitBean.StockSymbol;
	        double range = initialPrice.Price * limitBean.LimitPct / 100;
	        double price = (initialPrice.Price - range + (range * 2 * random.NextDouble()));

	        double priceReducedPrecision = To1tenthPrecision(price);

	        if (priceReducedPrecision < (initialPrice.Price - range))
	        {
	            priceReducedPrecision = initialPrice.Price;
	        }

	        if (priceReducedPrecision > (initialPrice.Price + range))
	        {
	            priceReducedPrecision = initialPrice.Price;
	        }

	        return new StockTick(stockSymbol, priceReducedPrecision);
	    }

	    public PriceLimit[] MakeLimits(String userName,
	                                          int numBeans,
	                                          double limit_pct_lower_boundary,
	                                          double limit_pct_upper_boundary)
	    {
	        PriceLimit[] limitBeans = new PriceLimit[numBeans];

	        for (int i = 0; i < numBeans; i++)
	        {
	            String stockSymbol = "SYM_" + i;

	            double diff = limit_pct_upper_boundary - limit_pct_lower_boundary;
	            double limitPct = limit_pct_lower_boundary + (random.NextDouble() * diff);

	            limitBeans[i] = new PriceLimit(userName, stockSymbol, To1tenthPrecision(limitPct));
	        }

	        return limitBeans;
	    }

	    public StockTick[] MakeInitialPriceStockTicks(PriceLimit[] limitBeans,
	                                                  double price_lower_boundary,
	                                                  double price_upper_boundary)
	    {
	        StockTick[] stockTickBeans = new StockTick[limitBeans.Length];

	        for (int i = 0; i < stockTickBeans.Length; i++)
	        {
	            String stockSymbol = limitBeans[i].StockSymbol;

	            // Determine a random price
	            double diff = price_upper_boundary - price_lower_boundary;
	            double price = price_lower_boundary + random.NextDouble() * diff;

	            stockTickBeans[i] = new StockTick(stockSymbol, To1tenthPrecision(price));
	        }

	        return stockTickBeans;
	    }

	    private static double To1tenthPrecision(double aDouble)
	    {
	        int intValue = (int) (aDouble * 10);
	        return intValue / 10.0;
	    }
	}
} // End of namespace
