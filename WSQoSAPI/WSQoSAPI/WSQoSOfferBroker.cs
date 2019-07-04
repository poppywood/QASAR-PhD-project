namespace WSQoSAPI
{

    public interface WSQoSOfferBroker
    {

        void Dispose();

        WSQoSAPI.QoSOffer GetBestOffer(WSQoSAPI.QoSRequirements Requirements, string uddiTModelKey);

        string GetUddiReqistryLocation();

    }

}

