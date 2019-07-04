using System;
using System.Collections;
using System.Threading;

namespace WSQoSAPI
{

    public abstract class CurrencyConverter
    {

        protected class ExchangeRate
        {

            public double Value;

            public ExchangeRate()
            {
                Value = 0.0;
            }

        } // class ExchangeRate

        public const string DEFAULT_CURRENCY = "EUR";
        public const int DEFAULT_UPDATE_INTERVAL = 60;

        private string _defaultCurrency;
        private ArrayList Currencies;
        private ArrayList ExchangeRates;
        private Mutex m;
        public int UpdateInterval;

        public static CurrencyConverter ActiveCurrencyConverter;

        public string ConverterUrl
        {
            get
            {
                return GetConverterUrl();
            }
            set
            {
                SetConverterUrl(value);
            }
        }

        public string DefaultCurrency
        {
            get
            {
                return _defaultCurrency;
            }
            set
            {
                _defaultCurrency = value;
                updateExchangeRates();
            }
        }

        public CurrencyConverter()
        {
            _defaultCurrency = "EUR\uFFFD";
            UpdateInterval = 60;
            Currencies = new ArrayList();
            ExchangeRates = new ArrayList();
            m = new Mutex();
            Thread thread = new Thread(new ThreadStart(updateExchangeRates));
            thread.Start();
        }

        static CurrencyConverter()
        {
            CurrencyConverter.ActiveCurrencyConverter = null;
        }

        protected abstract string GetConverterUrl();

        protected abstract Uri getCurrencyEncodingOverviewDocument();

        public double GetExchangeRate(string currency)
        {
            double d = 0.0;
            m.WaitOne();
            int i = Currencies.IndexOf(currency);
            if (i == -1)
            {
                CurrencyConverter.ExchangeRate exchangeRate = new CurrencyConverter.ExchangeRate();
                exchangeRate.Value = retrieveExchangeRate(currency, DefaultCurrency);
                Currencies.Add(currency);
                ExchangeRates.Add(exchangeRate);
                d = exchangeRate.Value;
            }
            else
            {
                d = ((CurrencyConverter.ExchangeRate)ExchangeRates[i]).Value;
            }
            m.ReleaseMutex();
            return d;
        }

        protected abstract double retrieveExchangeRate(string FromCurrency, string ToCurrency);

        protected abstract void SetConverterUrl(string url);

        public void updateExchangeRates()
        {
            // trial
        }

    } // class CurrencyConverter

}

