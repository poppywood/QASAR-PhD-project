using System.Collections;
using System.Threading;

namespace WSQoSAPI
{

    public class ServiceUpdateList
    {

        private Mutex _m;
        private ArrayList _services;

        public ServiceUpdateList()
        {
            _services = new ArrayList();
            _m = new Mutex();
        }

        public void AddService(ServiceModel NewService)
        {
            _m.WaitOne();
            _services.Add(NewService);
            _m.ReleaseMutex();
        }

        public ServiceModel GetNextService()
        {
            // trial
            return null;
        }

    } // class ServiceUpdateList

}

