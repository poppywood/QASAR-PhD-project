namespace WSQoSAPI
{

    public interface WSQoSOfferBroker
    {

        void Dispose();

        QoSOffer GetBestOffer(QoSRequirements Requirements, string uddiTModelKey);

        string GetUddiReqistryLocation();

    }

}

